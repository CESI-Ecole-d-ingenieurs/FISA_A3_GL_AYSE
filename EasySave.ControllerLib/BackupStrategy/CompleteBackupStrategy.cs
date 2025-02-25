using EasySave.IviewLib;
using EasySave.ModelLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

            //backupView.DisplayProgress();

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
          
            // Groupe les fichiers par priorité d'extension
            var groups = sortedFiles.GroupBy(f => Path.GetExtension(f)).ToList();
            foreach (var group in groups)
            {
                List<Task> tasks = new List<Task>();
                foreach (var file in group)
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
                                //var fileInfo = new FileInfo(file);
                                //if ((fileInfo.Length / 1024.0) > 40)
                                //{
                                //    await CompleteBackupStrategy.largeFileSemaphore.WaitAsync();
                                //    Console.WriteLine("Entered semaphore.");
                                //    try
                                //    {
                                //        await Task.Run(() =>
                                //         {
                                //             token.ThrowIfCancellationRequested(); // Vérifie à nouveau avant d'exécuter des opérations longues
                                //             BackupStateJournal.UpdateProgress(nameBackup); // Real-time update
                                //             Thread.Sleep(500); // Slow down the process for better visualization
                                //         }, token); // Passez le token ici aussi pour permettre l'annulation pendant l'exécution de Task.Run

                                //        BackupFile(file, source, target);
                                //        run = false;
                                //    }
                                //    catch (OperationCanceledException)
                                //    {
                                //        Console.WriteLine("Operation was cancelled.");
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        Console.WriteLine($"An exception occurred: {ex.Message}");
                                //    }
                                //    finally
                                //    {
                                //        CompleteBackupStrategy.largeFileSemaphore.Release();
                                //    }
                                //}
                                //else
                                //{

                                //    await Task.Run(() =>
                                //    {
                                //        token.ThrowIfCancellationRequested(); // Vérifie à nouveau avant d'exécuter des opérations longues
                                //        BackupStateJournal.UpdateProgress(nameBackup); // Real-time update
                                //        Thread.Sleep(500); // Slow down the process for better visualization
                                //    }, token); // Passez le token ici aussi pour permettre l'annulation pendant l'exécution de Task.Run

                                //    BackupFile(file, source, target);
                                //    run = false;
                                //}
                                FileInfo fileInfo = new FileInfo(file);

                                if ((fileInfo.Length / 1024.0) > 40)
                                {
                                    // Traitement des grands fichiers avec sémaphore
                                    var task = ProcessLargeFileAsync(source, target, nameBackup, file, token);
                                    tasks.Add(task);
                                }
                                else
                                {
                                    // Traitement des petits fichiers immédiatement sans attendre
                                    var task = ProcessSmallFileAsync(source, target, nameBackup, file, token);
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

                    //bool run = false;
                    //do
                    //{
                    //    if (IsBusinessSoftwareRunning() )
                    //    {
                    //        Console.WriteLine("Sauvegarde annulée : Un logiciel métier est en cours d'exécution.");
                    //        //File.AppendAllText(GlobalVariables.LogFilePath, $"[{DateTime.Now}] Tentative de lancement d'une sauvegarde bloquée car un logiciel métier est actif.\n");
                    //        state.State = "Blocked BY BUSINESSS SOFTWARE";
                    //        BackupStateJournal.UpdateState(state);
                    //        run = true;
                    //    }
                    //    else
                    //    {
                    //        await Task.Run(() =>
                    //    {
                    //        BackupStateJournal.UpdateProgress(nameBackup); // Real-time update

                    //        Thread.Sleep(500); // Slow down the process for better visualization
                    //    }
                    //        );
                    //        BackupFile(file, source, target);

                    //        run = false;
                    //    }
                    //}while(run);
                }
                await Task.WhenAll(tasks);
            }
        }


    }

}
