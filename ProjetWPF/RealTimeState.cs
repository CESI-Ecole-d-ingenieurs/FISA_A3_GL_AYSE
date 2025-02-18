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
        public void Update(BackupState state)
        {

            //var states = BackupStateJournal.GetState();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var State_t = (TextBox)mainWindow.FindName("State_t");
            var Test = (TextBox)mainWindow.FindName("Test");
            // _backupView.DisplayProgress();

            var states = BackupStateJournal.GetState();
            //Console.Clear();
            //Console.WriteLine("Mise à jour de la progression..."); // Check

            //Console.WriteLine("Progression des sauvegardes :");
            foreach (var statee in states)
            {
             //  Console.WriteLine($"{statee.Name} : {statee.Progress}% - {statee.State}");
                Test.Text += new String(statee.Name);
                int progressBlockss = (int)(statee.Progress);
                Test.Text += new String(':',progressBlockss);
               
            }

            //realTimeState.Text += "[";
            int progressBarWidth = 30;
            int progressBlocks = (int)(state.Progress / 100.0 * progressBarWidth);
            //realTimeState.Text += new string('█', progressBlocks);
            //realTimeState.Text += new string('-', progressBarWidth - progressBlocks);
            //realTimeState.Text += "]" + state.Progress + "%";
            //realTimeState.Text += "\n";
            // Use Dispatcher to ensure UI thread updates the TextBox safely
           
            // Brief delay to make updates visible.
            Thread.Sleep(500);
        }
    }
}
