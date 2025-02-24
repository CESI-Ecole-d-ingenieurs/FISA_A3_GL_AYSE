﻿using EasySave.IviewLib;
using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ControllerLib.TranslateStrategy;
namespace EasySave.ControllerLib
{

    public class LanguageController
    {
        private LanguageModel model;
        private ILanguageView view;

        /// Initializes the LanguageController with the model and view.
        public LanguageController(LanguageModel model, ILanguageView view)
        {
            this.model = model;
            this.view = view;
        }

        /// Handles navigation for selecting a language.
        /// Allows users to scroll through available options and select one.
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
                        languageSelected = ApplyLanguageSelection();
                        break;
                }
            }
        }

        /// Applies the selected language setting.
        /// Updates the translation strategy based on user selection.
        public bool ApplyLanguageSelection()
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
