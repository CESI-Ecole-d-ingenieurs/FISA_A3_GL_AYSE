using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.IviewLib
{
    // Interface for the menu view
    public interface IMenuView
    {
        // This method display the choices in a menu
         Task DisplayActions(List<string> actions, int selectedIndex);
        // This method display a message in the console
         void DisplayInputPrompt(string message);
        // This method clear the console
         void ClearScreen();
    }
}
