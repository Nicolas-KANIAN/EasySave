
/*******************************************
*
*   Ce fichier gère l'enregistrement des informations des sauvegardes 
*   dans des fichiers JSON.
*   - Il crée automatiquement le dossier de log s'il n'existe pas 
*   - Écrit les logs détaillés des sauvegarde dans un fichier journalier 
*   - Met à jour l'état des sauvegardes dans stat.json
*
*   La classe Logger centralise la gestion de l'écriture des 
*   fichiers de suivi et des fichiers de journalisation.
*
******************************************/

using System.Text.Json;

namespace EasyLog
{
    public class Logger
    {
        private readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static Logger? _instance;
        private static readonly object _lock = new object();

        private Logger()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        // Cette méthode ajoute des logs dans le fichier journalier.
        // Le fichier a comme nom la date actuelle : yyyy-MM-dd.
        // S'il exsite déjà, le contenu est relu et écrit les nouveaux logs 
        // La liste complète des logs est réécrite dans le fichier.
        public static Logger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                    return _instance;
                }
            }
        }

        public void WriteDailyLog(LogEntry entry)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = Path.Combine(_logDirectory, $"{date}.json");

            var options = new JsonSerializerOptions { WriteIndented = true };
            List<LogEntry> logs = new List<LogEntry>();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    logs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                }
            }

            logs.Add(entry);
            File.WriteAllText(filePath, JsonSerializer.Serialize(logs, options));
        }

        // Cette méthode met à jour le state.json
        // Il contient l'état actuel des sauvegardes 
        // Le contenu est entièrement réécrit à chaques mise à jour 
        public void UpdateState(List<StateEntry> states)
        {
            string filePath = Path.Combine(_logDirectory, "state.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(states, options));
        }
    }
}