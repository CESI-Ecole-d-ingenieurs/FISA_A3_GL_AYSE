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
using System.Reflection;

namespace EasySave.ControllerLib.BackupStrategy
{

    internal class DifferentialBackupStrategy : BaseBackupStrategy
    {
        /// Executes a differential backup by copying only modified files since the last backup.
        public DifferentialBackupStrategy(IBackupView backupview) : base(backupview)
        {

        }
        public override async Task ExecuteBackup(string source, string target, String nameBackup, Dictionary<string, bool> _isPaused = null, Dictionary<string, CancellationTokenSource> _cancellationTokens = null)
            {
            DirectoryExist(target);

            var state = BackupStateJournal.ComputeState("DifferentialBackup", source, target);

            backupView.DisplayProgress();

            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);




            var groups = MakeGroupsPrior(files);
            foreach (var group in groups)
            {

                double networkLoad = GetNetworkUtilization();
                int maxParallelTasks = networkLoad > 90 ? 1 : networkLoad > 70 ? 2 : networkLoad > 50 ? 3 : 5; // Limits the number of parallel tasks depending on network load

                List<Task> tasks = new List<Task>();

                foreach (var file in group)
                {
                    var targetFile = file.Replace(source, target);
                    DateTime lastBackupTime = await DateOfLastBackup(Logger.Logger.GetLogFileName(), file, targetFile);
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTimeUtc > lastBackupTime)
                    {
                        BackupOneFile(state, source, file, target, nameBackup, tasks, _isPaused, _cancellationTokens);
                    }
                }
                do
                {
                    await Task.WhenAll(tasks.Take(maxParallelTasks));
                    tasks = tasks.Skip(maxParallelTasks).ToList();
                } while (tasks.Count > 0);
            }
            }

        /// Determines the last backup date of a given file based on the log history.
        /// It searches the log file to find the most recent backup date of the given file.
        private async Task<DateTime> DateOfLastBackup(string logFile, string source, string target)
        {
            DateTime lastBackupTime = DateTime.MinValue;

            // Checks if the log file does not exist or is empty
            if (!File.Exists(logFile) || new FileInfo(logFile).Length == 0)
            {
                return DateTime.MinValue; // Limits the number of parallel tasks depending on network load
            }

            string jsonLog = File.ReadAllText(logFile);
            if (string.IsNullOrWhiteSpace(jsonLog))
            {
                return DateTime.MinValue;
            }

            try  
            {
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
            catch (Exception ex)
            {
                Console.WriteLine(await Translation.Instance.Translate($" Erreur lors de la lecture du fichier log : {ex.Message}"));
                return DateTime.MinValue;
            }

            return lastBackupTime;
        }
    }
}
