using EasyLog;
using EasySave.Models;
using EasySave.Patterns.Bridge;
using EasySave.Patterns.Factory;
using EasySave.Patterns.Observer;
using EasySave.Patterns.Strategy;
using System.Diagnostics;

namespace EasySave.Services
{
    public class BackupEngine
    {
        private readonly IFileSystem _fileSystem;
        private readonly List<IBackupObserver> _observers;
        private readonly AppConfig _config;
        private readonly EncryptionService _encryptionService;
        private readonly BusinessSoftwareMonitor _businessMonitor;

        private static readonly SemaphoreSlim _largeFileLock = new SemaphoreSlim(1, 1);
        private readonly ManualResetEvent _pauseEvent = new ManualResetEvent(true);
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly object _stateLock = new object();

        private bool _isUserPaused = false;
        private bool _isMonitorPaused = false;
        private readonly object _pauseLock = new object();

        private BackupJob? _activeJob;

        public BackupEngine(AppConfig config, BusinessSoftwareMonitor monitor, IFileSystem fileSystem, EncryptionService encryptionService)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(monitor);
            ArgumentNullException.ThrowIfNull(fileSystem);
            ArgumentNullException.ThrowIfNull(encryptionService);

            _fileSystem = fileSystem;
            _observers = new List<IBackupObserver>();
            _config = config;
            _encryptionService = encryptionService;
            _businessMonitor = monitor;

            _businessMonitor.SoftwareStarted += (s, e) =>
            {
                lock (_pauseLock)
                {
                    _isMonitorPaused = true;
                    UpdatePauseState();
                }

                Logger.Instance.WriteDailyLog(new LogEntry
                {
                    BackupName = _activeJob != null ? _activeJob.Name : "EasySave System",
                    SourceFile = "Business software detected",
                    TargetFile = "Job PAUSED",
                    FileSize = 0,
                    TransferTime = -1,
                    EncryptionTime = 0
                });
            };

            _businessMonitor.SoftwareStopped += (s, e) =>
            {
                lock (_pauseLock)
                {
                    _isMonitorPaused = false;
                    UpdatePauseState();
                }

                Logger.Instance.WriteDailyLog(new LogEntry
                {
                    BackupName = _activeJob != null ? _activeJob.Name : "EasySave System",
                    SourceFile = "Business software closed",
                    TargetFile = "Job RESUMED",
                    FileSize = 0,
                    TransferTime = 0,
                    EncryptionTime = 0
                });
            };

            AttachObserver(new StateLoggerObserver());
        }

        public void AttachObserver(IBackupObserver observer) => _observers.Add(observer);

        private void NotifyObservers(StateEntry state)
        {
            lock (_stateLock)
            {
                foreach (var observer in _observers) observer.Update(state);
            }
        }

        public void PauseJob()
        {
            lock (_pauseLock)
            {
                _isUserPaused = true;
                UpdatePauseState();
            }
        }

        public void ResumeJob()
        {
            lock (_pauseLock)
            {
                _isUserPaused = false;
                UpdatePauseState();
            }
        }

        private void UpdatePauseState()
        {
            if (_isUserPaused || _isMonitorPaused)
            {
                _pauseEvent.Reset();
                if (_activeJob != null) _activeJob.State = JobState.Paused;
            }
            else
            {
                _pauseEvent.Set();
                if (_activeJob != null) _activeJob.State = JobState.Active;
            }
        }

        public void StopJob()
        {
            _cts.Cancel();
            _pauseEvent.Set();
        }

