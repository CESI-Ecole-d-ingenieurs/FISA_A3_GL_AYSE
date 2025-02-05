using ProjetV0._1.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translation;
namespace ProjetV0._1.View
{
    internal class MenuView
    {
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

        public void DisplayInputPrompt(string message)
        {
            Console.WriteLine(message);
        }

        public void ClearScreen()
        {
            Console.Clear();
        }
    }
}
