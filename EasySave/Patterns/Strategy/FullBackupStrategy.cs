using EasySave.Patterns.Bridge;

namespace EasySave.Patterns.Strategy
{
    // Concrete strategy for full backups.
    // Simply returns the entire list of source files, as a full backup copies everything without filtering.
    public class FullBackupStrategy : IBackupStrategy
    {
        public List<string> GetFilesToCopy(string sourceDirectory, string targetDirectory, List<string> allFiles, IFileSystem fileSystem)
        {
            return allFiles;
        }
    }
}