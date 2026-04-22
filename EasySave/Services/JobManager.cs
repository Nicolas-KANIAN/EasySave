using EasySave.Models;
using System.Text.Json;

namespace EasySave.Services
{
    // Manages the configuration and persistence of backup jobs.
    // Handles loading/saving from JSON and enforces business rules like the 5-job limit.
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
        public void SaveJobs()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_configPath, JsonSerializer.Serialize(Jobs, options));
        }

        // Adds a new backup job if the maximum limit of 5 is not reached.
        // Automatically persists changes to the JSON file.
        public void CreateJob(BackupJob newJob)
        {
            if (Jobs.Count >= 5)
            {
                return;
            }
            Jobs.Add(newJob);
            SaveJobs();
        }

        // Removes a job by its index and updates the configuration file.
        public void DeleteJob(int index)
        {
            if (index >= 0 && index < Jobs.Count)
            {
                Jobs.RemoveAt(index);
                SaveJobs();
            }
        }
    }
}