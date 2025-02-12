using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProjetV0._1.Model
{
    /// Manages backup states, tracks progress, and notifies observers.
    public class BackupStateJournal
    {
            private static readonly string stateFilePath = GlobalVariables.PathTempsReel;
            private static readonly object lockObj = new();
            private static List<IObserver> observers = new List<IObserver>();

            /// Adds an observer to be notified of state changes.
            public static void AddObserver(IObserver observer)
            {
                observers.Add(observer);
            }

            /// Notify all observers
            private static void NotifyObservers(BackupState state)
            {
                foreach (var observer in observers)
                {
                    observer.Update(state);
                }
            }

            /// Computes the initial state of a backup before execution.
            public static BackupState ComputeState(string name, string sourceDirectory, string targetDirectory)
            {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException("Source directory does not exist.");
            }
            // Retrieve all files from the source directory
            string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                long totalSize = 0;

                
                // Calculate the total size of all files.
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    totalSize += fi.Length;
                }

                // Create and return the initial backup state.
                var state = new BackupState
                {
                    Name = name,
                    SourceFilePath = sourceDirectory,
                    TargetFilePath = targetDirectory,
                    State = "ACTIVE",
                    TotalFilesToCopy = totalFiles,
                    TotalFilesSize = totalSize,
                    RemainingFiles = totalFiles,
                    Progress = 0
                };

                NotifyObservers(state);
                return state;
            }

            /// Updates the state of a backup operation.
            public static void UpdateState(BackupState state)
            {
                lock (lockObj)
                {
                    List<BackupState> states = LoadState();
                    states.RemoveAll(e => e.Name == state.Name);
                    states.Add(state);
                    SaveState(states);
                    NotifyObservers(state);
                }
            }

            /// Updates the progress of a backup and notifies observers.
            public static void UpdateProgress(string name)
            {
                lock (lockObj)
                {
                    List<BackupState> states = LoadState();
                    BackupState state = states.Find(e => e.Name == name);

                    if (state != null && state.RemainingFiles > 0)
                    {
                        // Decrement remaining files and update progress percentage.
                        state.RemainingFiles--;
                        state.Progress = 100 - (state.RemainingFiles * 100 / state.TotalFilesToCopy);
                        // If no files remain, mark the backup as completed.
                        if (state.RemainingFiles == 0)
                        {
                            state.State = "COMPLETED";
                        }

                        SaveState(states);
                        NotifyObservers(state);

                    Console.WriteLine($"Mise à jour : {state.Name} -> {state.Progress}%"); 
                }
                }
            }

            /// Loads the current backup states from the JSON file.
            public static List<BackupState> LoadState()
            {
                if (!File.Exists(stateFilePath)) return new List<BackupState>();

                string existingData = File.ReadAllText(stateFilePath);
            //return string.IsNullOrEmpty(existingData) ? new List<BackupState>() : JsonSerializer.Deserialize<List<BackupState>>(existingData) ?? new List<BackupState>();
            if(string.IsNullOrEmpty(existingData)) return new List<BackupState>();

            if (CheckFileExtension(stateFilePath, ".xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<BackupState>), new XmlRootAttribute("Root")); // Remplacez "Root" par le nom de l'élément racine approprié dans votre XML

                using (StreamReader reader = new StreamReader(stateFilePath))
                {
                    return (List<BackupState>)serializer.Deserialize(reader) ?? new List<BackupState>();
                }
            }
            else
            {
                // Deserialize JSON
                return JsonConvert.DeserializeObject<List<BackupState>>(existingData) ?? new List<BackupState>();
            }
        
            }
            
            /// Saves the backup states to the JSON file.
            private static void SaveState(List<BackupState> states)
            {
            // string jsonData = JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true });
            string jsonData = JsonConvert.SerializeObject(states, Formatting.Indented);

            if (CheckFileExtension(stateFilePath, ".xml"))
            {
                var xmlDoc = JsonConvert.DeserializeXNode($"{{'Root':{jsonData}}}", "Root");
                xmlDoc.Save(stateFilePath);
            }
            else
            {
                File.WriteAllText(stateFilePath, jsonData);
            }
            }

        ///To get the state of the backup
        public static List<BackupState> GetState()
        {
            lock (lockObj)
            {
                return LoadState();
            }
        }
        public static bool CheckFileExtension(string fileName, string extension)
        {
            return fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }
    }
}
