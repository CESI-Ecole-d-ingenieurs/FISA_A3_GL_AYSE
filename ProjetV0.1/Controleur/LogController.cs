﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void AddLog(Sauvegarde backup, long fileSize, double fileTransferTime)
        {
            _logger.WriteLog(backup, fileSize, fileTransferTime);
        }
    }
}
