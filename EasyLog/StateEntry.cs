/*******************************************
*
*   La classe StateEntry représente l'état d'un travail de sauvegarde.
*   Elle permet de stocker les infroamtions pour suivre l'éxecution
*   de la sauvegarde à un moment donné.
*   
*   Les données enregsitrées sont :
*   
*   - le nom de la sauvegarde 
*   - la date et l'heure de l'état enregsitré 
*   - l'état de la sauvegarde 
*   - le nombre total de fichier à copier 
*   - la taille totale des fichiers
*   - le nombre de fichiers restants 
*   - la progression
*   - le fichier en cours de traitement
*   - le fihcier cible courant 
*   
*   La classe StateEntry est utilisé pour suivre l'état d'une sauvegarde 
*   dans le fichier state.json.
*
******************************************/

namespace EasyLog
{
    public class StateEntry
    {
        public string Name { get; set; }
        public string Timestamp { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public int Progression { get; set; }
        public string CurrentSourceFile { get; set; }
        public string CurrentTargetFile { get; set; }
        public long RemainingFilesSize { get; set; }


        // Constructeur, initialise la date et l'heure au moment 
        // de la création de l'objet.
        public StateEntry()
        {
            Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}