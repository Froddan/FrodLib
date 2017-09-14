using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrodLib.CQI;

namespace FrodLib.CQI.Commands
{
    [Metadata("Command", "ClearHistory")]
    [Metadata("Description", "Clears the command history")]
    class ClearHistoryCommand : ICQICommand
    {
        public void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args)
        {
            if (commandPrompt is ICommandManager)
            {
                var prompt = (ICommandManager)commandPrompt;
                prompt.ClearCommandHistory();
            }
        }
    }
}
