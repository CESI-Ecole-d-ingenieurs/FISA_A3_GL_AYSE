using ProjetV0._1.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controleur
{
    public class BackupController
    {
        public BackupState CreateBackup(string name, string source, string target)
        {
            var state = BackupStateJournal.ComputeState(name, source, target);
            BackupStateJournal.UpdateState(state);
            return state;
        }

        public void UpdateProgress(string name)
        {
            BackupStateJournal.UpdateProgress(name);
        }
    }
}
