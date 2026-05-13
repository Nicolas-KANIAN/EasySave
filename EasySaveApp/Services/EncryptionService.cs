using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace EasySave.Services
{
    public class EncryptionService
    {
        private const int _timeoutMilliseconds = 60_000;
        private static readonly SemaphoreSlim _cryptoInstanceLock = new SemaphoreSlim(1, 1);

        private readonly AppConfig _config;

        public EncryptionService(AppConfig config)
        {
            _config = config;
        }

        public long Encrypt(string filePath, string key)
        {
            string cryptoPath = _config.CryptoSoftPath;

            if (string.IsNullOrWhiteSpace(cryptoPath) ||
                !File.Exists(cryptoPath) ||
                !cryptoPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                return -2;
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

                startInfo.ArgumentList.Add(cryptoPath);

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