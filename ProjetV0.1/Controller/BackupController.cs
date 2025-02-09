using ProjetV0._1.Controller.BackupFactory;
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
            //Console.WriteLine("Indice", sauvegardesIndice);
            foreach (var index in BackupIndex)
            {
                if (index - 1 < BackupList.Count && index > 0)
                {
                    if (BackupList[index - 1].Type == "Complète")
                    {
                        _BackupStrategyFactory = new CompleteBackupFactory() ;
                        _BackupStrategyFactory.CreateBackupStrategy().ExecuteBackup(BackupList[index - 1].Source, BackupList[index - 1].Target);
                    }
                    else
                    {
                        _BackupStrategyFactory = new DifferentialBackupFactory();
                        _BackupStrategyFactory.CreateBackupStrategy().ExecuteBackup(BackupList[index - 1].Source, BackupList[index - 1].Target);
                    }
                }
            }

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
