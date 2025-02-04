using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Vue
{
    internal class LanguageView
    {
        public void DisplayLanguages(List<string> languages, int selectionIndex)
        {
            Console.Clear();
            Console.WriteLine("Veuillez choisir une langue / Choose a language :");

            for (int i = 0; i < languages.Count(); i++)
            {
                if (i == selectionIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {languages[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {languages[i]}");
                }
            }
        }
    }
}
