using ProjetV0._1.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetV0._1.Vue
{
    public class ConsoleView : IObserver
    {
        public void Update(BackupState state)
        {
            Console.WriteLine($"[Backup: {state.Name}] Progress: {state.Progress}% - State: {state.State}");
        }
    }
}
