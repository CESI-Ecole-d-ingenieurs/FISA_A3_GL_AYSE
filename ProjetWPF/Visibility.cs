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
    public class Visible
    {
        public void Show(string name)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            var settings = (Grid)mainWindow.FindName("Settings");
            var languages = (Grid)mainWindow.FindName("Languages");
            var creation = (Grid)mainWindow.FindName("Creation");
            var execution = (Grid)mainWindow.FindName("Execution");
            var logs = (Grid)mainWindow.FindName("Logs");

            settings.Visibility = Visibility.Collapsed;
            languages.Visibility = Visibility.Collapsed;
            creation.Visibility = Visibility.Collapsed;
            execution.Visibility = Visibility.Collapsed;
            logs.Visibility = Visibility.Collapsed;

            var visible = (Grid)mainWindow.FindName(name);
            visible.Visibility = Visibility.Visible;
        }
    }
}
