using EasySave.IviewLib;
using EasySave.ModelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasySave.ControllerLib.BackupFactory;
using System.Diagnostics;
using System.Reflection;
namespace EasySave.ControllerLib
{

    public class BackupController
    {
        private List<BackupModel> BackupList;
        private BackupStrategyFactory _BackupStrategyFactory;
        private IBackupView backupview;
        private Dictionary<string, bool> _isPaused = new Dictionary<string, bool>();
        private Dictionary<string, CancellationTokenSource> _cancellationTokens = new Dictionary<string, CancellationTokenSource>();

        // This method verify if a busness software defined by the user is running in the system.
        public bool IsBusinessSoftwareRunning()
        {
            if (!File.Exists("config.txt")) // "config.txt" contains all the business software defined by the user in the settings
                return false;

            var businessSoftwareList = File.ReadAllLines("config.txt")
                                           .Select(s => s.Trim().ToLower())
                                           .Where(s => !string.IsNullOrEmpty(s))
                                           .ToList();

            return businessSoftwareList.Any(software => Process.GetProcesses()
                                                                .Any(p => p.ProcessName.ToLower().Contains(software)));
        }

        // This method change the value of a backup in the _isPaused dictionnary to true. It is called when we pressed the "Pause" button on the interface.
        public void PauseBackup(int index)
        {
            string name = GetBackupNameByIndex(index);
            
            if (name != "null")
            {
                if (_isPaused.ContainsKey(name))
                {
                    _isPaused[name] = true;
                }
            }
        }

        // This method change the value of a backup in the _isPaused dictionnary to false. It is called when we pressed the "Resume" button on the interface.
        public void ResumeBackup(int index)
        {
            string name = GetBackupNameByIndex(index);
            
            if (name != "null")
            {
                if (_isPaused.ContainsKey(name))
                {
                    _isPaused[name] = false;
                }
            }
        }

        // This method cancel the cancellation token of a backup in the _cancellationTokens dictionnary to false. It is called when we pressed the "Stop" button on the interface.
        public void StopBackup(int index)
        {
            string name = GetBackupNameByIndex(index);
            
            if (name != "null")
            {
                if (_cancellationTokens.ContainsKey(name))
                {
                    _cancellationTokens[name].Cancel();
                }
            }
        }


