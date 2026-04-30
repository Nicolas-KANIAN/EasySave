using EasyLog;
using EasySave.Models;
using EasySave.Patterns.Bridge;
using EasySave.Patterns.Factory;
using EasySave.Patterns.Observer;
using EasySave.Patterns.Strategy;
using System.Diagnostics;

namespace EasySave.Services
{
    // The core execution engine responsible for managing the backup lifecycle.
    public class BackupEngine
    {
        private readonly IFileSystem _fileSystem;
        private readonly List<IBackupObserver> _observers;
        private readonly AppConfig _config;
        private readonly EncryptionService _encryptionService;

        // V2.0: Passing AppConfig to access settings (Extensions, BusinessSoftware)
        public BackupEngine(AppConfig config)
        {
            _fileSystem = new LocalFileSystem();
            _observers = new List<IBackupObserver>();
            _config = config;
            _encryptionService = new EncryptionService();

            AttachObserver(new StateLoggerObserver());
        }

        public void AttachObserver(IBackupObserver observer)
        {
            _observers.Add(observer);
        }

        private void NotifyObservers(StateEntry state)
        {
            foreach (var observer in _observers)
            {
                observer.Update(state);
            }
        }

        // V2.0: Method to check if the business software is running
        private bool IsBusinessSoftwareRunning()
        {
            if (string.IsNullOrWhiteSpace(_config.BusinessSoftware)) return false;

            // Process.GetProcessesByName ignores the .exe extension
            string processName = _config.BusinessSoftware;
            if (processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                processName = processName.Substring(0, processName.Length - 4);
            }

            return Process.GetProcessesByName(processName).Length > 0;
        }

        public void ExecuteJob(BackupJob job)
        {
            Console.WriteLine($"\n[INFO] Starting backup job: {job.Name} ({job.Type})");

            // Ensure progress bar is at 0 at start
            job.Progress = 0;

            // SHOW THE PROGRESS BAR (it will remain visible at the end)
            job.ShowProgress = true;

            // V2.0: Block launch if business software is detected
            if (IsBusinessSoftwareRunning())
            {
                Console.WriteLine($"[WARNING] Business software '{_config.BusinessSoftware}' is running. Backup job '{job.Name}' cannot be launched.");
                return;
            }

            if (!_fileSystem.DirectoryExists(job.SourceDirectory))
            {
                Console.WriteLine($"[ERROR] Source directory does not exist: {job.SourceDirectory}");
                return;
            }

            if (!_fileSystem.DirectoryExists(job.TargetDirectory))
            {
                _fileSystem.CreateDirectory(job.TargetDirectory);
            }

            var allFiles = _fileSystem.GetFilesRecursive(job.SourceDirectory);
            IBackupStrategy strategy = BackupFactory.CreateStrategy(job.Type);
            var filesToCopy = strategy.GetFilesToCopy(job.SourceDirectory, job.TargetDirectory, allFiles, _fileSystem);

            int totalFiles = filesToCopy.Count;
            long totalSize = filesToCopy.Sum(f => _fileSystem.GetFileSize(f));

            if (totalFiles == 0)
            {
                Console.WriteLine("[INFO] No files to copy. Backup is up to date.");
                job.Progress = 100; // Set to 100% directly since everything is up to date
                return;
            }

            int filesCopied = 0;

            StateEntry currentState = new StateEntry
            {
                Name = job.Name,
                State = "ACTIVE",
                TotalFilesToCopy = totalFiles,
                TotalFilesSize = totalSize,
                NbFilesLeftToDo = totalFiles,
                RemainingFilesSize = totalSize,
                Progression = 0
            };

            foreach (var sourceFile in filesToCopy)
            {
                // V2.0: Detect business software DURING backup
                if (IsBusinessSoftwareRunning())
                {
                    Console.WriteLine($"[WARNING] Business software '{_config.BusinessSoftware}' detected! Halting job '{job.Name}'.");

                    currentState.State = "INTERRUPTED";
                    NotifyObservers(currentState);

                    Logger.Instance.WriteDailyLog(new LogEntry
                    {
                        BackupName = job.Name,
                        SourceFile = "SHUTDOWN",
                        TargetFile = $"Business software {_config.BusinessSoftware} detected",
                        FileSize = 0,
                        TransferTime = 0,
                        EncryptionTime = 0
                    });

                    break; // Clean exit from the loop
                }

                string relativePath = sourceFile.Substring(job.SourceDirectory.Length + 1);
                string targetFile = Path.Combine(job.TargetDirectory, relativePath);
                string targetDir = Path.GetDirectoryName(targetFile) ?? string.Empty;

                if (!_fileSystem.DirectoryExists(targetDir))
                {
                    _fileSystem.CreateDirectory(targetDir);
                }

                currentState.CurrentSourceFile = sourceFile;
                currentState.CurrentTargetFile = targetFile;

                Stopwatch sw = Stopwatch.StartNew();

                try
                {
                    long currentFileSize = _fileSystem.GetFileSize(sourceFile);

                    // 1. Standard file copy
                    _fileSystem.CopyFile(sourceFile, targetFile, true);
                    sw.Stop();
                    long transferTime = sw.ElapsedMilliseconds;

                    // 2. V2.0: Encrypt if the extension matches
                    long encryptionTime = 0;
                    string extension = Path.GetExtension(targetFile);

                    if (_config.ExtensionsToEncrypt.Contains(extension))
                    {
                        encryptionTime = _encryptionService.Encrypt(targetFile, _config.CryptoKey);
                    }

                    filesCopied++;

                    // 3. Update state
                    currentState.NbFilesLeftToDo = totalFiles - filesCopied;
                    currentState.RemainingFilesSize -= currentFileSize;
                    currentState.Progression = (int)((double)filesCopied / totalFiles * 100);

                    // Update the UI progress bar
                    job.Progress = currentState.Progression;

                    NotifyObservers(currentState);

                    // 4. Log with EncryptionTime
                    LogEntry log = new LogEntry
                    {
                        BackupName = job.Name,
                        SourceFile = sourceFile,
                        TargetFile = targetFile,
                        FileSize = currentFileSize,
                        TransferTime = transferTime,
                        EncryptionTime = encryptionTime
                    };
                    Logger.Instance.WriteDailyLog(log);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to copy/encrypt {sourceFile}: {ex.Message}");
                }
            }

            // Set to INACTIVE only if the job was not interrupted
            if (currentState.State == "ACTIVE" || currentState.State == "INACTIVE")
            {
                currentState.State = "INACTIVE";
                currentState.RemainingFilesSize = 0;
                job.Progress = 100; // Safety check to ensure the bar finishes at 100%
                NotifyObservers(currentState);
                Console.WriteLine($"[INFO] Backup {job.Name} finished successfully.");
            }
        }
    }
}