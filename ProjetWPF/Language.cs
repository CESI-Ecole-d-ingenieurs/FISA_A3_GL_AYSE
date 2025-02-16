using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ControllerLib;
using EasySave.IviewLib;
using EasySave.ModelLib;
using System.Windows.Controls;
using System.Windows;

namespace ProjetWPF
{
    public class LanguagesView : ILanguageView
    {
        public void DisplayLanguages(List<string> languages, int selectionIndex)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var list = (ComboBox)mainWindow.FindName("Language");

            list.Items.Clear();
            for (int i = 0; i < languages.Count - 1; i++)
            {
                list.Items.Add(languages[i]);
            }
        }
    }
}
