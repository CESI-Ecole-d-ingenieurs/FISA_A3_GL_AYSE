using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1
{
    public interface StrategieSauvegarde
    {
        public void ExecuterSauvegarde(String Source, String Destination);
    }

    public class StrategieSauvegardeComplete : StrategieSauvegarde
    {
        public void ExecuterSauvegarde(String Source, String Destination)
        {
                Console.WriteLine($"Sauvegarde de {Source} à {Destination}.");

            // Si destination n'existe pas cette focntion.Net le crée
                RepertoireExiste(Destination);

                // Copiez tous les répertoires et les sous-répertoires même vides
                foreach (var directory in Directory.GetDirectories(Source, "*", SearchOption.AllDirectories))
                {
                    var targetDirectory = directory.Replace(Source, Destination);
                    RepertoireExiste(targetDirectory);
                }

                // Puis copiez tous les fichiers
                foreach (var file in Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories))
                {
                    var targetFile = file.Replace(Source, Destination);
                    File.Copy(file, targetFile, true);
                }

                Console.WriteLine("Sauvegarde Complete reussi");
            }

            // Méthode pour s'assurer que le répertoire existe
            private void RepertoireExiste(string path)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        
    }

    }
