using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.IviewLib;
using static System.Net.Mime.MediaTypeNames;
namespace ProjetV0._1.View
{
    /// ConsoleView class implementing the IObserver interface to display real-time backup progress.
    public class ConsoleView : IObserver
    {
        private BackupView _backupView = new BackupView();


        /// Updates the console with the current state of the backup.
        /// Displays both a textual and graphical progress representation.
        public void Update(BackupState state)
        {
            Console.Clear();
            var Text = "";
            BackupState ss = state;
            string[] lines = Text.Split('\n');
            Dictionary<string, string> backupLines = new Dictionary<string, string>();

            // Verify if the name of the backup contains "CompleteBackup" or "DifferentialBackup" and filtered it.
            string backupName = state.Name;
            if (backupName.Contains("CompleteBackup") || backupName.Contains("DifferentialBackup"))
            {
                return;
            }

            // Get the existing backups
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

            // Generates the new progress bar
            int progressBarWidth = 20;
            int progressBlocks = (int)(state.Progress / 100.0 * progressBarWidth);
            string progressBar = $"[{new string('█', progressBlocks)}{new string('-', progressBarWidth - progressBlocks)}]{state.Progress}%";

            // Replace or add the line for the ongoing backup
            backupLines[backupName] = $"[{backupName}] {progressBar}";

            // Rebuild the text with all the modifications
            Text = string.Join("\n", backupLines.Values);
            Console.Write(Text);
        }
    }
}