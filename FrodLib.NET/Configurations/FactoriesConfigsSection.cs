using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Configurations
{
    [ExcludeFromCodeCoverage]
    public sealed class FactoriesConfigsSection : ConfigurationSection
    {
        private const string FactoryElementTag = "";

        // Set or get the ConsoleElement. 
        [ConfigurationProperty(FactoryElementTag, IsRequired = true, IsDefaultCollection = true)]
        public FactoryCollection Factories
        {
            get
            {
                return (FactoryCollection)this[FactoryElementTag];
            }
            set
            {
                this[FactoryElementTag] = value;
            }
        }
    }
}
