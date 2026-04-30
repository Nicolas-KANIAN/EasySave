using CommunityToolkit.Mvvm.Input;
using EasyLog;
using EasySave.Models;
using EasySave.Services;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace EasySaveApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // --- Services & Managers ---
        private readonly JobManager _jobManager;
        private readonly BackupEngine _backupEngine;
        private readonly ConfigManager _configManager;

        private int _validationMessageVersion;
        private string _runningJobName = string.Empty;

        // --- Collections for UI Binding ---
        public ObservableCollection<BackupJob> Jobs { get; set; }
        public ObservableCollection<SelectableBackupJob> SelectableJobs { get; set; }
        public ObservableCollection<string> ActivityMessages { get; set; }
        public ObservableCollection<string> BackupTypes { get; set; }
        public ObservableCollection<string> LogFormats { get; set; }

        // --- Selected Job Property ---
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
                    SelectedBackupType = _selectedJob.Type == BackupType.Full ? FullText : DiffText;
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

        private string _selectedBackupType = "Full";
        public string SelectedBackupType { get => _selectedBackupType; set => SetProperty(ref _selectedBackupType, value); }

        // --- Settings Properties ---
        private string _selectedLogFormat = "Json";
        public string SelectedLogFormat { get => _selectedLogFormat; set => SetProperty(ref _selectedLogFormat, value); }

        private string _businessSoftware = string.Empty;
        public string BusinessSoftware { get => _businessSoftware; set => SetProperty(ref _businessSoftware, value); }

        private string _extensionsToEncrypt = string.Empty;
        public string ExtensionsToEncrypt { get => _extensionsToEncrypt; set => SetProperty(ref _extensionsToEncrypt, value); }

        private string _cryptoKey = string.Empty;
        public string CryptoKey { get => _cryptoKey; set => SetProperty(ref _cryptoKey, value); }

        // --- State Properties (IsBusy prevents spamming buttons) ---
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsReady));
                    // Update buttons state dynamically
                    (RunJobsCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                    (RunAllJobsCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                    (DeleteJobCommand as IRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        // --- Validation & Feedback Messages ---
        private string _validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                SetProperty(ref _validationMessage, value);
                OnPropertyChanged(nameof(HasValidationMessage));
            }
        }

        private SelectableBackupJob? _selectedSelectableJob;
        public SelectableBackupJob? SelectedSelectableJob
        {
            get => _selectedSelectableJob;
            set
            {
                SetProperty(ref _selectedSelectableJob, value);
                SelectedJob = _selectedSelectableJob?.Job;
            }
        }

        private string _validationMessageColor = "#1E425A";
        public string ValidationMessageColor { get => _validationMessageColor; set => SetProperty(ref _validationMessageColor, value); }

        private string _validationMessageBackground = "#EAF3F8";
        public string ValidationMessageBackground { get => _validationMessageBackground; set => SetProperty(ref _validationMessageBackground, value); }

        public bool HasValidationMessage => !string.IsNullOrWhiteSpace(ValidationMessage);

        // --- Run Log & Progress Properties ---
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

        private bool _isFrench;

        // --- Translations ---
        public string FullText => _isFrench ? "Complet" : "Full";
        public string DiffText => _isFrench ? "Différentiel" : "Differential";
        public string TabJobHeader => _isFrench ? "Tâches" : "Jobs";
        public string TabSettingsHeader => _isFrench ? "Paramètres" : "Settings";
        public string TabLogsHeader => _isFrench ? "Journaux" : "Logs";
        public string DateLabel => _isFrench ? "Date" : "Date";

        public string JobsTitle => _isFrench ? "Travaux de sauvegarde" : "Backup jobs";
        public string JobsSubtitle => _isFrench ? "Creer, modifier, supprimer et lancer les travaux." : "Create, update, delete and run jobs.";
        public string CreateJobTitle => _isFrench ? "Travail" : "Job";
        public string NameLabel => _isFrench ? "Nom" : "Name";
        public string SourceLabel => _isFrench ? "Repertoire source" : "Source directory";
        public string TargetLabel => _isFrench ? "Repertoire cible" : "Target directory";
        public string BackupTypeLabel => _isFrench ? "Type de sauvegarde" : "Backup type";
        public string SettingsTitle => _isFrench ? "Parametres" : "Settings";
        public string LogsTitle => "Logs";
        public string LogSettingsTitle => _isFrench ? "Logs" : "Logs";
        public string EncryptionSettingsTitle => _isFrench ? "Cryptage" : "Encryption";
        public string BusinessSoftwareSettingsTitle => _isFrench ? "Logiciel metier" : "Business software";
        public string LogFormatLabel => _isFrench ? "Format des logs" : "Log format";
        public string BusinessSoftwareLabel => _isFrench ? "Processus logiciel metier" : "Business software process";
        public string ExtensionsLabel => _isFrench ? "Extensions a chiffrer" : "Extensions to encrypt";
        public string CryptoKeyLabel => _isFrench ? "Cle CryptoSoft" : "Crypto key";
        public string ActivityTitle => _isFrench ? "Activite" : "Activity";
        public string RunLogsTitle => _isFrench ? "Logs en temps reel" : "Real-time logs";

        public string RunText => _isFrench ? "Lancer" : "Run";

        public string RunAllText => _isFrench ? "Tout lancer" : "Run all";
        public string DeleteSelectedText => _isFrench ? "Supprimer" : "Delete";
        public string CreateText => _isFrench ? "Creer" : "Create";
        public string UpdateText => _isFrench ? "Modifier" : "Update";
        public string ClearText => _isFrench ? "Effacer" : "Clear";
        public string SaveSettingsText => _isFrench ? "Enregistrer" : "Save";
        public string LoadLogsText => _isFrench ? "Ouvrir les logs" : "Open logs";
        public string LoadTodayLogsText => _isFrench ? "Logs du jour" : "Today logs";

        public MainWindowViewModel()
        {
            _configManager = new ConfigManager();
            Logger.Instance.Format = _configManager.Config.LogFormat;

            _jobManager = new JobManager();
            _backupEngine = new BackupEngine(_configManager.Config);

            Jobs = new ObservableCollection<BackupJob>(_jobManager.Jobs);
            SelectableJobs = new ObservableCollection<SelectableBackupJob>();
            foreach (BackupJob job in Jobs)
            {
                SelectableJobs.Add(new SelectableBackupJob(job));
            }

            ActivityMessages = new ObservableCollection<string>();

            BackupTypes = new ObservableCollection<string> { FullText, DiffText };
            SelectedBackupType = FullText;

            LogFormats = new ObservableCollection<string> { "Json", "Xml" };

            SelectedLogFormat = _configManager.Config.LogFormat.ToString();
            BusinessSoftware = _configManager.Config.BusinessSoftware;
            ExtensionsToEncrypt = string.Join("; ", _configManager.Config.ExtensionsToEncrypt);
            CryptoKey = _configManager.Config.CryptoKey;

            CreateJobCommand = new RelayCommand(CreateJob);
            UpdateJobCommand = new RelayCommand(UpdateJob);

            RunJobsCommand = new AsyncRelayCommand(RunCheckedJobs, () => IsReady);
            RunAllJobsCommand = new AsyncRelayCommand(RunAllJobs, () => IsReady);
            DeleteJobCommand = new RelayCommand(DeleteSelectedJob, () => IsReady && HasSelectedJob);

            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ClearFormCommand = new RelayCommand(ClearForm);
            LoadLogsCommand = new RelayCommand(LoadLogsForSelectedDate);
            LoadTodayLogsCommand = new RelayCommand(LoadTodayLogs);
            SetEnglishCommand = new RelayCommand(SetEnglish);
            SetFrenchCommand = new RelayCommand(SetFrench);

            AddActivity("EasySave GUI initialized.");
        }

        private void SetEnglish() { _isFrench = false; RefreshLanguage(); SetValidation("Language changed to English."); }
        private void SetFrench() { _isFrench = true; RefreshLanguage(); SetValidation("Langue changee en francais."); }

        private void RefreshLanguage()
        {
            OnPropertyChanged(nameof(FullText));
            OnPropertyChanged(nameof(DiffText));
            OnPropertyChanged(nameof(TabJobHeader));
            OnPropertyChanged(nameof(TabSettingsHeader));
            OnPropertyChanged(nameof(TabLogsHeader));
            OnPropertyChanged(nameof(DateLabel));
            OnPropertyChanged(nameof(JobsTitle));
            OnPropertyChanged(nameof(JobsSubtitle));
            OnPropertyChanged(nameof(CreateJobTitle));
            OnPropertyChanged(nameof(NameLabel));
            OnPropertyChanged(nameof(SourceLabel));
            OnPropertyChanged(nameof(TargetLabel));
            OnPropertyChanged(nameof(BackupTypeLabel));
            OnPropertyChanged(nameof(SettingsTitle));
            OnPropertyChanged(nameof(LogsTitle));
            OnPropertyChanged(nameof(LogSettingsTitle));
            OnPropertyChanged(nameof(EncryptionSettingsTitle));
            OnPropertyChanged(nameof(BusinessSoftwareSettingsTitle));
            OnPropertyChanged(nameof(LogFormatLabel));
            OnPropertyChanged(nameof(BusinessSoftwareLabel));
            OnPropertyChanged(nameof(ExtensionsLabel));
            OnPropertyChanged(nameof(CryptoKeyLabel));
            OnPropertyChanged(nameof(ActivityTitle));
            OnPropertyChanged(nameof(RunLogsTitle));
            OnPropertyChanged(nameof(RunText));
            OnPropertyChanged(nameof(RunAllText));
            OnPropertyChanged(nameof(DeleteSelectedText));
            OnPropertyChanged(nameof(CreateText));
            OnPropertyChanged(nameof(UpdateText));
            OnPropertyChanged(nameof(ClearText));
            OnPropertyChanged(nameof(SaveSettingsText));
            OnPropertyChanged(nameof(LoadLogsText));
            OnPropertyChanged(nameof(LoadTodayLogsText));

            bool isFull = SelectedBackupType == "Full" || SelectedBackupType == "Complet";
            BackupTypes.Clear();
            BackupTypes.Add(FullText);
            BackupTypes.Add(DiffText);
            SelectedBackupType = isFull ? FullText : DiffText;
        }

        private void CreateJob()
        {
            if (!ValidateJobForm()) return;

            string name = JobName.Trim();
            string source = SourceDirectory.Trim().Trim('"');
            string target = TargetDirectory.Trim().Trim('"');
            BackupType type = SelectedBackupType == FullText ? BackupType.Full : BackupType.Differential;

            BackupJob newJob = new BackupJob(name, source, target, type);

            _jobManager.CreateJob(newJob);
            Jobs.Add(newJob);
            SelectableJobs.Add(new SelectableBackupJob(newJob));

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

            BackupType type = SelectedBackupType == FullText ? BackupType.Full : BackupType.Differential;
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
            List<BackupJob> checkedJobs = new List<BackupJob>();

            foreach (SelectableBackupJob selectableJob in SelectableJobs)
            {
                if (selectableJob.IsSelected)
                {
                    checkedJobs.Add(selectableJob.Job);
                }
            }

            if (checkedJobs.Count == 0)
            {
                SetValidation("Error: select at least one checked job to run.");
                return;
            }

            IsBusy = true;
            try
            {
                AddActivity($"Sequential execution started for {checkedJobs.Count} job(s).");

                foreach (BackupJob job in checkedJobs)
                {
                    _runningJobName = job.Name;
                    RunLogText = string.Empty;
                    BackupProgress = 0;
                    AddActivity($"Starting '{job.Name}' ({job.Type}).");

                    Task runTask = Task.Run(() => _backupEngine.ExecuteJob(job));
                    await RefreshRunLogsWhileRunning(runTask);

                    AddActivity($"Job '{job.Name}' executed.");
                    BackupProgress = 100;
                }

                SetValidation("Selected jobs executed.");
            }
            finally
            {
                _runningJobName = string.Empty;
                IsBusy = false;
            }
        }

        private async Task RunAllJobs()
        {
            if (Jobs.Count == 0) { SetValidation("Error: no backup jobs to run."); return; }

            IsBusy = true;
            try
            {
                AddActivity("Sequential execution started for all jobs.");

                foreach (BackupJob job in Jobs)
                {
                    _runningJobName = job.Name;
                    RunLogText = string.Empty;
                    BackupProgress = 0;
                    AddActivity($"Starting '{job.Name}' ({job.Type}).");

                    Task runTask = Task.Run(() => _backupEngine.ExecuteJob(job));
                    await RefreshRunLogsWhileRunning(runTask);

                    AddActivity($"Job '{job.Name}' executed.");
                    BackupProgress = 100;
                }

                SetValidation("All jobs executed.");
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
                LoadRunLogsForDate(DateTime.Now.ToString("yyyy-MM-dd"));
                LoadCurrentProgress();

                if (BackupProgress < 95)
                    BackupProgress++;

                await Task.Delay(100);
            }

            await runTask;
            LoadRunLogsForDate(DateTime.Now.ToString("yyyy-MM-dd"));
            LoadCurrentProgress();
        }

        private void LoadCurrentProgress()
        {
            if (string.IsNullOrWhiteSpace(_runningJobName)) return;

            string statePath = GetStatePath();
            if (!File.Exists(statePath)) return;

            try
            {
                if (SelectedLogFormat == "Xml")
                {
                    // UTILISATION DE LA LECTURE SÉCURISÉE
                    string xml = ReadFileSafely(statePath);
                    string startTag = "<Progression>";
                    string endTag = "</Progression>";
                    int startIndex = xml.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
                    int endIndex = xml.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        if (!xml.Contains("<Name>" + _runningJobName + "</Name>", StringComparison.OrdinalIgnoreCase)) return;
                        if (!xml.Contains("<State>ACTIVE</State>", StringComparison.OrdinalIgnoreCase)) return;

                        startIndex += startTag.Length;
                        string value = xml.Substring(startIndex, endIndex - startIndex);
                        if (int.TryParse(value, out int progress)) BackupProgress = progress;
                    }
                    return;
                }

                // UTILISATION DE LA LECTURE SÉCURISÉE
                string json = ReadFileSafely(statePath);
                using JsonDocument document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind == JsonValueKind.Array &&
                    document.RootElement.GetArrayLength() > 0 &&
                    document.RootElement[0].TryGetProperty("Name", out JsonElement name) &&
                    document.RootElement[0].TryGetProperty("State", out JsonElement state) &&
                    document.RootElement[0].TryGetProperty("Progression", out JsonElement progression))
                {
                    string? stateJobName = name.GetString();
                    string? stateValue = state.GetString();

                    if (string.Equals(stateJobName, _runningJobName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(stateValue, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                    {
                        BackupProgress = progression.GetInt32();
                    }
                }
            }
            catch (Exception ex)
            {
                // En cas de conflit bref, on évite le crash et on prévient dans la console
                Console.WriteLine($"[WARNING] Failed to read state file: {ex.Message}");
            }
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

            SelectedJob = null;
            ClearForm();
            SetValidation($"Job '{jobNameToDelete}' deleted successfully.");
        }

        private void SaveSettings()
        {
            if (SelectedLogFormat == "Xml")
            {
                _configManager.Config.LogFormat = LogFormat.Xml;
                Logger.Instance.Format = LogFormat.Xml;
            }
            else
            {
                _configManager.Config.LogFormat = LogFormat.Json;
                Logger.Instance.Format = LogFormat.Json;
            }

            _configManager.Config.BusinessSoftware = BusinessSoftware.Trim();
            _configManager.Config.CryptoKey = CryptoKey.Trim();
            _configManager.Config.ExtensionsToEncrypt = GetExtensionsToEncrypt();

            _configManager.SaveConfig();

            SetValidation("Settings saved successfully.");
        }

        private List<string> GetExtensionsToEncrypt()
        {
            List<string> extensions = new List<string>();
            string[] parts = ExtensionsToEncrypt.Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string extension = part.Trim();
                if (!extension.StartsWith('.')) extension = "." + extension;

                bool exists = false;
                foreach (string existing in extensions)
                {
                    if (string.Equals(existing, extension, StringComparison.OrdinalIgnoreCase)) exists = true;
                }

                if (!exists) extensions.Add(extension);
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

            try
            {
                LoadedLogText = ReadFileSafely(logPath);
                SetValidation($"Logs loaded for {date}.");
            }
            catch (Exception ex)
            {
                LoadedLogText = $"[Error] Cannot load logs right now: {ex.Message}";
            }
        }

        private void LoadRunLogsForDate(string date)
        {
            string logPath = GetStatePath();
            if (!File.Exists(logPath)) { RunLogText = "No log entry yet."; return; }

            try
            {
                RunLogText = ReadFileSafely(logPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Could not read realtime logs: {ex.Message}");
            }
        }
        private string ReadFileSafely(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                return streamReader.ReadToEnd();
            }
        }
        private string GetLogPath(string date) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", date + (SelectedLogFormat == "Xml" ? ".xml" : ".json"));
        private string GetStatePath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "state" + (SelectedLogFormat == "Xml" ? ".xml" : ".json"));

        private void ClearForm()
        {
            JobName = string.Empty;
            SourceDirectory = string.Empty;
            TargetDirectory = string.Empty;
            SelectedBackupType = FullText;
            ValidationMessage = string.Empty;
        }

        private async void SetValidation(string message)
        {
            _validationMessageVersion++;
            int currentVersion = _validationMessageVersion;

            if (message.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
            {
                ValidationMessageColor = "#B94A48";
                ValidationMessageBackground = "#FBE9E7";
            }
            else if (message.Contains("success", StringComparison.OrdinalIgnoreCase) ||
                     message.Contains("executed", StringComparison.OrdinalIgnoreCase) ||
                     message.Contains("loaded", StringComparison.OrdinalIgnoreCase) ||
                     message.Contains("saved", StringComparison.OrdinalIgnoreCase))
            {
                ValidationMessageColor = "#2F7D4A";
                ValidationMessageBackground = "#E7F4EA";
            }
            else
            {
                ValidationMessageColor = "#1E425A";
                ValidationMessageBackground = "#EAF3F8";
            }

            ValidationMessage = message;
            AddActivity(message);

            // Hide message after 10 seconds if a new message hasn't been triggered
            await Task.Delay(10000);
            if (currentVersion == _validationMessageVersion) ValidationMessage = string.Empty;
        }

        private void AddActivity(string message)
        {
            ActivityMessages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
            while (ActivityMessages.Count > 40) ActivityMessages.RemoveAt(ActivityMessages.Count - 1);
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