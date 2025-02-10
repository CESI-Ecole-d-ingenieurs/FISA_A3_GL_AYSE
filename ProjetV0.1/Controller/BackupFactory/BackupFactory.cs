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

    /// Factory class responsible for creating complete backup strategies.
    /// Implements the BackupStrategyFactory interface.
    public class CompleteBackupFactory : BackupStrategyFactory
    {
        /// Creates and returns a new instance of the CompleteBackupStrategy.
        public BackupStrategy CreateBackupStrategy()
        {
            return new CompleteBackupStrategy();
        }
    }

    /// Factory class responsible for creating differential backup strategies.
    /// Implements the BackupStrategyFactory interface.
    public class DifferentialBackupFactory : BackupStrategyFactory
    {
        /// Creates and returns a new instance of the DifferentialBackupStrategy.
        public BackupStrategy CreateBackupStrategy()
        {
            return new DifferentialBackupStrategy();
        }
    }
}
