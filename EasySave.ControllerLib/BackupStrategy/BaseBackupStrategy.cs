using CryptoSoft;
using EasySave.IviewLib;
using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Logger;
using System.Threading;
using System.Diagnostics;
namespace EasySave.ControllerLib.BackupStrategy
{

    /// Implements common methods for backup operations and logging.
    public abstract class BaseBackupStrategy : BackupStrategy
    {
        private static SemaphoreSlim largeFileSemaphore = new SemaphoreSlim(1, 1);
        /// Logger instance for recording backup logs.
        protected Logger.Logger logger = new Logger.Logger();
        /// BackupView instance for displaying backup progress.
        protected IBackupView backupView;

        public BaseBackupStrategy(IBackupView backupview)
        {
            backupView = backupview;
        }
        /// Abstract method that must be implemented by derived classes to execute a specific backup strategy.
        public abstract  Task ExecuteBackup(string source, string target, String nameBackup, Dictionary<string, bool> _isPaused = null, Dictionary<string, CancellationTokenSource> _cancellationTokens = null) ;
        /// Ensures that a directory exists. If not, it creates the directory.
        protected void DirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// Copies a file from the source directory to the target directory, logs the operation, and updates real-time progress.
        public void BackupFile(string file, string source, string target)
        {
            var targetFile = file.Replace(source, target);
            var startTime = DateTime.Now;
            int ElapsedTime = 0;
            String test = Path.GetExtension(file);
            String f = file;
            string TR = targetFile;
            String edx = Path.GetExtension(targetFile);
            String[] testing = GlobalVariables.CryptedFileExt.Where(ext => ("." + ext.Trim()).Equals(".txt")).ToArray();

            bool m = GlobalVariables.CryptedFileExt.Any(ext => edx.Equals("." + ext.Trim(), StringComparison.OrdinalIgnoreCase));
            int r = 0;
            try
            {
                File.Copy(file, targetFile, true);

                if (GlobalVariables.CryptedFileExt.Any(ext => Path.GetExtension(targetFile).Equals("." + ext.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        var fileManager = new FileManager(targetFile, GlobalVariables.Key);
                        ElapsedTime = fileManager.TransformFile();
                    }


                    // Copy the file to the target location
                    var endTime = DateTime.Now;
                    var duration = (endTime - startTime).TotalMilliseconds;
                    var fileInfo = new FileInfo(file);
                    var fileSize = fileInfo.Length;
                    // Log the backup operation
                    if (CheckFileExtension(GlobalVariables.LogFilePath, ".xml"))
                    {
                        logger.WriteLogXML(Path.GetFileName(file), file, targetFile, fileSize, duration, ElapsedTime);

                    }
                    else
                    {
                        logger.WriteLog(Path.GetFileName(file), file, targetFile, fileSize, duration, ElapsedTime);
                    }


                    // Real-time status updates
                    BackupStateJournal.UpdateProgress(Path.GetFileName(file));

                    // Progress Display
                     backupView.DisplayProgress();
                
            }
            catch (Exception ex)
            {
                // Log the error if file copy fails
                logger.WriteLog(Path.GetFileName(file), file, targetFile, 0, 0, 0, true);
                Console.WriteLine($"Error copying file {file}: {ex.Message}");
            }

        }
        public static bool CheckFileExtension(string fileName, string extension)
        {
            return fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }
        public bool IsBusinessSoftwareRunning()
        {
            if (!File.Exists("config.txt"))
                return false;

            var businessSoftwareList = File.ReadAllLines("config.txt")
                                           .Select(s => s.Trim().ToLower())
                                           .Where(s => !string.IsNullOrEmpty(s))
                                           .ToList();

            return businessSoftwareList.Any(software => Process.GetProcesses()
                                                                .Any(p => p.ProcessName.ToLower().Contains(software)));
        }
        public async Task ProcessLargeFileAsync(string source, string target, String nameBackup, string file, CancellationToken token)
        {
            await CompleteBackupStrategy.largeFileSemaphore.WaitAsync();
            try
            {
                await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    BackupStateJournal.UpdateProgress(nameBackup);
                    Thread.Sleep(500); // Considérez changer ceci en Task.Delay si cela n'affecte pas d'autres parties
                }, token);

                BackupFile(file, source, target);
            }
            finally
            {
                CompleteBackupStrategy.largeFileSemaphore.Release();
            }
        }

        public async Task ProcessSmallFileAsync(string source, string target, String nameBackup, string file, CancellationToken token)
        {
            await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                BackupStateJournal.UpdateProgress(nameBackup);
                Thread.Sleep(500); // De même, changez en Task.Delay si possible
            }, token);

            BackupFile(file, source, target);
        }
    }
}
