using EasySave.IviewLib;
using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public override async Task ExecuteBackup(string source, string target,String nameBackup)
        {
            DirectoryExist(target);

            var state = BackupStateJournal.ComputeState("CompleteBackup", source, target);

            backupView.DisplayProgress();

            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
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
}
