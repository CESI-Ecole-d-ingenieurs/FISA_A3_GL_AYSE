using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using translation;
using EasySave.ControllerLib;
using EasySave.IviewLib;
namespace ProjetV0._1.View
{
    internal class MenuView : IMenuView
    {
        /// Displays the available actions in the menu and highlights the selected one.
        public async Task DisplayActions(List<string> actions, int selectedIndex)
        {
            Console.Clear();
            for (int i = 0; i < actions.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Highlight the selected action.
                    Console.WriteLine($"> " + await Translation.Instance.Translate(actions[i])); // Display the selected action with a cursor.
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("  " + await Translation.Instance.Translate(actions[i])); // Display the other actions without a cursor.
                }
            }
        }
        /// Displays an input prompt message to the user.
        public void DisplayInputPrompt(string message)
        {
            Console.WriteLine(message);
        }
        /// Clears the console screen.
        public void ClearScreen()
        {
            Console.Clear();
        }
    }
}