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
    internal abstract class BaseBackupStrategy : BackupStrategy
    {
        protected Logger logger = new Logger();
        protected BackupView backupView = new BackupView();

        public abstract void ExecuteBackup(string source, string target);

        protected void DirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void BackupFile(string file, string source, string target)
        {
            var targetFile = file.Replace(source, target);
            var startTime = DateTime.Now;

            try
            {
                File.Copy(file, targetFile, true);
                var endTime = DateTime.Now;
                var duration = (endTime - startTime).TotalMilliseconds;
                var fileInfo = new FileInfo(file);
                var fileSize = fileInfo.Length;

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
                logger.WriteLog(Path.GetFileName(file), file, targetFile, 0, 0, true);
                Console.WriteLine($"Error copying file {file}: {ex.Message}");
            }
        }
    }
}
