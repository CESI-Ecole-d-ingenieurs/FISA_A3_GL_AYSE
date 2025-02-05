using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controleur.Strategy
{
    public interface BackupStrategy
    {
        public void ExecuteBackup(string Source, string Target);
    }


}
