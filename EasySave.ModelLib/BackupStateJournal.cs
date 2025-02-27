using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace EasySave.ModelLib
{
    /// Manages backup states, tracks progress, and notifies observers.
    public class BackupStateJournal
    {
        private static readonly string stateFilePath = GlobalVariables.PathTempsReel;
        private static readonly object lockObj = new object();
        private static List<IObserver> observers = new List<IObserver>();

        /// Adds an observer to be notified of state changes.
        public static void AddObserver( IObserver observer)
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
                    state.RemainingFiles--;

                    if (state.TotalFilesToCopy > 0)
                    {
                        state.Progress = 100 - (state.RemainingFiles * 100 / state.TotalFilesToCopy);
                    }

                    if (state.RemainingFiles == 0)
                    {
                        state.State = "COMPLETED";
                    }

                    SaveState(states);
                    NotifyObservers(state);
                }
            }
        }


        /// Loads the current backup states from the JSON file.
        public static List<BackupState> LoadState()
        {
            if (!File.Exists(stateFilePath)) return new List<BackupState>();

            string existingData = File.ReadAllText(stateFilePath);
            if (string.IsNullOrEmpty(existingData)) return new List<BackupState>();

            if (CheckFileExtension(stateFilePath, ".xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<BackupState>), new XmlRootAttribute("Root")); // Replace "Root" by the name of the root element in your XML file

                using (StreamReader reader = new StreamReader(stateFilePath))
                {
                    List < BackupState > s = (List<BackupState>)serializer.Deserialize(reader) ?? new List<BackupState>(); 
                    return s;
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
            if (CheckFileExtension(stateFilePath, ".xml"))
            {
                // Use XmlSerializer to serialize in XML directly
                var serializer = new XmlSerializer(typeof(List<BackupState>), new XmlRootAttribute("Root"));
                using (var writer = new StreamWriter(stateFilePath))
                {
                    serializer.Serialize(writer, states);
                }
            }
            else
            {
                // Serialization en JSON
                string jsonData = JsonConvert.SerializeObject(states, Newtonsoft.Json.Formatting.Indented);
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

        /// This method is used to compare the extension of a file with a given extension.
        public static bool CheckFileExtension(string fileName, string extension)
        {
            return fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }
    }
}
