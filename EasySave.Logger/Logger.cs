using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Dynamic;

namespace EasySave.Logger
{
    public class Logger
    {
        //public class LogEntry
        //{
        //    public string Name { get; set; }
        //    public string FileSource { get; set; }
        //    public string FileTarget { get; set; }
        //    public long FileSize { get; set; }
        //    public double FileTransferTime { get; set; }
        //    public string Date { get; set; }
        //}

        // Synchronization object to ensure thread safety during file writing
        private static object lockObj = new object();
        private readonly string logFilePath; // Stores the path of the log file

        /// Constructor initializes the log file path.
        public Logger()
        {
            logFilePath = GetLogFileName();
        }

        /// Property to retrieve the log file path.
        public string LogFilePath
        {
            get { return logFilePath; }
        }

        /// Generates the log file name based on the current date and stores logs in a specific directory.
        public static string GetLogFileName()
        {
            //setx EASYSAVE_LOG_PATH "C:\ProgramData\CESI\EasySave\Logs" 
            string basePath = Environment.GetEnvironmentVariable("EASYSAVE_LOG_PATH") ??
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CESI", "EasySave", "Logs");
            // Create directory if it doesn't exist
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            return Path.Combine(basePath, $"backup_log_{DateTime.Today:dd-MM-yyyy}.json");
        }

        /// Writes a log entry for a backup operation.
        public void WriteLog(String name,String fileSource,String fileTarget, long fileSize, double fileTransferTime, bool isError = false)
        {
           // string logFile = logFilePath;
            lock (lockObj) // Ensures thread safety while writing to the log file
            {
                var LogEntry = new
                {
                    Name = name,
                    FileSource =  fileSource,
                    FileTarget = fileTarget,
                    FileSize = fileSize,
                    FileTransferTime = isError ? -1 : fileTransferTime,
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                // Convert log entry to JSON format
                string jsonData = JsonConvert.SerializeObject(LogEntry, Formatting.Indented);
                //File.AppendAllText(logFile, jsonData + Environment.NewLine);
                // Check if the file exists and is not empty
                if (!File.Exists(logFilePath) || new FileInfo(logFilePath).Length == 0)
                {
                    // Initialize file with an empty array if it doesn't exist or is empty
                    File.WriteAllText(logFilePath, "[" + jsonData + "]" + Environment.NewLine);
                }
                else
                {
                    // Read existing content and remove the last bracket
                    string existingData = File.ReadAllText(logFilePath);
                    existingData = existingData.TrimEnd(']', '\r', '\n', ' ');

                    // Append the new log entry with a leading comma and close the array
                    existingData += "," + Environment.NewLine + jsonData + "]";
                    // Write the updated log data back to the file
                    File.WriteAllText(logFilePath, existingData);
                }
                Console.Write($"nom: {logFilePath}");
            }
        }
    }
}
//ajoute une référence à la DLL en exécutant :
//dotnet add reference C:\Users\salem\source\repos\Aysee2\FISA_A3_GL_AYSE2\EasySave.Logger/EasySave.Logger.csproj

