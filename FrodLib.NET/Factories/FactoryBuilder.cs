using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FrodLib.Configurations;
using System.Reflection;
using FrodLib.Attributes;
using System.Diagnostics.CodeAnalysis;
using FrodLib.Resources;
using FrodLib.Interfaces;
using FrodLib.Utils;
using FrodLib.IoC;

namespace FrodLib.Factories
{
    /// <summary>
    /// Creates a factory that is configured in the applications config file.
    /// <para>If no factory is configured in the config file for a certain interface it will try to</para>
    /// <para>build one from a factory that is marked with the DefaultFactoryAttribute for that type.</para>
    /// <para>Factories can be singleton if they are marked with the FrodLib.Attributes.Singleton attribute</para>
    /// <para></para>
    /// <para>Created by: Fredrik Schmidt</para>
    /// <para>Date: 2012-07-24</para>
    /// <example>
    /// 
    ///     <!--Example config file-->
    ///     <configuration>
    ///         <configSections>
    ///             <section name="factoriesConfig" type="FrodLib.Configurations.FactoriesConfigsSection, FrodLib"/>
    ///         </configSections>
    ///         <factoriesConfig>
    ///             <add interface="MyNameSpace.IFactory, MyAsm" implementation="MyNameSpace.Factory, MyAsm" />
    ///         </factoriesConfig>
    ///     </configuration>
    ///     
    /// </example>
    /// <para></para>
    /// <exception cref="InvalidOperationException">
    /// Is thrown when no implementation is configured for a interface
    /// </exception>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FactoryBuilder : IIoCRegistry
    {
        //cache factories so we dont have to find them again when it is to be constructed
        private Dictionary<Type, Type> _cachedFactories = new Dictionary<Type, Type>();

        private static FactoryBuilder s_customDefaultInstance = null;

        public static FactoryBuilder Default
        {
            get
            {

                if (s_customDefaultInstance != null) return s_customDefaultInstance;
                return Singleton<FactoryBuilder>.Instance;
            }
            set
            {
                s_customDefaultInstance = value;
            }
        }

        public Type GetImplementationType<T>() where T : class
        {
            Type factoryInterfaceType = typeof(T);
            return GetImplementationType(factoryInterfaceType);
        }

        public Type GetImplementationType(Type factoryInterfaceType)
        {
            Type factoryType = null;
            lock (_cachedFactories)
            {
                if (_cachedFactories.TryGetValue(factoryInterfaceType, out factoryType) == false)
                { 
                    factoryType = GetFactoryTypeFromConfig(factoryInterfaceType);
                    if (factoryType == null)
                    {
                        factoryType = GetDefaultFactoryTypeByAttribute(factoryInterfaceType);
                    }
                    if (factoryType != null)
                    {
                        _cachedFactories.Add(factoryInterfaceType, factoryType);
                    }
                }
            }
            return factoryType;
        }

        public object CreateInstance(Type factoryInterfaceType)
        {
            Type factoryType = GetImplementationType(factoryInterfaceType);
            if (factoryType != null)
            {

                SingletonAttribute singletonAttr = factoryType.GetCustomAttributes(typeof(SingletonAttribute), true).OfType<SingletonAttribute>().FirstOrDefault();
                if (singletonAttr != null)
                {
                    if (singletonAttr.AccessType == SingletonInstanceAccessType.Property)
                    {
                        var pInfo = factoryType.GetProperty(singletonAttr.InstanceAccsessorName, BindingFlags.Static | BindingFlags.Public);
                        return pInfo.GetValue(null, null);
                    }
                    else if (singletonAttr.AccessType == SingletonInstanceAccessType.Method)
                    {
                        var mInfo = factoryType.GetMethod(singletonAttr.InstanceAccsessorName, BindingFlags.Static | BindingFlags.Public);
                        return mInfo.Invoke(null, null);
                    }
                    else
                    {
                        return Activator.CreateInstance(factoryType);
                    }
                }
                else
                {
                    return Activator.CreateInstance(factoryType);
                }
            }
            else
            {
                throw new FactoryNotFoundException(string.Format(StringResources.NoImplementationConfigured, factoryInterfaceType));
            }
        }

        public T CreateInstance<T>() where T : class
        {
            Type factoryInterfaceType = typeof(T);
            return (T)CreateInstance(factoryInterfaceType);

        }

        /// <summary>
        /// Clears all cached factories
        /// </summary>
        public void ClearCachedFactories()
        {
            _cachedFactories.Clear();
        }

        private Type GetFactoryTypeFromConfig(Type factoryInterfaceType)
        {
            Type factoryType = null;
            var factoriesConfig = GetConfiguration();
            if (factoriesConfig != null)
            {
                foreach (FactoryConfigElement factoryConfig in factoriesConfig.Factories)
                {
                    if (factoryConfig.Interface.IsAssignableFrom(factoryInterfaceType))
                    {
                        factoryType = factoryConfig.Implementation;
                        break;
                    }
                }
            }
            return factoryType;
        }

        private Type GetDefaultFactoryTypeByAttribute(Type factoryInterfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type factoryType = null;
            foreach (Assembly asm in assemblies)
            {
                var types = asm.GetTypes();
                var factoryBaseTypes = types.Where(t => factoryInterfaceType.IsAssignableFrom(t));
                foreach (var asmType in factoryBaseTypes)
                {
                    DefaultFactoryAttribute defaultFactoryAttr = asmType.GetCustomAttributes(typeof(DefaultFactoryAttribute), true).OfType<DefaultFactoryAttribute>().FirstOrDefault();
                    if (defaultFactoryAttr != null)
                    {
                        factoryType = asmType;
                        break;
                    }
                }
                if (factoryType != null)
                {
                    break;
                }
            }
            return factoryType;
        }

        private FactoriesConfigsSection GetConfiguration()
        {
            const string sectionName = "factoriesConfig";

#if DEBUG
            string applicationName = Environment.GetCommandLineArgs()[0];
#else
            string applicationName = Environment.GetCommandLineArgs()[0] + ".exe";
#endif

            string exePath = System.IO.Path.Combine(
                Environment.CurrentDirectory, applicationName);

            // Get the configuration file. The file name has
            // this format appname.exe.config.
            System.Configuration.Configuration config =
              ConfigurationManager.OpenExeConfiguration(exePath);

            return config.GetSection(sectionName) as FactoriesConfigsSection;
        }


        bool IIoCRegistry.HasImplementationForType(Type type)
        {
            try
            {
                return GetImplementationType(type) != null;
            }
            catch
            {
                return false;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FactoryBuilder() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
