using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1
{
    public interface UsineSauvegarde
    {
         StrategieSauvegarde CreerStrategieSauvegarde();
    }
    public class UsineSauvegardeComplete :UsineSauvegarde
    {
        public StrategieSauvegarde CreerStrategieSauvegarde()
        {
            return new StrategieSauvegardeComplete();
        }
    }
    public class UsineSauvegardeDiff : UsineSauvegarde
    {
        public StrategieSauvegarde CreerStrategieSauvegarde()
        {
            return new StrategieSauvegardeDiff();
        }
    }
}
