using EasySave.Models;
using System.Text.Json;

namespace EasySave.Services
{
    // Manages the configuration and persistence of backup jobs.
    // Handles loading/saving from JSON. (V2.0: Unlimited jobs)
    public class JobManager
    {
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jobs.json");
        public List<BackupJob> Jobs { get; private set; }

        public JobManager()
        {
            string dir = Path.GetDirectoryName(_configPath) ?? string.Empty;
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            LoadJobs();
        }

        // Reads backup jobs from the JSON configuration file. 
        // Initializes an empty list if the file does not exist.
        public void LoadJobs()
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                Jobs = JsonSerializer.Deserialize<List<BackupJob>>(json) ?? new List<BackupJob>();
            }
            else
            {
                Jobs = new List<BackupJob>();
            }
        }

        // Serializes and saves the current list of backup jobs to the configuration file.
        public bool SaveJobs()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_configPath, JsonSerializer.Serialize(Jobs, options));
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[ERROR] Fichier bloqué lors de la sauvegarde des jobs : {ex.Message}");
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"[ERROR] Accès refusé lors de la sauvegarde des jobs : {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erreur inattendue lors de la sauvegarde : {ex.Message}");
                return false;
            }
        }

        // Adds a new backup job.
        public bool CreateJob(BackupJob newJob)
        {
            Jobs.Add(newJob);
            return SaveJobs();
        }

        // Updates an existing job and saves changes to the JSON file.
        public bool UpdateJob(int index, BackupJob updatedJob)
        {
            if (index >= 0 && index < Jobs.Count)
            {
                Jobs[index] = updatedJob;
                return SaveJobs();
            }
            return false;
        }

        // Removes a job by its index and updates the configuration file.
        public bool DeleteJob(int index)
        {
            if (index >= 0 && index < Jobs.Count)
            {
                Jobs.RemoveAt(index);
                return SaveJobs();
            }
            return false;
        }
    }
}