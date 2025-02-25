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
    public class ControllerBackup
    {
        public async Task DisplayBackups()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var backups = (TextBox)mainWindow.FindName("Available_t");

            FileInfo fileinfo = new FileInfo(GlobalVariables.PathBackup);
            if (fileinfo.Length > 0) // Check if the file is not empty
            {
                int lineNumber = 1;
                backups.Text = await Translation.Instance.Translate("Nom - Source - Destination - Type");
                foreach (string line in File.ReadLines(GlobalVariables.PathBackup))
                {
                    backups.Text += "\n" + lineNumber + " " + line;
                    lineNumber++;
                }
            }
            else
            {
                backups.Text = await Translation.Instance.Translate("Aucune sauvegarde enregistrée.");
            }
        }
    }
}
