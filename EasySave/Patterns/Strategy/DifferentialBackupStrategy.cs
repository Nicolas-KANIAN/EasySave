using EasySave.Patterns.Bridge;

namespace EasySave.Patterns.Strategy
{
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        public List<string> GetFilesToCopy(string sourceDirectory, string targetDirectory, List<string> allFiles, IFileSystem fileSystem)
        {
            var filesToCopy = new List<string>();
            foreach (var file in allFiles)
            {
                string relativePath = file.Substring(sourceDirectory.Length + 1);
                string targetFile = Path.Combine(targetDirectory, relativePath);

                if (!fileSystem.FileExists(targetFile) || fileSystem.GetLastWriteTime(file) > fileSystem.GetLastWriteTime(targetFile))
                {
                    filesToCopy.Add(file);
                }
            }
            return filesToCopy;
        }
    }
}