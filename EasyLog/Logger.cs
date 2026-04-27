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

        // Determines the format used for writing log files.
        public LogFormat Format { get; set; } = LogFormat.Json;

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
                            logs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                        }
                    }

                    logs.Add(entry);
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    File.WriteAllText(filePath, JsonSerializer.Serialize(logs, options));
                }
                else if (Format == LogFormat.Xml)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));

                    if (File.Exists(filePath))
                    {
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            try
                            {
                                logs = (List<LogEntry>?)serializer.Deserialize(reader) ?? new List<LogEntry>();
                            }
                            catch
                            {
                                // Handles corrupted or empty XML files gracefully
                            }
                        }
                    }

                    logs.Add(entry);
                    using (StreamWriter writer = new StreamWriter(filePath))
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
    }
}