using System.Diagnostics;
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
using EasySave.ControllerLib;
using EasySave.IviewLib;
using EasySave.ModelLib;
using Microsoft.Win32;

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

        Backup backup = new Backup();
        ControllerBackup controllerBackup = new ControllerBackup();

        Logs logs = new Logs();

        ExtensionController extensionController = new ExtensionController();

        private Thread monitoringThread;
        private bool isMonitoring = true;
        private bool alertShown = false;

        // This method initialize and launch the Graphical User Interface
        public MainWindow()
        {
            InitializeComponent();
            ShowLanguages();
            StartMonitoringBusinessSoftware();
            LoadAvailableSoftware();
            LoadBusinessSoftware();
            MonitorBusinessSoftware();
        }

        // This method displays all the business software that are running in text zone.
        private void LoadBusinessSoftware()
        {
            if (File.Exists("config.txt"))
            {
                // List the running business software.
                var businessSoftwareList = File.ReadAllLines("config.txt")
                                               .Select(s => s.Trim())
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .ToList();

                // Display the running businnes software
                BusinessSoftwareTextBox.Text = string.Join(", ", businessSoftwareList);
            }
        }

        // This method verify if one (ore more) business software defined by the user is running.
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
                            // Make a list with the business software defined by the user.
                            var businessSoftwareList = File.ReadAllLines("config.txt")
                                                           .Select(s => s.Trim().ToLower())
                                                           .Where(s => !string.IsNullOrEmpty(s))
                                                           .ToList();

                            // Check if one (or more) of the business software are running.
                            bool softwareRunning = businessSoftwareList.Any(software  => Process.GetProcesses()
                                                                                      .Any(p => p.ProcessName.ToLower().Contains(software)));

                            // Inform the user if one (or more) of the business software are running.
                            if (softwareRunning && !alertShown)
                            {
                                alertShown = true;
                                MessageBox.Show("One or more business software are running. The backups are blocked", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        // This method verify if one (ore more) business software defined by the user is running during the backups execution.
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
                            // Inform the user if one (or more) of the business software are running.
                            if (!alertShown)
                            {
                                alertShown = true;
                                MessageBox.Show("One or more business software are running. The new backups are blocked.",
                                    "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            alertShown = false;
                        }
                    });

                    Thread.Sleep(5000); // Check every 5 seconds
                }
            });
        }

        // This method check if the business software define by the user are running.
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

        // This method displays all the software that are running.
        private void LoadAvailableSoftware()
        {
            var softwareList = new List<string>();

            // Get all the software installed in the Windows register.
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

            // Add the running software to a list
            var runningProcesses = Process.GetProcesses()
                                          .Select(p => p.ProcessName)
                                          .Distinct()
                                          .OrderBy(p => p)
                                          .ToList();
            softwareList.AddRange(runningProcesses);
            softwareList = softwareList.Distinct().OrderBy(p => p).ToList();

            // Display the list in a ComboBox.
            ProcessComboBox.ItemsSource = softwareList;
        }

        // This method allows the user to click on an item in the list of the running software. It add the selected software to the list of the software that are not compatible with the backup execution.
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

        // This method is used when the user closed the application.
        private void Window_Closed(object sender, EventArgs e)
        {
            isMonitoring = false;
        }


        // This method shows the elements related to the settings part.
        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            format.ClearScreen();
            format.DisplayActions(menuModel.LogFormats, 0);

            visible.Show("Settings");
        }

        // This method shows the elements related to the language part.
        private void ShowLanguages()
        {
            languageView.DisplayLanguages(languageModel.Languages, 0);
        }

        // This method shows the elements related to the backup creation part.
        private void ShowCreation(object sender, RoutedEventArgs e)
        {
            visible.Show("Creation");
        }

        // This method shows the elements related to the backup execution part.
        private async void ShowExecution(object sender, RoutedEventArgs e)
        {
            await controllerBackup.DisplayBackups();

            visible.Show("Execution");
        }

        // This method shows the elements related to the log file part.
        private async void ShowLogs(object sender, RoutedEventArgs e)
        {
            await logs.DisplayLogs();

            visible.Show("Logs");
        }

        // This method close the application by clicking on "Exit".
        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // This method translates all the displayed texts in the application.
        private async void LanguageChange(object sender, EventArgs e)
        {
            languageModel.SelectedLanguage = Language.SelectedIndex;

            LanguageController languageController = new LanguageController(languageModel, languageView);

            languageController.ApplyLanguageSelection();

            Exit_b.Content = await Translation.Instance.Translate("Quitter");

            Settings_b.Content = await Translation.Instance.Translate("Paramètrage");
            Format.Text = await Translation.Instance.Translate("Format d'historique :");
            Crypt.Text = await Translation.Instance.Translate("Fichiers à crypter :");
            Software.Text = await Translation.Instance.Translate("Logiciels métier :");

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
        }

        // This method changes the format of the log file, creates a list with the extension that need to be encrypted, and creates a list with the software that are not compatible with the backup execution.
        private async void SettingsChange(object sender, EventArgs e)
        {
            MenuController menuController = new MenuController(menuModel, format);
            // Change the log file format
            await menuController.HandleLogFormat(Format_list.SelectedIndex);
            // Get the extension that need to be encrypted
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

                // Remove from the list the business software that are not defined by the user.
                var updatedSoftwareList = existingSoftware.Where(s => newSoftwareList.Contains(s)).ToList();

                // Add the business software that are defined by the user to the list.
                foreach (var software in newSoftwareList)
                {
                    if (!updatedSoftwareList.Contains(software))
                    {
                        updatedSoftwareList.Add(software);
                    }
                }

                // Add the business software that are defined by the user to the file.
                File.WriteAllLines("config.txt", updatedSoftwareList);

                // Reload the displayed business software.
                LoadBusinessSoftware();

                MessageBox.Show(await Translation.Instance.Translate("Logiciel(s) métier mis à jour avec succès !"), "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Remove from the list all the business software if there is no entry from the user.
            if (string.IsNullOrEmpty(softwareNames))
            {
                BusinessSoftwareTextBox.Clear();
                if (File.Exists("config.txt"))
                {
                    File.Delete("config.txt");
                    MessageBox.Show(await Translation.Instance.Translate("Aucun logiciel métier ne sera pris en compte."), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }

        // This method is used to create a backup.
        private async void BackupCreation(object sender, EventArgs e)
        {
            BackupController backupController = new BackupController(backup);

            await backupController.CreateBackup();

            Creation_OK.Text = await Translation.Instance.Translate("La sauvegarde a été créer avec succès.");
        }

        // This method is used to execute a backup.
        private async void BackupExecution(object sender, EventArgs e)
        {
            if (IsBusinessSoftwareRunning())
            {
                MessageBox.Show(await Translation.Instance.Translate("Un logiciel métier est en cours d'exécution. Impossible de démarrer une nouvelle sauvegarde."),
                    "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                File.AppendAllText(GlobalVariables.PathBackup, await Translation.Instance.Translate($"[{DateTime.Now}] Tentative de lancement d'une sauvegarde bloquée car un logiciel métier est actif.\n"));
                return;
            }
            BackupController backupController = new BackupController(backup);
            RealTimeState realTimeState = new RealTimeState();
            await backupController.ExecuteBackupAsync(ToDo_t.Text, realTimeState);
        }
    }
}