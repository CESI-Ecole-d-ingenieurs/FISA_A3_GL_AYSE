using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Logger;
using System.Reflection;
using EasySave.IviewLib;
using EasySave.ModelLib;
namespace EasySave.ControllerLib
{

    public class MenuController
    {
        private MenuModel model;  // Stores the menu model containing available actions
        private IMenuView view;    // Handles user interface interactions
        private int selectionIndex = 0;  // Keeps track of the current selected action

        /// Constructor to initialize the menu controller with a model and view.
        public MenuController(MenuModel model, IMenuView view)
        {
            this.model = model;
            this.view = view;
        }

        /// Manages the main menu actions, allowing the user to navigate and execute tasks.
        public async Task ManageActions(IBackupView backupView, EasySave.ModelLib.IObserver consoleView)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                await view.DisplayActions(model.Actions, selectionIndex); // Display available menu actions
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
                      
                        exit = await ExecuteAction(selectionIndex,backupView, consoleView); // Execute the selected action
                        break;
                }
            }
        }

        /// This method handle the choice of the user for the log file format
        public async Task ChoisirFroamtLog()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                await view.DisplayActions(model.LogFormats, selectionIndex);// Display available menu actions
                ConsoleKeyInfo _key = Console.ReadKey();
                // Handle user key input for navigation
                switch (_key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectionIndex = (selectionIndex == 0) ? model.LogFormats.Count - 1 : selectionIndex - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectionIndex = (selectionIndex == model.LogFormats.Count - 1) ? 0 : selectionIndex + 1;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        exit = await HandleLogFormat(selectionIndex); // Execute the selected action
                        break;
                }
            }
        }

        /// Executes the action corresponding to the selected menu index.
        private async Task<bool> ExecuteAction(int index, IBackupView backupView, EasySave.ModelLib.IObserver consoleView)
        {
            switch (index)
            {
                case 0: // Create Backup
                    
                    await HandleBackupCreation(backupView);
                    return false;
                case 1: // Execute Backup
                  
                    await HandleBackupExecution(backupView, consoleView);
                    return false;
                case 2: // View Logs
                    try
                    {

                        string content = File.ReadAllText(GlobalVariables.LogFilePath); // Retrieve log content
                        view.DisplayInputPrompt(content); // Display logs to the user
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
        private async Task HandleBackupCreation(IBackupView backupView)
        {
            BackupController backupController = new BackupController(backupView);
            await backupController.CreateBackup();
        }

        /// Handles the backup execution process by displaying available backups and executing the selected one.
        private async Task HandleBackupExecution(IBackupView backupView, EasySave.ModelLib.IObserver consoleView)
        {
            BackupController backupController = new BackupController(backupView);
            view.DisplayInputPrompt(await Translation.Instance.Translate("Choisissez le type du fichier log que vous voulez créér"));
            await ChoisirFroamtLog();
             backupController.DisplayExistingBackups(); // Show available backups to the user
            view.DisplayInputPrompt(await Translation.Instance.Translate("Entrez l'indice de la sauvegarde à exécuter, par ex., '1-3' pour exécuter automatiquement les sauvegardes 1 à 3 :"));
            string indexes = Console.ReadLine();
            GlobalVariables.CryptedFileExt = CryptedFileFormat();
            await backupController.ExecuteBackupAsync(indexes, consoleView); // Execute the selected backup
        }

        // This method ask and get the file extensions that the user wants to crypt
        private string[] CryptedFileFormat()
        {
            Console.WriteLine("Entrez les extensions de fichiers à sauvegarder (séparées par une virgule):");
            string input = Console.ReadLine();
            string[] extensions = input.Split(',');
            return extensions;
        }

        // This method handle the choice of the user for the log file format
        public async Task<bool> HandleLogFormat(int index)
        {
            switch (index)
            {
                case 0: //JSON
                    await AddBackupExtension(".json");
                    return true;
                case 1: // XML
                    await AddBackupExtension(".xml");
                    return true;
                default:
                    return false;
            }
        }

        // This method add an extension to the logs files based on the user's choice
        private async Task AddBackupExtension(String ext)
        {
            GlobalVariables.LogFilePath = Path.ChangeExtension(GlobalVariables.LogFilePath, ext);
            GlobalVariables.PathTempsReel = Path.ChangeExtension(GlobalVariables.PathTempsReel, ext);
        }
    }
}
