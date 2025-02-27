using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ModelLib
{
    /// This class is responsible for managing translation settings in the application.
    internal class TranslateModel
    {
        /// Lock object for thread safety in a multi-threaded environment.
        public static readonly object _lock = new object();
    }
}
