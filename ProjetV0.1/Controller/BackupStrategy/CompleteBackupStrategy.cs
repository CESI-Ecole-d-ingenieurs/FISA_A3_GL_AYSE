using EasySave.Logger;
using ProjetV0._1.Controller.BackupStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.Strategy
{
    internal class CompleteBackupStrategy : BaseBackupStrategy
    {
      
        public override  void ExecuteBackup(string Source, string Target)
        {
            //Console.WriteLine($"Sauvegarde de {Source} à {Target}.");

            // Si destination n'existe pas cette focntion.Net le crée
            DirectoryExist(Target);

            // Copiez tous les répertoires et les sous-répertoires même vides
            foreach (var directory in Directory.GetDirectories(Source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(Source, Target);
                DirectoryExist(targetDirectory);
            }

            // Puis copiez tous les fichiers
            foreach (var file in Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories))
            {
                BackupFile(file, Source, Target);
            }

           // Console.WriteLine("Sauvegarde Complete reussi");
        }

        // Méthode pour s'assurer que le répertoire existe
    

    }


}
