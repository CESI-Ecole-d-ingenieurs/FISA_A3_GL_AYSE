using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.IviewLib
{
    // This interface is used for the actions.
    public interface IMenuView
    {
        Task DisplayActions(List<string> actions, int selectedIndex);
        void DisplayInputPrompt(string message);
        void ClearScreen();
    }
}