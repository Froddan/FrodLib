using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Configurations
{
    /// <summary>
    /// Configuration elements taht is used by the factory config to map an interface to an implementation
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class FactoryConfigElement : ConfigurationElement
    {
        private const string interfaceNameTag = "interface";

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty(interfaceNameTag, IsRequired = true, IsKey = true)]
        public Type Interface
        {
            get
            {
                return (Type)this[interfaceNameTag];
            }
            set
            {
                this[interfaceNameTag] = value;
            }
        }

        private const string implementationNameTag = "implementation";

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty(implementationNameTag, IsRequired = true)]
        public Type Implementation
        {
            get
            {
                return (Type)this[implementationNameTag];
            }
            set
            {
                this[implementationNameTag] = value;
            }
        }


        private const string singletonNameTag = "singleton";

        [ConfigurationProperty(singletonNameTag, DefaultValue = false, IsRequired = false)]
        public bool Singleton
        {
            get
            {
                return (bool)this[singletonNameTag];
            }
            set
            {
                this[singletonNameTag] = value;
            }
        }

    }
}
