using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ModelLib
{
    public interface IObserver
    {
        /// Updates the observer with the latest backup state.
        void Update(BackupState state);
    }
}
