using CommunityToolkit.Mvvm.ComponentModel;

namespace EasySave.Models
{
    // Represents a backup job configuration containing its name, source, target, and type.
    // Inherits from ObservableObject to notify the UI of changes in real-time.
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

        // The property that feeds the progress bar
        private double _progress;
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        // The property that shows/hides the progress bar
        private bool _showProgress;
        public bool ShowProgress
        {
            get => _showProgress;
            set => SetProperty(ref _showProgress, value);
        }

        // Empty constructor (often required for JSON/XML deserialization)
        public BackupJob()
        {
        }

        public BackupJob(string name, string sourceDirectory, string targetDirectory, BackupType type)
        {
            _name = name;
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _type = type;
            _progress = 0;
            _showProgress = false;
        }
    }
}