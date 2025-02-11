using ProjetV0._1.Controller.BackupFactory;
using ProjetV0._1.Controller.Strategy;
using ProjetV0._1.Model;
using ProjetV0._1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;

namespace ProjetV0._1.Controller
{
    internal class BackupController
    {
        private List<BackupModel> BackupList ;
        private BackupStrategyFactory _BackupStrategyFactory;
        private BackupView _backupView = new BackupView();
        //private string RegisteredBackupsPath = "C:\\Users\\lisaj\\OneDrive - Association Cesi Viacesi mail\\A3\\Génie_logiciel\\Projet\\FISA_A3_GL_AYSE\\ProjetV0.1\\RegisteredBackups.txt";
        private string RegisteredBackupsPath = "..\\..\\..\\RegisteredBackups.txt";
        //private StrategieSauvegarde _StrategieSauvegarde;
        //public GestionnaireDeSauvegarde(StrategieSauvegarde strategieSauvegarde)
        //{
        //    _StrategieSauvegarde = strategieSauvegarde; 
        //}

        /// Executes selected backups based on user input.
        /// It retrieves the backup index, initializes the strategy (complete or differential),
        /// copies the files, updates the state, and logs the execution time.

        public BackupController()
        {
            string filePath = GlobalVariables.PathBackup;  // Assurez-vous que le chemin est correct
            BackupList = new List<BackupModel>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(" - ");
                        if (parts.Length >= 4)
                        {
                            BackupModel sauvegarde = new BackupModel(parts[0], parts[1], parts[2], parts[3]);


                            BackupList.Add(sauvegarde);
                            Console.WriteLine("hu");
                        }
                    }
                }
                Console.WriteLine("hu");
            }
            catch { }
            }
        
        public void ExecuteBackup(string input)
        {
            List<int> BackupIndex = ParseJobIndex(input);
            BackupStateJournal.AddObserver(new ConsoleView()); // Add observer for real-time progress display
                    foreach (var index in BackupIndex)
            {
                if (index - 1 < NumberLinesFile() && index > 0)
                {
                    BackupModel backup = BackupList[index - 1];
                    _BackupStrategyFactory = backup.Type == "Complète"
                        ? new CompleteBackupFactory()
                        : new DifferentialBackupFactory();

                    var strategy = _BackupStrategyFactory.CreateBackupStrategy();
                    strategy.ExecuteBackup(BackupList[index - 1].Source, BackupList[index - 1].Target);
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    BackupState state = BackupStateJournal.ComputeState(backup.Name, backup.Source, backup.Target);
                    BackupStateJournal.UpdateState(state);

                    string[] files = Directory.GetFiles(backup.Source, "*", SearchOption.AllDirectories);
                    int totalFiles = files.Length;
                    int processedFiles = 0;

                    foreach (var file in files)
                    {
                        string destFile = file.Replace(backup.Source, backup.Target);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                        File.Copy(file, destFile, true);

                        processedFiles++;
                        BackupStateJournal.UpdateProgress(backup.Name); // Real-time update
                        Thread.Sleep(500); // Slow down the process for better visualization
                    }

                    stopwatch.Stop();
                    state.Progress = 100;
                    state.State = "END";
                    BackupStateJournal.UpdateState(state);

                    Console.WriteLine($"Sauvegarde {backup.Name} terminée en {stopwatch.Elapsed.TotalSeconds} secondes.");
                }
            }
        }

        /// Displays the list of existing backups saved in a file.
        /// If backups exist, it lists them with their name, source, destination, and type.
        /// If no backups are found, it notifies the user.
        public async Task DisplayExistingBackups()
        {
            Console.WriteLine(await Translation.Instance.Translate("Sauvegardes disponibles :"));
            FileInfo fileinfo = new FileInfo(RegisteredBackupsPath);
            if (fileinfo.Length > 0) // Check if the file is not empty
            {
                int lineNumber = 1;
                Console.WriteLine(await Translation.Instance.Translate("Nom - Source - Destination - Type"));
                foreach (string line in File.ReadLines(RegisteredBackupsPath))
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
            if (BackupList.Count >= 5)
            {
                Console.WriteLine(await Translation.Instance.Translate("Maximum  5 Sauvegarde"));
                return;
            }
            BackupModel sauvegarde = await _backupView.UserAsk();
            BackupList.Add(sauvegarde);
            File.AppendAllText(RegisteredBackupsPath, $"{sauvegarde.Name} - {sauvegarde.Source} - {sauvegarde.Target} - {sauvegarde.Type}\n");
            Console.WriteLine(await Translation.Instance.Translate($"Sauvegarde'{sauvegarde.Name}' ajouté."));
        }
        //focntion indice 

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
        //    foreach (int Index in Indexes)
        //{
        //        Console.WriteLine(Index);
        //    }

            return Indexes;
        }
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

               // Console.WriteLine("Le nombre de lignes dans le fichier est : " + lineCount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Une erreur s'est produite lors de la lecture du fichier:");
                Console.WriteLine(e.Message);
            }
            return lineCount;
        }
    
    }

}
