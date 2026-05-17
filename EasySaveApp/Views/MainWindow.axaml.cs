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
        private MainWindowViewModel? _currentViewModel;

        // Stores the reference to the currently active language dictionary
        private ResourceInclude? _currentLanguageDictionary;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            // 1. Unsubscribe from the old ViewModel to prevent memory leaks
            if (_currentViewModel != null)
            {
                _currentViewModel.LanguageChangeRequested -= OnLanguageChangeRequested;
            }

            base.OnDataContextChanged(e);

            // 2. Subscribe to the new ViewModel
            if (DataContext is MainWindowViewModel newVm)
            {
                newVm.LanguageChangeRequested += OnLanguageChangeRequested;
                _currentViewModel = newVm;

                OnLanguageChangeRequested(this, "en-US");
            }
            else
            {
                _currentViewModel = null;
            }
        }

        // 3. Clean up when the window is closed
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_currentViewModel != null)
            {
                _currentViewModel.LanguageChangeRequested -= OnLanguageChangeRequested;
                _currentViewModel = null;
            }
        }

        // Handles the actual update of the graphical interface (Avalonia Dictionaries)
        private void OnLanguageChangeRequested(object? sender, string lang)
        {
            try
            {
                var uri = new Uri($"avares://EasySaveApp/Assets/{lang}.axaml");
                var translations = new ResourceInclude(uri) { Source = uri };

                var mergedDicts = Application.Current!.Resources.MergedDictionaries;

                // Finds and replaces only the language dictionary
                if (_currentLanguageDictionary != null && mergedDicts.Contains(_currentLanguageDictionary))
                {
                    int index = mergedDicts.IndexOf(_currentLanguageDictionary);
                    mergedDicts[index] = translations;
                }
                else
                {
                    mergedDicts.Add(translations);
                }

                // Updates the reference for the next change
                _currentLanguageDictionary = translations;

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