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
using BackupServer;
using EasySave.ControllerLib;
using EasySave.ControllerLib.BackupStrategy;
using EasySave.IviewLib;
using EasySave.ModelLib;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace ProjetWPF
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        // UI components and controllers
        Visible visible = new Visible();

        LanguageModel languageModel = new LanguageModel();
        LanguagesView languageView = new LanguagesView();

        MenuModel menuModel = new MenuModel();
        Format format = new Format();

        BackupController backupController = new BackupController(new Backup());
        ControllerBackup controllerBackup = new ControllerBackup();

        Logs logs = new Logs();

        ExtensionController extensionController = new ExtensionController();

        private Thread monitoringThread;
        private bool isMonitoring = true;
        private bool alertShown = false;

        private ServerController serverController;

        /// Constructor - Initializes UI and starts necessary background processes.
        public MainWindow()
        {
            InitializeComponent();
            ShowLanguages();
            StartMonitoringBusinessSoftware();
            LoadAvailableSoftware();
            LoadBusinessSoftware();
            MonitorBusinessSoftware();
            LoadExtensions();
            serverController = new ServerController();
            _ = StartServerAsync();
        }

        /// Starts the server asynchronously.
        private async Task StartServerAsync()
        {
            await serverController.StartServerAsync();
        }

        /// Loads business-related software names from configuration.
        private void LoadBusinessSoftware()
        {
            if (File.Exists("config.txt"))
            {
                var businessSoftwareList = File.ReadAllLines("config.txt")
                                               .Select(s => s.Trim())
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .ToList();

                BusinessSoftwareTextBox.Text = string.Join(", ", businessSoftwareList);
            }
        }

        /// Loads file extensions that should be monitored.
        private void LoadExtensions()
        {
            if (File.Exists("extensions.txt"))
            {
                var extensionsList = File.ReadAllLines("extensions.txt")
                                               .Select(s => s.Trim())
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .ToList();


                Extensions.Text = string.Join(", ", extensionsList);
            }
        }

        /// Monitors if business software is running and prevents backup execution if detected.
        private void StartMonitoringBusinessSoftware()
        {
            // Create a new thread for continuous monitoring
            monitoringThread = new Thread(() =>
            {
                while (isMonitoring) // Keep checking while monitoring is active
                {
                    Dispatcher.Invoke(() => // Ensure UI updates are performed on the main thread
                    {
                        if (File.Exists("config.txt")) // Check if the configuration file exists
                        {
                            // Read and parse the business software list from the config file
                            var businessSoftwareList = File.ReadAllLines("config.txt")
                                                           .Select(s => s.Trim().ToLower())
                                                           .Where(s => !string.IsNullOrEmpty(s))
                                                           .ToList();

                            bool softwareRunning = businessSoftwareList.Any(software => Process.GetProcesses()
                                                                                      .Any(p => p.ProcessName.ToLower().Contains(software)));

                            if (softwareRunning && !alertShown) // If detected and no alert has been shown yet
                            {
                                alertShown = true;
                                MessageBox.Show("Un ou plusieurs logiciels métiers sont en cours d'exécution. Les sauvegardes sont bloquées.", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (!softwareRunning) // If no business software is running, reset the alert
                            {
                                alertShown = false;
                            }
                        }
                    });
                    Thread.Sleep(5000); // Wait 5 seconds before the next check
                }
            });
            monitoringThread.IsBackground = true; // Set thread as background so it stops with the application
            monitoringThread.Start(); // Start the monitoring thread
        }

        /// Loads available software from Windows registry and running processes.
        private void MonitorBusinessSoftware()
        {
            Task.Run(() => // Run this monitoring process asynchronously
            {
                while (true) // Infinite loop to continuously check
                {
                    Dispatcher.Invoke(() => // Ensure UI modifications are done on the main thread
                    {
                        if (IsBusinessSoftwareRunning()) // Check if any business software is running
                        {
                            // Display an alert if business software is detected
                            if (!alertShown)
                            {
                                alertShown = true; // Set flag to avoid multiple alerts
                                MessageBox.Show("Un logiciel métier est en cours d'exécution. Les nouvelles sauvegardes sont bloquées.",
                                    "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            alertShown = false; // Reset the flag if no business software is running
                        }
                    });

                    Thread.Sleep(5000); // Checks every 5 seconds
                }
            });
        }


        public bool IsBusinessSoftwareRunning()
        {
            // If the configuration file does not exist, return false
            if (!File.Exists("config.txt"))
                return false;

            // Read the list of business software from config.txt and clean up the entries
            var businessSoftwareList = File.ReadAllLines("config.txt")
                                           .Select(s => s.Trim().ToLower()) // Normalize to lowercase
                                           .Where(s => !string.IsNullOrEmpty(s)) // Remove empty lines
                                           .ToList();

            // Check if any of the listed software is currently running
            return businessSoftwareList.Any(software => Process.GetProcesses()
                                                                .Any(p => p.ProcessName.ToLower().Contains(software)));
        }

        /// Loads available software from Windows registry and running processes.
        private void LoadAvailableSoftware()
        {
            var softwareList = new List<string>(); // List to store software names

            // Recover all installed software from the Windows registry
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            // Open the registry key in read-only mode
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey)) 
            {
                if (key != null) // Ensure the registry key exists
                {
                    foreach (string subkeyName in key.GetSubKeyNames()) // Iterate through all subkeys
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkeyName)) // Open each subkey
                        {
                            // Retrieve the application name from the registry
                            var displayName = subkey?.GetValue("DisplayName") as string;
                            // Add the application name to the list if it is not null or empty
                            if (!string.IsNullOrEmpty(displayName))
                            {
                                softwareList.Add(displayName);
                            }
                        }
                    }
                }
            }

            // Add currently running processes
            var runningProcesses = Process.GetProcesses()
                                          .Select(p => p.ProcessName) // Get process names
                                          .Distinct() // Remove duplicates
                                          .OrderBy(p => p) // Sort alphabetically
                                          .ToList();
            softwareList.AddRange(runningProcesses); // Merge with installed software list
            softwareList = softwareList.Distinct().OrderBy(p => p).ToList();     // Ensure the list contains unique values and remains sorted

            // Display in ComboBox
            ProcessComboBox.ItemsSource = softwareList;
        }

        /// Handles the event when the user selects an item from the ProcessComboBox.
        /// Updates the BusinessSoftwareTextBox with the selected process name.
        private void ProcessComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProcessComboBox.SelectedItem != null) // Ensure an item is selected
            {
                // If the text box is not empty, append the selected process name with a comma
                if (!string.IsNullOrEmpty(BusinessSoftwareTextBox.Text))
                {
                    BusinessSoftwareTextBox.Text += ", " + ProcessComboBox.SelectedItem.ToString();
                }
                else // Otherwise, set it as the first entry
                {
                    BusinessSoftwareTextBox.Text = ProcessComboBox.SelectedItem.ToString();
                }
            }
        }

        /// Handles window closing event to stop monitoring.
        private void Window_Closed(object sender, EventArgs e)
        {
            isMonitoring = false;
        }

        /// Displays the settings page.
        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            format.ClearScreen();
            format.DisplayActions(menuModel.LogFormats, 0);

            visible.Show("Settings");
        }

        /// Displays the language selection page.
        private void ShowLanguages()
        {
            languageView.DisplayLanguages(languageModel.Languages, 0);

        }

        /// Displays the backup creation page.
        private void ShowCreation(object sender, RoutedEventArgs e)
        {
            visible.Show("Creation");
        }

        /// Displays the backup execution page after loading available backups.
        private async void ShowExecution(object sender, RoutedEventArgs e)
        {
            await controllerBackup.DisplayBackups();

            visible.Show("Execution");
        }

        /// Shows the logs page and loads log data.
        private async void ShowLogs(object sender, RoutedEventArgs e)
        {
            await logs.DisplayLogs();

            visible.Show("Logs");
        }

        /// Exits the application.
        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// Changes the application language settings.
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

        }

        /// Updates application settings based on user input.
        private async void SettingsChange(object sender, EventArgs e)
        {
            // Update log format using the selected index from the dropdown list
            MenuController menuController = new MenuController(menuModel, format);
            await menuController.HandleLogFormat(Format_list.SelectedIndex);
            // Update file extensions for encryption
            extensionController.ExtensionsChange();
            // Display confirmation message for settings update
            Settings_OK.Text = await Translation.Instance.Translate("Les paramètres ont été modifiés avec succès.");
            // Retrieve and process business software names from the text box
            string softwareNames = BusinessSoftwareTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(softwareNames))
            {
                var existingSoftware = new List<string>();

                // Check if the config file exists and load existing software names
                if (File.Exists("config.txt"))
                {
                    existingSoftware = File.ReadAllLines("config.txt").Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                }

                // Split the new input by commas, trim spaces, and remove empty entries
                var newSoftwareList = softwareNames.Split(',')
                                                   .Select(s => s.Trim())
                                                   .Where(s => !string.IsNullOrEmpty(s))
                                                   .ToList();

                // Delete only software no longer in BusinessSoftwareTextBox
                var updatedSoftwareList = existingSoftware.Where(s => newSoftwareList.Contains(s)).ToList();

                // Add new software only
                foreach (var software in newSoftwareList)
                {
                    if (!updatedSoftwareList.Contains(software))
                    {
                        updatedSoftwareList.Add(software);
                    }
                }

                // Update config.txt with the updated list
                File.WriteAllLines("config.txt", updatedSoftwareList);

                // Reload the list of displayed software
                LoadBusinessSoftware();

                MessageBox.Show("Logiciel(s) métier mis à jour avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // If the TextBox is empty, delete all business software
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



            // Retrieve and process file extensions from the text box
            string extensions = Extensions.Text.Trim();

            if (!string.IsNullOrEmpty(extensions))
            {
                var existingExtensions = new List<string>();

                // Check if the extensions file exists and load existing entries
                if (File.Exists("extensions.txt"))
                {
                    existingExtensions = File.ReadAllLines("extensions.txt").Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                }

                // Process new extensions: trim, remove empty values, and ensure they start with a dot
                var newExtensionsList = extensions.Split(',')
                                          .Select(s => s.Trim())
                                          .Where(s => !string.IsNullOrEmpty(s))
                                          .Select(s => s.StartsWith(".") ? s : "." + s) // Ensure a dot prefix
                                          .Distinct()
                                          .ToList();

                // Remove extensions that are no longer listed
                var updatedExtensionsList = existingExtensions.Where(s => newExtensionsList.Contains(s)).ToList();


                // Add only new extensions that are not already in the list
                foreach (var extensionName in newExtensionsList)
                {
                    if (!updatedExtensionsList.Contains(extensions))
                    {
                        updatedExtensionsList.Add(extensions);
                    }
                }

                // Save the updated extensions list to extensions.txt
                File.WriteAllLines("extensions.txt", newExtensionsList);


                // Reload the displayed extensions list
                LoadExtensions();


                // Notify the user that the extensions list has been updated
                MessageBox.Show("Extensions prioritaires mises à jour avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // If the text box is empty, remove all extensions
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



            // Validate and update the maximum file size for backups
            if (Regex.IsMatch(NKoctetsTextBox.Text, @"^\d+$")) // Ensure input is numeric
            {
                GlobalVariables.maximumSize = int.Parse(NKoctetsTextBox.Text); // Convert and store the value
            }
            else
            {
                // Display an error message if the input is invalid
                MessageBox.Show(await Translation.Instance.Translate("La taille maximale des fichiers doit être un nombre."), "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// Initiates the creation of a backup asynchronously.
        private async void BackupCreation(object sender, EventArgs e)
        {
            // Call the backup controller to create a new backup
            await backupController.CreateBackup();

            Creation_OK.Text = await Translation.Instance.Translate("La sauvegarde a été créer avec succès.");
        }

        /// Executes the backup process if no business software is running.
        private void BackupExecution(object sender, EventArgs e)
        {
            // Check if business software is running; if so, block execution
            if (IsBusinessSoftwareRunning())
            {
                MessageBox.Show("Un logiciel métier est en cours d'exécution. Impossible de démarrer une nouvelle sauvegarde.",
                    "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Log the blocked backup attempt
                File.AppendAllText(GlobalVariables.PathBackup, $"[{DateTime.Now}] Tentative de lancement d'une sauvegarde bloquée car un logiciel métier est actif.\n");
                return; // Block launch
            }

            // Update UI buttons to control backup operations
            Play_b.Click -= BackupExecution;
            Pause_b.Click += PauseSelectedBackups;
            Stop_b.Click += StopSelectedBackups;


            // Start the backup execution
            RealTimeState realTimeState = new RealTimeState(serverController);
             backupController.ExecuteBackupAsync(ToDo_t.Text, realTimeState);
        }

        /// Pauses the selected backups.
        public async void PauseSelectedBackups(object sender, RoutedEventArgs e)
        {
            // Retrieve the value from the UI thread
            string todoText = "";
            Application.Current.Dispatcher.Invoke(() =>
            {
                todoText = ToDo_t.Text; // Retrieves the value of ToDo_t.Text on the UI thread
            });

            // Now we can use the variable outside the Dispatcher
            List<int> backupIndexes = backupController.ParseJobIndex(todoText);


            // Pause each selected backup process
            foreach (int index in backupIndexes)
            {
                backupController.PauseBackup(index);
            }
            // Enable the resume button functionality
            Resume_b.Click += ResumeSelectedBackups;

            // Update the UI to indicate that backups are paused
            Application.Current.Dispatcher.Invoke(async () =>
            {
                Play_Pause_Stop.Content = await Translation.Instance.Translate("Les sauvegardes sont en pause.");
            });

        }

        /// Resumes the selected paused backups.
        public async void ResumeSelectedBackups(object sender, RoutedEventArgs e)
        {
            // Safely retrieve the value from the UI thread
            string toDoText = "";
            Application.Current.Dispatcher.Invoke(() =>
            {
                toDoText = ToDo_t.Text; // Safely retrieve text from the UI
            });

            // Parse the job indexes from the input text
            List<int> backupIndexes = backupController.ParseJobIndex(toDoText);


            // Resume each selected backup process
            foreach (int index in backupIndexes)
            {
                backupController.ResumeBackup(index);
            }
            // Update the UI to indicate that backups have resumed
            string translatedText = await Translation.Instance.Translate("Les sauvegardes ont repris.");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Play_Pause_Stop.Content = translatedText;
            });
        }

        /// Stops the selected backups.
        public async void StopSelectedBackups(object sender, RoutedEventArgs e)
        {
            string toDoText = "";

            // Safely retrieve the value from the UI
            Application.Current.Dispatcher.Invoke(() =>
            {
                toDoText = ToDo_t.Text;
            });

            // Parse the job indexes from the input text
            List<int> backupIndexes = backupController.ParseJobIndex(toDoText);

            // Stop each selected backup process
            foreach (int index in backupIndexes)
            {
                backupController.StopBackup(index);
            }

            // Reset the button functionalities
            Play_b.Click += BackupExecution;
            Pause_b.Click -= PauseSelectedBackups;
            Resume_b.Click -= ResumeSelectedBackups;
            Stop_b.Click -= StopSelectedBackups;

            // Update the UI to indicate that backups have stopped
            string translatedText = await Translation.Instance.Translate("Les sauvegardes sont arrêtées.");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Play_Pause_Stop.Content = translatedText;
            });

        }
        private void Language_b_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}