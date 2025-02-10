using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Logger;

namespace ProjetV0._1.Controller.Strategy
{
    /// Defines the interface for backup strategies.
    /// Each backup strategy must implement the ExecuteBackup method.
    public interface BackupStrategy
    {
        /// Executes a backup operation from the source directory to the target directory.
        /// Implementations of this method define the specific backup logic.
        public void ExecuteBackup(string Source, string Target);
    }


}
