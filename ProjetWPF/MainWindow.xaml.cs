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

        public MainWindow()
        {
            InitializeComponent();
            ShowLanguages();
            StartMonitoringBusinessSoftware();
            LoadAvailableSoftware();
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
                    existingSoftware = File.ReadAllLines("config.txt").ToList();
                }

                var newSoftwareList = softwareNames.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
                foreach (var software in newSoftwareList)
                {
                    if (!existingSoftware.Contains(software))
                    {
                        existingSoftware.Add(software);
                    }
                }

                File.WriteAllLines("config.txt", existingSoftware);
                MessageBox.Show("Logiciel(s) métier enregistré(s) avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BackupCreation(object sender, EventArgs e)
        {
            BackupController backupController = new BackupController(backup);

            await backupController.CreateBackup();

            Creation_OK.Text = await Translation.Instance.Translate("La sauvegarde a été créer avec succès.");
        }

        private void BackupExecution(object sender, EventArgs e)
        {
            BackupController backupController = new BackupController(backup);
            RealTimeState realTimeState = new RealTimeState();
            backupController.ExecuteBackup(ToDo_t.Text, realTimeState);
        }

        private void Language_b_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}