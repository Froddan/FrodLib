using System;
using System.Collections.Generic;
using System.Text;

namespace FrodLib.IoC
{
    public class IoCArgumentResolver : Dictionary<Type, object>, IIoCArgumentResolver
    {
        public bool Resolve(int argIndex, string argName, Type argType, out object argValue)
        {
            object instance;
            if (this.TryGetValue(argType, out instance))
            {
                argValue = instance;
                return true;
            }
            else
            {
                argValue = null;
                return false;
            }
        }
    }
}
