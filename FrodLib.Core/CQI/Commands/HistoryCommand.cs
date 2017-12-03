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
    class HistoryCommand : ICQIInternalCommand
    {
        public void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args)
        {
            throw new NotImplementedException();
        }

        void ICQIInternalCommand.ExecuteCommand(ICommandManager commandManager, ICommandManagerOutput commandOutput, string[] args)
        {
            commandManager.PrintCommandHistory();
        }

    }
}
