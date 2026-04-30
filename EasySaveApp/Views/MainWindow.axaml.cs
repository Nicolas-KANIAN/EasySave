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

        // Méthode appelée quand on clique sur "..." pour la Source
        private async void OnBrowseSourceClick(object? sender, RoutedEventArgs e)
        {
            // Récupère la fenêtre actuelle
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            // Ouvre la boîte de dialogue de sélection de dossier
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Sélectionner le répertoire source",
                AllowMultiple = false
            });

            // Si l'utilisateur a choisi un dossier (et n'a pas fait "Annuler")
            if (folders.Count >= 1)
            {
                // On met à jour le ViewModel pour que le chemin s'affiche dans l'interface
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.SourceDirectory = folders[0].Path.LocalPath;
                }
            }
        }

        // Méthode appelée quand on clique sur "..." pour la Cible
        private async void OnBrowseTargetClick(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Sélectionner le répertoire cible",
                AllowMultiple = false
            });

            if (folders.Count >= 1)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.TargetDirectory = folders[0].Path.LocalPath;
                }
            }
        }
    }
}