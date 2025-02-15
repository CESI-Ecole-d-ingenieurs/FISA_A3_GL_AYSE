using ProjetV0._1.Controller.Strategy;
using ProjetV0._1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.IviewLib;
namespace ProjetV0._1.Controller.BackupFactory;


/// Factory interface for creating backup strategies.
/// Defines a method to generate a backup strategy.
public interface BackupStrategyFactory
{
    /// Creates and returns an instance of a backup strategy.
    BackupStrategy CreateBackupStrategy(IBackupView backupview);
}


/// Implements the BackupStrategyFactory interface.
public class CompleteBackupFactory : BackupStrategyFactory
{
    public BackupStrategy CreateBackupStrategy(IBackupView backupview)
    {
        return new CompleteBackupStrategy(backupview);
    }
}

/// Implements the BackupStrategyFactory interface.
public class DifferentialBackupFactory : BackupStrategyFactory
{
    public BackupStrategy CreateBackupStrategy(IBackupView backupview)
    {
        return new DifferentialBackupStrategy(backupview);
    }
}
