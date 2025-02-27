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
    /// Class responsible for managing the display and formatting options.
    /// Implements the IMenuView interface.
    public class Format : IMenuView
    {
        /// Displays a list of available format actions in the ComboBox UI element.
        public Task DisplayActions(List<string> formats, int index)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var list = (ComboBox)mainWindow.FindName("Format_list");

            // Add each format option to the ComboBox
            foreach (string item in formats)
            {
                list.Items.Add(item);
            }

            return Task.CompletedTask;
        }

        public void DisplayInputPrompt(string message)
        {

        }

        /// Clears the format selection list in the UI.
        public void ClearScreen()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var list = (ComboBox)mainWindow.FindName("Format_list");

            list.Items.Clear();
        }
    }
}
