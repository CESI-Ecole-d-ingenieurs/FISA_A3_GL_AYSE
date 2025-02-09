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
        protected Logger logger = new Logger();
        public static string LogFilePath { get; set; }=Logger.GetLogFileName();
    
    }
}
