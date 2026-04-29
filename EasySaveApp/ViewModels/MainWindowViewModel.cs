using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        private BackupJob? _selectedJob;
        public BackupJob? SelectedJob
        {
            get => _selectedJob;
            set => SetProperty(ref _selectedJob, value);
        }

        public ICommand RunJobCommand { get; }

        public MainWindowViewModel()
        {
            _configManager = new ConfigManager();
            EasyLog.Logger.Instance.Format = _configManager.Config.LogFormat;
            _jobManager = new JobManager();
            _backupEngine = new BackupEngine(_configManager.Config);

            Jobs = new ObservableCollection<BackupJob>(_jobManager.Jobs);

            RunJobCommand = new RelayCommand(RunSelectedJob);
        }

        private void RunSelectedJob()
        {
            if (SelectedJob != null)
            {
                _backupEngine.ExecuteJob(SelectedJob);
            }
        }
    }
}