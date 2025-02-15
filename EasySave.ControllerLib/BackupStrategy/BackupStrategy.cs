using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ControllerLib.BackupStrategy
{
    /// Defines the interface for backup strategies.
    public interface BackupStrategy
    {
        /// Implementations of this method define the specific backup logic.
         void ExecuteBackup(string Source, string Target);
    }
}
