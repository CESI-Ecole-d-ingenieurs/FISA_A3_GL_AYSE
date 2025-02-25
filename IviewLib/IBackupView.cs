using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IviewLib
{
    public interface IBackupView
    {
        public void DisplayProgress();
        public Task<BackupModel> UserAsk();
    }
}
