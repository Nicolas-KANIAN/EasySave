using EasyLog;
using EasySave.Models;
using EasySave.Services;

namespace EasySave
{
    // Main entry point for the EasySave application.
    // Manages the user interface, job orchestration, and command-line argument parsing.
    class Program
    {
        // Application startup: initializes services and handles the execution flow.
        static void Main(string[] args)
        {
            ConfigManager configManager = new ConfigManager();
            Logger.Instance.Format = configManager.Config.LogFormat;

            JobManager jobManager = new JobManager();
            BackupEngine engine = new BackupEngine();

            // CLI Mode: Executes specific jobs if arguments are provided (e.g., "1-3" or "1;3")
            if (args.Length > 0)
            {
                string command = string.Join("", args);
                ExecuteCommandLine(command, jobManager, engine);
                return;
            }

            // Interactive Mode: Language selection
            Console.WriteLine("Choose language / Choisissez la langue :");
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            Console.Write("> ");
            bool isFrench = Console.ReadLine() == "2";

            bool isRunning = true;
            while (isRunning)
            {
                // Main Menu UI
                Console.WriteLine(isFrench ? "\n=== Menu EasySave Version Console ===" : "\n=== EasySave Menu Console Version ===");
                Console.WriteLine(isFrench ? "1. Créer un travail de sauvegarde" : "1. Create a backup job");
                Console.WriteLine(isFrench ? "2. Afficher les travaux" : "2. List backup jobs");
                Console.WriteLine(isFrench ? "3. Lancer une sauvegarde" : "3. Run a backup job");
                Console.WriteLine(isFrench ? "4. Supprimer un travail" : "4. Delete a backup job");
                Console.WriteLine(isFrench ? "5. Paramètres (Format des logs)" : "5. Settings (Log Format)");
                Console.WriteLine(isFrench ? "6. Quitter" : "6. Exit");
                Console.Write("> ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (jobManager.Jobs.Count >= 5)
                        {
                            Console.WriteLine(isFrench ? "[ERREUR] Limite de 5 travaux atteinte." : "[ERROR] Limit of 5 jobs reached.");
                            break;
                        }

                        Console.Write(isFrench ? "Nom du travail : " : "Job Name: ");
                        string name = Console.ReadLine();

                        Console.Write(isFrench ? "Répertoire Source (ex: C:\\Dossier) : " : "Source Directory (e.g., C:\\Folder): ");
                        string source = Console.ReadLine()?.Trim('"');

                        Console.Write(isFrench ? "Répertoire Cible (ex: D:\\Backup) : " : "Target Directory (e.g., D:\\Backup): ");
                        string target = Console.ReadLine()?.Trim('"');

                        Console.Write(isFrench ? "Type (0 = Complet, 1 = Différentiel) : " : "Type (0 = Full, 1 = Differential): ");
                        BackupType type = Console.ReadLine() == "0" ? BackupType.Full : BackupType.Differential;

                        jobManager.CreateJob(new BackupJob(name, source, target, type));
                        Console.WriteLine(isFrench ? "=> Travail sauvegardé avec succès !" : "=> Job saved successfully!");
                        break;

                    case "2":
                        if (jobManager.Jobs.Count == 0)
                        {
                            Console.WriteLine(isFrench ? "[INFO] Aucun travail de sauvegarde existant." : "[INFO] No existing backup jobs.");
                            break;
                        }

                        Console.WriteLine(isFrench ? "\n=== Liste des travaux ===" : "\n=== List of Jobs ===");
                        for (int i = 0; i < jobManager.Jobs.Count; i++)
                        {
                            var j = jobManager.Jobs[i];
                            Console.WriteLine($"[{i + 1}] Nom : {j.Name} | Type : {j.Type}");
                            Console.WriteLine($"    Source : {j.SourceDirectory}");
                            Console.WriteLine($"    Cible  : {j.TargetDirectory}");
                            Console.WriteLine(new string('-', 50));
                        }
                        Console.WriteLine();
                        break;

                    case "3":
                        if (jobManager.Jobs.Count == 0)
                        {
                            Console.WriteLine(isFrench ? "[INFO] Aucun travail de sauvegarde existant." : "[INFO] No existing backup jobs.");
                            break;
                        }

                        Console.WriteLine(isFrench ? "\n=== Travaux existants ===" : "\n=== Existing Jobs ===");
                        for (int i = 0; i < jobManager.Jobs.Count; i++)
                        {
                            var j = jobManager.Jobs[i];
                            Console.WriteLine($"[{i + 1}] Nom : {j.Name} | Type : {j.Type}");
                            Console.WriteLine($"    Source : {j.SourceDirectory}");
                            Console.WriteLine($"    Cible  : {j.TargetDirectory}");
                            Console.WriteLine(new string('-', 50));
                        }
                        Console.WriteLine();

                        Console.Write(isFrench ? "Entrez l'index (1-5) ou 'all' pour tout lancer : " : "Enter job index (1-5) or 'all' to run all: ");
                        string input = Console.ReadLine()?.Trim().ToLower();

                        if (input == "all")
                        {
                            Console.WriteLine(isFrench ? "[INFO] Lancement séquentiel de tous les travaux..." : "[INFO] Sequential execution of all jobs...");
                            foreach (var job in jobManager.Jobs)
                            {
                                engine.ExecuteJob(job);
                            }
                        }
                        else if (int.TryParse(input, out int index) && index > 0 && index <= jobManager.Jobs.Count)
                        {
                            engine.ExecuteJob(jobManager.Jobs[index - 1]);
                        }
                        else
                        {
                            Console.WriteLine(isFrench ? "Saisie invalide." : "Invalid input.");
                        }
                        break;

                    case "4":
                        if (jobManager.Jobs.Count == 0)
                        {
                            Console.WriteLine(isFrench ? "[INFO] Aucun travail de sauvegarde existant." : "[INFO] No existing backup jobs.");
                            break;
                        }

                        Console.WriteLine(isFrench ? "\n=== Travaux existants ===" : "\n=== Existing Jobs ===");
                        for (int i = 0; i < jobManager.Jobs.Count; i++)
                        {
                            var j = jobManager.Jobs[i];
                            Console.WriteLine($"[{i + 1}] Nom : {j.Name} | Type : {j.Type}");
                            Console.WriteLine($"    Source : {j.SourceDirectory}");
                            Console.WriteLine($"    Cible  : {j.TargetDirectory}");
                            Console.WriteLine(new string('-', 50));
                        }
                        Console.WriteLine();

                        Console.Write(isFrench ? "Entrez l'index à supprimer (ou 'q' pour annuler) : " : "Enter index to delete (or 'q' to cancel): ");
                        string inputDelete = Console.ReadLine()?.Trim().ToLower();

                        if (inputDelete == "q")
                        {
                            Console.WriteLine(isFrench ? "Suppression annulée." : "Deletion cancelled.");
                            break;
                        }

                        if (int.TryParse(inputDelete, out int deleteIndex) && deleteIndex > 0 && deleteIndex <= jobManager.Jobs.Count)
                        {
                            string jobNameToDelete = jobManager.Jobs[deleteIndex - 1].Name;
                            jobManager.DeleteJob(deleteIndex - 1);
                            Console.WriteLine(isFrench ? $"[INFO] Travail '{jobNameToDelete}' supprimé avec succès." : $"[INFO] Job '{jobNameToDelete}' successfully deleted.");
                        }
                        else
                        {
                            Console.WriteLine(isFrench ? "[ERREUR] Index invalide." : "[ERROR] Invalid index.");
                        }
                        break;

                    case "5":
                        Console.WriteLine(isFrench
                            ? $"\nFormat actuel des logs : {configManager.Config.LogFormat}"
                            : $"\nCurrent log format: {configManager.Config.LogFormat}");

                        Console.WriteLine(isFrench
                            ? "Choisissez le nouveau format (1 = JSON, 2 = XML, q = Annuler) :"
                            : "Choose new format (1 = JSON, 2 = XML, q = Cancel):");
                        Console.Write("> ");

                        string formatChoice = Console.ReadLine()?.Trim().ToLower() ?? "";

                        if (formatChoice == "1")
                        {
                            configManager.Config.LogFormat = LogFormat.Json;
                            Logger.Instance.Format = LogFormat.Json;
                            configManager.SaveConfig();
                            Console.WriteLine(isFrench ? "=> Format changé en JSON." : "=> Format changed to JSON.");
                        }
                        else if (formatChoice == "2")
                        {
                            configManager.Config.LogFormat = LogFormat.Xml;
                            Logger.Instance.Format = LogFormat.Xml;
                            configManager.SaveConfig();
                            Console.WriteLine(isFrench ? "=> Format changé en XML." : "=> Format changed to XML.");
                        }
                        else if (formatChoice != "q")
                        {
                            Console.WriteLine(isFrench ? "Saisie invalide." : "Invalid input.");
                        }
                        break;

                    case "6":
                        isRunning = false;
                        break;
                }
            }
        }

        // Parses command-line arguments to execute one or multiple jobs.
        // Supports single index ("1"), ranges ("1-3"), and lists ("1;3").
        static void ExecuteCommandLine(string command, JobManager jobManager, BackupEngine engine)
        {
            List<int> indexesToRun = new List<int>();
            command = command.Replace(" ", "");

            try
            {
                if (command.Contains("-"))
                {
                    var parts = command.Split('-');
                    int start = int.Parse(parts[0]);
                    int end = int.Parse(parts[1]);
                    for (int i = start; i <= end; i++) indexesToRun.Add(i);
                }
                else if (command.Contains(";"))
                {
                    var parts = command.Split(';');
                    foreach (var part in parts) indexesToRun.Add(int.Parse(part));
                }
                else
                {
                    indexesToRun.Add(int.Parse(command));
                }

                foreach (int index in indexesToRun)
                {
                    if (index > 0 && index <= jobManager.Jobs.Count)
                    {
                        engine.ExecuteJob(jobManager.Jobs[index - 1]);
                    }
                    else
                    {
                        Console.WriteLine($"[WARNING] Job index {index} does not exist.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("[ERROR] Invalid command format. Expected '1-3' or '1;3'.");
            }
        }
    }
}