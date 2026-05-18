using CommunityToolkit.Mvvm.Input;
using EasyLog;
using EasySave.Models;
using EasySave.Patterns.Bridge;
using EasySave.Services;
using EasySaveApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EasySaveApp.ViewModels
{
    public enum ValidationState { Info, Success, Error }

    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        // --- Services & Managers ---
        private readonly JobManager _jobManager;
        private readonly BackupEngine _backupEngine;
        private readonly ConfigManager _configManager;
        private readonly BusinessSoftwareMonitor _businessMonitor;
        private readonly LogReaderService _logReader;

        private int _validationMessageVersion;
        private string _runningJobName = string.Empty;

        // --- View Events ---
        public event EventHandler<string>? LanguageChangeRequested; // Requests the view to change the language

        // --- Collections ---
        public ObservableCollection<BackupJob> Jobs { get; set; }
        public ObservableCollection<SelectableBackupJob> SelectableJobs { get; set; }
        public ObservableCollection<string> ActivityMessages { get; set; }
        public ObservableCollection<string> BackupTypes { get; set; }
        public ObservableCollection<string> LogFormats { get; set; }
        public ObservableCollection<string> LogDestinations { get; set; }

        // --- Selected Job ---
        private BackupJob? _selectedJob;
        public BackupJob? SelectedJob
        {
            get => _selectedJob;
            set
            {
                SetProperty(ref _selectedJob, value);
                OnPropertyChanged(nameof(HasSelectedJob));
                (DeleteJobCommand as IRelayCommand)?.NotifyCanExecuteChanged();

                if (_selectedJob != null)
                {
                    JobName = _selectedJob.Name;
                    SourceDirectory = _selectedJob.SourceDirectory;
                    TargetDirectory = _selectedJob.TargetDirectory;
                    SelectedBackupType = _selectedJob.Type == BackupType.Full && BackupTypes.Count > 0 ? BackupTypes[0] : (BackupTypes.Count > 1 ? BackupTypes[1] : string.Empty);
                }
            }
        }

        // --- Form Properties ---
        private string _jobName = string.Empty;
        public string JobName { get => _jobName; set => SetProperty(ref _jobName, value); }

        private string _sourceDirectory = string.Empty;
        public string SourceDirectory { get => _sourceDirectory; set => SetProperty(ref _sourceDirectory, value); }

        private string _targetDirectory = string.Empty;
        public string TargetDirectory { get => _targetDirectory; set => SetProperty(ref _targetDirectory, value); }

        private string _selectedBackupType = string.Empty;
        public string SelectedBackupType { get => _selectedBackupType; set => SetProperty(ref _selectedBackupType, value); }

        // --- Settings Properties ---
        private string _selectedLogFormat = "Json";
        public string SelectedLogFormat { get => _selectedLogFormat; set => SetProperty(ref _selectedLogFormat, value); }

        private string _selectedLogDestination = "Local";
        public string SelectedLogDestination { get => _selectedLogDestination; set => SetProperty(ref _selectedLogDestination, value); }

        private string _businessSoftware = string.Empty;
        public string BusinessSoftware { get => _businessSoftware; set => SetProperty(ref _businessSoftware, value); }

        private string _extensionsToEncrypt = string.Empty;
        public string ExtensionsToEncrypt { get => _extensionsToEncrypt; set => SetProperty(ref _extensionsToEncrypt, value); }

        private string _cryptoKey = string.Empty;
        public string CryptoKey { get => _cryptoKey; set => SetProperty(ref _cryptoKey, value); }

        private string _priorityExtensions = string.Empty;
        public string PriorityExtensions { get => _priorityExtensions; set => SetProperty(ref _priorityExtensions, value); }

        private long _maxFileSize = 10000;
        public long MaxFileSize { get => _maxFileSize; set => SetProperty(ref _maxFileSize, value); }

        public string JobsCountText => $"{Jobs?.Count ?? 0}";

        // --- State Properties ---
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsReady));
                    (RunJobsCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                    (RunAllJobsCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                    (DeleteJobCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                    (PauseJobCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                    (ResumeJobCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                    (StopJobCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        // --- Validation & Feedback ---
        private string _validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => _validationMessage;
            set { SetProperty(ref _validationMessage, value); OnPropertyChanged(nameof(HasValidationMessage)); }
        }

        private ValidationState _currentValidationState = ValidationState.Info;

        public string ValidationMessageColor => _currentValidationState switch
        {
            ValidationState.Error => "#B94A48",
            ValidationState.Success => "#2F7D4A",
            _ => "#1E425A"
        };

        public string ValidationMessageBackground => _currentValidationState switch
        {
            ValidationState.Error => "#FBE9E7",
            ValidationState.Success => "#E7F4EA",
            _ => "#EAF3F8"
        };

        public bool HasValidationMessage => !string.IsNullOrWhiteSpace(ValidationMessage);

        private SelectableBackupJob? _selectedSelectableJob;
        public SelectableBackupJob? SelectedSelectableJob
        {
            get => _selectedSelectableJob;
            set { SetProperty(ref _selectedSelectableJob, value); SelectedJob = _selectedSelectableJob?.Job; }
        }

        // --- Run Log & Progress ---
        private string _runLogText = string.Empty;
        public string RunLogText { get => _runLogText; set => SetProperty(ref _runLogText, value); }

        private int _backupProgress;
        public int BackupProgress { get => _backupProgress; set => SetProperty(ref _backupProgress, value); }

        private string _logDate = DateTime.Now.ToString("yyyy-MM-dd");
        public string LogDate { get => _logDate; set => SetProperty(ref _logDate, value); }

        private string _loadedLogText = string.Empty;
        public string LoadedLogText { get => _loadedLogText; set => SetProperty(ref _loadedLogText, value); }

        public bool HasSelectedJob => SelectedJob != null;
        public bool IsReady => !IsBusy;

        // --- Commands ---
        public ICommand CreateJobCommand { get; }
        public ICommand UpdateJobCommand { get; }
        public ICommand RunJobsCommand { get; }
        public ICommand RunAllJobsCommand { get; }
        public ICommand DeleteJobCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand LoadLogsCommand { get; }
        public ICommand LoadTodayLogsCommand { get; }
        public ICommand SetEnglishCommand { get; }
        public ICommand SetFrenchCommand { get; }
        public ICommand PauseJobCommand { get; }
        public ICommand ResumeJobCommand { get; }
        public ICommand StopJobCommand { get; }

        public MainWindowViewModel()
        {
            _configManager = new ConfigManager();
            Logger.Instance.Format = _configManager.Config.LogFormat;

            _businessMonitor = new BusinessSoftwareMonitor();
            _businessMonitor.SetSoftwareName(_configManager.Config.BusinessSoftware);
            _businessMonitor.Start();

            _jobManager = new JobManager();

            // Dependency Injection for BackupEngine
            var fileSystem = new LocalFileSystem();
            var encryptionService = new EncryptionService(_configManager.Config);
            _backupEngine = new BackupEngine(_configManager.Config, _businessMonitor, fileSystem, encryptionService);

            // Data service initialization
            _logReader = new LogReaderService();

            Jobs = new ObservableCollection<BackupJob>(_jobManager.Jobs);
            SelectableJobs = new ObservableCollection<SelectableBackupJob>();
            foreach (BackupJob job in Jobs) SelectableJobs.Add(new SelectableBackupJob(job));

            ActivityMessages = new ObservableCollection<string>();

            BackupTypes = new ObservableCollection<string> { "Full", "Differential" };

            LogFormats = new ObservableCollection<string> { "Json", "Xml" };
            SelectedLogFormat = _configManager.Config.LogFormat.ToString();

            LogDestinations = new ObservableCollection<string> { "Local", "Centralized", "Both" };
            SelectedLogDestination = _configManager.Config.LogDestination.ToString();

            BusinessSoftware = _configManager.Config.BusinessSoftware;
            ExtensionsToEncrypt = string.Join("; ", _configManager.Config.ExtensionsToEncrypt);
            CryptoKey = _configManager.Config.CryptoKey;

            PriorityExtensions = string.Join("; ", _configManager.Config.PriorityExtensions);
            MaxFileSize = _configManager.Config.MaxFileSizeKbForSimultaneous;

            CreateJobCommand = new RelayCommand(CreateJob);
            UpdateJobCommand = new RelayCommand(UpdateJob);
            RunJobsCommand = new AsyncRelayCommand(RunCheckedJobs, () => IsReady);
            RunAllJobsCommand = new AsyncRelayCommand(RunAllJobs, () => IsReady);
            DeleteJobCommand = new RelayCommand(DeleteSelectedJob, () => IsReady && HasSelectedJob);
            PauseJobCommand = new RelayCommand(PauseJobs, () => IsBusy);
            ResumeJobCommand = new RelayCommand(ResumeJobs, () => IsBusy);
            StopJobCommand = new RelayCommand(StopJobs, () => IsBusy);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ClearFormCommand = new RelayCommand(ClearForm);
            LoadLogsCommand = new RelayCommand(LoadLogsForSelectedDate);
            LoadTodayLogsCommand = new RelayCommand(LoadTodayLogs);

            SetEnglishCommand = new RelayCommand(() => RequestLanguageChange("en-US", "Language changed to English."));
            SetFrenchCommand = new RelayCommand(() => RequestLanguageChange("fr-FR", "Langue changée en français."));

            AddActivity("EasySave GUI initialized.");
        }

        private void RequestLanguageChange(string lang, string message)
        {
            // The ViewModel triggers the event. The View handles the UI work.
            LanguageChangeRequested?.Invoke(this, lang);
            SetValidation(message);
        }

        public void UpdateBackupTypesDisplay(string fullText, string diffText)
        {
            bool wasFull = SelectedBackupType == BackupTypes.FirstOrDefault() || string.IsNullOrEmpty(SelectedBackupType);
            BackupTypes.Clear();
            BackupTypes.Add(fullText);
            BackupTypes.Add(diffText);
            SelectedBackupType = wasFull ? BackupTypes[0] : BackupTypes[1];
        }

        private void PauseJobs()
        {
            _backupEngine.PauseJob();
            AddActivity("Backup PAUSED.");
        }

        private void ResumeJobs()
        {
            _backupEngine.ResumeJob();
            AddActivity("Backup RESUMED.");
        }

        private void StopJobs()
        {
            _backupEngine.StopJob();
            AddActivity("Backup CANCELED by user.");
        }

        private void CreateJob()
        {
            if (!ValidateJobForm()) return;

            string name = JobName.Trim();
            string source = SourceDirectory.Trim().Trim('"');
            string target = TargetDirectory.Trim().Trim('"');
            BackupType type = (BackupTypes.Count > 0 && SelectedBackupType == BackupTypes[0]) ? BackupType.Full : BackupType.Differential;

            BackupJob newJob = new BackupJob(name, source, target, type);
            _jobManager.CreateJob(newJob);
            Jobs.Add(newJob);
            SelectableJobs.Add(new SelectableBackupJob(newJob));

            OnPropertyChanged(nameof(JobsCountText));
            SelectedJob = newJob;
            ClearForm();
            SetValidation($"Job '{newJob.Name}' created successfully.");
        }

        private void UpdateJob()
        {
            if (SelectedJob == null) { SetValidation("Error: select a job before updating it."); return; }
            if (!ValidateJobForm()) return;

            string updatedJobName = JobName.Trim();
            int index = Jobs.IndexOf(SelectedJob);

            if (index < 0) { SetValidation("Error: selected job was not found."); return; }

            BackupType type = (BackupTypes.Count > 0 && SelectedBackupType == BackupTypes[0]) ? BackupType.Full : BackupType.Differential;
            BackupJob updatedJob = new BackupJob(updatedJobName, SourceDirectory.Trim().Trim('"'), TargetDirectory.Trim().Trim('"'), type);

            _jobManager.UpdateJob(index, updatedJob);
            Jobs[index] = updatedJob;
            SelectableJobs[index] = new SelectableBackupJob(updatedJob);
            SelectedJob = updatedJob;

            SetValidation($"Job '{updatedJobName}' updated successfully.");
        }

        private bool ValidateJobForm()
        {
            if (string.IsNullOrWhiteSpace(JobName) || string.IsNullOrWhiteSpace(SourceDirectory) || string.IsNullOrWhiteSpace(TargetDirectory))
            {
                SetValidation("Error: name, source and target are required.");
                return false;
            }
            return true;
        }

        private async Task RunCheckedJobs()
        {
            var checkedJobs = SelectableJobs.Where(j => j.IsSelected).Select(j => j.Job).ToList();

            if (checkedJobs.Count == 0)
            {
                SetValidation("Error: select at least one checked job to run.");
                return;
            }

            await ExecuteJobsList(checkedJobs);
        }

        private async Task RunAllJobs()
        {
            if (Jobs.Count == 0) { SetValidation("Error: no backup jobs to run."); return; }
            await ExecuteJobsList(Jobs.ToList());
        }

        private async Task ExecuteJobsList(List<BackupJob> jobsToRun)
        {
            IsBusy = true;
            try
            {
                AddActivity($"Execution started for {jobsToRun.Count} job(s).");
                bool wasAborted = false;

                foreach (BackupJob job in jobsToRun)
                {
                    _runningJobName = job.Name;
                    RunLogText = string.Empty;
                    BackupProgress = 0;
                    AddActivity($"Starting '{job.Name}' ({job.Type}).");

                    Task runTask = Task.Run(() => _backupEngine.ExecuteJob(job));
                    await RefreshRunLogsWhileRunning(runTask);

                    if (job.State == JobState.Aborted)
                    {
                        BackupProgress = 0;
                        wasAborted = true;
                        break;
                    }

                    AddActivity($"Job '{job.Name}' executed.");
                    BackupProgress = 100;
                }

                SetValidation(wasAborted ? "Jobs execution aborted." : "Jobs execution finished.");
            }
            finally
            {
                _runningJobName = string.Empty;
                IsBusy = false;
            }
        }

        private async Task RefreshRunLogsWhileRunning(Task runTask)
        {
            while (!runTask.IsCompleted)
            {
                LoadRunLogsForDate();
                LoadCurrentProgress();
                await Task.Delay(100);
            }

            await runTask;
            LoadRunLogsForDate();
            LoadCurrentProgress();
        }

        private void LoadCurrentProgress()
        {
            string statePath = GetStatePath();
            int? progress = _logReader.GetJobProgress(statePath, _runningJobName, _configManager.Config.LogFormat);
            if (progress.HasValue) BackupProgress = progress.Value;
        }

        private void DeleteSelectedJob()
        {
            if (SelectedJob == null) { SetValidation("Error: select a job before deleting it."); return; }

            int index = Jobs.IndexOf(SelectedJob);
            if (index < 0) { SetValidation("Error: selected job was not found."); return; }

            string jobNameToDelete = SelectedJob.Name;

            _jobManager.DeleteJob(index);
            Jobs.RemoveAt(index);
            SelectableJobs.RemoveAt(index);

            OnPropertyChanged(nameof(JobsCountText));
            SelectedJob = null;
            ClearForm();
            SetValidation($"Job '{jobNameToDelete}' deleted successfully.");
        }

        private void SaveSettings()
        {
            _configManager.Config.LogFormat = SelectedLogFormat == "Xml" ? LogFormat.Xml : LogFormat.Json;
            Logger.Instance.Format = _configManager.Config.LogFormat;

            if (Enum.TryParse<LogDestination>(SelectedLogDestination, out var destination))
            {
                _configManager.Config.LogDestination = destination;
                Logger.Instance.Destination = destination;
            }

            _configManager.Config.BusinessSoftware = BusinessSoftware.Trim();
            _configManager.Config.CryptoKey = CryptoKey.Trim();

            _configManager.Config.ExtensionsToEncrypt = ParseExtensions(ExtensionsToEncrypt);
            _configManager.Config.PriorityExtensions = ParseExtensions(PriorityExtensions);
            _configManager.Config.MaxFileSizeKbForSimultaneous = MaxFileSize;

            _businessMonitor.SetSoftwareName(_configManager.Config.BusinessSoftware);
            _configManager.SaveConfig();
            SetValidation("Settings saved successfully.");
        }

        private List<string> ParseExtensions(string input)
        {
            List<string> extensions = new List<string>();
            string[] parts = input?.Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            foreach (string part in parts)
            {
                string extension = part.Trim();
                if (!extension.StartsWith('.')) extension = "." + extension;
                if (!extensions.Any(e => string.Equals(e, extension, StringComparison.OrdinalIgnoreCase))) extensions.Add(extension);
            }
            return extensions;
        }

        private void LoadTodayLogs() { LogDate = DateTime.Now.ToString("yyyy-MM-dd"); LoadLogsForSelectedDate(); }

        private void LoadLogsForSelectedDate()
        {
            if (string.IsNullOrWhiteSpace(LogDate)) { SetValidation("Error: enter a date with format yyyy-MM-dd."); return; }
            string date = LogDate.Trim();
            string logPath = GetLogPath(date);

            if (!File.Exists(logPath))
            {
                LoadedLogText = $"No log file found for {date}.";
                SetValidation($"No log file found for {date}.");
                return;
            }

            LoadedLogText = _logReader.ReadFileSafely(logPath);
            SetValidation($"Logs loaded for {date}.");
        }

        private void LoadRunLogsForDate()
        {
            string logPath = GetStatePath();
            RunLogText = File.Exists(logPath) ? _logReader.ReadFileSafely(logPath) : "No log entry yet.";
        }

        private string GetLogPath(string date) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", date + (SelectedLogFormat == "Xml" ? ".xml" : ".json"));
        private string GetStatePath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "state" + (SelectedLogFormat == "Xml" ? ".xml" : ".json"));

        private void ClearForm()
        {
            JobName = string.Empty;
            SourceDirectory = string.Empty;
            TargetDirectory = string.Empty;
            SelectedBackupType = BackupTypes.Count > 0 ? BackupTypes[0] : string.Empty;
            ValidationMessage = string.Empty;
        }

        private async void SetValidation(string message)
        {
            _validationMessageVersion++;
            int currentVersion = _validationMessageVersion;

            if (message.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
                _currentValidationState = ValidationState.Error;
            else if (message.Contains("success", StringComparison.OrdinalIgnoreCase) || message.Contains("executed", StringComparison.OrdinalIgnoreCase) || message.Contains("loaded", StringComparison.OrdinalIgnoreCase) || message.Contains("saved", StringComparison.OrdinalIgnoreCase) || message.Contains("finished", StringComparison.OrdinalIgnoreCase) || message.Contains("terminée", StringComparison.OrdinalIgnoreCase))
                _currentValidationState = ValidationState.Success;
            else
                _currentValidationState = ValidationState.Info;

            OnPropertyChanged(nameof(ValidationMessageColor));
            OnPropertyChanged(nameof(ValidationMessageBackground));

            ValidationMessage = message;
            AddActivity(message);

            await Task.Delay(10000);
            if (currentVersion == _validationMessageVersion) ValidationMessage = string.Empty;
        }

        private void AddActivity(string message)
        {
            ActivityMessages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
            while (ActivityMessages.Count > 40) ActivityMessages.RemoveAt(ActivityMessages.Count - 1);
        }

        public void Dispose()
        {
            if (_businessMonitor != null)
            {
                _businessMonitor.Stop();
            }
            GC.SuppressFinalize(this);
        }
    }

    public class SelectableBackupJob : ViewModelBase
    {
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

        public BackupJob Job { get; set; }
        public string Name => Job.Name;
        public string SourceDirectory => Job.SourceDirectory;
        public string TargetDirectory => Job.TargetDirectory;
        public BackupType Type => Job.Type;

        public SelectableBackupJob(BackupJob job) { Job = job; }
    }
}