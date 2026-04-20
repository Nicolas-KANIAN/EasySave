using System.Diagnostics;
using EasyLog;
using EasySave.Models;
using EasySave.Patterns.Bridge;
using EasySave.Patterns.Factory;
using EasySave.Patterns.Observer;
using EasySave.Patterns.Strategy;

namespace EasySave.Services
{
    public class BackupEngine
    {
        private readonly IFileSystem _fileSystem;
        private readonly List<IBackupObserver> _observers;

        public BackupEngine()
        {
            _fileSystem = new LocalFileSystem();
            _observers = new List<IBackupObserver>();

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

        public void ExecuteJob(BackupJob job)
        {
            Console.WriteLine($"\n[INFO] Starting backup job: {job.Name} ({job.Type})");

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

                    _fileSystem.CopyFile(sourceFile, targetFile, true);
                    sw.Stop();
                    filesCopied++;

                    currentState.NbFilesLeftToDo = totalFiles - filesCopied;
                    currentState.RemainingFilesSize -= currentFileSize;
                    currentState.Progression = (int)((double)filesCopied / totalFiles * 100);

                    NotifyObservers(currentState);

                    LogEntry log = new LogEntry
                    {
                        BackupName = job.Name,
                        SourceFile = sourceFile,
                        TargetFile = targetFile,
                        FileSize = currentFileSize,
                        TransferTime = sw.ElapsedMilliseconds
                    };
                    Logger.Instance.WriteDailyLog(log);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to copy {sourceFile}: {ex.Message}");
                }
            }

            currentState.State = "INACTIVE";
            currentState.RemainingFilesSize = 0;
            NotifyObservers(currentState);
            Console.WriteLine($"[INFO] Backup {job.Name} finished successfully.");
        }
    }
}