using System.Text.Json;

namespace EasyLog
{
    public class Logger
    {
        private readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static Logger? _instance;
        private static readonly object _lock = new object();

        // Centralizes the management of log writing and real-time tracking in JSON format.
        // Automatically creates the log directory, writes detailed daily logs, and updates the real-time state of backup jobs.
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

        // Appends a new log entry to the daily log file (named yyyy-MM-dd.json).
        // If the file already exists, it reads the previous logs to append the new one without data loss.
        public void WriteDailyLog(LogEntry entry)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = Path.Combine(_logDirectory, $"{date}.json");

            var options = new JsonSerializerOptions { WriteIndented = true };
            List<LogEntry> logs = new List<LogEntry>();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    logs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                }
            }

            logs.Add(entry);
            File.WriteAllText(filePath, JsonSerializer.Serialize(logs, options));
        }

        // Updates the state.json file with the current progress of backup jobs.
        // The entire file content is overwritten on each update to reflect the real-time status.
        public void UpdateState(List<StateEntry> states)
        {
            string filePath = Path.Combine(_logDirectory, "state.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(states, options));
        }
    }
}