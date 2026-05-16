using Avalonia;
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

            // Dynamically fetch the translated title from the active Resource Dictionary
            string dialogTitle = Application.Current?.TryGetResource("SourceLabel", null, out var resource) == true
                ? resource?.ToString() ?? "Select Source Directory"
                : "Select Source Directory";

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = dialogTitle,
                AllowMultiple = false
            });

            if (folders.Count >= 1 && DataContext is MainWindowViewModel viewModel)
            {
                viewModel.SourceDirectory = folders[0].Path.LocalPath;
            }
        }

        // Event handler for the "Browse" button next to the Target directory input
        private async void OnBrowseTargetClick(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            // Dynamically fetch the translated title from the active Resource Dictionary
            string dialogTitle = Application.Current?.TryGetResource("TargetLabel", null, out var resource) == true
                ? resource?.ToString() ?? "Select Target Directory"
                : "Select Target Directory";

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = dialogTitle,
                AllowMultiple = false
            });

            if (folders.Count >= 1 && DataContext is MainWindowViewModel viewModel)
            {
                viewModel.TargetDirectory = folders[0].Path.LocalPath;
            }
        }
    }
}