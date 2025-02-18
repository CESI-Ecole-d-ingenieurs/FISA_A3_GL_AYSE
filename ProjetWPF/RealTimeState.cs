using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ControllerLib;
using EasySave.ModelLib;
using System.Windows.Controls;
using System.Windows;

namespace ProjetWPF
{
    public class RealTimeState : IObserver
    {
        public void Update(BackupState state)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var realTimeState = (TextBox)mainWindow.FindName("State_t");

            realTimeState.Text += "[";
            int progressBarWidth = 20;
            int progressBlocks = (int)(state.Progress / 100.0 * progressBarWidth);
            realTimeState.Text += new string('█', progressBlocks);
            realTimeState.Text += new string('-', progressBarWidth - progressBlocks);
            realTimeState.Text += "]" + state.Progress + "%";
            realTimeState.Text += "\n";
            // Brief delay to make updates visible.
            Thread.Sleep(300);
        }
    }
}
