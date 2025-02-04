using ProjetV0._1.Controleur.BackupFactory;
using ProjetV0._1.Controleur.Strategy;
using ProjetV0._1.Modele;
using ProjetV0._1.Vue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controleur
{
    internal class BackupController
    {
        private List<BackupModel> sauvegardeList = new List<BackupModel>();
        private BackupStrategyFactory _BackupStrategyFactory;
        private BackupView _backupView = new BackupView();
        //private StrategieSauvegarde _StrategieSauvegarde;
        //public GestionnaireDeSauvegarde(StrategieSauvegarde strategieSauvegarde)
        //{
        //    _StrategieSauvegarde = strategieSauvegarde;
        //}
        public void ExecuteSauvegarde(string input)
        {
            Console.WriteLine("INPUT", input);
            List<int> sauvegardesIndice = ParseJobIndices(input);
            Console.WriteLine("Indice", sauvegardesIndice);
            foreach (var index in sauvegardesIndice)
            {
                if (index - 1 < sauvegardeList.Count && index > 0)
                {
                    if (sauvegardeList[index - 1].Type == "Complète")
                    {
                        _BackupStrategyFactory = new CompleteBackupFactory() ;
                        _BackupStrategyFactory.CreateBackupStrategy().ExecuteBackup(sauvegardeList[index - 1].Source, sauvegardeList[index - 1].Destination);
                    }
                    else
                    {
                        _BackupStrategyFactory = new DifferentialBackupFactory();
                        _BackupStrategyFactory.CreateBackupStrategy().ExecuteBackup(sauvegardeList[index - 1].Source, sauvegardeList[index - 1].Destination);
                    }
                }
            }

        }
        public async Task CreateBackup()
    {
            if (sauvegardeList.Count >= 5)
        {
                Console.WriteLine("Maximum  5 Sauvegarde");
                return;
        }
            BackupModel sauvegarde = await _backupView.UserAsk();
            sauvegardeList.Add(sauvegarde);
            Console.WriteLine($"Sauvegarde'{sauvegarde.Nom}' ajouté.");
        }
        //focntion indice 
        public List<int> ParseJobIndices(string input)
        {
            var indices = new List<int>();
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
                            indices.Add(i);
                        }
                    }
                }
                else if (int.TryParse(part, out int singleIndex))
                {
                    indices.Add(singleIndex);
                }
            }
            foreach (int indice in indices)
        {
                Console.WriteLine(indice);
            }

            return indices;
        }
    }
}
