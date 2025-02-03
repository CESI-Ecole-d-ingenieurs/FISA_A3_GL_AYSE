using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjetV0._1
{
    internal class Sauvegarde
    {
        public string Nom { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Type { get; set; }
        public Sauvegarde(string nom, string source, string destination, string type)
        {
            Nom = nom;  
            Source = source;
            Destination = destination;
            Type = type;
        }
    }
}
