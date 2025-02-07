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

        public Logger(string logFilePath, string stateFilePath)
        {
            
        }
        public string GetLogFileName()
        {
            return $"backup_log_{DateTime.Today:dd-MM-yyyy}.json";
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
                File.AppendAllText(logFile, jsonData + Environment.NewLine);
                Console.Write($"nom: {logFile}");
            }
        }
    }
}
//joute une référence à la DLL en exécutant :
//sh
//Copier
//dotnet add reference ../EasySave.Logger/EasySave.Logger.csproj
//(Remplace ../EasySave.Logger/ par le chemin correct vers le projet DLL si nécessaire).

