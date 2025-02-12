using ProjetV0._1.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Model
{
    internal class MenuModel
    {
        public List<string> Actions { get; } = new List<string> { "Création de sauvegarde", "Exécution de sauvegarde", "Consulter les logs", "Quitter l'application" };
        public List<string> LogFormats { get; } = new List<string> { "JSON", "XML" };

        /// BackupController instance to manage backup operations.
        public BackupController _BackupController { get; set; } = new BackupController();

    }
}
