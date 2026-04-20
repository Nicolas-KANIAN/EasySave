/***************************************
* 
*   Ce fichier gère les logs d'entré d'un fichier. 
*   La class LogEntry représente la journalisation liée à une sauvegarde.
*   Le fichier stocke :
*   - la date et l'heure du log 
*   - le nom de la sauvegarde 
*   - le fichier cible
*   - la taille du fichier
*   - le temps de transfère
*
*   Toutes les données sont stockées dans un fichier JSON.
*   
****************************************/
namespace EasyLog
{
    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string BackupName { get; set; }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }
        public long FileSize { get; set; }
        public long TransferTime { get; set; }

        // Constructeur, intialise automatiquement la date et l'heure du log 
        // au moment de la création du log  
        public LogEntry()
        {
            Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}