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
        private MenuModel model;  // Stores the menu model containing available actions
        private MenuView view;    // Handles user interface interactions
        private int selectionIndex = 0;  // Keeps track of the current selected action

        /// Constructor to initialize the menu controller with a model and view.
        public MenuController(MenuModel model, MenuView view)
        {
            this.model = model;
            this.view = view;
        }

        /// Manages the main menu actions, allowing the user to navigate and execute tasks.
        public async Task ManageActions()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                await view.DisplayActions(model.Actions, selectionIndex);// Display available menu actions
                ConsoleKeyInfo _key = Console.ReadKey();
                // Handle user key input for navigation
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
                        exit = await ExecuteAction(selectionIndex); // Execute the selected action
                        break;
                }
            }
        }

        /// Executes the action corresponding to the selected menu index.
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
                      
                        string content = File.ReadAllText(GlobalVariables.LogFilePath); // Retrieve log content
                        view.DisplayInputPrompt(content); // Display logs to the user
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

        /// Handles the backup creation process by invoking the BackupController.
        private async Task HandleBackupCreation()
        {
           
             await model._BackupController.CreateBackup();
        }

        /// Handles the backup execution process by displaying available backups and executing the selected one.
        private async Task HandleBackupExecution()
        {
            model._BackupController.DisplayExistingBackups(); // Show available backups to the user
            view.DisplayInputPrompt(await Translation.Instance.Translate("Entrez l'indice de la sauvegarde à exécuter, par ex., '1-3' pour exécuter automatiquement les sauvegardes 1 à 3 :"));
            string indexes = Console.ReadLine();
            model._BackupController.ExecuteBackup(indexes); // Execute the selected backup
        }
    }
}
