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
    public class Format : IMenuView
    {
        // This method fills the list for the choice of the format of the log file.
        public Task DisplayActions(List<string> formats, int index)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var list = (ComboBox)mainWindow.FindName("Format_list");

            foreach (string item in formats)
            {
                list.Items.Add(item);
            }

            return Task.CompletedTask;
        }

        public void DisplayInputPrompt(string message)
        {

        }

        // This method clear the selected item in the list for the log file format choice.
        public void ClearScreen()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var list = (ComboBox)mainWindow.FindName("Format_list");

            list.Items.Clear();
        }
    }
}
