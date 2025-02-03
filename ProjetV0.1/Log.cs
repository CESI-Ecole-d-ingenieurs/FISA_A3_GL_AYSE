using ProjetV0._1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ProjetV0._1
{
    internal class LogEntry
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public string Time { get; set; }
    }

    internal class Journalisation
    {
        private static readonly Journalisation instance = new();
        private static readonly object lockObj = new();

        // Constructeur privé pour empêcher l'instanciation extérieure
        private Journalisation() { }

        public static Journalisation Instance => instance;

        // Méthode pour obtenir le nom du fichier de log du jour
        public string GetLogFileName()
        {
            // Génère un nom de fichier au format "backup_log_2025-02-03.json"
            return $"backup_log_{DateTime.Today:dd-MM-yyyy}.json";
        }
        public string filepath = $"{DateTime.Today.ToString()}_backup_log.json";

        public void EcrireLog(Sauvegarde sauvegardelog, long fileSize, double fileTransferTime)
        {
            // Calcule le nom du fichier pour le jour actuel à chaque appel
            string logFile = GetLogFileName();
            

            lock (lockObj)
            {
                List<LogEntry> logs = new();

                // Si le fichier existe, on lit son contenu
                if (File.Exists(filepath))
                {
                    string existingData = File.ReadAllText(filepath);
                    if (!string.IsNullOrEmpty(existingData))
                    {
                        logs = JsonSerializer.Deserialize<List<LogEntry>>(existingData) ?? new List<LogEntry>();
                    }
                }

                // Ajout d'une nouvelle entrée de log
                logs.Add(new LogEntry
                {
                    //Name = name,
                    Name= sauvegardelog.Nom,
                    FileSource = sauvegardelog.Source,
                    FileTarget = sauvegardelog.Destination,
                    FileSize = fileSize,
                    FileTransferTime = fileTransferTime,
                    Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                });

                // Sérialisation de la liste des logs au format JSON
                string jsonData = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });

                // Écriture dans le fichier de log du jour
                File.WriteAllText(filepath, jsonData);
                Console.Write($"nom: {filepath}");
            }
        }
    }
}
