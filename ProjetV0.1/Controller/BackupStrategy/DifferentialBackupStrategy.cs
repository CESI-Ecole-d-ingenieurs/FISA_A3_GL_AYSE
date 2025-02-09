using EasySave.Logger;
using Newtonsoft.Json.Linq;
using ProjetV0._1.Controller;
using ProjetV0._1.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.Strategy
{
    internal class DifferentialBackupStrategy : BaseBackupStrategy
    {

        public override void ExecuteBackup(string source, string target)
        {
            DirectoryExist(target);

            var state = BackupStateJournal.ComputeState("DifferentialBackup", source, target);
            backupView.DisplayProgress(new List<BackupState> { state });

            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                var targetFile = file.Replace(source, target);
                DateTime lastBackupTime = DateOfLastBackup(Logger.GetLogFileName(), file, targetFile);
                var fileInfo = new FileInfo(file);

                if (fileInfo.LastWriteTimeUtc > lastBackupTime)
                {
                    BackupFile(file, source, target);
                }
            }
        }

        private DateTime DateOfLastBackup(string logFile, string source, string target)
        {
            DateTime lastBackupTime = DateTime.MinValue;

            if (File.Exists(logFile))
            {
                string jsonLog = File.ReadAllText(logFile);
                var logs = Newtonsoft.Json.Linq.JArray.Parse(jsonLog);

                foreach (var log in logs)
                {
                    string logSource = (string)log["FileSource"];
                    string logDestination = (string)log["FileTarget"];

                    if (logSource == source && logDestination == target)
                    {
                        DateTime logTime = DateTime.Parse((string)log["Date"], System.Globalization.CultureInfo.InvariantCulture);
                        if (logTime > lastBackupTime)
                        {
                            lastBackupTime = logTime;
                        }
                    }
                }
            }
            return lastBackupTime;
        }
    }
}

