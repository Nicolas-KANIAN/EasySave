using System.Diagnostics;

namespace EasySave.Services
{
    public class EncryptionService
    {
        private readonly string _cryptoSoftExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoSoft.exe");

        private const int _timeoutMilliseconds = 60_000;

        private static readonly SemaphoreSlim _cryptoInstanceLock = new SemaphoreSlim(1, 1);

        public long Encrypt(string filePath, string key)
        {
            if (!File.Exists(_cryptoSoftExe)) return -2;

            _cryptoInstanceLock.Wait();

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _cryptoSoftExe,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

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