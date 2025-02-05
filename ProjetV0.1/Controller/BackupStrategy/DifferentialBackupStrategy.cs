using Newtonsoft.Json.Linq;
using ProjetV0._1.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.Strategy
{
    public class DifferentialBackupStrategy : BackupStrategy
    {
        public void ExecuteBackup(string Source, string target)
        {
            //Console.WriteLine($"Sauvegarde de {Source} à {target}.");
            string logfile = GlobalVariables.LogFile;
            // Si destination n'existe pas cette focntion.Net le crée
            DirectoryExist(target);

            // Copiez tous les répertoires et les sous-répertoires même vides
            foreach (var directory in Directory.GetDirectories(Source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(Source, target);
                DirectoryExist(targetDirectory);
            }

            foreach (var file in Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories))
            {
                var targetFile = file.Replace(Source, target);
                DateTime TimeOfLastBackup = DateOfLastBackup(logfile, targetFile, target);
                var fileInfo = new FileInfo(file);
                if (fileInfo.LastWriteTimeUtc > TimeOfLastBackup)
                {
                    File.Copy(file, targetFile, true);
                }

            }


            Console.WriteLine("Sauvegarde Diff reussi");
        }

        // Méthode pour s'assurer que le répertoire existe
        private void DirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

        }


        private DateTime DateOfLastBackup(string LogFile, string source, string tagret)
        {
            DateTime LastBackupTime = DateTime.MinValue;

            if (File.Exists(LogFile))
            {
                string jsonLog = File.ReadAllText(LogFile);
                JArray logs = JArray.Parse(jsonLog);

                foreach (JObject log in logs)
                {
                    string logSource = (string)log["FileSource"];
                    string logDestination = (string)log["FileTarget"];

                    if (logSource == source && logDestination == tagret)
                    {
                        DateTime LogTime = DateTime.Parse((string)log["time"], CultureInfo.InvariantCulture);

                        // Update lastBackupTime if the current log's time is later than what was previously found.
                        if (LogTime > LastBackupTime)
                        {
                            LastBackupTime = LogTime;
                        }
                    }
                }
            }

            return LastBackupTime;
        }
    }
}
