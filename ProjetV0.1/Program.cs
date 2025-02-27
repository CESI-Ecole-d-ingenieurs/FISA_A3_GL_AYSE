// See https://aka.ms/new-console-template for more information
using ProjetV0._1;

using System;


using EasySave.ModelLib;
using ProjetV0._1.View;
using EasySave.IviewLib;
using EasySave.ControllerLib;
class Program
{
    static async Task Main(string[] args)
    {
        // Initialize language selection
        LanguageModel languageModel = new LanguageModel();
        ILanguageView view = new LanguageView();
        LanguageController languages_menu = new LanguageController(languageModel, view);
        languages_menu.NavigateLanguages(); // User selects the language

        // Initialize the menu system
        MenuModel menuModel = new MenuModel();
        IMenuView menuView = new MenuView();
        MenuController menuController = new MenuController(menuModel, menuView);
        IBackupView backupView = new BackupView();
        EasySave.ModelLib.IObserver consoleView = new ConsoleView();
        await menuController.ManageActions(backupView, consoleView); // Starts the menu loop
    }
}