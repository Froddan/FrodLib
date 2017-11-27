using System;
using System.Collections.Generic;
using System.Text;

namespace FrodLib.IoC
{
    public interface IIoCArgumentResolver
    {
        bool Resolve(int argIndex, string argName, Type argType, out object argValue);
    }
}
