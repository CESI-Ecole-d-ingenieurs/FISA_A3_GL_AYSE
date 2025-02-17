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
    public class ExtensionController
    {
        public void ExtensionsChange()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var extensions = (TextBox)mainWindow.FindName("Extensions_t");
            string text = extensions.Text.ToString();
            string[] list = text.Split(',');

            GlobalVariables.CryptedFileExt = list;
        }
    }
}
