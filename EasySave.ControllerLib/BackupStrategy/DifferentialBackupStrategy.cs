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

            var extensionPriority = File.Exists("extensions.txt") ?
        File.ReadAllLines("extensions.txt").ToList() :
        new List<string>();

            var sortedFiles = files.OrderBy(f =>
            {
                var ext = Path.GetExtension(f);
                int index = extensionPriority.IndexOf(ext);
                return index >= 0 ? index : int.MaxValue; // Les fichiers non prioritaires passent à la fin
            }).ThenBy(f => f).ToList();

            foreach (var file in sortedFiles)
            {
                var targetFile = file.Replace(source, target);

                DateTime lastBackupTime = await DateOfLastBackup(Logger.Logger.GetLogFileName(), file, targetFile);
                var fileInfo = new FileInfo(file);

                if (fileInfo.LastWriteTimeUtc > lastBackupTime)
                {
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
                                    await Task.Run(() =>
                                    {
                                        token.ThrowIfCancellationRequested(); // Vérifie à nouveau avant d'exécuter des opérations longues
                                        BackupStateJournal.UpdateProgress(nameBackup); // Real-time update
                                        Thread.Sleep(500); // Slow down the process for better visualization
                                    }, token); // Passez le token ici aussi pour permettre l'annulation pendant l'exécution de Task.Run

                                    BackupFile(file, source, target);
                                    run = false;
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
                    }
                }
        }

        /// Determines the last backup date of a given file based on the log history.
        /// It searches the log file to find the most recent backup date of the given file.
        private async Task<DateTime> DateOfLastBackup(string logFile, string source, string target)
        {
            DateTime lastBackupTime = DateTime.MinValue;

            // Vérifie si le fichier log n'existe pas ou est vide
            if (!File.Exists(logFile) || new FileInfo(logFile).Length == 0)
            {
                //Console.WriteLine(" Aucun log trouvé, la sauvegarde différentielle copiera tous les fichiers.");
                return DateTime.MinValue; // Force la copie de tous les fichiers
            }

            string jsonLog = File.ReadAllText(logFile);
            if (string.IsNullOrWhiteSpace(jsonLog))
            {
                //Console.WriteLine(" Fichier log vide, la sauvegarde différentielle copiera tous les fichiers.");
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
