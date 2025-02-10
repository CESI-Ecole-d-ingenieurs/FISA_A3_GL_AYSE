using EasySave.Logger;
using ProjetV0._1.Controller;
using ProjetV0._1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.Strategy
{
    /// Implements the complete backup strategy, where all files and directories
    /// from the source are copied entirely to the target directory.
    internal class CompleteBackupStrategy : BaseBackupStrategy
    {
        /// Executes a complete backup of all files and directories.
        /// Ensures the target directory exists, initializes the backup state,
        /// and copies all files from the source to the target.
        public override void ExecuteBackup(string source, string target)
        {
            // Ensure the target directory exists
            DirectoryExist(target);

            // Initialize the backup state in the journal
            var state = BackupStateJournal.ComputeState("CompleteBackup", source, target);

            // Display initial progress
            backupView.DisplayProgress();

            // Ensure all directories from source exist in the target
            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            // Copy all files from source to target, including subdirectories
            foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                BackupFile(file, source, target);
            }
        }


    }
}
