using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace FrodLib.Attributes
{
    /// <summary>
    /// Defines the factory as the default factory for an interface. If more then one factory is marked as default for an interface an invalid operation Exception will be thrown
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DefaultFactoryAttribute : Attribute
    {
        private static Dictionary<Type, Type> defaultFactories = new Dictionary<Type, Type>();

        public Type FactoryInterfaceType { get; private set; }

        public DefaultFactoryAttribute(Type factoryInterface, Type factoryImplementation)
        {
            if (defaultFactories.ContainsKey(factoryInterface) && defaultFactories[factoryInterface].FullName != factoryImplementation.FullName)
            {
                throw new InvalidOperationException(string.Format(StringResources.MoreThenOneFactorySpecified, factoryInterface));
            }
            else if (!defaultFactories.ContainsKey(factoryInterface))
            {
                defaultFactories.Add(factoryInterface, factoryImplementation);
            }
            FactoryInterfaceType = factoryInterface;
        }
    }
}
