using ProjetV0._1.Controller.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.BackupFactory
{
    public interface BackupStrategyFactory
    {
        BackupStrategy CreateBackupStrategy();
    } 
    public class CompleteBackupFactory : BackupStrategyFactory
    {
        public BackupStrategy CreateBackupStrategy()
        {
            return new CompleteBackupStrategy();
        }
    }
    public class DifferentialBackupFactory : BackupStrategyFactory
    {
        public BackupStrategy CreateBackupStrategy()
        {
            return new DifferentialBackupStrategy();
        }
    }
}
