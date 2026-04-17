
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasyLog
{
    public class Logger
    {
        // Répertoire des fichiers de logs 
        private readonly string logDirectory = @"C:\EasySave\Logs";

        // Vérifie si le dossier existe ou non
        public Logger()
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        // Cette méthode ajoute des logs dans le fichier journalier.
        // Le fichier a comme nom la date actuelle : yyyy-MM-dd.
        // S'il exsite déjà, le contenu est relu et écrit les nouveaux logs 
        // La liste complète des logs est réécrite dans le fichier.
        public void WriteDailyLog(LogEntry entry)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = Path.Combine(logDirectory, $"{date}.json");

            var options = new JsonSerializerOptions { WriteIndented = true };
            List<LogEntry> logs = new List<LogEntry>();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    logs = JsonSerializer.Deserialize<List<LogEntry>>(json);
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
            string filePath = Path.Combine(logDirectory, "state.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(states, options));
        }
    }
}