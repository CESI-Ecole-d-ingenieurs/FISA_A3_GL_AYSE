using ProjetV0._1.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;
using EasySave.IviewLib;
namespace ProjetV0._1.View
{
    //public interface IMenuView
    //{
    //    public  Task DisplayActions(List<string> actions, int selectedIndex);
    //    public void DisplayInputPrompt(string message);
    //    public void ClearScreen();
    //}
    internal class MenuView: IMenuView
    {
        /// Displays the available actions in the menu and highlights the selected one.
        public async Task DisplayActions(List<string> actions, int selectedIndex)
        {
            Console.Clear();
            for (int i = 0; i < actions.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> " + await Translation.Instance.Translate(actions[i]));
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("  " + await Translation.Instance.Translate(actions[i]));
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
