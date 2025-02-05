using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Model
{
    public class BackupState
    {
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int RemainingFiles { get; set; }
        public int Progress { get; set; }
    }
}
