using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Vue
{
    public class LogView
    {
        public void DisplayLog(string filePath)
        {
            if (File.Exists(filePath))
            {
                string logs = File.ReadAllText(filePath);
                Console.WriteLine("---- Log File Content ----");
                Console.WriteLine(logs);
            }
            else
            {
                Console.WriteLine("No log found for today.");
            }
        }
    }
}
