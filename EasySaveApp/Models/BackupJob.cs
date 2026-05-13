using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace EasySave.Models
{
    public enum JobState
    {
        Inactive,
        Active,
        Paused,
        Aborted,
        Completed
    }

    public class BackupJob : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _sourceDirectory;
        public string SourceDirectory
        {
            get => _sourceDirectory;
            set => SetProperty(ref _sourceDirectory, value);
        }

        private string _targetDirectory;
        public string TargetDirectory
        {
            get => _targetDirectory;
            set => SetProperty(ref _targetDirectory, value);
        }

        private BackupType _type;
        public BackupType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private double _progress;
        [JsonIgnore]
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private bool _showProgress;
        [JsonIgnore]
        public bool ShowProgress
        {
            get => _showProgress;
            set => SetProperty(ref _showProgress, value);
        }

        private JobState _state;
        [JsonIgnore]
        public JobState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public BackupJob()
        {
            _state = JobState.Inactive;
        }

        public BackupJob(string name, string sourceDirectory, string targetDirectory, BackupType type)
        {
            _name = name;
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _type = type;
            _progress = 0;
            _showProgress = false;
            _state = JobState.Inactive;
        }
    }
}