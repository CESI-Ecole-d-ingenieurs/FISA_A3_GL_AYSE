using ProjetV0._1;
using ProjetV0._1.Modele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ProjetV0._1
{
    public class Logger
    {
        private static Logger instance = new Logger();
        private static object lockObj = new object();
        private Logger() { }
        public static Logger Instance => instance;
        public string GetLogFileName()
        {
            return $"backup_log_{DateTime.Today:dd-MM-yyyy}.json";
        }

        public void WriteLog(Sauvegarde sauvegardelog, long fileSize, double fileTransferTime)
        {
            string logFile = GetLogFileName();
            lock (lockObj)
            {
                List<LogEntry> logs = new();
                if (File.Exists(logFile))
                {
                    string existingData = File.ReadAllText(logFile);
                    if (!string.IsNullOrEmpty(existingData))
                    {
                        logs = JsonSerializer.Deserialize<List<LogEntry>>(existingData) ?? new List<LogEntry>();
                    }
                }
                logs.Add(new LogEntry
                {
                    Name = sauvegardelog.Nom,
                    FileSource = sauvegardelog.Source,
                    FileTarget = sauvegardelog.Destination,
                    FileSize = fileSize,
                    FileTransferTime = fileTransferTime,
                    Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                });
                string jsonData = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(logFile, jsonData);
                Console.Write($"nom: {logFile}");
            }
        }
    }
    
}
