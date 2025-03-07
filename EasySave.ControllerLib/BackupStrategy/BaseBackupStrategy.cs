﻿using CryptoSoft;
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
        public async Task ProcessLargeFileAsync(string source, string target, String nameBackup, string file, CancellationToken token, Dictionary<string, bool> _isPaused = null)
        {
            
            await CompleteBackupStrategy.largeFileSemaphore.WaitAsync();
            while (_isPaused[nameBackup])
            {
                await Task.Delay(500);
            }
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

        public async Task ProcessSmallFileAsync(string source, string target, String nameBackup, string file, CancellationToken token, Dictionary<string, bool> _isPaused = null)
        {
            while (_isPaused[nameBackup])
            {
                await Task.Delay(500);
            }
            await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                BackupStateJournal.UpdateProgress(nameBackup);
                Thread.Sleep(500); // De même, changez en Task.Delay si possible
            }, token);

            BackupFile(file, source, target);
        }

        public List<IGrouping<String,String>> MakeGroupsPrior(string[] files)
        {
            var extensionPriority = File.Exists("extensions.txt") ?
           File.ReadAllLines("extensions.txt").ToList() :
            new List<string>();

            var sortedFiles = files.OrderBy(f =>
            {
                var ext = Path.GetExtension(f);
                int index = extensionPriority.IndexOf(ext);
                return index >= 0 ? index : int.MaxValue; // Les fichiers non prioritaires passent à la fin
            }).ThenBy(f => f).ToList();

            // Groupe les fichiers par priorité d'extension
            var groups = sortedFiles.GroupBy(f => Path.GetExtension(f)).ToList();
            return groups;
        }
        public async Task BackupOneFile(BackupState state, string source, string file, string target, String nameBackup, List<Task> tasks = null, Dictionary<string, bool> _isPaused = null, Dictionary<string, CancellationTokenSource> _cancellationTokens = null)
        {
            CancellationToken token = _cancellationTokens[nameBackup].Token;
            if (_cancellationTokens[nameBackup].Token.IsCancellationRequested)
            {
                state.State = "STOPPED";
                BackupStateJournal.UpdateState(state);
                return;
            }

            while (_isPaused[nameBackup])
            {
                await Task.Delay(500);
            }
            bool run = false;
            do
            {
                try
                {
                    token.ThrowIfCancellationRequested(); // Vérifie si une annulation a été demandée avant de commencer la boucle

                    if (IsBusinessSoftwareRunning())
                    { 
                        Console.WriteLine("Sauvegarde annulée : Un logiciel métier est en cours d'exécution.");
                        //File.AppendAllText(GlobalVariables.LogFilePath, $"[{DateTime.Now}] Tentative de lancement d'une sauvegarde bloquée car un logiciel métier est actif.\n");
                        state.State = "Blocked BY BUSINESS SOFTWARE";
                        BackupStateJournal.UpdateState(state);
                        run = true;
                    }
                    else
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Length / 1024.0) > GlobalVariables.maximumSize)
                        {
                            // Traitement des grands fichiers avec sémaphore
                            var task = ProcessLargeFileAsync(source, target, nameBackup, file, token,_isPaused);
                            tasks.Add(task);
                        }
                        else
                        {
                            // Traitement des petits fichiers immédiatement sans attendre
                            var task = ProcessSmallFileAsync(source, target, nameBackup, file, token, _isPaused);
                            tasks.Add(task);
                        }

                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation was canceled by user.");
                    // Logique optionnelle pour gérer l'annulation ici
                    // Par exemple, nettoyer les ressources, informer les utilisateurs, etc.
                }
            } while (run);
        }


        public static double GetNetworkUtilization()
        {
            try
            {
                const string categoryName = "Network Interface";
                const string counterName = "Bytes Total/sec"; // Ce compteur mesure le total des bytes envoyés et reçus par seconde.
                const string instanceName = "Intel[R] Wi-Fi 6 AX201 160MHz"; //  le nom de votre interface réseau.

                PerformanceCounter performanceCounter = new PerformanceCounter(categoryName, counterName, instanceName);
                float bytesPerSec = performanceCounter.NextValue(); // Première lecture souvent à 0
                System.Threading.Thread.Sleep(1000); // Attendre une seconde
                bytesPerSec = performanceCounter.NextValue(); // Deuxième lecture pour obtenir la valeur actuelle

                // Convertir en pourcentage d'utilisation (exemple fictif)
                // Supposons que vous ayez une bande passante maximale de 100 Mbps
                double maxBandwidth = 100 * 1024 * 1024 / 8; // Convertir en bytes par seconde
                double networkUtilization = (bytesPerSec / maxBandwidth) * 100;

                return networkUtilization;
            }
            catch (Exception ex)
            {
                // Gérer l'exception ou retourner une valeur par défaut
                Console.WriteLine("Erreur lors de l'obtention de la charge réseau: " + ex.Message);
                return 0.0; // Retourner 0 ou toute autre valeur appropriée en cas d'erreur
            }
        }

    }
}
