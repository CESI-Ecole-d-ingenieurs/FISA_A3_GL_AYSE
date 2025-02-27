using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ModelLib;
namespace EasySave.IviewLib
{
    // Interface used to manage backups.
    public interface IBackupView
    {
         void DisplayProgress();
         Task<BackupModel> UserAsk();
    }
}
