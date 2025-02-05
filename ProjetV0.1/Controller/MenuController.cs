using ProjetV0._1.Model;
using ProjetV0._1.View;
using System;
using translation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller
{
    internal class MenuController
    {
        private MenuModel model;
        private MenuView view;
        private int selectionIndex = 0;

        public MenuController(MenuModel model, MenuView view)
        {
            this.model = model;
            this.view = view;
        }

        public async Task ManageActions()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                await view.DisplayActions(model.Actions, selectionIndex);
                ConsoleKeyInfo _key = Console.ReadKey();
                switch (_key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectionIndex = (selectionIndex == 0) ? model.Actions.Count - 1 : selectionIndex - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectionIndex = (selectionIndex == model.Actions.Count - 1) ? 0 : selectionIndex + 1;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        exit = await ExecuteAction(selectionIndex);
                        break;
                }
            }
        }

        private async Task<bool> ExecuteAction(int index)
        {
            switch (index)
            {
                case 0: // Création de sauvegarde
                    await HandleBackupCreation();
                    return false;
                case 1: // Exécution de sauvegarde
                    await HandleBackupExecution();
                    return false;
                case 2: // Consulter les logs
                    view.DisplayInputPrompt("Consulting logs...");
                    Console.ReadKey();
                    return false;
                case 3: // Quitter
                    return true;
                default:
                    return false;
            }
        }

        private async Task HandleBackupCreation()
        {
           
             await model._BackupController.CreateBackup();
        }

        private async Task HandleBackupExecution()
        {
            view.DisplayInputPrompt(await Translation.Instance.Translate( "Enter the index of the backup to execute, e.g., '1-3' to execute backups 1 to 3 automatically:"));
            string indexes = Console.ReadLine();
            model._BackupController.ExecuteBackup(indexes);
        }
    }
}
