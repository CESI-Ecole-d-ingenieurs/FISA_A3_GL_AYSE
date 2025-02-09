using EasySave.Logger;
using ProjetV0._1.Controller;
using ProjetV0._1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.Strategy
{
    internal class CompleteBackupStrategy : BaseBackupStrategy
    {
        public override void ExecuteBackup(string source, string target)
        {
            DirectoryExist(target);

            var state = BackupStateJournal.ComputeState("CompleteBackup", source, target);
            backupView.DisplayProgress();

            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                BackupFile(file, source, target);
            }
        }


    }
}