        // This method search all the registered backups in a file and put them in a list when we create an instance of this class.
        public BackupController(IBackupView backupView)
        {
            backupview = backupView;
            string filePath = GlobalVariables.PathBackup;  // Assurez-vous que le chemin est correct
            BackupList = new List<BackupModel>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 4)
                        {
                            BackupModel sauvegarde = new BackupModel(parts[0], parts[1], parts[2], parts[3]);


                            BackupList.Add(sauvegarde);
                        }
                    }
                }
            }
            catch { }
        }

        /// This method makes a list with every backup that the user would like to execute
        /// All the backups in the list are added to the _isPaused dictionary with the value false (or the value is initialized to false if the backup is already in the dictionnary)
        /// All the backups in the list are added to the _cancellationTokens dictionary with a new token (or the value is reinitialized if the backup is already in the dictionnary)
        public async Task ExecuteBackupAsync(string input, EasySave.ModelLib.IObserver consoleView)
        {
            List<int> BackupIndex = ParseJobIndex(input);
            BackupStateJournal.AddObserver(consoleView); // Add observer for real-time progress display
            
            foreach (var index in BackupIndex)
            {
                BackupModel backup = BackupList[index - 1];
                
                if (!_isPaused.ContainsKey(backup.Name))
                {
                    _isPaused.Add(backup.Name, false);
                }
                else
                {
                    _isPaused[backup.Name] = false;
                }

                if (!_cancellationTokens.ContainsKey(backup.Name))
                {
                    _cancellationTokens.Add(backup.Name, new CancellationTokenSource());
                }
                else
                {
                    _cancellationTokens[backup.Name] = new CancellationTokenSource();
                }
                CancellationToken token = _cancellationTokens[backup.Name].Token;
                Task.Run(() => ExecuteOneBackup(index,token));
            }
        }

        /// This method creates an instance of CompleteBackupFactory or DifferentialBackupFactory and execute a backup
        public async Task ExecuteOneBackup(int index, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested(); // Vérifie à nouveau avant d'exécuter des opérations longues
            if (index - 1 < NumberLinesFile() && index > 0)
            {

                if (BackupList != null && index > 0 && index <= BackupList.Count)
                {

                    BackupModel backup = BackupList[index - 1];

                    _BackupStrategyFactory = backup.Type == "Complète"
                    ? (BackupStrategyFactory)new CompleteBackupFactory()
                        : (BackupStrategyFactory)new DifferentialBackupFactory();
                    BackupState state = BackupStateJournal.ComputeState(backup.Name, backup.Source, backup.Target);
                    if (IsBusinessSoftwareRunning())
                    {
                        Console.WriteLine("Sauvegarde annulée : Un logiciel métier est en cours d'exécution.");
                        state.State = "Finished BY BUSINESSS SOFTWARE";
                        BackupStateJournal.UpdateState(state);
                        return;
                    }

                    BackupStateJournal.UpdateState(state);
                    var strategy = _BackupStrategyFactory.CreateBackupStrategy(backupview);

                    await strategy.ExecuteBackup(BackupList[index - 1].Source, BackupList[index - 1].Target, backup.Name, _isPaused, _cancellationTokens);
                    token.ThrowIfCancellationRequested(); // Vérifie à nouveau avant d'exécuter des opérations longues
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();


                    string[] files = Directory.GetFiles(backup.Source, "*", SearchOption.AllDirectories);
                    int totalFiles = files.Length;
                    int processedFiles = 0;

                    stopwatch.Stop();
                    state.Progress = 100;
                    state.State = "END";
                    BackupStateJournal.UpdateState(state);
                }
                else
                {
                    // Handle the error case or log that the index was out of bounds
                    Console.WriteLine("Index is out of range.");
                }
            }
        }

        /// Displays the list of existing backups saved in a file.
        /// If backups exist, it lists them with their name, source, destination, and type.
        /// If no backups are found, it notifies the user.
        public async Task DisplayExistingBackups()
        {
            Console.WriteLine(await Translation.Instance.Translate("Sauvegardes disponibles :"));
            FileInfo fileinfo = new FileInfo(GlobalVariables.PathBackup);
            if (fileinfo.Length > 0) // Check if the file is not empty
            {
                int lineNumber = 1;
                Console.WriteLine(await Translation.Instance.Translate("Nom-Source-Destination-Type"));
                foreach (string line in File.ReadLines(GlobalVariables.PathBackup))
                {
                    Console.WriteLine($"{lineNumber}. {line}");
                    lineNumber++;
                }
            }
            else
            {
                Console.WriteLine(await Translation.Instance.Translate("Aucune sauvegarde enregistrée."));
            }
        }

        /// Retrieves the number of backups currently stored in memory.
        public int GetBackupCount()
        {
            return BackupList.Count;
        }

        /// Creates a new backup by asking the user for input.
        /// The backup details are stored in a file and added to the list. 
        public async Task CreateBackup()
        {
            BackupModel sauvegarde = await backupview.UserAsk();

            BackupList.Add(sauvegarde);
            if (!Directory.Exists(Path.GetDirectoryName(GlobalVariables.PathBackup)))
            {

                Directory.CreateDirectory(Path.GetDirectoryName(GlobalVariables.PathBackup));
            }
            File.AppendAllText(GlobalVariables.PathBackup, $"{sauvegarde.Name}-{sauvegarde.Source}-{sauvegarde.Target}-{sauvegarde.Type}\n");
        }

        /// Parses the user input to determine which backup jobs to execute.
        /// Supports individual and range selections (e.g., "1-3" or "1;3").
        public List<int> ParseJobIndex(string input)
        {
            var Indexes = new List<int>();
            var parts = input.Split(';');

            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    var rangeParts = part.Split('-');
                    if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int start) && int.TryParse(rangeParts[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            Indexes.Add(i);
                        }
                    }
                }
                else if (int.TryParse(part, out int singleIndex))
                {
                    Indexes.Add(singleIndex);
                }
            }
            return Indexes;
        }

        // This method count and return the number of lines (number of backups) in the file were the backups are saved.
        public int NumberLinesFile()
        {
            int lineCount = 0;

            try
            {
                using (StreamReader sr = new StreamReader(GlobalVariables.PathBackup))
                {
                    while (sr.ReadLine() != null)
                    {
                        lineCount++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Une erreur s'est produite lors de la lecture du fichier:");
                Console.WriteLine(e.Message);
            }
            return lineCount;
        }

        // This method return the name of a backup based on its index.
        private string GetBackupNameByIndex(int index)
        {
            if (index > 0 && index <= BackupList.Count)
            {
                return BackupList[index - 1].Name;
            }
            return "null";
        }

    }
}
