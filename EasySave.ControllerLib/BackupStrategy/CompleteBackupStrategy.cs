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


        /// <summary>
        /// Executes the backup process.
        /// It first ensures the target directory exists, then processes all directories and files.
        /// Files are grouped and copied in parallel, with limitations based on network utilization.
        /// </summary>
        /// <param name="source">The source directory for backup.</param>
        /// <param name="target">The destination directory for backup.</param>
        /// <param name="nameBackup">The name of the backup process.</param>
        /// <param name="_isPaused">A dictionary tracking whether the backup process is paused.</param>
        /// <param name="_cancellationTokens">A dictionary of cancellation tokens to allow stopping the process.</param>
        public override async Task ExecuteBackup(string source, string target, String nameBackup, Dictionary<string, bool> _isPaused = null, Dictionary<string, CancellationTokenSource> _cancellationTokens = null)
        {
            // Ensure the target directory exists
            DirectoryExist(target);

            var state = BackupStateJournal.ComputeState("CompleteBackup", source, target);
            double networkLoad ;
            int maxParallelTasks; // Limits the number of parallel tasks depending on network load

            // Create all subdirectories in the target directory
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
                // Adjust the number of parallel tasks based on network usage
                maxParallelTasks = networkLoad > 90 ? 1 : networkLoad > 70 ? 2 : networkLoad > 50 ? 3 : 5; // Limits the number of parallel tasks depending on network load

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

                    // Wait for the current batch of parallel tasks to complete
                    await Task.WhenAll(tasks.Take(maxParallelTasks));
                    // Remove completed tasks from the list
                    tasks = tasks.Skip(maxParallelTasks).ToList();
                }while (tasks.Count > 0);   
            }
        }
    }
}
