using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using EasySave.Services;
using EasySaveApp.ViewModels;
using EasySaveApp.Views;
using System.Linq;

namespace EasySaveApp
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var configManager = new ConfigManager();

            EasyLog.Logger.Instance.Destination = configManager.Config.LogDestination;
            EasyLog.Logger.Instance.CentralServerIp = configManager.Config.LogServerIp;
            EasyLog.Logger.Instance.CentralServerPort = configManager.Config.LogServerPort;

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}