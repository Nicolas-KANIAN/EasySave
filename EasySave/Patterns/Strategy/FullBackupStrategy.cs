using EasySave.Patterns.Bridge;
using System.Collections.Generic;

namespace EasySave.Patterns.Strategy
{
    public class FullBackupStrategy : IBackupStrategy
    {
        public List<string> GetFilesToCopy(string sourceDirectory, string targetDirectory, List<string> allFiles, IFileSystem fileSystem)
        {
            return allFiles;
        }
    }
}