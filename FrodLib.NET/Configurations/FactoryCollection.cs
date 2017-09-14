using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Configurations
{
    [ExcludeFromCodeCoverage]
    public sealed class FactoryCollection : ConfigurationElementCollection 
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FactoryConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FactoryConfigElement)element).Interface;
        }
    }
}
