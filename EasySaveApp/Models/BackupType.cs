namespace EasySave.Models
{
    // Defines the available backup strategies: a complete copy (Full) or only modified files (Differential).
    public enum BackupType
    {
        Full,
        Differential
    }
}