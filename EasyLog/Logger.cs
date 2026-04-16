using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasyLog
{
    public class Logger
    {
        private readonly string logDirectory = @"C:\EasySave\Logs";

        public Logger()
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public void WriteDailyLog(LogEntry entry)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = Path.Combine(logDirectory, $"{date}.json");

            var options = new JsonSerializerOptions { WriteIndented = true };
            List<LogEntry> logs = new List<LogEntry>();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    logs = JsonSerializer.Deserialize<List<LogEntry>>(json);
                }
            }

            logs.Add(entry);
            File.WriteAllText(filePath, JsonSerializer.Serialize(logs, options));
        }

        public void UpdateState(List<StateEntry> states)
        {
            string filePath = Path.Combine(logDirectory, "state.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(states, options));
        }
    }
}