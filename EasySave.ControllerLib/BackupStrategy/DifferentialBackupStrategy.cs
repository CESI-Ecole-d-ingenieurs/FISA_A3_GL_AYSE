using EasySave.IviewLib;
using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Logger;
using Newtonsoft.Json;
using System.Threading;

namespace EasySave.ControllerLib.BackupStrategy
{

    internal class DifferentialBackupStrategy : BaseBackupStrategy
    {
        /// Executes a differential backup by copying only modified files since the last backup.
        public DifferentialBackupStrategy(IBackupView backupview) : base(backupview)
        {

        }
        public override async Task ExecuteBackup(string source, string target, String nameBackup)
        {
            DirectoryExist(target);

            var state = BackupStateJournal.ComputeState("DifferentialBackup", source, target);

            backupView.DisplayProgress();

            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                var targetFile = file.Replace(source, target);

                DateTime lastBackupTime = DateOfLastBackup(Logger.Logger.GetLogFileName(), file, targetFile);
                var fileInfo = new FileInfo(file);

                if (fileInfo.LastWriteTimeUtc > lastBackupTime)
                {
                    await Task.Run(() =>
                    {
                        BackupStateJournal.UpdateProgress(nameBackup); // Real-time update

                        Thread.Sleep(500); // Slow down the process for better visualization
                    }
                   );
                    BackupFile(file, source, target);
                }
            }
        }

        /// Determines the last backup date of a given file based on the log history.
        /// It searches the log file to find the most recent backup date of the given file.
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
