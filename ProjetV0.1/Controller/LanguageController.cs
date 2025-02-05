using ProjetV0._1.Model;
using ProjetV0._1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;

namespace ProjetV0._1.Controllers
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
                view.DisplayLanguages(model.Languages, model.SelectedLanguage);
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        model.SelectedLanguage = Math.Max(0, model.SelectedLanguage - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        model.SelectedLanguage = Math.Min(model.Languages.Count - 1, model.SelectedLanguage + 1);
                        break;
                    case ConsoleKey.Enter:
                        ApplyLanguageSelection();
                        return;
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
