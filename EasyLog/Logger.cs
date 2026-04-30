using System.Text.Json;
using System.Xml.Serialization;

namespace EasyLog
{
    // Defines the supported formats for log files.
    public enum LogFormat
    {
        Json,
        Xml
    }

    public class Logger
    {
        private readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static Logger? _instance;
        private static readonly object _lock = new object();

        private LogFormat _format = LogFormat.Json;

        // Determines the format used for writing log files.
        public LogFormat Format
        {
            get => _format;
            set
            {
                lock (_lock)
                {
                    if (_format == value) return;
                    _format = value;

                    // Deletes the state file of the old format to prevent stale data.
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

        // Centralizes the management of log writing and real-time tracking.
        // Automatically creates the log directory.
        private Logger()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        // Gets the thread-safe Singleton instance of the Logger.
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

        // Appends a new log entry to the daily log file.
        // Serializes in JSON or XML based on the Format property.
        public void WriteDailyLog(LogEntry entry)
        {
            lock (_lock) // Ensures thread-safety during Read/Write operations
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string extension = Format == LogFormat.Json ? ".json" : ".xml";
                string filePath = Path.Combine(_logDirectory, $"{date}{extension}");

                List<LogEntry> logs = new List<LogEntry>();

                if (Format == LogFormat.Json)
                {
                    if (File.Exists(filePath))
                    {
                        string json = File.ReadAllText(filePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            try
                            {
                                logs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                            }
                            catch (JsonException)
                            {
                                logs = new List<LogEntry>();
                            }
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
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            try
                            {
                                logs = (List<LogEntry>?)serializer.Deserialize(reader) ?? new List<LogEntry>();
                            }
                            catch (InvalidOperationException)
                            {
                                logs = new List<LogEntry>();
                            }
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

        // Updates the state file with the current progress of backup jobs.
        // Overwrites the file in JSON or XML based on the Format property.
        public void UpdateState(List<StateEntry> states)
        {
            lock (_lock)
            {
                string extension = Format == LogFormat.Json ? ".json" : ".xml";
                string filePath = Path.Combine(_logDirectory, $"state{extension}");

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
        }

        public string ReadStateFileSafely(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}