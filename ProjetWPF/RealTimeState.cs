using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ControllerLib;
using EasySave.ModelLib;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using BackupServer;

namespace ProjetWPF
{
    /// Observer class responsible for real-time tracking of backup states.
    /// Updates the UI and sends progress information to the server.
    public class RealTimeState : IObserver
    {
        private readonly ServerController _serverController; // Server communication handler

        /// Constructor that initializes the real-time state observer with a server controller
        public RealTimeState(ServerController serverController)
        {
            _serverController = serverController;
        }

        /// Updates the real-time backup progress in the UI and sends updates to the server.
        public void Update(BackupState state)
        {
            // Ensure UI updates are executed on the main thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var realTimeState = (TextBox)mainWindow.FindName("State_t");

                if (realTimeState != null) // Ensure the TextBox exists
                {
                    string[] lines = realTimeState.Text.Split('\n');
                    Dictionary<string, string> backupLines = new Dictionary<string, string>(); // Store backup entries

                    // Check whether the name contains “CompleteBackup” or “DifferentialBackup” and filter it
                    string backupName = state.Name;
                    if (backupName.Contains("CompleteBackup") || backupName.Contains("DifferentialBackup"))
                    {
                        return; // Do not display this entry
                    }

                    // Recover existing backups
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line)) // Ignore empty lines
                        {
                            string[] parts = line.Split(']');
                            if (parts.Length > 1)
                            {
                                string name = parts[0].TrimStart('['); // Extract the backup name
                                backupLines[name] = line; // Store the existing line
                            }
                        }
                    }

                    // Generate new progress bar
                    int progressBarWidth = 20;
                    int progressBlocks = (int)(state.Progress / 100.0 * progressBarWidth);
                    string progressBar = $"[{new string('█', progressBlocks)}{new string('-', progressBarWidth - progressBlocks)}]{state.Progress}%";

                    // Replace or add the line for the current backup
                    backupLines[backupName] = $"[{backupName}] {progressBar}";

                    // Rebuild text with updates
                    realTimeState.Text = string.Join("\n", backupLines.Values);

                    // Send the updated backup progress to the server
                    string message = realTimeState.Text;
                    _serverController.SendToClient(message);

                }
            });
        }
    }
}