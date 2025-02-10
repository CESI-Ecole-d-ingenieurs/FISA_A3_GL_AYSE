using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjetV0._1.Model
{
    public class BackupStateJournal
    {
            private static readonly string stateFilePath = "backup_state.json";
            private static readonly object lockObj = new();
            private static List<IObserver> observers = new List<IObserver>();

            // Add observer
            public static void AddObserver(IObserver observer)
            {
                observers.Add(observer);
            }

            // Notify all observers
            private static void NotifyObservers(BackupState state)
            {
                foreach (var observer in observers)
                {
                    observer.Update(state);
                }
            }

            public static BackupState ComputeState(string name, string sourceDirectory, string targetDirectory)
            {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException("Source directory does not exist.");
            }

            string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                long totalSize = 0;

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    totalSize += fi.Length;
                }

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

            public static void UpdateProgress(string name)
            {
                lock (lockObj)
                {
                    List<BackupState> states = LoadState();
                    BackupState state = states.Find(e => e.Name == name);

                    if (state != null && state.RemainingFiles > 0)
                    {
                        state.RemainingFiles--;
                        state.Progress = 100 - (state.RemainingFiles * 100 / state.TotalFilesToCopy);

                        if (state.RemainingFiles == 0)
                        {
                            state.State = "END";
                        }

                        SaveState(states);
                        NotifyObservers(state);

                    Console.WriteLine($"Mise à jour : {state.Name} -> {state.Progress}%"); // Vérification
                }
                }
            }


            public static List<BackupState> LoadState()
            {
                if (!File.Exists(stateFilePath)) return new List<BackupState>();

                string existingData = File.ReadAllText(stateFilePath);
                return string.IsNullOrEmpty(existingData) ? new List<BackupState>() : JsonSerializer.Deserialize<List<BackupState>>(existingData) ?? new List<BackupState>();
            }

            private static void SaveState(List<BackupState> states)
            {
                string jsonData = JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(stateFilePath, jsonData);
            }

        //To get the state of the backup
        public static List<BackupState> GetState()
        {
            lock (lockObj)
            {
                return LoadState();
            }
        }

    }
}
