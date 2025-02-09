﻿using ProjetV0._1.Controller.BackupFactory;
using ProjetV0._1.Controller.Strategy;
using ProjetV0._1.Model;
using ProjetV0._1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;

namespace ProjetV0._1.Controller
{
    internal class BackupController
    {
        private List<BackupModel> BackupList = new List<BackupModel>();
        private BackupStrategyFactory _BackupStrategyFactory;
        private BackupView _backupView = new BackupView();
        //private StrategieSauvegarde _StrategieSauvegarde;
        //public GestionnaireDeSauvegarde(StrategieSauvegarde strategieSauvegarde)
        //{
        //    _StrategieSauvegarde = strategieSauvegarde;
        //}
        public void ExecuteBackup(string input)
        {
            List<int> BackupIndex = ParseJobIndex(input);
            BackupStateJournal.AddObserver(new ConsoleView()); // 🔹 Ajout de ConsoleView pour afficher la progression

            foreach (var index in BackupIndex)
            {
                if (index - 1 < BackupList.Count && index > 0)
                {
                    BackupModel backup = BackupList[index - 1];
                    _BackupStrategyFactory = backup.Type == "Complète"
                        ? new CompleteBackupFactory()
                        : new DifferentialBackupFactory();

                    var strategy = _BackupStrategyFactory.CreateBackupStrategy();

                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    BackupState state = BackupStateJournal.ComputeState(backup.Name, backup.Source, backup.Target);
                    BackupStateJournal.UpdateState(state);

                    string[] files = Directory.GetFiles(backup.Source, "*", SearchOption.AllDirectories);
                    int totalFiles = files.Length;
                    int processedFiles = 0;

                    foreach (var file in files)
                    {
                        string destFile = file.Replace(backup.Source, backup.Target);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                        File.Copy(file, destFile, true);

                        processedFiles++;
                        BackupStateJournal.UpdateProgress(backup.Name); // 🔹 Mise à jour en temps réel
                        Thread.Sleep(500); // 🔹 Ralentissement du programme pour voir la progression
                    }

                    stopwatch.Stop();
                    state.Progress = 100;
                    state.State = "Terminé";
                    BackupStateJournal.UpdateState(state);

                    Console.WriteLine($"Sauvegarde {backup.Name} terminée en {stopwatch.Elapsed.TotalSeconds} secondes.");
                }
            }
        }

        public async Task DisplayExistingBackups()
        {
            Console.WriteLine(await Translation.Instance.Translate(" Sauvegardes disponibles :"));
            for (int i = 0; i < BackupList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {BackupList[i].Name} ({BackupList[i].Type})");
            }
        }

        public int GetBackupCount()
        {
            return BackupList.Count;
        }

        public async Task CreateBackup()
    {
            if (BackupList.Count >= 5)
        {
                Console.WriteLine(await Translation.Instance.Translate("Maximum  5 Sauvegarde"));
                return;
        }
            BackupModel sauvegarde = await _backupView.UserAsk();
            BackupList.Add(sauvegarde);
            Console.WriteLine(await Translation.Instance.Translate($"Sauvegarde'{sauvegarde.Name}' ajouté."));
        }
        //focntion indice 
        public List<int> ParseJobIndex(string input)
        {
            var Indexes = new List<int>();
            var parts = input.Split(';');

            foreach (var part in parts)
            {
                if (part.Contains("-")) 
                {
                    var rangeParts = part.Split('-');
                    if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int start) && int.TryParse(rangeParts[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            Indexes.Add(i);
                        }
                    }
                }
                else if (int.TryParse(part, out int singleIndex))
                {
                    Indexes.Add(singleIndex);
                }
            }
        //    foreach (int Index in Indexes)
        //{
        //        Console.WriteLine(Index);
        //    }

            return Indexes;
        }
    }
}
