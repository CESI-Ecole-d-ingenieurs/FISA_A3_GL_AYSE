﻿using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BackupServer;
using EasySave.ControllerLib;
using EasySave.ControllerLib.BackupStrategy;
using EasySave.IviewLib;
using EasySave.ModelLib;
using Microsoft.Win32;
using System.Text.RegularExpressions;
//using static System.Net.Mime.MediaTypeNames;

namespace ProjetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Visible visible = new Visible();

        LanguageModel languageModel = new LanguageModel();
        LanguagesView languageView = new LanguagesView();

        MenuModel menuModel = new MenuModel();
        Format format = new Format();

        //Backup backup = new Backup();
        BackupController backupController = new BackupController(new Backup());
        ControllerBackup controllerBackup = new ControllerBackup();

        Logs logs = new Logs();

        ExtensionController extensionController = new ExtensionController();

        private Thread monitoringThread;
        private bool isMonitoring = true;
        private bool alertShown = false;

        private ServerController serverController;

        public MainWindow()
        {
            InitializeComponent();
            ShowLanguages();
            StartMonitoringBusinessSoftware();
            LoadAvailableSoftware();
            LoadBusinessSoftware();
            MonitorBusinessSoftware();
            LoadExtensions();
            serverController = new ServerController(this);
            _ = StartServerAsync();
        }

        private async Task StartServerAsync()
        {
            await serverController.StartServerAsync();
        }

        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    serverController.StopServer();
        //}

        //private async void StartBackup_Click(object sender, RoutedEventArgs e)
        //{
        //    await serverController.StartBackupAsync();
        //}

        private void LoadBusinessSoftware()
        {
            if (File.Exists("config.txt"))
            {
                var businessSoftwareList = File.ReadAllLines("config.txt")
                                               .Select(s => s.Trim())
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .ToList();

                // Afficher les logiciels dans la TextBox
                BusinessSoftwareTextBox.Text = string.Join(", ", businessSoftwareList);
            }
        }

        private void LoadExtensions()
        {
            if (File.Exists("extensions.txt"))
            {
                var extensionsList = File.ReadAllLines("extensions.txt")
                                               .Select(s => s.Trim())
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .ToList();

                // Afficher les logiciels dans la TextBox
                Extensions.Text = string.Join(", ", extensionsList);
            }
        }


        private void StartMonitoringBusinessSoftware()
        {
            monitoringThread = new Thread(() =>
            {
                while (isMonitoring)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (File.Exists("config.txt"))
                        {
                            var businessSoftwareList = File.ReadAllLines("config.txt")
                                                           .Select(s => s.Trim().ToLower())
                                                           .Where(s => !string.IsNullOrEmpty(s))
                                                           .ToList();

                            bool softwareRunning = businessSoftwareList.Any(software => Process.GetProcesses()
                                                                                      .Any(p => p.ProcessName.ToLower().Contains(software)));

                            if (softwareRunning && !alertShown)
                            {
                                alertShown = true;
                                MessageBox.Show("Un ou plusieurs logiciels métiers sont en cours d'exécution. Les sauvegardes sont bloquées.", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (!softwareRunning)
                            {
                                alertShown = false;
                            }
                        }
                    });
                    Thread.Sleep(5000);
                }
            });
            monitoringThread.IsBackground = true;
            monitoringThread.Start();
        }

        private void MonitorBusinessSoftware()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (IsBusinessSoftwareRunning())
                        {
                            // Afficher une alerte si un logiciel métier est détecté
                            if (!alertShown)
                            {
                                alertShown = true;
                                MessageBox.Show("Un logiciel métier est en cours d'exécution. Les nouvelles sauvegardes sont bloquées.",
                                    "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            alertShown = false;
                        }
                    });

                    Thread.Sleep(5000); // Vérifie toutes les 5 secondes
                }
            });
        }


        public bool IsBusinessSoftwareRunning()
        {
            if (!File.Exists("config.txt"))
                return false;

            var businessSoftwareList = File.ReadAllLines("config.txt")
                                           .Select(s => s.Trim().ToLower())
                                           .Where(s => !string.IsNullOrEmpty(s))
                                           .ToList();

            return businessSoftwareList.Any(software => Process.GetProcesses()
                                                                .Any(p => p.ProcessName.ToLower().Contains(software)));
        }

        private void LoadAvailableSoftware()
        {
            var softwareList = new List<string>();

            // Récupérer tous les logiciels installés depuis le registre Windows
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (key != null)
                {
                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            var displayName = subkey?.GetValue("DisplayName") as string;
                            if (!string.IsNullOrEmpty(displayName))
                            {
                                softwareList.Add(displayName);
                            }
                        }
                    }
                }
            }

            // Ajouter les processus actuellement en cours d'exécution
            var runningProcesses = Process.GetProcesses()
                                          .Select(p => p.ProcessName)
                                          .Distinct()
                                          .OrderBy(p => p)
                                          .ToList();
            softwareList.AddRange(runningProcesses);
            softwareList = softwareList.Distinct().OrderBy(p => p).ToList();

            // Afficher dans la ComboBox
            ProcessComboBox.ItemsSource = softwareList;
        }

        private void ProcessComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProcessComboBox.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(BusinessSoftwareTextBox.Text))
                {
                    BusinessSoftwareTextBox.Text += ", " + ProcessComboBox.SelectedItem.ToString();
                }
                else
                {
                    BusinessSoftwareTextBox.Text = ProcessComboBox.SelectedItem.ToString();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            isMonitoring = false;
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            format.ClearScreen();
            format.DisplayActions(menuModel.LogFormats, 0);

            visible.Show("Settings");
        }

        private void ShowLanguages()
        {
            languageView.DisplayLanguages(languageModel.Languages, 0);

           // visible.Show("Languages");
        }

        private void ShowCreation(object sender, RoutedEventArgs e)
        {
            visible.Show("Creation");
        }

        private async void ShowExecution(object sender, RoutedEventArgs e)
        {
            await controllerBackup.DisplayBackups();

            visible.Show("Execution");
        }

        private async void ShowLogs(object sender, RoutedEventArgs e)
        {
            await logs.DisplayLogs();

            visible.Show("Logs");
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LanguageChange(object sender, EventArgs e)
        {
            //Language_OK.Text = await Translation.Instance.Translate("Traduction en cours...");

            languageModel.SelectedLanguage = Language.SelectedIndex;

            LanguageController languageController = new LanguageController(languageModel, languageView);

            languageController.ApplyLanguageSelection();

            Exit_b.Content = await Translation.Instance.Translate("Quitter");

            Settings_b.Content = await Translation.Instance.Translate("Paramètrage");
            Format.Text = await Translation.Instance.Translate("Format d'historique :");
            Crypt.Text = await Translation.Instance.Translate("Fichiers à crypter :");
            Software.Text = await Translation.Instance.Translate("Logiciels métier :");
            NKoctets.Text = await Translation.Instance.Translate("Taille maximale des fichiers à sauvegarder :");
            Ext.Text = await Translation.Instance.Translate("Extensions prioritaires :");

            Create_b.Content = await Translation.Instance.Translate("Création de sauvegarde");
            Name.Text = await Translation.Instance.Translate("Nom :");
            Source.Text = await Translation.Instance.Translate("Source :");
            Destination.Text = await Translation.Instance.Translate("Destination :");
            Type.Text = await Translation.Instance.Translate("Type :");
            Complete.Content = await Translation.Instance.Translate("Complète");
            Differential.Content = await Translation.Instance.Translate("Différentielle");

            Execute_b.Text = (await Translation.Instance.Translate("Exécuter une / plusieurs sauvegardes")).ToString();
            Available.Text = await Translation.Instance.Translate("Sauvegardes disponibles :");
            ToDo.Text = await Translation.Instance.Translate("Sauvegardes à faire :");
            ToDo_l.Content = await Translation.Instance.Translate("Ecrivez '1;3' pour exécuter les sauvegardes 1 et 3. Ecrivez '1-3' pour exécuter les sauvegardes 1 à 3.");
            State.Text = await Translation.Instance.Translate("Etat en temps réel :");

            Consult_b.Content = await Translation.Instance.Translate("Consulter l'historique");
            History.Text = await Translation.Instance.Translate("Historique :");

            //Language_OK.Text = await Translation.Instance.Translate("La langue a été modifiée avec succès");
        }

        private async void SettingsChange(object sender, EventArgs e)
        {
            MenuController menuController = new MenuController(menuModel, format);
            await menuController.HandleLogFormat(Format_list.SelectedIndex);
            extensionController.ExtensionsChange();

            Settings_OK.Text = await Translation.Instance.Translate("Les paramètres ont été modifiés avec succès.");

            string softwareNames = BusinessSoftwareTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(softwareNames))
            {
                var existingSoftware = new List<string>();
                if (File.Exists("config.txt"))
                {
                    existingSoftware = File.ReadAllLines("config.txt").Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                }

                var newSoftwareList = softwareNames.Split(',')
                                                   .Select(s => s.Trim())
                                                   .Where(s => !string.IsNullOrEmpty(s))
                                                   .ToList();

                // 🔹 Supprimer uniquement les logiciels qui ne sont plus dans BusinessSoftwareTextBox
                var updatedSoftwareList = existingSoftware.Where(s => newSoftwareList.Contains(s)).ToList();

                // 🔹 Ajouter uniquement les nouveaux logiciels
                foreach (var software in newSoftwareList)
                {
                    if (!updatedSoftwareList.Contains(software))
                    {
                        updatedSoftwareList.Add(software);
                    }
                }

                // 🔹 Mettre à jour config.txt avec la liste mise à jour
                File.WriteAllLines("config.txt", updatedSoftwareList);

                // 🔹 Recharger la liste des logiciels affichés
                LoadBusinessSoftware();

                MessageBox.Show("Logiciel(s) métier mis à jour avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // 🔹 Si la TextBox est vide, on supprime tous les logiciels métiers
            if (string.IsNullOrEmpty(softwareNames))
            {
                BusinessSoftwareTextBox.Clear();
                if (File.Exists("config.txt"))
                {
                    File.Delete("config.txt");
                    MessageBox.Show("Aucun logiciel métier ne sera pris en compte.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }



            string extensions = Extensions.Text.Trim();

            if (!string.IsNullOrEmpty(extensions))
            {
                var existingExtensions = new List<string>();
                if (File.Exists("extensions.txt"))
                {
                    existingExtensions = File.ReadAllLines("extensions.txt").Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                }

                var newExtensionsList = extensions.Split(',')
                                          .Select(s => s.Trim())
                                          .Where(s => !string.IsNullOrEmpty(s))
                                          .Select(s => s.StartsWith(".") ? s : "." + s) // Ajoute un point si absent
                                          .Distinct()
                                          .ToList();

                // 🔹 Supprimer uniquement les logiciels qui ne sont plus dans BusinessSoftwareTextBox
                var updatedExtensionsList = existingExtensions.Where(s => newExtensionsList.Contains(s)).ToList();

                // 🔹 Ajouter uniquement les nouveaux logiciels
                foreach (var extensionName in newExtensionsList)
                {
                    if (!updatedExtensionsList.Contains(extensions))
                    {
                        updatedExtensionsList.Add(extensions);
                    }
                }

                // 🔹 Mettre à jour config.txt avec la liste mise à jour
                File.WriteAllLines("extensions.txt", newExtensionsList);

                // 🔹 Recharger la liste des logiciels affichés
                LoadExtensions();

                MessageBox.Show("Extensions prioritaires mises à jour avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // 🔹 Si la TextBox est vide, on supprime tous les logiciels métiers
            if (string.IsNullOrEmpty(extensions))
            {
                Extensions.Clear();
                if (File.Exists("extensions.txt"))
                {
                    File.Delete("extensions.txt");
                    MessageBox.Show("Aucune extension ne sera prise en compte.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }


            // Récupération nKoctets
            if (Regex.IsMatch(NKoctetsTextBox.Text, @"^\d+$"))
            {
                GlobalVariables.maximumSize = int.Parse(NKoctetsTextBox.Text);
            }
            else
            {
                MessageBox.Show(await Translation.Instance.Translate("La taille maximale des fichiers doit être un nombre."), "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private async void BackupCreation(object sender, EventArgs e)
        {
            //BackupController backupController = new BackupController(backup);

            await backupController.CreateBackup();

            Creation_OK.Text = await Translation.Instance.Translate("La sauvegarde a été créer avec succès.");
        }

        private void BackupExecution(object sender, EventArgs e)
        {
            if (IsBusinessSoftwareRunning())
            {
                MessageBox.Show("Un logiciel métier est en cours d'exécution. Impossible de démarrer une nouvelle sauvegarde.",
                    "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                File.AppendAllText(GlobalVariables.PathBackup, $"[{DateTime.Now}] Tentative de lancement d'une sauvegarde bloquée car un logiciel métier est actif.\n");
                return; // Bloque le lancement
            }
            //BackupController backupController = new BackupController(backup);

            Play_b.Click -= BackupExecution;
            Pause_b.Click += PauseSelectedBackups;
            Stop_b.Click += StopSelectedBackups;
            // Play_Pause_Stop.Content = await Translation.Instance.Translate("Les sauvegardes sont lancées.");

            RealTimeState realTimeState = new RealTimeState(serverController);
             backupController.ExecuteBackupAsync(ToDo_t.Text, realTimeState);
        }

        public async void PauseSelectedBackups(object sender, RoutedEventArgs e)
        {
            //BackupController backupController = new BackupController(backup);

            string todoText = "";
            Application.Current.Dispatcher.Invoke(() =>
            {
                todoText = ToDo_t.Text; // Récupère la valeur de ToDo_t.Text sur le thread UI
            });

            // Maintenant, on peut utiliser la variable en dehors du Dispatcher
            List<int> backupIndexes = backupController.ParseJobIndex(todoText);


            //List<int> backupIndexes = backupController.ParseJobIndex(ToDo_t.Text);
            
            foreach (int index in backupIndexes)
            {
                backupController.PauseBackup(index);
            }

            Resume_b.Click += ResumeSelectedBackups;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                Play_Pause_Stop.Content = await Translation.Instance.Translate("Les sauvegardes sont en pause.");
            });

            //Play_Pause_Stop.Content = await Translation.Instance.Translate("Les sauvegardes sont en pause.");
        }
        public async void ResumeSelectedBackups(object sender, RoutedEventArgs e)
        {
            //BackupController backupController = new BackupController(backup);

            string toDoText = "";
            Application.Current.Dispatcher.Invoke(() =>
            {
                toDoText = ToDo_t.Text; // 🔹 Récupère le texte en toute sécurité depuis l'UI
            });

            List<int> backupIndexes = backupController.ParseJobIndex(toDoText);


            //List<int> backupIndexes = backupController.ParseJobIndex(ToDo_t.Text);

            foreach (int index in backupIndexes)
            {
                backupController.ResumeBackup(index);
            }
            string translatedText = await Translation.Instance.Translate("Les sauvegardes ont repris.");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Play_Pause_Stop.Content = translatedText;
            });
            //Play_Pause_Stop.Content = await Translation.Instance.Translate("Les sauvegardes ont repris.");
        }
        public async void StopSelectedBackups(object sender, RoutedEventArgs e)
        {
            //BackupController backupController = new BackupController(backup);
            string toDoText = "";

            // 🔹 Récupérer la valeur en toute sécurité depuis l'UI
            Application.Current.Dispatcher.Invoke(() =>
            {
                toDoText = ToDo_t.Text;
            });

            List<int> backupIndexes = backupController.ParseJobIndex(toDoText);

            foreach (int index in backupIndexes)
            {
                backupController.StopBackup(index);
            }

            Play_b.Click += BackupExecution;
            Pause_b.Click -= PauseSelectedBackups;
            Resume_b.Click -= ResumeSelectedBackups;
            Stop_b.Click -= StopSelectedBackups;

            string translatedText = await Translation.Instance.Translate("Les sauvegardes sont arrêtées.");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Play_Pause_Stop.Content = translatedText;
            });

            //Play_Pause_Stop.Content = await Translation.Instance.Translate("Les sauvegardes sont arrêtées.");
        }

        private void Language_b_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}