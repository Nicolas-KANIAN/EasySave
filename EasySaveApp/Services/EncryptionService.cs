using System.Diagnostics;

namespace EasySave.Services
{
    public class EncryptionService
    {
        private const int _timeoutMilliseconds = 60_000;
        private static readonly SemaphoreSlim _cryptoInstanceLock = new SemaphoreSlim(1, 1);

        public long Encrypt(string filePath, string key)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string cryptoSoftDll = Path.Combine(baseDir, "CryptoSoft.dll");

            if (!File.Exists(cryptoSoftDll))
            {
                string fallbackDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "CryptoSoft", "bin", "Release", "net8.0"));
                cryptoSoftDll = Path.Combine(fallbackDir, "CryptoSoft.dll");

                if (!File.Exists(cryptoSoftDll)) return -2;
            }

            _cryptoInstanceLock.Wait();

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                startInfo.ArgumentList.Add(cryptoSoftDll);
                startInfo.ArgumentList.Add(filePath);
                startInfo.ArgumentList.Add(key);

                using Process? process = Process.Start(startInfo);

                if (process is null) return -3;

                if (!process.WaitForExit(_timeoutMilliseconds))
                {
                    try { process.Kill(true); } catch { }
                    return -4;
                }

                return process.ExitCode;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                _cryptoInstanceLock.Release();
            }
        }
    }
}