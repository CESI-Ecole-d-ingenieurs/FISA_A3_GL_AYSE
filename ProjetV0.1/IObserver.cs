using ProjetV0._1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1
{
    public interface IObserver
    {
        /// Updates the observer with the latest backup state.
        void Update(BackupState state);
    }
}
