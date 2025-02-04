using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controleur.Strategy
{
    public class DifferentialBackupStrategy : BackupStrategy
    {
        public void ExecuteBackup(string Source, string target)
        {
            Console.WriteLine($"Sauvegarde de {Source} à {target}.");
            string logfile = "";
            // Si destination n'existe pas cette focntion.Net le crée
            RepertoireExiste(target);

            // Copiez tous les répertoires et les sous-répertoires même vides
            foreach (var directory in Directory.GetDirectories(Source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(Source, target);
                RepertoireExiste(targetDirectory);
            }

            foreach (var file in Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories))
            {
                var targetFile = file.Replace(Source, target);
                DateTime TempsDernierSauvegarde = DateDernierSauvegarde(logfile, targetFile, target);
                var fileInfo = new FileInfo(file);
                if (fileInfo.LastWriteTimeUtc > TempsDernierSauvegarde)
                {
                    File.Copy(file, targetFile, true);
                }

            }


            Console.WriteLine("Sauvegarde Diff reussi");
        }

        // Méthode pour s'assurer que le répertoire existe
        private void RepertoireExiste(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

        }


        private DateTime DateDernierSauvegarde(string FichierLog, string source, string destination)
        {
            DateTime TempsDernierSauvegarde = DateTime.MinValue;

            if (File.Exists(FichierLog))
            {
                string jsonLog = File.ReadAllText(FichierLog);
                JArray logs = JArray.Parse(jsonLog);

                foreach (JObject log in logs)
                {
                    string logSource = (string)log["FileSource"];
                    string logDestination = (string)log["FileTarget"];

                    if (logSource == source && logDestination == destination)
                    {
                        DateTime Tempslog = DateTime.Parse((string)log["time"], CultureInfo.InvariantCulture);

                        // Update lastBackupTime if the current log's time is later than what was previously found.
                        if (Tempslog > TempsDernierSauvegarde)
                        {
                            TempsDernierSauvegarde = Tempslog;
                        }
                    }
                }
            }

            return TempsDernierSauvegarde;
        }
    }
}
