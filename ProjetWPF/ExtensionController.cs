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

            // Retrieve the text from the TextBox and convert it to a string
            string text = extensions.Text.ToString();

            // Split the text input into an array of file extensions using ',' as a separator
            string[] list = text.Split(',');

            // Update the global variable storing the encrypted file extensions
            GlobalVariables.CryptedFileExt = list;
        }
    }
}
