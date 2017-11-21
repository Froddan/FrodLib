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
        void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args);
    }
}
