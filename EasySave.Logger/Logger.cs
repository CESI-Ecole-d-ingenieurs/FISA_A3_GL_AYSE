using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

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

        private static object lockObj = new object();

       
        public string GetLogFileName()
        {
            //setx EASYSAVE_LOG_PATH "C:\ProgramData\CESI\EasySave\Logs" 
            string basePath = Environment.GetEnvironmentVariable("EASYSAVE_LOG_PATH") ??
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CESI", "EasySave", "Logs");
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            return Path.Combine(basePath, $"backup_log_{DateTime.Today:dd-MM-yyyy}.json");
        }
        public void WriteLog(String name,String fileSource,String fileTarget, long fileSize, double fileTransferTime, bool isError = false)
        {
            string logFile = GetLogFileName();
            lock (lockObj)
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
               
                string jsonData = JsonConvert.SerializeObject(LogEntry, Formatting.Indented);
                //File.AppendAllText(logFile, jsonData + Environment.NewLine);
                if (!File.Exists(logFile) || new FileInfo(logFile).Length == 0)
                {
                    // Initialize file with an empty array if it doesn't exist or is empty
                    File.WriteAllText(logFile, "[" + jsonData + "]" + Environment.NewLine);
                }
                else
                {
                    // Read existing content and remove the last bracket
                    string existingData = File.ReadAllText(logFile);
                    existingData = existingData.TrimEnd(']', '\r', '\n', ' ');

                    // Append the new log entry with a leading comma and close the array
                    existingData += "," + Environment.NewLine + jsonData + "]";
                    File.WriteAllText(logFile, existingData);
                }
                Console.Write($"nom: {logFile}");
            }
        }
    }
}
//joute une référence à la DLL en exécutant :
//dotnet add reference C:\Users\salem\source\repos\Aysee2\FISA_A3_GL_AYSE2\EasySave.Logger/EasySave.Logger.csproj

