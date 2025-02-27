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
    /// Controller class responsible for handling changes in file extensions to be encrypted.
    public class ExtensionController
    {
        /// Updates the global list of encrypted file extensions based on user input.
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
