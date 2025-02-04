// See https://aka.ms/new-console-template for more information
using ProjetV0._1;

using System;
using interactive_menus;
using translation;
using ProjetV0._1.Controleur;
using ProjetV0._1.Modele;
using ProjetV0._1.Vue;

class Program
{
    static async Task Main(string[] args)
    {
        List<string> languages = new List<string> { "Français", "English", "Quitter / Exit" };
        LanguageChoice languages_menu = new LanguageChoice(languages);

        Translation translation = languages_menu.LanguagesMenu();

        ActionChoice actions_menu = new ActionChoice();
        await actions_menu.ActionsMenu(translation);

        //-------------------------------------------------------------
        // TEST BACKUPSTATE
        //-------------------------------------------------------------

        // Initialization
        ConsoleView view = new ConsoleView();
        BackupStateJournal.AddObserver(view);
        BackupController controller = new BackupController();

        // Creating a backup
        var backup = controller.CreateBackup("Backup1", "C:\\Source", "D:\\Backup");

        // Updating progress
        for (int i = 0; i < backup.TotalFilesToCopy; i++)
        {
            controller.UpdateProgress("Backup1");
            System.Threading.Thread.Sleep(500); // Simulate processing delay
        }

        Console.WriteLine("Backup completed.");

        //-------------------------------------------------------------
        // TEST LOG
        //-------------------------------------------------------------

        LogController logController = new LogController();
            LogView logView = new LogView();

            //Backup backup = new Backup
            //{
            //    Name = "Project Backup",
            //    SourcePath = "C:/Project",
            //    DestinationPath = "D:/Backup"
            //};

            long fileSize = 1048576; // 1MB
            double transferTime = 2.5; // 2.5s

            // Add a log entry
            //logController.AddLog(backup, fileSize, transferTime);

            // Display logs
            string logFile = $"backup_log_{DateTime.Today:dd-MM-yyyy}.json";
            logView.DisplayLog(logFile);
    }


}
//GestionnaireDeSauvegarde s= new GestionnaireDeSauvegarde();
////s.ExecuterSauvegarde("C:\\Users\\salem\\OneDrive\\Bureau\\Aya", "C:\\Users\\salem\\OneDrive\\Bureau\\SALEM");
//s.ParseJobIndices("1-4-6");


//namespace ProjetV0._1 {
//class Program
//{
//    static void Main()
//    {
//        Console.Write("Entrez le nom de la sauvegarde: ");
//        string name = Console.ReadLine();
//        Console.Write("Entrez le chemin du répertoire source: ");
//        string sourceDirectory = Console.ReadLine();
//        Console.Write("Entrez le chemin du répertoire cible: ");
//        string targetDirectory = Console.ReadLine();



//        //ici simulation de l'avancement de la sauvegarde, pour vraie sauvegarde, enlever le for (le calcul de la progression est faite dans la méthode CalculerEtat)
//        try
//        {
//            EtatSauvegarde etat = EtatSauvegardeJournal.CalculerEtat(name, sourceDirectory, targetDirectory);
//            EtatSauvegardeJournal.MettreAJourEtat(etat);
//            Journalisation.Instance.EcrireLog(name, sourceDirectory, targetDirectory, etat.TotalFilesSize, 0.0);
//            Console.Write("kjbc");

//            for (int i = 0; i < etat.TotalFilesToCopy; i++)
//            {
//                EtatSauvegardeJournal.MettreAJourProgression(name);
//                //if (etat.NbFilesLeftToDo == 0)
//                //{
//                //    Journalisation.Instance.EcrireLog(name, sourceDirectory, targetDirectory, etat.TotalFilesSize, 0.0);
//                //}
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Erreur: {ex.Message}");
//        }
//    }
//}
//}

