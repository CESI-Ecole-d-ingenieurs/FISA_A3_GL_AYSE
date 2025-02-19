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
    public class Backup : IBackupView
    {
        public void DisplayProgress()
        {

        }

        // This method gets and returns the user's entry for the parameters of a backup.
        public async Task<BackupModel> UserAsk()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var name = (TextBox)mainWindow.FindName("Name_t");
            var source = (TextBox)mainWindow.FindName("Source_t");
            var target = (TextBox)mainWindow.FindName("Destination_t");
            var listType = (ComboBox)mainWindow.FindName("Type_t");
            string type;
            // Get the type of the backup with the index of the selection in the list.
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

            BackupModel backupModel = new BackupModel(name.Text.ToString(), source.Text.ToString(), target.Text.ToString(), type);

            name.Clear();
            source.Clear();
            target.Clear();

            return backupModel;
        }
    }
}
