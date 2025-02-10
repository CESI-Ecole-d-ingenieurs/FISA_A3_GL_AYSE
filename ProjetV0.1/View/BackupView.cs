using ProjetV0._1.Controller;
using ProjetV0._1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;

namespace ProjetV0._1.View
{
    /// Displays the progress of the backup process.
    /// Retrieves the backup states and displays their progress percentage.
    internal class BackupView
    {
        public void DisplayProgress()
        {
            var states = BackupStateJournal.GetState();
            Console.Clear();
            Console.WriteLine("Mise à jour de la progression..."); // Check

            Console.WriteLine("Progression des sauvegardes :");
            foreach (var state in states)
            {
                Console.WriteLine($"{state.Name} : {state.Progress}% - {state.State}");
            }
        }

        /// Asks the user for backup details (name, source, destination, and type).
        /// Provides an interactive menu to select between "Complete" and "Differential" backup.
        public async Task<BackupModel> UserAsk()
        {

            Console.WriteLine(await Translation.Instance.Translate("Entrez le nom de la sauvegarde :"));
            string name = Console.ReadLine();
            Console.Clear();

            Console.WriteLine(await Translation.Instance.Translate("Entrez la source de la sauvegarde :"));
            string source = Console.ReadLine();
            Console.Clear();

            Console.WriteLine(await Translation.Instance.Translate("Entrez la destination de la sauvegarde :"));
            string destination = Console.ReadLine();
            Console.Clear();

            string type = "";
            List<string> types = new List<string> { await Translation.Instance.Translate("Complète"), await Translation.Instance.Translate("Différentielle") };
            int selectionIndex = 0;
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine(await Translation.Instance.Translate("Choisissez le type de la sauvegarde :"));

                // Display the list of backup types with a selection cursor
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

                // Handle user input for navigating the selection menu
                ConsoleKeyInfo fleche = Console.ReadKey();
                switch (fleche.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectionIndex = selectionIndex == 0 ? types.Count() - 1 : selectionIndex - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectionIndex = selectionIndex == types.Count() - 1 ? 0 : selectionIndex + 1;
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
            // Create and return a BackupModel object with user input
            BackupModel _backup = new BackupModel(name, source, destination, type);
            return _backup;
        } 

    }
}
