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
    /// Implements the differential backup strategy, where only modified or new files 
    /// since the last backup are copied to the target directory.
    internal class DifferentialBackupStrategy : BaseBackupStrategy
    {
        /// Executes a differential backup by copying only modified files since the last backup.
        /// Ensures directories exist before proceeding.
        public override void ExecuteBackup(string source, string target)
        {
            // Ensure the target directory exists
            DirectoryExist(target);

            // Initialize the backup state in the journal
            var state = BackupStateJournal.ComputeState("DifferentialBackup", source, target);

            // Display initial progress
            backupView.DisplayProgress();

            // Ensure all directories from source exist in the target
            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            // Iterate through all files in the source directory
            foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                var targetFile = file.Replace(source, target);
                // Get the last backup time for this file

                DateTime lastBackupTime = DateOfLastBackup(Logger.GetLogFileName(), file, targetFile);
                var fileInfo = new FileInfo(file);

                // Copy only if the file has been modified since the last backup
                if (fileInfo.LastWriteTimeUtc > lastBackupTime)
                {
                    BackupFile(file, source, target);
                }
            }
        }

        /// Determines the last backup date of a given file based on the log history.
        /// It searches the log file to find the most recent backup date of the given file.
        private DateTime DateOfLastBackup(string logFile, string source, string target)
        {
            DateTime lastBackupTime = DateTime.MinValue;

            // Check if the log file exists
            if (File.Exists(logFile))
            {
                string jsonLog = File.ReadAllText(logFile);
                var logs = Newtonsoft.Json.Linq.JArray.Parse(jsonLog);

                // Iterate through the log entries
                foreach (var log in logs)
                {
                    string logSource = (string)log["FileSource"];
                    string logDestination = (string)log["FileTarget"];

                    // Check if the current log entry corresponds to the given file
                    if (logSource == source && logDestination == target)
                    {
                        DateTime logTime = DateTime.Parse((string)log["Date"], System.Globalization.CultureInfo.InvariantCulture);
                        // Update last backup time if this log entry is more recent
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

