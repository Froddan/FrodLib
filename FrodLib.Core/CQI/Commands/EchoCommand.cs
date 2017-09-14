using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrodLib.CQI;

namespace FrodLib.CQI.Commands
{
    [Metadata("Command", "Echo")]
    [Metadata("Description", "Echoes the first input")]
    class EchoCommand : ICQICommand
    {
        public void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args)
        {
            if(args.Length == 0)
            {
                commandPrompt.WriteLine(string.Empty);
            }
            else
            {
                commandPrompt.WriteLine(args[0]);
            }
        }
    }
}
