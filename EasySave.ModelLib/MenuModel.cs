using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ModelLib
{
    public  class MenuModel
    {
        // Store the actions to display in the menu (version 1.0)
        public List<string> Actions { get; } = new List<string> { "Création de sauvegarde", "Exécution de sauvegarde", "Consulter les logs", "Quitter l'application" };
        // Store the extensions of the log file to display in the menu (version 1.1)
        public List<string> LogFormats { get; } = new List<string> { "JSON", "XML" };
    }
}
