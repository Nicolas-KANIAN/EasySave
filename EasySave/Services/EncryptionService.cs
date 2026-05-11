using System.Diagnostics;

namespace EasySave.Services
{
    public class EncryptionService
    {
        private readonly string _cryptoSoftExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoSoft.exe");

        public long Encrypt(string filePath, string key)
        {
            if (!File.Exists(_cryptoSoftExe)) return -2;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _cryptoSoftExe,
                Arguments = $"\"{filePath}\" \"{key}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                return process.ExitCode;
            }
        }
    }
}