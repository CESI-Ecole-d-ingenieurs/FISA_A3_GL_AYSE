using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.IviewLib
{
    // Interface for the language view
    public interface ILanguageView
    {
        // This method display the available languages to the user.
         void DisplayLanguages(List<string> languages, int selectionIndex);
    }
}
