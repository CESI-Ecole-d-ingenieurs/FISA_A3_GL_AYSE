﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.IviewLib;
namespace ProjetV0._1.View
{
    //public interface ILanguageView
    //{
    //    public void DisplayLanguages(List<string> languages, int selectionIndex);
    //}
    internal class LanguageView: ILanguageView
    {
        /// Displays the list of available languages and highlights the currently selected option.
        public void DisplayLanguages(List<string> languages, int selectionIndex)
        {
            Console.Clear();
            Console.WriteLine("Veuillez choisir une langue / Choose a language :");

            for (int i = 0; i < languages.Count(); i++)
            {
                if (i == selectionIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Highlight the selected language.
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
