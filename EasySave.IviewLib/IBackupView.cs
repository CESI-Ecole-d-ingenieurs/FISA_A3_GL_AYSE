using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ModelLib;
namespace EasySave.IviewLib
{
    public interface IBackupView
    {
         void DisplayProgress();
         Task<BackupModel> UserAsk();
    }
}
