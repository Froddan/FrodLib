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
    class HelpCommand : ICQIInternalCommand
    {
        public void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args)
        {
            throw new NotImplementedException();
        }

        void ICQIInternalCommand.ExecuteCommand(ICommandManager commandManager, ICommandManagerOutput commandOutput, string[] args)
        {
            commandManager.PrintAvailableCommands();
        }
    }
}
