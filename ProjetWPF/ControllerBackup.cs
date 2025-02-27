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
using System.IO;
using System.Windows.Navigation;

namespace ProjetWPF
{
    /// Controller class responsible for managing backup display operations.
    public class ControllerBackup
    {
        /// Asynchronously displays the list of available backups in the UI.
        public async Task DisplayBackups()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var backups = (TextBox)mainWindow.FindName("Available_t");

            // Create a FileInfo object to check if the backup file is empty
            FileInfo fileinfo = new FileInfo(GlobalVariables.PathBackup);
            if (fileinfo.Length > 0) // Check if the file is not empty
            {
                int lineNumber = 1;
                backups.Text = await Translation.Instance.Translate("Nom - Source - Destination - Type");
                // Read each line from the backup file and display it
                foreach (string line in File.ReadLines(GlobalVariables.PathBackup))
                {
                    backups.Text += "\n" + lineNumber + " " + line;
                    lineNumber++;
                }
            }
            else
            {
                // Display a translated message when no backups are recorded
                backups.Text = await Translation.Instance.Translate("Aucune sauvegarde enregistrée.");
            }
        }
    }
}
