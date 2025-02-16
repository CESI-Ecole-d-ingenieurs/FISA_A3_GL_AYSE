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

        public void ClearScreen()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var list = (ComboBox)mainWindow.FindName("Format_list");

            list.Items.Clear();
        }
    }
}
