using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrodLib.CQI;

namespace FrodLib.CQI.Commands
{
    [Metadata("Command", "Help")]
    [Metadata("Description", "Lists all available commands")]
    class HelpCommand : ICQICommand
    {
        public void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args)
        {
            if (commandPrompt is ICommandManager)
            {
                var prompt = (ICommandManager)commandPrompt;
                prompt.PrintAvailableCommands();
                
            }
        }
    }
}
