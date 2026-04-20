using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasyLog
{
    public class Logger
    {
        private readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static Logger? _instance;
        private static readonly object _lock = new object();

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

        public void UpdateState(List<StateEntry> states)
        {
            string filePath = Path.Combine(_logDirectory, "state.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(states, options));
        }
    }
}