using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1
{
    internal class GestionnaireDeSauvegarde
    {
        private List<Sauvegarde> sauvegardeList=new List<Sauvegarde>();
        //private StrategieSauvegarde _StrategieSauvegarde;
        //public GestionnaireDeSauvegarde(StrategieSauvegarde strategieSauvegarde)
        //{
        //    _StrategieSauvegarde = strategieSauvegarde;
        //}
        public void ExecuteSauvegarde( string input)
        {
            Console.WriteLine("INPUT", input);
            List<int> sauvegardesIndice = ParseJobIndices(input);
            Console.WriteLine("Indice", sauvegardesIndice);
            foreach (var index in sauvegardesIndice)
            {
                if (index - 1 < sauvegardeList.Count && index > 0)
                {
                    if(sauvegardeList[index - 1].Type== "Complète")
                    {
                        StrategieSauvegarde _StrategieSauvegarde = new StrategieSauvegardeComplete();
                        _StrategieSauvegarde.ExecuterSauvegarde(sauvegardeList[index - 1].Source, sauvegardeList[index - 1].Destination);
                    }
                   else
                    {

                        StrategieSauvegarde _StrategieSauvegarde = new StrategieSauvegardeDiff();
                        _StrategieSauvegarde.ExecuterSauvegarde(sauvegardeList[index - 1].Source, sauvegardeList[index - 1].Destination);
                    }
                }
            }
                   
        }
        public void AjouterSauvegarde(Sauvegarde sauvegarde)
        {
            if (sauvegardeList.Count >= 5)
            {
                Console.WriteLine("Maximum  5 Sauvegarde");
                return;
            }
            sauvegardeList.Add(sauvegarde);
            Console.WriteLine($"Sauvegarde'{sauvegarde.Nom}' ajouté.");
        }
        //focntion indice 
        public List<int> ParseJobIndices(string input)
        {
            var indices = new List<int>();
            var parts = input.Split(';');

            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    var rangeParts = part.Split('-');
                    if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int start) && int.TryParse(rangeParts[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            indices.Add(i);
                        }
                    }
                }
                else if (int.TryParse(part, out int singleIndex))
                {
                    indices.Add(singleIndex);
                }
            }
            foreach (int indice in indices)
            {
                Console.WriteLine(indice);
            }
           
            return indices;
        }
    }
}
