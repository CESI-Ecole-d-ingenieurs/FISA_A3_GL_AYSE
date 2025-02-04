using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectV0._1.Models;
using ProjetV0._1;

namespace ProjetV0._1.Controleur
{
    public class LogController
    {
        private readonly Logger _logger;

        public LogController()
        {
            _logger = Logger.Instance;
        }

        public void AddLog(Backup backup, long fileSize, double fileTransferTime, bool isError = false)
        {
            _logger.WriteLog(backup, fileSize, fileTransferTime, isError);
        }
    }
}
