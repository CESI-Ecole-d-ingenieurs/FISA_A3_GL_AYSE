using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EasySave.ControllerLib.BackupStrategy
{
    /// Defines the interface for backup strategies.
    public interface BackupStrategy
    {
        /// Implementations of this method define the specific backup logic.
         Task ExecuteBackup(string source, string target, String nameBackup, Dictionary<string, bool> _isPaused = null, Dictionary<string, CancellationTokenSource> _cancellationTokens = null) ;
    }
}