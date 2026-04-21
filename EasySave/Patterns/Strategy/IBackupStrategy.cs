using EasySave.Patterns.Bridge;

namespace EasySave.Patterns.Strategy
{
    public interface IBackupStrategy
    {
        List<string> GetFilesToCopy(string sourceDirectory, string targetDirectory, List<string> allFiles, IFileSystem fileSystem);
    }
}