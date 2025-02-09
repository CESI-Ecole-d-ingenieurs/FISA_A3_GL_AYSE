using ProjetV0._1.Model;
using ProjetV0._1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;

namespace ProjetV0._1.Controller
{
    internal class LanguageController
    {
        private LanguageModel model;
        private LanguageView view;

        public LanguageController(LanguageModel model, LanguageView view)
        {
            this.model = model;
            this.view = view;
        }

        public void NavigateLanguages()
        {
            ConsoleKeyInfo key;
            bool languageSelected = false;

            while (!languageSelected)
            {
                view.DisplayLanguages(model.Languages, model.SelectedLanguage);
                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        model.SelectedLanguage = (model.SelectedLanguage == 0) ? model.Languages.Count - 1 : model.SelectedLanguage - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        model.SelectedLanguage = (model.SelectedLanguage == model.Languages.Count - 1) ? 0 : model.SelectedLanguage + 1;
                        break;

                    case ConsoleKey.Enter:
                        languageSelected = ApplyLanguageSelection(); // Valider le choix
                        break;
                }
            }
        }

        private bool ApplyLanguageSelection()
        {
            switch (model.Languages[model.SelectedLanguage])
            {
                case "Français":
                    Translation.Instance.Set_strategy(new French());
                    //Console.WriteLine(await traduction.Traduire(texte));
                    //await menu_actions.MenuActions();
                    return true;
                //Console.ReadKey();
                //break;

                case "English":
                    Translation.Instance.Set_strategy(new English());
                    //Console.WriteLine(await traduction.Traduire(texte));
                    //await menu_actions.MenuActions();
                    return true;
                //Console.ReadKey();
                //break;

                //case "Quitter / Quit":
                //    exit = true;
                //    break;
                //break;
                default:
                    return false;
            }
            
        }
}
}
