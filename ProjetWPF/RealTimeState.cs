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

namespace ProjetWPF
{
    public class RealTimeState : IObserver
    {
        // This method diplay the real time state of a backup execution in a text zone.
        public void Update(BackupState state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var realTimeState = (TextBox)mainWindow.FindName("State_t");

                if (realTimeState != null)
                {
                    string[] lines = realTimeState.Text.Split('\n');
                    Dictionary<string, string> backupLines = new Dictionary<string, string>();

                    // Vérify if the name contains "CompleteBackup" or "DifferentialBackup" and filter it.
                    string backupName = state.Name;
                    if (backupName.Contains("CompleteBackup") || backupName.Contains("DifferentialBackup"))
                    {
                        return; // Don't display this line
                    }

                    // Get the existants backups.
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split(']');
                            if (parts.Length > 1)
                            {
                                string name = parts[0].TrimStart('[');
                                backupLines[name] = line;
                            }
                        }
                    }

                    // Generate the new progress bar.
                    int progressBarWidth = 20;
                    int progressBlocks = (int)(state.Progress / 100.0 * progressBarWidth);
                    string progressBar = $"[{new string('█', progressBlocks)}{new string('-', progressBarWidth - progressBlocks)}]{state.Progress}%";

                    // Replace or add the line for the backup in execution.
                    backupLines[backupName] = $"[{backupName}] {progressBar}";

                    // Rebuild the text with the updates.
                    realTimeState.Text = string.Join("\n", backupLines.Values);
                }
            });
        }
    }
}
