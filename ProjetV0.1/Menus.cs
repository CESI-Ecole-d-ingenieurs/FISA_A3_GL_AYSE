using ProjetV0._1;
using translation;

namespace interactive_menus
{
    public class LanguageChoice
    {
        private List<string> languages = new List<string> {};

        public List<String> get_languages()
        {
            return languages;
        }
        public LanguageChoice(List<String> language)
        {
            languages = language;
        }

        public Translation LanguagesMenu()
        {
            Translation translation = new Translation(); //MIEUX DECLARER UN VARIABLE GLOBAL
            int selectionIndex = 0;
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Veuillez choisir une langue / Choose a language :");

                for (int i = 0; i < languages.Count(); i++)
                {
                    if (i == selectionIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {languages[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {languages[i]}");
                    }
                }

                ConsoleKeyInfo fleche = Console.ReadKey();
                switch (fleche.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectionIndex = (selectionIndex == 0) ? languages.Count() - 1 : selectionIndex - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectionIndex = (selectionIndex == languages.Count() - 1) ? 0 : selectionIndex + 1;
                        break;

                    case ConsoleKey.Enter:
                        Console.Clear();
                        //string texte = "Bonjour";
                        switch (languages[selectionIndex])
                        {
                            case "Français":
                                translation.Set_strategy(new French());
                                //Console.WriteLine(await traduction.Traduire(texte));
                                //await menu_actions.MenuActions();
                                return translation;
                                //Console.ReadKey();
                                //break;

                            case "English":
                                translation.Set_strategy(new English());
                                //Console.WriteLine(await traduction.Traduire(texte));
                                //await menu_actions.MenuActions();
                                return translation;
                                //Console.ReadKey();
                                //break;

                            case "Quitter / Quit":
                                exit = true;
                                return null;
                                //break;
                        }
                        break;
                }
            }
            return null;
        }
    }

    public class ActionChoice
    {
        private List<string> actions = new List<string> {"Création de sauvegarde", "Exécution de sauvegarde", "Consulter les logs", "Quitter l'application"};
        public bool exitt = false;
        public List<String> get_actions()
        {
            return actions;
        }

        public async Task<string> ActionsMenu(Translation translation)
        {
            int selectionIndex = 0;

            GestionnaireDeSauvegarde gestionnaireDeSauvegarde = new GestionnaireDeSauvegarde();
            while (!exitt)
            {
               // Console.Clear();
                Console.WriteLine(await translation.Translate("Veuillez choisir une action :"));

                for (int i = 0; i < actions.Count(); i++)
                {
                    if (i == selectionIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        //Console.WriteLine(await traduction.Traduire($"> {actions[i]}"));
                        Console.WriteLine($"> " + await translation.Translate(actions[i]));
                        Console.ResetColor();
                    }
                    else
                    {
                        //Console.WriteLine(await traduction.Traduire($"  {actions[i]}"));
                        Console.WriteLine($"  " + await translation.Translate(actions[i]));
                    }
                }

                ConsoleKeyInfo fleche = Console.ReadKey();
                switch (fleche.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectionIndex = (selectionIndex == 0) ? actions.Count() - 1 : selectionIndex - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectionIndex = (selectionIndex == actions.Count() - 1) ? 0 : selectionIndex + 1;
                        break;

                    case ConsoleKey.Enter:
                        //Console.Clear();
                       
                        switch (selectionIndex)
                        {
                            case 0:
                                //Console.WriteLine("créer");
                                //Console.ReadKey();

                                Viewsauvegarde viewsauvegarde = new Viewsauvegarde();
                                Sauvegarde sauvegarde = await viewsauvegarde.UserAsk(translation);
                                StrategieSauvegarde strategieSauvegarde;
                                if (translation.Translate(sauvegarde.Type) == translation.Translate("Différentielle"))
                                {
                                    strategieSauvegarde = new StrategieSauvegardeDiff();
                                }
                                else
                                {
                                    strategieSauvegarde = new StrategieSauvegardeComplete();
                                }
                               
                                gestionnaireDeSauvegarde.AjouterSauvegarde(sauvegarde);
                                
                                break;

                            case 1:

                                Console.WriteLine(await translation.Translate("Donnez l'indexe du sauvegarde à executer exemple 1-3 pour exécuter automatiquement les sauvegardes 1 à 3  exemple 2 : 1 ;3 pour exécuter automatiquement les sauvegardes 1 et 3\r\n"));
                                string indexe = Console.ReadLine();
                                Console.Clear();
                                gestionnaireDeSauvegarde.ExecuteSauvegarde(indexe);
                                break;

                            case 2:
                                Console.WriteLine("consulter");
                                Console.ReadKey();
                                break;

                            case 3:
                                //exit = true;
                                return null;
                        }
                        break;
                }
            }
            return null;
        }
    }
}