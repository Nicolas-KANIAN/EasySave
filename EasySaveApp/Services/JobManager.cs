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
        public void SaveJobs()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_configPath, JsonSerializer.Serialize(Jobs, options));
        }

        // Adds a new backup job. (V2.0: No more 5-job limit)
        // Automatically persists changes to the JSON file.
        public void CreateJob(BackupJob newJob)
        {
            Jobs.Add(newJob);
            SaveJobs();
        }

        // Updates an existing job and saves changes to the JSON file.
        public void UpdateJob(int index, BackupJob updatedJob)
        {
            if (index >= 0 && index < Jobs.Count)
            {
                Jobs[index] = updatedJob;
                SaveJobs();
            }
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