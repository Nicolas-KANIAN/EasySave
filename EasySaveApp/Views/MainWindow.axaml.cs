using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform.Storage;
using EasySaveApp.ViewModels;
using System;

namespace EasySaveApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (DataContext is MainWindowViewModel vm)
            {
                // Subscribe to the language event (unsubscribe first for safety)
                vm.LanguageChangeRequested -= OnLanguageChangeRequested;
                vm.LanguageChangeRequested += OnLanguageChangeRequested;

                OnLanguageChangeRequested(this, "en-US");
            }
        }

        // Handles the actual update of the graphical interface (Avalonia Dictionaries)
        private void OnLanguageChangeRequested(object? sender, string lang)
        {
            try
            {
                var uri = new Uri($"avares://EasySaveApp/Assets/{lang}.axaml");
                var translations = new ResourceInclude(uri) { Source = uri };

                if (Application.Current!.Resources.MergedDictionaries.Count > 0)
                {
                    Application.Current.Resources.MergedDictionaries[0] = translations;
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(translations);
                }

                // Fetch the translations to update the ComboBox in the ViewModel
                string fullType = Application.Current.TryGetResource("FullText", null, out var f) ? f?.ToString() ?? "Full" : "Full";
                string diffType = Application.Current.TryGetResource("DiffText", null, out var d) ? d?.ToString() ?? "Differential" : "Differential";

                if (DataContext is MainWindowViewModel vm)
                {
                    vm.UpdateBackupTypesDisplay(fullType, diffType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Failed to load language dictionary '{lang}': {ex.Message}");
            }
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