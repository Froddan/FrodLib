using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    public interface ICQICommand
    {
        void ExecuteCommand(ICommandManagerOutput commandOutput, string[] args);
    }

    internal interface ICQIInternalCommand : ICQICommand
    {
        void ExecuteCommand(ICommandManager commandManager, ICommandManagerOutput commandOutput, string[] args);
    }
}
