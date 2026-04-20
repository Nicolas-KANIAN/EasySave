using EasySave.Models;
using EasySave.Patterns.Strategy;

namespace EasySave.Patterns.Factory
{
    public static class BackupFactory
    {
        public static IBackupStrategy CreateStrategy(BackupType type)
        {
            return type switch
            {
                BackupType.Full => new FullBackupStrategy(),
                BackupType.Differential => new DifferentialBackupStrategy(),
                _ => throw new ArgumentException("Unknown backup type.")
            };
        }
    }
}