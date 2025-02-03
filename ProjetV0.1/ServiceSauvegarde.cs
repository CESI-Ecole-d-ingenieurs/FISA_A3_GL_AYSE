using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1
{
    internal class ServiceSauvegarde
    {
        private StrategieSauvegarde _StrategieSauvegarde;
        public ServiceSauvegarde(StrategieSauvegarde strategieSauvegarde)
        {
            _StrategieSauvegarde = strategieSauvegarde;
        }
        public void ExecuteSauvegarde(String Source, String Destination)
        {
            _StrategieSauvegarde.ExecuterSauvegarde(Source, Destination);
        }
    }
}
