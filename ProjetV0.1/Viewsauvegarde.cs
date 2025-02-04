using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;

namespace ProjetV0._1
{
    internal class Viewsauvegarde
    {
        
        public async Task<Sauvegarde> UserAsk(Translation translation)
        {
            
            Console.WriteLine(await translation.Translate("Entrez le nom de la sauvegarde :"));
            string name = Console.ReadLine();
            Console.Clear();

            Console.WriteLine(await translation.Translate("Entrez la source de la sauvegarde :"));
            string source = Console.ReadLine();
            Console.Clear();

            Console.WriteLine(await translation.Translate("Entrez la destination de la sauvegarde :"));
            string destination = Console.ReadLine();
            Console.Clear();

            string type = "";
            List<string> types = new List<string> { await translation.Translate("Complète"), await translation.Translate("Différentielle") };
            int selectionIndex = 0;
            bool exit = false;

            while (!exit)
            {
               // Console.Clear();
                Console.WriteLine(await translation.Translate("Choisissez le type de la sauvegarde :"));

                for (int i = 0; i < types.Count(); i++)
                {
                    if (i == selectionIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {types[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {types[i]}");
                    }
                }

                ConsoleKeyInfo fleche = Console.ReadKey();
                switch (fleche.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectionIndex = (selectionIndex == 0) ? types.Count() - 1 : selectionIndex - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectionIndex = (selectionIndex == types.Count() - 1) ? 0 : selectionIndex + 1;
                        break;

                    case ConsoleKey.Enter:
                        Console.Clear();
                        switch (selectionIndex)
                        {
                            case 0:
                                type = "Complète";
                                exit = true;
                                break;

                            case 1:
                                type = "Différentielle";
                                exit = true;
                                break;
                        }
                        break;
                }
            }
            Sauvegarde _sauvegarde =new Sauvegarde(name,source,destination,type);
            return _sauvegarde;
        }
    
}
}
