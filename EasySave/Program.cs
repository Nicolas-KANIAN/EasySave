using System;
using System.Collections.Generic;
using EasySave.Models;
using EasySave.Services;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            JobManager jobManager = new JobManager();
            BackupEngine engine = new BackupEngine();

            if (args.Length > 0)
            {
                ExecuteCommandLine(args[0], jobManager, engine);
                return;
            }

            Console.WriteLine("Choose language / Choisissez la langue :");
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            Console.Write("> ");
            bool isFrench = Console.ReadLine() == "2";

            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine(isFrench ? "\n=== Menu EasySave ===" : "\n=== EasySave Menu ===");
                Console.WriteLine(isFrench ? "1. Créer un travail de sauvegarde" : "1. Create a backup job");
                Console.WriteLine(isFrench ? "2. Afficher les travaux" : "2. List backup jobs");
                Console.WriteLine(isFrench ? "3. Lancer une sauvegarde" : "3. Run a backup job");
                Console.WriteLine(isFrench ? "4. Quitter" : "4. Exit");
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
                        string source = Console.ReadLine().Trim('"');

                        Console.Write(isFrench ? "Répertoire Cible (ex: D:\\Backup) : " : "Target Directory (e.g., D:\\Backup): ");
                        string target = Console.ReadLine().Trim('"');

                        Console.Write(isFrench ? "Type (0 = Complet, 1 = Différentiel) : " : "Type (0 = Full, 1 = Differential): ");
                        BackupType type = Console.ReadLine() == "0" ? BackupType.Full : BackupType.Differential;

                        jobManager.CreateJob(new BackupJob(name, source, target, type));
                        Console.WriteLine(isFrench ? "=> Travail sauvegardé avec succès !" : "=> Job saved successfully!");
                        break;

                    case "2":
                        Console.WriteLine("\n--- Jobs ---");
                        for (int i = 0; i < jobManager.Jobs.Count; i++)
                        {
                            var j = jobManager.Jobs[i];
                            Console.WriteLine($"[{i + 1}] {j.Name} | {j.Type} | {j.SourceDirectory} -> {j.TargetDirectory}");
                        }
                        break;

                    case "3":
                        Console.Write(isFrench ? "Entrez l'index du travail (ex: 1) : " : "Enter job index (e.g., 1): ");
                        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= jobManager.Jobs.Count)
                        {
                            engine.ExecuteJob(jobManager.Jobs[index - 1]);
                        }
                        else
                        {
                            Console.WriteLine(isFrench ? "Index invalide." : "Invalid index.");
                        }
                        break;

                    case "4":
                        isRunning = false;
                        break;
                }
            }
        }

        static void ExecuteCommandLine(string command, JobManager jobManager, BackupEngine engine)
        {
            List<int> indexesToRun = new List<int>();

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