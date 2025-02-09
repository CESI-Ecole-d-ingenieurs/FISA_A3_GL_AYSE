using EasySave.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Controller.Strategy
{
    public abstract class BaseBackupStrategy: BackupStrategy
    {
        protected Logger logger = new Logger();

        public abstract void ExecuteBackup(string source, string target);

        protected void DirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public void BackupFile(String file,String Source, String Target)
        {
            var targetFile = file.Replace(Source, Target);
            var startTime = DateTime.Now;
            try
            {
                File.Copy(file, targetFile, true);
                var endTime = DateTime.Now;
                var duration = (endTime - startTime).TotalMilliseconds;
                var fileInfo = new FileInfo(file);
                var fileSize = fileInfo.Length;
                logger.WriteLog(Path.GetFileName(file), file, targetFile, fileSize, duration);
            }
            catch (Exception ex)
            {

                logger.WriteLog(Path.GetFileName(file), file, targetFile, 0, 0, true);
                Console.WriteLine($"Error copying file {file}: {ex.Message}");
            }
        }
    }
}
