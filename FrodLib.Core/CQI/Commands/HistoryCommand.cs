using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrodLib.CQI;

namespace FrodLib.CQI.Commands
{
    [Metadata("Command", "History")]
    [Metadata("Description", "Shows the command history")]
    class HistoryCommand : ICQICommand
    {
        public void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args)
        {
            if (commandPrompt is ICommandManager)
            {
                var prompt = (ICommandManager)commandPrompt;
                prompt.PrintCommandHistory();
            }
        }
    }
}
