// Represents a JSON-serializable log entry containing backup details (timestamp, name, source, target, size, and duration).
namespace EasyLog
{
    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string BackupName { get; set; }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }
        public long FileSize { get; set; }
        public long TransferTime { get; set; }
        public LogEntry()
        {
            Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}