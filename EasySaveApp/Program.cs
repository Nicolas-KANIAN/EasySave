using Avalonia;
using EasySave.Services;
using EasySave.Models;

namespace EasySaveApp
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // If there are command-line arguments (e.g., "1-3"), run in CLI mode
            if (args.Length > 0)
            {
                RunCommandLine(args[0]).GetAwaiter().GetResult();
                return;
            }

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                #if DEBUG
                .WithDeveloperTools()
                #endif
                .WithInterFont()
                .LogToTrace();

        private static async Task RunCommandLine(string arg)
        {
            Console.WriteLine("Executing in console mode...");

            var configManager = new ConfigManager();
            var jobManager = new JobManager();
            var businessMonitor = new BusinessSoftwareMonitor();

            businessMonitor.SetSoftwareName(configManager.Config.BusinessSoftware);
            businessMonitor.Start();

            var backupEngine = new BackupEngine(configManager.Config, businessMonitor);
            var jobsToRun = new List<BackupJob>();

            try
            {
                // Parse sequence format (e.g., "1-3")
                if (arg.Contains('-'))
                {
                    var parts = arg.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
                    {
                        for (int i = start - 1; i < end; i++)
                        {
                            if (i >= 0 && i < jobManager.Jobs.Count) jobsToRun.Add(jobManager.Jobs[i]);
                        }
                    }
                }
                // Parse specific selection format (e.g., "1;3")
                else if (arg.Contains(';'))
                {
                    var parts = arg.Split(';');
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part, out int index))
                        {
                            if (index - 1 >= 0 && index - 1 < jobManager.Jobs.Count) jobsToRun.Add(jobManager.Jobs[index - 1]);
                        }
                    }
                }
                // Parse single job format (e.g., "2")
                else if (int.TryParse(arg, out int singleIndex))
                {
                    if (singleIndex - 1 >= 0 && singleIndex - 1 < jobManager.Jobs.Count)
                    {
                        jobsToRun.Add(jobManager.Jobs[singleIndex - 1]);
                    }
                }

                if (jobsToRun.Count == 0)
                {
                    Console.WriteLine("No valid backup job found with these arguments.");
                    return;
                }

                Console.WriteLine($"{jobsToRun.Count} job(s) to execute in parallel.");

                var tasks = new List<Task>();
                foreach (var job in jobsToRun)
                {
                    Console.WriteLine($"> Starting {job.Name}...");
                    tasks.Add(Task.Run(() => backupEngine.ExecuteJob(job)));
                }

                await Task.WhenAll(tasks);
                Console.WriteLine("All console backups completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during console execution: {ex.Message}");
            }
            finally
            {
                businessMonitor.Stop();
            }
        }
    }
}