using EasySave.Patterns.Bridge;
using System.Collections.Generic;

namespace EasySave.Patterns.Strategy
{
    public interface IBackupStrategy
    {
        List<string> GetFilesToCopy(string sourceDirectory, string targetDirectory, List<string> allFiles, IFileSystem fileSystem);
    }
}