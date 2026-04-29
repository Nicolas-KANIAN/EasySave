namespace EasySave.Patterns.Bridge
{
    public interface IFileSystem
    {
        // Defines an abstraction layer for physical file system operations.
        // Decouples the application from the OS to allow for easy unit testing (mocking).
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        bool FileExists(string path);
        DateTime GetLastWriteTime(string path);
        void CopyFile(string source, string target, bool overwrite);
        long GetFileSize(string path);
        List<string> GetFilesRecursive(string directory);
    }
}