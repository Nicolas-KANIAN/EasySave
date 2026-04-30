using System.Diagnostics;
using System.Text;

namespace CryptoSoft
{
    public class FileManager
    {
        private string FilePath { get; }
        private string Key { get; }

        public FileManager(string path, string key)
        {
            FilePath = path;
            Key = key;
        }

        private bool CheckFile()
        {
            if (File.Exists(FilePath)) return true;
            return false;
        }

        public int TransformFile()
        {
            if (!CheckFile()) return -1;
            Stopwatch stopwatch = Stopwatch.StartNew();
            var fileBytes = File.ReadAllBytes(FilePath);
            var keyBytes = Encoding.UTF8.GetBytes(Key);

            // Méthode XOR
            var result = new byte[fileBytes.Length];
            for (var i = 0; i < fileBytes.Length; i++)
            {
                result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            File.WriteAllBytes(FilePath, result);
            stopwatch.Stop();
            return (int)stopwatch.ElapsedMilliseconds;
        }
    }
}