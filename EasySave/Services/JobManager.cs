using EasySave.Models;
using System.Text.Json;

namespace EasySave.Services
{
    public class JobManager
    {
        private readonly string _configPath = @"C:\EasySave\jobs.json";
        public List<BackupJob> Jobs { get; private set; }

        public JobManager()
        {
            string dir = Path.GetDirectoryName(_configPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            LoadJobs();
        }

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

        public void SaveJobs()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_configPath, JsonSerializer.Serialize(Jobs, options));
        }

        public void CreateJob(BackupJob newJob)
        {
            if (Jobs.Count >= 5)
            {
                return;
            }
            Jobs.Add(newJob);
            SaveJobs();
        }
    }
}