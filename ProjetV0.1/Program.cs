// See https://aka.ms/new-console-template for more information
using Projet_Génie_Logiciel;
using ProjetV0._1;


Console.WriteLine("Hello, World!");
//GestionnaireDeSauvegarde s= new GestionnaireDeSauvegarde();
////s.ExecuterSauvegarde("C:\\Users\\salem\\OneDrive\\Bureau\\Aya", "C:\\Users\\salem\\OneDrive\\Bureau\\SALEM");
//s.ParseJobIndices("1-4-6");

namespace ProjetV0._1 {
class Program
{
    static void Main()
    {
        Console.Write("Entrez le nom de la sauvegarde: ");
        string name = Console.ReadLine();
        Console.Write("Entrez le chemin du répertoire source: ");
        string sourceDirectory = Console.ReadLine();
        Console.Write("Entrez le chemin du répertoire cible: ");
        string targetDirectory = Console.ReadLine();


        //ici simulation de l'avancement de la sauvegarde, pour vraie sauvegarde, enlever le for (le calcul de la progression est faite dans la méthode CalculerEtat)
        try
        {
            EtatSauvegarde etat = EtatSauvegardeJournal.CalculerEtat(name, sourceDirectory, targetDirectory);
            EtatSauvegardeJournal.MettreAJourEtat(etat);
            Journalisation.Instance.EcrireLog(name, sourceDirectory, targetDirectory, etat.TotalFilesSize, 0.0);
            Console.Write("kjbc");

            for (int i = 0; i < etat.TotalFilesToCopy; i++)
            {
                EtatSauvegardeJournal.MettreAJourProgression(name);
                //if (etat.NbFilesLeftToDo == 0)
                //{
                //    Journalisation.Instance.EcrireLog(name, sourceDirectory, targetDirectory, etat.TotalFilesSize, 0.0);
                //}
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
        }
    }
}
}