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
       public static String   Key="5";
        /// Static variable that stores the path of the log file.
        /// It retrieves the log file name from the Logger class DLL.
        public static string LogFilePath { get; set; }=Logger.GetLogFileName();

        private static String BasePath = Environment.GetEnvironmentVariable("EASYSAVE_Backup_PATH") ??
                   Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CESI", "EasySave", "Backup");
        public static string PathBackup = Path.Combine(BasePath, "Backup.txt") ;
        public static string PathTempsReel = Path.Combine(BasePath, "backup_state.json");




    }
}
