using EasySave.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Model
{
    internal class GlobalVariables
    {
        /// Instance of the Logger class to manage logging operations.
        protected Logger logger = new Logger();

        /// Static variable that stores the path of the log file.
        /// It retrieves the log file name from the Logger class DLL.
        public static string LogFilePath { get; set; }=Logger.GetLogFileName();
        public static string PathBackup = Path.Combine(Environment.GetEnvironmentVariable("EASYSAVE_Backup_PATH") ??
                   Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CESI", "EasySave", "Backup"), "Backup.txt") ;
       



    }
}
