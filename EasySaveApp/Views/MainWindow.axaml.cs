using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EasySaveApp.ViewModels;

namespace EasySaveApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handler for the "Browse" button next to the Source directory input
        private async void OnBrowseSourceClick(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            // On vérifie le DataContext avant pour récupérer la bonne langue
            if (DataContext is MainWindowViewModel viewModel)
            {
                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = viewModel.SourceLabel, // Utilise la traduction du ViewModel
                    AllowMultiple = false
                });

                if (folders.Count >= 1)
                {
                    viewModel.SourceDirectory = folders[0].Path.LocalPath;
                }
            }
        }

        // Event handler for the "Browse" button next to the Target directory input
        private async void OnBrowseTargetClick(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            // On vérifie le DataContext avant pour récupérer la bonne langue
            if (DataContext is MainWindowViewModel viewModel)
            {
                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = viewModel.TargetLabel, // Utilise la traduction du ViewModel
                    AllowMultiple = false
                });

                if (folders.Count >= 1)
                {
                    viewModel.TargetDirectory = folders[0].Path.LocalPath;
                }
            }
        }
    }
}