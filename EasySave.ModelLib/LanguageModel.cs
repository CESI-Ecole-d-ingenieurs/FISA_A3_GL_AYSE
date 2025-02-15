using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ModelLib
{
    /// Represents the language selection model for the application.
    public class LanguageModel
    {
        public List<string> Languages { get; set; } = new List<string> { "Français", "English", "Quitter / Exit" };
        public int SelectedLanguage { get; set; }
    }
}
