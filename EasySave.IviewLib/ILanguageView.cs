using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.IviewLib
{
    public interface ILanguageView
    {
         void DisplayLanguages(List<string> languages, int selectionIndex);
    }
}
