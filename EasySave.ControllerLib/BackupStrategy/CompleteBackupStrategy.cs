using EasySave.IviewLib;
using EasySave.ModelLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasySave.ControllerLib.BackupStrategy
{

    internal class CompleteBackupStrategy : BaseBackupStrategy
    {
        /// Executes a complete backup of all files and directories.
        /// Ensures the target directory exists, initializes the backup state,
        /// and copies all files from the source to the target.
        
        public CompleteBackupStrategy(IBackupView backupview) : base(backupview)
        {
            
        }



        public override async Task ExecuteBackup(string source, string target, String nameBackup, Dictionary<string, bool> _isPaused = null, Dictionary<string, CancellationTokenSource> _cancellationTokens = null)
        {
            DirectoryExist(target);

            var state = BackupStateJournal.ComputeState("CompleteBackup", source, target);
            double networkLoad ;
            int maxParallelTasks; // Limite le nombre de tâches parallèles selon la charge réseau

            //backupView.DisplayProgress();

            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            var groups = MakeGroupsPrior( files);
            foreach (var group in groups)
            {
                 networkLoad = GetNetworkUtilization();
                maxParallelTasks = networkLoad > 90 ? 1 : networkLoad > 70 ? 2 : networkLoad > 50 ? 3 : 5; // Limite le nombre de tâches parallèles selon la charge réseau
                int n = 0;
                List<Task> tasks = new List<Task>();
                foreach (var file in group)
                {
                    n++;
                    BackupOneFile(state, source, file, target, nameBackup, tasks, _isPaused, _cancellationTokens);
                  
                }
                do
                {
                    while (_isPaused[nameBackup])
                    {
                        await Task.Delay(500);
                    }
                    await Task.WhenAll(tasks.Take(maxParallelTasks)); //prob
                    tasks = tasks.Skip(maxParallelTasks).ToList();
                }while (tasks.Count > 0);   
            }
        }


    }

}
