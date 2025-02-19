using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ModelLib
{
    // Represents the action and format selection model for the application.
    public class MenuModel
    {
        public List<string> Actions { get; } = new List<string> { "Création de sauvegarde", "Exécution de sauvegarde", "Consulter les logs", "Quitter l'application" };
        public List<string> LogFormats { get; } = new List<string> { "JSON", "XML" };
    }
}
