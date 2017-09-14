using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    internal interface ICommandManager
    {
        void PrintCommandHistory();

        void PrintAvailableCommands();
        void ClearCommandHistory();
    }
}
