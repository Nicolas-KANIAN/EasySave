using System;
using System.Collections.Generic;

namespace EasySave.Patterns.Bridge
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        bool FileExists(string path);
        DateTime GetLastWriteTime(string path);
        void CopyFile(string source, string target, bool overwrite);
        long GetFileSize(string path);
        List<string> GetFilesRecursive(string directory);
    }
}