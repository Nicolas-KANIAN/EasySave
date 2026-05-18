using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace EasyLog
{
    // Defines the supported formats for local log files
    public enum LogFormat
    {
        Json,
        Xml
    }

    // Defines where the daily activity logs should be routed
    public enum LogDestination
    {
        Local,
        Centralized,
        Both
    }

    public class Logger
    {
        private readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static Logger? _instance;
        private static readonly object _lock = new object();
        private static readonly SemaphoreSlim _networkSemaphore = new SemaphoreSlim(5, 5); // Limits concurrent network connections

        private LogFormat _format = LogFormat.Json;

        // Network configuration
        public LogDestination Destination { get; set; } = LogDestination.Local;
        public string LogServerIp { get; set; } = "127.0.0.1";
        public int LogServerPort { get; set; } = 12345;

        public LogFormat Format
        {
            get => _format;
            set
            {
                lock (_lock)
                {
                    if (_format == value) return;
                    _format = value;

                    string staleExt = value == LogFormat.Json ? ".xml" : ".json";
                    string stalePath = Path.Combine(_logDirectory, $"state{staleExt}");

                    if (File.Exists(stalePath))
                    {
                        try
                        {
                            File.Delete(stalePath);
                        }
                        catch (IOException ex)
                        {
                            Console.Error.WriteLine($"[WARNING] Cannot delete stale state file (File is locked): {ex.Message}");
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.Error.WriteLine($"[WARNING] Cannot delete stale state file (Access denied): {ex.Message}");
                        }
                    }
                }
            }
        }

        private Logger()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public static Logger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                    return _instance;
                }
            }
        }

        public void WriteDailyLog(LogEntry entry)
        {
            // 1. CENTRALIZED LOGGING
            if (Destination == LogDestination.Centralized || Destination == LogDestination.Both)
            {
                Task.Run(async () => await SendLogToCentralServerAsync(entry));
            }

            // 2. LOCAL LOGGING
            if (Destination == LogDestination.Local || Destination == LogDestination.Both)
            {
                WriteLocalDailyLog(entry);
            }
        }

        public void UpdateState(List<StateEntry> states)
        {
            lock (_lock)
            {
                string extension = Format == LogFormat.Json ? ".json" : ".xml";
                string filePath = Path.Combine(_logDirectory, $"state{extension}");

                try
                {
                    if (Format == LogFormat.Json)
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        File.WriteAllText(filePath, JsonSerializer.Serialize(states, options));
                    }
                    else if (Format == LogFormat.Xml)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<StateEntry>));
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            serializer.Serialize(writer, states);
                        }
                    }
                }
                catch (IOException)
                {
                    // Silent failure if the UI is reading the file simultaneously.
                    // It will be safely overwritten during the next tick.
                }
            }
        }

        private void WriteLocalDailyLog(LogEntry entry)
        {
            lock (_lock)
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string extension = Format == LogFormat.Json ? ".json" : ".xml";
                string filePath = Path.Combine(_logDirectory, $"{date}{extension}");

                List<LogEntry> logs = new List<LogEntry>();

                if (Format == LogFormat.Json)
                {
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            string json = File.ReadAllText(filePath);
                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                logs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                            }
                        }
                        catch (JsonException)
                        {
                            logs = new List<LogEntry>();
                        }
                    }

                    logs.Add(entry);
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    File.WriteAllText(filePath, JsonSerializer.Serialize(logs, options));
                }
                else if (Format == LogFormat.Xml)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));

                    if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                    {
                        try
                        {
                            using (StreamReader reader = new StreamReader(filePath))
                            {
                                logs = (List<LogEntry>?)serializer.Deserialize(reader) ?? new List<LogEntry>();
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            logs = new List<LogEntry>();
                        }
                    }

                    logs.Add(entry);
                    var xmlSettings = new System.Xml.XmlWriterSettings { Indent = true };
                    using (var writer = System.Xml.XmlWriter.Create(filePath, xmlSettings))
                    {
                        serializer.Serialize(writer, logs);
                    }
                }
            }
        }

        private async Task SendLogToCentralServerAsync(LogEntry entry)
        {
            await _networkSemaphore.WaitAsync();
            try
            {
                // Serialize the log entry into a single unindented line for TCP transmission
                string jsonLog = JsonSerializer.Serialize(entry);
                int maxRetries = 3;
                int delayMs = 500;

                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        using TcpClient client = new TcpClient();

                        var connectTask = client.ConnectAsync(LogServerIp, LogServerPort);
                        if (await Task.WhenAny(connectTask, Task.Delay(2000)) == connectTask)
                        {
                            await connectTask;
                            using NetworkStream stream = client.GetStream();
                            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

                            await writer.WriteLineAsync(jsonLog);
                            await writer.FlushAsync();
                            return;
                        }
                        else
                        {
                            throw new TimeoutException("Connection timed out.");
                        }
                    }
                    catch (Exception)
                    {
                        if (i == maxRetries - 1) throw;

                        await Task.Delay(delayMs);
                        delayMs *= 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[WARNING] Failed to send log to central server at {LogServerIp}:{LogServerPort} - {ex.Message}");
            }
            finally
            {
                _networkSemaphore.Release();
            }
        }

        public string ReadStateFileSafely(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return string.Empty;

            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(fileStream);
                return streamReader.ReadToEnd();
            }
            catch (IOException)
            {
                return string.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                return string.Empty;
            }
        }
    }
}