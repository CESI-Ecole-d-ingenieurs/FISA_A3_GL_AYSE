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
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var realTimeState = (TextBox)mainWindow.FindName("State_t");

                if (realTimeState != null)
                {
                    string[] lines = realTimeState.Text.Split('\n');
                    Dictionary<string, string> backupLines = new Dictionary<string, string>();

                    // Vérifier si le nom contient "CompleteBackup" ou "DifferentialBackup" et le filtrer
                    string backupName = state.Name;
                    if (backupName.Contains("CompleteBackup") || backupName.Contains("DifferentialBackup"))
                    {
                        return; // Ne pas afficher cette entrée
                    }

                    // Récupérer les sauvegardes existantes
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

                    // Générer la nouvelle barre de progression
                    int progressBarWidth = 20;
                    int progressBlocks = (int)(state.Progress / 100.0 * progressBarWidth);
                    string progressBar = $"[{new string('█', progressBlocks)}{new string('-', progressBarWidth - progressBlocks)}]{state.Progress}%";

                    // Remplacer ou ajouter la ligne pour la sauvegarde en cours
                    backupLines[backupName] = $"[{backupName}] {progressBar}";

                    // Reconstruire le texte avec les mises à jour
                    realTimeState.Text = string.Join("\n", backupLines.Values);
                }
            });
        }




    }
}
