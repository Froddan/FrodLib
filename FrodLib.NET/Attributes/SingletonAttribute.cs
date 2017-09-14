using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Attributes
{
    /// <summary>
    /// Defines if a class uses Singleton Design pattern and defines what it's instance Property is called so it can be used by reflection to get a singleton reference
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public sealed class SingletonAttribute : Attribute
    {
        public string InstanceAccsessorName { get; private set; }
        
        public SingletonInstanceAccessType AccessType { get; private set; }

        /// <summary>
        /// Defaults values to a property called Instance
        /// </summary>
        public SingletonAttribute()
        {
            InstanceAccsessorName = "Instance";
            AccessType = SingletonInstanceAccessType.Property;
        }

        /// <summary>
        /// Defaults values to a property with user chosen name
        /// </summary>
        /// <param name="instanceAccsessorName"></param>
        public SingletonAttribute(string instanceAccsessorName)
        {
            InstanceAccsessorName = instanceAccsessorName;
            AccessType = SingletonInstanceAccessType.Property;
        }

        /// <summary>
        /// If the instance is accessed thru a method instead of a property
        /// </summary>
        /// <param name="instanceAccsessorName"></param>
        /// <param name="instanceAccessorType"></param>
        public SingletonAttribute(string instanceAccsessorName, SingletonInstanceAccessType instanceAccessorType)
        {
            InstanceAccsessorName = instanceAccsessorName;
            AccessType = instanceAccessorType;
        }
    }

    public enum SingletonInstanceAccessType
    {
        Property,
        Method
    }
}
