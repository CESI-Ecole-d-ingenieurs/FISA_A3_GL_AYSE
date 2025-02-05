using ProjetV0._1.Controleur.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controleur.BackupFactory
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
