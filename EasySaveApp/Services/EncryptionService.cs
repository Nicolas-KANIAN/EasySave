using System;
using System.Diagnostics;
using System.IO;

namespace EasySave.Services
{
    public class EncryptionService
    {
        private readonly string _cryptoSoftExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoSoft.exe");

        private const int TimeoutMilliseconds = 60_000;

        public long Encrypt(string filePath, string key)
        {
            if (!File.Exists(_cryptoSoftExe)) return -2;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _cryptoSoftExe,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add(filePath);
            startInfo.ArgumentList.Add(key);

            try
            {
                using Process? process = Process.Start(startInfo);

                if (process is null) return -3;

                if (!process.WaitForExit(TimeoutMilliseconds))
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
        }
    }
}