using EasySave.Patterns.Bridge;

namespace EasySave.Patterns.Strategy
{
    // Defines the strict contract for all backup algorithms.
    // Allows the BackupEngine to use different backup behaviors (Full, Differential) interchangeably.
    public interface IBackupStrategy
    {
        List<string> GetFilesToCopy(string sourceDirectory, string targetDirectory, List<string> allFiles, IFileSystem fileSystem);
    }
}