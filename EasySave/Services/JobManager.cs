/********************************
*
*   La classe JobManager gère la configurations des sauvegardes.
*   Elle doit : 
*   - Charger les jobs du fichier JSON.
*   - Les sauvegarder .
*   - Ajouter des nouveaux jobs de backups.
*
* 
*********************************/

using System.Text.Json;
using EasySave.Models;

namespace EasySave.Services
{
    public class JobManager
    {
        private readonly string _configPath = @"C:\EasySave\jobs.json";
        public List<BackupJob> Jobs { get; private set; }

        // Constructeur, crée le dossier de configuration si besoin
        // et charge les jobs depuis le fichier JSON.
        public JobManager()
        {
            string dir = Path.GetDirectoryName(_configPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            LoadJobs();
        }


        // LoadJobs() permet de charger les jobs depuis le fichiers de configuration.
        // Si le fichier existe, le JSON est lu et son contenu est converti en liste de BackupJob,
        // sinon il crée une liste vide.
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


        // SaveJobs() enregsitre la liste actuelle des jobs
        // dans le fichier de configuration en JSON.
        public void SaveJobs()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_configPath, JsonSerializer.Serialize(Jobs, options));
        }

        // CreateJobs() ajoute un nouveau travail dans la liste.
        // Si il y a 5 jobs, alors l'ajout est ignoré.
        // Sinon il est ajouté dans le fihcier JSON.
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