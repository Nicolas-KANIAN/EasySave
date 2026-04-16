using EasyLog;
using EasySave.Models;
using System.Diagnostics;

namespace EasySave.Services
{
    public class BackupEngine
    {
        private Logger _logger;

        public BackupEngine()
        {
            _logger = new Logger();
        }

        public void ExecuteJob(BackupJob job)
        {
            Console.WriteLine($"\n[INFO] Starting backup job: {job.Name} ({job.Type})");

            if (!Directory.Exists(job.SourceDirectory))
            {
                Console.WriteLine($"[ERROR] Source directory does not exist: {job.SourceDirectory}");
                return;
            }

            if (!Directory.Exists(job.TargetDirectory))
            {
                Directory.CreateDirectory(job.TargetDirectory);
            }

            var allFiles = GetFilesRecursive(job.SourceDirectory);
            var filesToCopy = new List<string>();

            foreach (var file in allFiles)
            {
                string relativePath = file.Substring(job.SourceDirectory.Length + 1);
                string targetFile = Path.Combine(job.TargetDirectory, relativePath);

                if (job.Type == BackupType.Full)
                {
                    filesToCopy.Add(file);
                }
                else if (job.Type == BackupType.Differential)
                {
                    if (!File.Exists(targetFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(targetFile))
                    {
                        filesToCopy.Add(file);
                    }
                }
            }

            int totalFiles = filesToCopy.Count;
            long totalSize = filesToCopy.Sum(f => new FileInfo(f).Length);

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
                Progression = 0
            };

            foreach (var sourceFile in filesToCopy)
            {
                string relativePath = sourceFile.Substring(job.SourceDirectory.Length + 1);
                string targetFile = Path.Combine(job.TargetDirectory, relativePath);
                string targetDir = Path.GetDirectoryName(targetFile);

                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                FileInfo fileInfo = new FileInfo(sourceFile);
                currentState.CurrentSourceFile = sourceFile;
                currentState.CurrentTargetFile = targetFile;

                Stopwatch sw = Stopwatch.StartNew();

                try
                {
                    File.Copy(sourceFile, targetFile, true);
                    sw.Stop();
                    filesCopied++;

                    currentState.NbFilesLeftToDo = totalFiles - filesCopied;
                    currentState.Progression = (int)((double)filesCopied / totalFiles * 100);
                    _logger.UpdateState(new List<StateEntry> { currentState });

                    LogEntry log = new LogEntry
                    {
                        BackupName = job.Name,
                        SourceFile = sourceFile,
                        TargetFile = targetFile,
                        FileSize = fileInfo.Length,
                        TransferTime = sw.ElapsedMilliseconds
                    };
                    _logger.WriteDailyLog(log);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to copy {sourceFile}: {ex.Message}");
                    LogEntry errorLog = new LogEntry
                    {
                        BackupName = job.Name,
                        SourceFile = sourceFile,
                        TargetFile = targetFile,
                        FileSize = fileInfo.Length,
                        TransferTime = -1
                    };
                    _logger.WriteDailyLog(errorLog);
                }
            }

            currentState.State = "INACTIVE";
            _logger.UpdateState(new List<StateEntry> { currentState });
            Console.WriteLine($"[INFO] Backup {job.Name} finished successfully.");
        }

        private List<string> GetFilesRecursive(string directory)
        {
            var files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(directory));
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    files.AddRange(GetFilesRecursive(dir));
                }
            }
            catch (UnauthorizedAccessException) { }
            return files;
        }
    }
}