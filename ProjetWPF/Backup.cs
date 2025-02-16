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

        public async Task<BackupModel> UserAsk()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var name = (TextBox)mainWindow.FindName("Name_t");
            var source = (TextBox)mainWindow.FindName("Source_t");
            var target = (TextBox)mainWindow.FindName("Destination_t");
            var listType = (ComboBox)mainWindow.FindName("Type_t");
            string type = await Translation.Instance.Translate(((ComboBoxItem)listType.SelectedItem).Content.ToString());

            BackupModel backupModel = new BackupModel(name.Text.ToString(), source.Text.ToString(), target.Text.ToString(), type);

            return backupModel;
        }
    }
}
