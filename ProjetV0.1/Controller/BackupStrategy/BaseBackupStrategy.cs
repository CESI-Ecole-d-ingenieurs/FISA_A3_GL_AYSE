using EasySave.Logger;
using ProjetV0._1.Model;
using ProjetV0._1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.Strategy
{
    /// Base class for backup strategies.
    /// Implements common methods for backup operations and logging.
    internal abstract class BaseBackupStrategy : BackupStrategy
    {
        /// Logger instance for recording backup logs.
        protected Logger logger = new Logger();
        /// BackupView instance for displaying backup progress.
        protected BackupView backupView = new BackupView();

        /// Abstract method that must be implemented by derived classes
        /// to execute a specific backup strategy.
        public abstract void ExecuteBackup(string source, string target);
        /// Ensures that a directory exists. If not, it creates the directory.
        protected void DirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// Copies a file from the source directory to the target directory,
        /// logs the operation, and updates real-time progress.
        public void BackupFile(string file, string source, string target)
        {
            var targetFile = file.Replace(source, target);
            var startTime = DateTime.Now;

            try
            {
                // Copy the file to the target location
                File.Copy(file, targetFile, true);
                var endTime = DateTime.Now;
                var duration = (endTime - startTime).TotalMilliseconds;
                var fileInfo = new FileInfo(file);
                var fileSize = fileInfo.Length;
                // Log the backup operation
                logger.WriteLog(Path.GetFileName(file), file, targetFile, fileSize, duration);

                // Real-time status updates
                BackupStateJournal.UpdateProgress(Path.GetFileName(file));

                // Check
                Console.WriteLine("Appel de DisplayProgress...");

                // Progress Display
                backupView.DisplayProgress();
            }
            catch (Exception ex)
            {
                // Log the error if file copy fails
                logger.WriteLog(Path.GetFileName(file), file, targetFile, 0, 0, true);
                Console.WriteLine($"Error copying file {file}: {ex.Message}");
            }
        }
    }
}
