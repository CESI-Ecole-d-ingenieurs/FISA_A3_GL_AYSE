using System;
using System.IO;
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
    /// Class responsible for displaying log history in the UI.
    public class Logs
    {
        /// Asynchronously reads and displays the contents of the log file in the UI.
        public async Task DisplayLogs()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var logs = (TextBox)mainWindow.FindName("History_t");
            try
            {
                // Read the content of the log file
                string content = File.ReadAllText(GlobalVariables.LogFilePath);

                // Display the content in the TextBox
                logs.Text = content;
            }
            catch (Exception ex)
            {
                logs.Text = await Translation.Instance.Translate("Une erreur est survenue lors de la lecture du fichier : " + ex.Message);

            }
        }
    }
}
