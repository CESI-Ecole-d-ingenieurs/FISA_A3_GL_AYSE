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
using System.CodeDom.Compiler;

namespace ProjetWPF
{
    /// Manages the visibility of different UI sections in the application.
    public class Visible
    {
        /// Displays a specific UI section while hiding all others.
        public void Show(string name)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            // Find all Grid elements that represent different sections
            var settings = (Grid)mainWindow.FindName("Settings");
            var languages = (Grid)mainWindow.FindName("Languages");
            var creation = (Grid)mainWindow.FindName("Creation");
            var execution = (Grid)mainWindow.FindName("Execution");
            var logs = (Grid)mainWindow.FindName("Logs");

            // Hide all sections by setting their visibility to Collapsed
            settings.Visibility = Visibility.Collapsed;
            creation.Visibility = Visibility.Collapsed;
            execution.Visibility = Visibility.Collapsed;
            logs.Visibility = Visibility.Collapsed;

            // Retrieve the specified Grid by name and make it visible
            var visible = (Grid)mainWindow.FindName(name);
            visible.Visibility = Visibility.Visible;
        }
    }
}
