namespace EasySave.Patterns.Bridge
{
    public class LocalFileSystem : IFileSystem
    {
        // Concrete implementation of IFileSystem. 
        // Executes actual file and folder operations on the local physical drive using System.IO.
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public bool FileExists(string path) => File.Exists(path);
        public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);
        public void CopyFile(string source, string target, bool overwrite) => File.Copy(source, target, overwrite);
        public long GetFileSize(string path) => new FileInfo(path).Length;

        public List<string> GetFilesRecursive(string directory)
        {
            var files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(directory));
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    files.AddRange(GetFilesRecursive(dir));
                }
            }
            catch (UnauthorizedAccessException) { }
            return files;
        }
    }
}