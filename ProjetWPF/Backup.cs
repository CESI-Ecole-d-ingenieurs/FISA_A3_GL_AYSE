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
    /// Class representing the display and interaction of backup in the WPF interface.
    /// Implements the IBackupView interface.
    public class Backup : IBackupView
    {
        public void DisplayProgress()
        {

        }

        /// Method to retrieve user input for a backup.
        public async Task<BackupModel> UserAsk()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            // Get input fields from the UI by their names
            var name = (TextBox)mainWindow.FindName("Name_t");
            var source = (TextBox)mainWindow.FindName("Source_t");
            var target = (TextBox)mainWindow.FindName("Destination_t");
            var listType = (ComboBox)mainWindow.FindName("Type_t");

            // Determine the backup type based on the user's selection
            string type;
            switch (listType.SelectedIndex)
            {
                case 0:
                    type = "Complète";
                    break;

                case 1:
                    type = "Différentielle";
                    break;

                default:
                    type = "Inconnu";
                    break;
            }

            // Create a BackupModel object containing the user-input data
            BackupModel backupModel = new BackupModel(name.Text.ToString(), source.Text.ToString(), target.Text.ToString(), type);

            name.Clear();
            source.Clear();
            target.Clear();

            return backupModel;
        }
    }
}
