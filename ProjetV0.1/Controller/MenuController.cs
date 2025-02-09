using ProjetV0._1.Model;
using ProjetV0._1.View;
using System;
using translation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using EasySave.Logger;

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
                case 0: // Create Backup
                    await HandleBackupCreation();
                    return false;
                case 1: // Execute Backup
                    await HandleBackupExecution();
                    return false;
                case 2: // View Logs
                    try
                    {
                      
                        string content = File.ReadAllText(GlobalVariables.LogFilePath);
                        view.DisplayInputPrompt(content);
                       // Console.WriteLine(content);
                    }
                    catch (Exception ex)
                    {
                        view.DisplayInputPrompt($"Une erreur est survenue lors de la lecture du fichier:{ex.Message}");
                     
                    }
                    
                    view.DisplayInputPrompt("Consulting logs...");
                    Console.ReadKey();
                    return false;
                case 3: // Exit
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
            model._BackupController.DisplayExistingBackups();

            if (model._BackupController.GetBackupCount() == 0)
            {
                Console.WriteLine(await Translation.Instance.Translate("Aucune sauvegarde trouvée. Retour au menu."));
                Console.ReadKey();
                return;
            }
            view.DisplayInputPrompt(await Translation.Instance.Translate("Entrez l'indice de la sauvegarde à exécuter, par ex., '1-3' pour exécuter automatiquement les sauvegardes 1 à 3 :"));
            string indexes = Console.ReadLine();
            model._BackupController.ExecuteBackup(indexes);
        }
    }
}
