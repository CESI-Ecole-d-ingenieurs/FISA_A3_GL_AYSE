using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjetV0._1.Model
{
    /// Represents a backup job with its properties such as name, source, target, and type.
    internal class BackupModel
    {
        /// Gets or sets the name of the backup.
        public string Name { get; set; }

        /// Gets or sets the source directory for the backup.
        public string Source { get; set; }

        /// Gets or sets the target directory where the backup will be stored
        public string Target { get; set; }

        /// Gets or sets the type of the backup (e.g., Complete or Differential).
        public string Type { get; set; }

        /// Initializes a new instance of the <see cref="BackupModel"/> class.
        public BackupModel(string name, string source, string target, string type)
        {
            Name = name;
            Source = source;
            Target = target;
            Type = type;
        }
    }
}
