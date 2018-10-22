using FrodLib.Interfaces;
using FrodLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FrodLib.IoC
{
    partial class IoCContainerConfiguration : IIoCContainerConfiguration, IDisposable
    {
        private readonly DefaultIoCRegistry m_defaultRegistry;
        private ImmutableList<IIoCRegistry> m_customRegistryes = ImmutableList.Create<IIoCRegistry>();

        /// <summary>
        /// Specifies the rule to select a default constructor if more then one exist and none has been marked as primary
        /// </summary>
        public ConstructorSelectorRule DefaultConstructorSelectionRule { get; set; } = ConstructorSelectorRule.MostArgs;



        public IoCContainerConfiguration(DefaultIoCRegistry defaultRegistry)
        {
            m_defaultRegistry = defaultRegistry;
        }

        public void AddRegistry(IIoCRegistry registry)
        {
            ArgumentValidator.IsNotNull(registry, nameof(registry));
            m_customRegistryes = m_customRegistryes.Add(registry);
        }

        public bool RemoveRegistry(IIoCRegistry registry)
        {
            ArgumentValidator.IsNotNull(registry, nameof(registry));
            int prevLentgh = m_customRegistryes.Count;
            m_customRegistryes = m_customRegistryes.Remove(registry);
            return prevLentgh > m_customRegistryes.Count;
        }

        public void Scan(Action<IIoCScanner> scanSetup)
        {
            if(scanSetup != null)
            {
                IoCScanner scanner = new IoCScanner();
                scanSetup(scanner);
                scanner.AddScannedTypesToRegistry(this);
            }
        }

        #region REGISTER INSTANCE

        /// <summary>
        /// Maps an instance to a contract
        /// <para>If possible please use the generic version for type safety</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="instance"></param>
        public void RegisterInstance(Type contract, object instance)
        {
            m_defaultRegistry.RegisterInstance(contract, instance);
        }



        /// <summary>
        /// Maps an instance to a contract
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="instance"></param>
        public void RegisterInstance<TContract>(TContract instance)
            where TContract : class
        {
            Type contractType = typeof(TContract);
            RegisterInstance(contractType, instance);
        }

        #endregion REGISTER INSTANCE


        #region REGISTER TYPE

        public IIoCContainerMapResult Register<TContract, TImplementation>(params object[] args)
            where TContract : class
            where TImplementation : TContract
        {
            return Register(typeof(TContract), typeof(TImplementation), args);
        }

        public IIoCContainerMapResult Register(Type contract, Type implementation, params object[] args)
        {
            return m_defaultRegistry.Register(contract, implementation, args);
        }

        #endregion REGISTER TYPE

        #region Register Decorator

        internal void RegisterDecorator<TContract, TDecorator>()
           where TContract : class
           where TDecorator : TContract
        {
            RegisterDecorator(typeof(TContract), typeof(TDecorator));
        }

        internal void RegisterDecorator(Type contract, Type decorator)
        {
            m_defaultRegistry.RegisterDecorator(contract, decorator);
        }

        #endregion

        #region Unregister

        /// <summary>
        /// Removes an mapped contract implementation
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Unregister(Type type)
        {
            return m_defaultRegistry.Unregister(type);
        }

        /// <summary>
        /// Removes an mapped contract implementation
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <returns></returns>
        public bool Unregister<TIn>() where TIn : class
        {
            Type type = typeof(TIn);

            return Unregister(type);
        }

        #endregion Unregister


        internal bool RemoveSingleInstance(Type contractType)
        {
            ArgumentValidator.IsNotNull(contractType, nameof(contractType));
            return m_defaultRegistry.RemoveSingleInstance(contractType);
        }

        internal object GetInstance(Type contract, IIoCArgumentResolver argResolver, bool resolveByMethodInject, IDictionary<Type, object> resolvedObjects)
        {
            var registries = m_customRegistryes;
            foreach (IIoCRegistry registry in registries)
            {
                if (registry.HasImplementationForType(contract))
                {
                    object instance = registry.CreateInstance(contract);
                    if (instance != null)
                    {
                        return instance;
                    }
                }
            }
            return m_defaultRegistry.CreateInstance(contract, argResolver, resolveByMethodInject, resolvedObjects);
        }


        internal IEnumerable GetManyInstances(Type contract, IIoCArgumentResolver argResolver)
        {
            var registries = m_customRegistryes;
            foreach (IIoCRegistry registry in registries)
            {
                if (registry.HasImplementationForType(contract))
                {
                    if(registry is IIntenalIoCRegistry)
                    {
                        foreach (var instance in ((IIntenalIoCRegistry)registry).CreateManyInstance(contract))
                        {
                            yield return instance;
                        }
                    }
                    else
                    {
                        object instance = registry.CreateInstance(contract);
                        if (instance != null)
                        {
                            yield return instance;
                        }
                    }
                }
            }

            foreach(var instance in m_defaultRegistry.CreateManyInstance(contract, argResolver))
            {
                yield return instance;
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
                    m_customRegistryes = m_customRegistryes.Clear();
                    m_defaultRegistry.Dispose();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~IoCContainer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
