using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ModelLib;
namespace EasySave.IviewLib
{
    // Interface for the backup view
    public interface IBackupView
    {
        // This method display the state of the backups in real time.
         void DisplayProgress();
        // This method read user's inputs for the parameters of a backup.
         Task<BackupModel> UserAsk();
    }
}
