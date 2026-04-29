using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasyLog;
using EasySave.Models;
using EasySave.Services;

namespace EasySaveApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly JobManager _jobManager;
        private readonly BackupEngine _backupEngine;
        private readonly ConfigManager _configManager;

        public ObservableCollection<BackupJob> Jobs { get; set; }
        public ObservableCollection<string> ActivityMessages { get; set; }
        public ObservableCollection<string> BackupTypes { get; set; }
        public ObservableCollection<string> LogFormats { get; set; }

        private BackupJob? _selectedJob;
        public BackupJob? SelectedJob
        {
            get => _selectedJob;
            set
            {
                SetProperty(ref _selectedJob, value);
                OnPropertyChanged(nameof(HasSelectedJob));
            }
        }

        private string _jobName = string.Empty;
        public string JobName
        {
            get => _jobName;
            set => SetProperty(ref _jobName, value);
        }

        private string _sourceDirectory = string.Empty;
        public string SourceDirectory
        {
            get => _sourceDirectory;
            set => SetProperty(ref _sourceDirectory, value);
        }

        private string _targetDirectory = string.Empty;
        public string TargetDirectory
        {
            get => _targetDirectory;
            set => SetProperty(ref _targetDirectory, value);
        }

        private string _selectedBackupType = "Full";
        public string SelectedBackupType
        {
            get => _selectedBackupType;
            set => SetProperty(ref _selectedBackupType, value);
        }

        private string _selectedLogFormat = "Json";
        public string SelectedLogFormat
        {
            get => _selectedLogFormat;
            set => SetProperty(ref _selectedLogFormat, value);
        }

        private string _businessSoftware = string.Empty;
        public string BusinessSoftware
        {
            get => _businessSoftware;
            set => SetProperty(ref _businessSoftware, value);
        }

        private string _extensionsToEncrypt = string.Empty;
        public string ExtensionsToEncrypt
        {
            get => _extensionsToEncrypt;
            set => SetProperty(ref _extensionsToEncrypt, value);
        }

        private string _cryptoKey = string.Empty;
        public string CryptoKey
        {
            get => _cryptoKey;
            set => SetProperty(ref _cryptoKey, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                OnPropertyChanged(nameof(IsReady));
            }
        }

        private string _statusMessage = "Ready.";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool HasSelectedJob
        {
            get => SelectedJob != null;
        }

        public bool IsReady
        {
            get => !IsBusy;
        }

        public ICommand CreateJobCommand { get; }
        public ICommand RunJobCommand { get; }
        public ICommand RunAllJobsCommand { get; }
        public ICommand DeleteJobCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand ClearFormCommand { get; }

        public MainWindowViewModel()
        {
            _configManager = new ConfigManager();
            Logger.Instance.Format = _configManager.Config.LogFormat;

            _jobManager = new JobManager();
            _backupEngine = new BackupEngine(_configManager.Config);

            Jobs = new ObservableCollection<BackupJob>(_jobManager.Jobs);
            ActivityMessages = new ObservableCollection<string>();
            BackupTypes = new ObservableCollection<string> { "Full", "Differential" };
            LogFormats = new ObservableCollection<string> { "Json", "Xml" };

            SelectedLogFormat = _configManager.Config.LogFormat.ToString();
            BusinessSoftware = _configManager.Config.BusinessSoftware;
            ExtensionsToEncrypt = string.Join("; ", _configManager.Config.ExtensionsToEncrypt);
            CryptoKey = _configManager.Config.CryptoKey;

            CreateJobCommand = new RelayCommand(CreateJob);
            RunJobCommand = new AsyncRelayCommand(RunSelectedJob);
            RunAllJobsCommand = new AsyncRelayCommand(RunAllJobs);
            DeleteJobCommand = new RelayCommand(DeleteSelectedJob);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ClearFormCommand = new RelayCommand(ClearForm);

            AddActivity("EasySave GUI initialized.");
        }

        private void CreateJob()
        {
            if (string.IsNullOrWhiteSpace(JobName) ||
                string.IsNullOrWhiteSpace(SourceDirectory) ||
                string.IsNullOrWhiteSpace(TargetDirectory))
            {
                SetStatus("Name, source and target are required.");
                return;
            }

            string name = JobName.Trim();
            string source = SourceDirectory.Trim().Trim('"');
            string target = TargetDirectory.Trim().Trim('"');
            BackupType type = SelectedBackupType == "Full" ? BackupType.Full : BackupType.Differential;

            BackupJob newJob = new BackupJob(name, source, target, type);

            _jobManager.CreateJob(newJob);
            Jobs.Add(newJob);

            SelectedJob = newJob;
            ClearForm();
            SetStatus($"Job '{newJob.Name}' created.");
        }

        private async Task RunSelectedJob()
        {
            if (SelectedJob == null)
            {
                SetStatus("Select a job before starting a backup.");
                return;
            }

            IsBusy = true;
            try
            {
                BackupJob job = SelectedJob;
                AddActivity($"Starting '{job.Name}' ({job.Type}).");
                await Task.Run(() => _backupEngine.ExecuteJob(job));
                SetStatus($"Job '{job.Name}' executed.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RunAllJobs()
        {
            if (Jobs.Count == 0)
            {
                SetStatus("No backup jobs to run.");
                return;
            }

            IsBusy = true;
            try
            {
                AddActivity("Sequential execution started for all jobs.");

                foreach (BackupJob job in Jobs)
                {
                    AddActivity($"Starting '{job.Name}' ({job.Type}).");
                    await Task.Run(() => _backupEngine.ExecuteJob(job));
                    AddActivity($"Job '{job.Name}' executed.");
                }

                SetStatus("All jobs executed.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void DeleteSelectedJob()
        {
            if (SelectedJob == null)
            {
                SetStatus("Select a job before deleting it.");
                return;
            }

            int index = Jobs.IndexOf(SelectedJob);
            if (index < 0)
            {
                SetStatus("Selected job was not found.");
                return;
            }

            string jobNameToDelete = SelectedJob.Name;

            _jobManager.DeleteJob(index);
            Jobs.RemoveAt(index);

            SelectedJob = null;
            SetStatus($"Job '{jobNameToDelete}' deleted.");
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

            SetStatus("Settings saved.");
        }

        private List<string> GetExtensionsToEncrypt()
        {
            List<string> extensions = new List<string>();
            string[] parts = ExtensionsToEncrypt.Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string extension = part.Trim();

                if (!extension.StartsWith('.'))
                {
                    extension = "." + extension;
                }

                if (!extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    extensions.Add(extension);
                }
            }

            return extensions;
        }

        private void ClearForm()
        {
            JobName = string.Empty;
            SourceDirectory = string.Empty;
            TargetDirectory = string.Empty;
            SelectedBackupType = "Full";
        }

        private void SetStatus(string message)
        {
            StatusMessage = message;
            AddActivity(message);
        }

        private void AddActivity(string message)
        {
            ActivityMessages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");

            while (ActivityMessages.Count > 40)
            {
                ActivityMessages.RemoveAt(ActivityMessages.Count - 1);
            }
        }
    }
}
