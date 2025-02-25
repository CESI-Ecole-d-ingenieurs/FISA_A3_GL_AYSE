using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ModelLib
{
    /// Represents a backup job with its properties such as name, source, target, and type.
    public class BackupModel
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Type { get; set; }
        public BackupModel(string name, string source, string target, string type)
        {
            Name = name;
            Source = source;
            Target = target;
            Type = type;
        }
    }
}
