using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    internal interface ICQICommandData
    {
        string Command { get;}

        [DefaultValue("")]
        string Description { get; }
    }
}
