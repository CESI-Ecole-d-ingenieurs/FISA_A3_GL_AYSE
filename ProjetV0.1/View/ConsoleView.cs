using ProjetV0._1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //Console.WriteLine($"[Backup: {state.Name}] Progress: {state.Progress}% - State: {state.State}");
            _backupView.DisplayProgress();

            // Progress bar visualization in the console.
            Console.Write("[");
            int progressBarWidth = 30;
            int progressBlocks = (int)(state.Progress / 100.0 * progressBarWidth);
            Console.Write(new string('█', progressBlocks));
            Console.Write(new string('-', progressBarWidth - progressBlocks));
            Console.Write($"] {state.Progress}%");
            Console.WriteLine();
            // Brief delay to make updates visible.
            Thread.Sleep(300);
        }
    }
}
