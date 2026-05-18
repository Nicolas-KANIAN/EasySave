using System.Diagnostics;

namespace EasySave.Services
{
    public class EncryptionService
    {
        private const int _timeoutMilliseconds = 60_000;
        private static readonly SemaphoreSlim _cryptoInstanceLock = new SemaphoreSlim(1, 1);

        private readonly AppConfig _config;

        public EncryptionService(AppConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public long Encrypt(string filePath, string key)
        {
            string cryptoPath = _config.CryptoSoftPath;

            if (string.IsNullOrWhiteSpace(cryptoPath) || !File.Exists(cryptoPath))
            {
                return -2;
            }

            _cryptoInstanceLock.Wait();

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true
                };

                if (cryptoPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    string? dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
                    string dotnetCmd = string.IsNullOrEmpty(dotnetRoot) ? "dotnet" : Path.Combine(dotnetRoot, "dotnet");

                    startInfo.FileName = dotnetCmd;
                    startInfo.ArgumentList.Add(cryptoPath);
                }
                else
                {
                    startInfo.FileName = cryptoPath;
                }

                startInfo.ArgumentList.Add(filePath);

                using Process? process = Process.Start(startInfo);

                if (process is null) return -3;

                using (StreamWriter writer = process.StandardInput)
                {
                    if (writer.BaseStream.CanWrite)
                    {
                        writer.WriteLine(key);
                    }
                }

                if (!process.WaitForExit(_timeoutMilliseconds))
                {
                    try
                    {
                        process.Kill(true);
                        process.WaitForExit();
                    }
                    catch { }

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