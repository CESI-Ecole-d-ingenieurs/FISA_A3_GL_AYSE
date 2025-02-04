using ProjetV0._1.Controleur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;
namespace ProjetV0._1.Modele
{
    internal class TranslateModel
    {
        private static Translation _instance;
        public static readonly object _lock = new object();
        public ITranslateStrategy _strategy = new English();

    }
}