        public void ExecuteJob(BackupJob job)
        {
            try
            {
                _activeJob = job;
                _cts = new CancellationTokenSource();

                job.Progress = 0;
                job.ShowProgress = true;

                lock (_pauseLock)
                {
                    _isUserPaused = false;
                    _isMonitorPaused = _businessMonitor.IsRunning;
                    UpdatePauseState();
                }

                if (!_fileSystem.DirectoryExists(job.SourceDirectory)) return;
                if (!_fileSystem.DirectoryExists(job.TargetDirectory)) _fileSystem.CreateDirectory(job.TargetDirectory);

                var allFiles = _fileSystem.GetFilesRecursive(job.SourceDirectory);
                IBackupStrategy strategy = BackupFactory.CreateStrategy(job.Type);

                var filesToCopy = strategy.GetFilesToCopy(job.SourceDirectory, job.TargetDirectory, allFiles, _fileSystem).ToList();

                List<string> priorityFiles = new List<string>();
                List<string> normalFiles = new List<string>();

                if (_config.PriorityExtensions?.Any() == true)
                {
                    foreach (var file in filesToCopy)
                    {
                        if (_config.PriorityExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                            priorityFiles.Add(file);
                        else
                            normalFiles.Add(file);
                    }
                }
                else
                {
                    normalFiles = filesToCopy;
                }

                int totalFiles = priorityFiles.Count + normalFiles.Count;
                long totalSize = filesToCopy.Sum(f => _fileSystem.GetFileSize(f));

                if (totalFiles == 0)
                {
                    job.Progress = 100;
                    job.State = JobState.Completed;
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

                try
                {
                    var options = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount,
                        CancellationToken = _cts.Token
                    };

                    Action<string> processFile = (sourceFile) =>
                    {
                        try
                        {
                            _pauseEvent.WaitOne();
                            options.CancellationToken.ThrowIfCancellationRequested();

                            long currentFileSize = _fileSystem.GetFileSize(sourceFile);
                            bool isLargeFile = currentFileSize > _config.MaxFileSizeKbForSimultaneous * 1024L;

                            if (isLargeFile)
                            {
                                _largeFileLock.Wait(options.CancellationToken);
                            }

                            try
                            {
                                _pauseEvent.WaitOne();
                                options.CancellationToken.ThrowIfCancellationRequested();

                                string relativePath = Path.GetRelativePath(job.SourceDirectory, sourceFile);
                                string targetFile = Path.Combine(job.TargetDirectory, relativePath);
                                string targetDir = Path.GetDirectoryName(targetFile) ?? string.Empty;

                                lock (_stateLock)
                                {
                                    if (!_fileSystem.DirectoryExists(targetDir)) _fileSystem.CreateDirectory(targetDir);
                                }

                                Stopwatch sw = Stopwatch.StartNew();
                                _fileSystem.CopyFile(sourceFile, targetFile, true);
                                sw.Stop();

                                long encryptionTime = 0;
                                if (_config.ExtensionsToEncrypt.Contains(Path.GetExtension(targetFile), StringComparer.OrdinalIgnoreCase))
                                {
                                    encryptionTime = _encryptionService.Encrypt(targetFile, _config.CryptoKey);
                                }

                                lock (_stateLock)
                                {
                                    filesCopied++;
                                    currentState.NbFilesLeftToDo = totalFiles - filesCopied;
                                    currentState.RemainingFilesSize -= currentFileSize;
                                    currentState.Progression = (int)((double)filesCopied / totalFiles * 100);
                                    currentState.CurrentSourceFile = sourceFile;
                                    currentState.CurrentTargetFile = targetFile;

                                    job.Progress = currentState.Progression;
                                    NotifyObservers(currentState);
                                }

                                Logger.Instance.WriteDailyLog(new LogEntry
                                {
                                    BackupName = job.Name,
                                    SourceFile = sourceFile,
                                    TargetFile = targetFile,
                                    FileSize = currentFileSize,
                                    TransferTime = sw.ElapsedMilliseconds,
                                    EncryptionTime = encryptionTime
                                });
                            }
                            finally
                            {
                                if (isLargeFile) _largeFileLock.Release();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                    };

                    if (priorityFiles.Count > 0)
                    {
                        Parallel.ForEach(priorityFiles, options, processFile);
                    }

                    if (normalFiles.Count > 0)
                    {
                        Parallel.ForEach(normalFiles, options, processFile);
                    }

                    if (_cts.IsCancellationRequested)
                    {
                        job.Progress = 0;
                        job.State = JobState.Aborted;
                        currentState.Progression = 0;
                        currentState.State = "ABORTED";
                        NotifyObservers(currentState);
                    }
                    else
                    {
                        job.State = JobState.Completed;
                        currentState.State = "COMPLETED";
                        NotifyObservers(currentState);
                    }
                }
                catch (OperationCanceledException)
                {
                    job.Progress = 0;
                    job.State = JobState.Aborted;
                    currentState.Progression = 0;
                    currentState.State = "ABORTED";
                    NotifyObservers(currentState);
                }
            }
            finally
            {
                _activeJob = null;
                _pauseEvent.Set();
            }
        }
    }
}