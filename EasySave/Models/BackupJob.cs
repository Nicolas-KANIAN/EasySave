/*******************************************
*
*   La classe BackupJob représente le travail de sauvegarde dans l'application. 
*   Elle regroupe les informations pour éxectuer la sauvegarde : 
*   - le nom du travail 
*   - le répertoire source 
*   - le répertoire cible 
*   - le type de sauvegarde choisi
*
******************************************/

namespace EasySave.Models
{
    public class BackupJob
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }

        //Constructeur par défaut 
        public BackupJob()
        {
        }

        // Constructeur avec paramètres pour initialiser 
        // toutes les propiétés à la création.
        public BackupJob(string name, string sourceDirectory, string targetDirectory, BackupType type)
        {
            Name = name;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            Type = type;
        }
    }
}