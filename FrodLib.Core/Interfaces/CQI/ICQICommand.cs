using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    internal interface ICQICommand
    {
        void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args);
    }
}
