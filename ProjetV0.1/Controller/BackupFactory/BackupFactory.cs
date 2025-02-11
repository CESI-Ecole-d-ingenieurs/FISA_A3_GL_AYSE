using ProjetV0._1.Controller.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.BackupFactory
{
    /// Factory interface for creating backup strategies.
    /// Defines a method to generate a backup strategy.
    public interface BackupStrategyFactory
    {
        /// Creates and returns an instance of a backup strategy.
        BackupStrategy CreateBackupStrategy();
    }


    /// Implements the BackupStrategyFactory interface.
    public class CompleteBackupFactory : BackupStrategyFactory
    {
        public BackupStrategy CreateBackupStrategy()
        {
            return new CompleteBackupStrategy();
        }
    }

    /// Implements the BackupStrategyFactory interface.
    public class DifferentialBackupFactory : BackupStrategyFactory
    {
        public BackupStrategy CreateBackupStrategy()
        {
            return new DifferentialBackupStrategy();
        }
    }
}
