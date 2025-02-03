using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Projet_Génie_Logiciel
{
    public class EtatSauvegarde
    {
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public int Progression { get; set; }
    }


    public class EtatSauvegardeJournal
    {
        private static readonly string stateFilePath = "backup_state.json";
        private static readonly object lockObj = new();

        public static EtatSauvegarde CalculerEtat(string name, string sourceDirectory, string targetDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException("Le répertoire source n'existe pas.");
            }

            string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            int totalFiles = files.Length;
            long totalSize = 0;
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                totalSize += fi.Length;
            }

            return new EtatSauvegarde
            {
                Name = name,
                SourceFilePath = sourceDirectory,
                TargetFilePath = targetDirectory,
                State = "ACTIVE",
                TotalFilesToCopy = totalFiles,
                TotalFilesSize = totalSize,
                NbFilesLeftToDo = totalFiles,
                Progression = 0
            };
        }

        public static void MettreAJourEtat(EtatSauvegarde etat)
        {
            lock (lockObj)
            {
                List<EtatSauvegarde> etats = new();
                if (File.Exists(stateFilePath))
                {
                    string existingData = File.ReadAllText(stateFilePath);
                    if (!string.IsNullOrEmpty(existingData))
                    {
                        etats = JsonSerializer.Deserialize<List<EtatSauvegarde>>(existingData) ?? new List<EtatSauvegarde>();
                    }
                }
                etats.RemoveAll(e => e.Name == etat.Name);
                etats.Add(etat);
                string jsonData = JsonSerializer.Serialize(etats, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(stateFilePath, jsonData);
            }
        }

        public static void MettreAJourProgression(string name)
        {
            lock (lockObj)
            {
                List<EtatSauvegarde> etats = new();
                if (File.Exists(stateFilePath))
                {
                    string existingData = File.ReadAllText(stateFilePath);
                    if (!string.IsNullOrEmpty(existingData))
                    {
                        etats = JsonSerializer.Deserialize<List<EtatSauvegarde>>(existingData) ?? new List<EtatSauvegarde>();
                    }
                }

                EtatSauvegarde etat = etats.Find(e => e.Name == name);
                if (etat != null && etat.NbFilesLeftToDo > 0)
                {
                    etat.NbFilesLeftToDo--;
                    etat.Progression = 100 - (etat.NbFilesLeftToDo * 100 / etat.TotalFilesToCopy);
                    if (etat.NbFilesLeftToDo == 0)
                    {
                        etat.State = "END";
                        Journalisation.Instance.EcrireLog(etat.Name, etat.SourceFilePath, etat.TargetFilePath, etat.TotalFilesSize, 0.0);
                    }
                }

                string jsonData = JsonSerializer.Serialize(etats, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(stateFilePath, jsonData);
            }
        }
    }
}
