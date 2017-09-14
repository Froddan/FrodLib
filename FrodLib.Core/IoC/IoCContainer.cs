using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using FrodLib.Interfaces;
using FrodLib.Utils;
using System.Collections.Immutable;

namespace FrodLib.IoC
{
    using System.Collections;
    using ObjPair = Tuple<string, object>;
    using ObjPairCollection = IList<Tuple<string, object>>;
    using ObjPairCollectionImpl = List<Tuple<string, object>>;

    /// <summary>
    /// Used when the caller wants to be able to send in a custom value for a constroctor argument during class initiation.
    /// <para>To use default value/instance lookup let this delegate return false</para>
    /// <para>For constroctor argument that should use a custom value this delegate should return true</para>
    /// </summary>
    /// <param name="argIndex"></param>
    /// <param name="argName"></param>
    /// <param name="argType"></param>
    /// <param name="argValue"></param>
    /// <returns>Return true if the resolved value should be use. Otherwise return false and the value will be looked up as normal</returns>
    public delegate bool ResolveConstructorArgumentDelegate(int argIndex, string argName, Type argType, out object argValue);



    public enum ConstructorSelectorRule
    {
        MostArgs, LeastArgs, RequirePrimary
    }

    public class IoCContainer : IIoCContainer, IDisposable
    {

        private static IIoCContainer s_customDefaultInstance;
        internal readonly IoCContainerConfiguration m_ioCConfiguration;
        private ImmutableDictionary<Type, ObjPairCollection> m_objectKeyMap;


        public IoCContainer()
        {
            var defaultRegistry = new DefaultIoCRegistry(this);
            m_ioCConfiguration = new IoCContainerConfiguration(defaultRegistry);
            m_objectKeyMap = ImmutableDictionary<Type, ObjPairCollection>.Empty;

        }

        public static IIoCContainer Default
        {
            get
            {
                if (s_customDefaultInstance != null) return s_customDefaultInstance;
                return Singleton<IoCContainer>.Instance;
            }
            set
            {
                s_customDefaultInstance = value;
            }
        }

        public void Configure(Action<IIoCContainerConfiguration> configureAction)
        {
            if (configureAction != null) configureAction(m_ioCConfiguration);
        }

        #region GET

        /// <summary>
        /// Returns an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public object GetInstance(Type contract)
        {
            return GetInstance(contract, null, null);
        }

        public object GetInstance(Type contract, string key)
        {
            return GetInstance(contract, key, null);
        }

        public object GetInstance(Type contract, ResolveConstructorArgumentDelegate argResolver)
        {
            return GetInstance(contract, null, argResolver);
        }

        internal object GetInstance(Type contract, ResolveConstructorArgumentDelegate argResolver, bool resolveByMethodInject, IDictionary<Type, object> resolvedObjects)
        {
            return _GetInstance(contract, null, argResolver, resolveByMethodInject, resolvedObjects);
        }

        public object GetInstance(Type contract, string key, ResolveConstructorArgumentDelegate argResolver)
        {
            return _GetInstance(contract, key, argResolver, true, new Dictionary<Type, object>());
        }

        private object _GetInstance(Type contract, string key, ResolveConstructorArgumentDelegate argResolver, bool resolveByMethodInject, IDictionary<Type, object> resolvedObjects)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));

            try
            {
                object instance;
                if (string.IsNullOrEmpty(key))
                {
                    instance = m_ioCConfiguration.GetInstance(contract, argResolver, resolveByMethodInject, resolvedObjects);
                    if (instance == null)
                    {
                        throw new IoCTypeNotMappedException(contract, string.Format(StringResources.TypeWasNotMapped, contract));
                    }
                    else
                    {
                        return instance;
                    }

                }
                else
                {
                    ObjPairCollection objPairs;
                    if (m_objectKeyMap.TryGetValue(contract, out objPairs))
                    {
                        instance = objPairs.Where(p => p.Item1.Equals(key)).Select(p => p.Item2).FirstOrDefault();
                        if (instance != null)
                        {
                            return instance;
                        }
                    }

                    instance = m_ioCConfiguration.GetInstance(contract, argResolver, resolveByMethodInject, resolvedObjects);
                    if (instance != null)
                    {
                        if (objPairs != null)
                        {
                            objPairs.Add(new ObjPair(key, instance));
                        }
                        else
                        {
                            m_objectKeyMap = m_objectKeyMap.Add(contract, new ObjPairCollectionImpl() { new ObjPair(key, instance) });
                        }
                        return instance;
                    }
                    else
                    {
                        throw new IoCTypeNotMappedException(contract, string.Format(StringResources.TypeWasNotMapped, contract));
                    }
                }
            }
            catch (IoCTypeNotMappedException)
            {
                //Just throw it again since we probably created it here
                throw;
            }
            catch (IoCCircularReferenceException)
            {
                //Just throw it again since we probably created it here
                throw;
            }
            catch (Exception e)
            {
                throw new IoCException(contract, "An error occured when trying to get instance for contract: " + contract, e);
            }
        }


        /// <summary>
        /// Returns an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>() where T : class
        {
            Type type = typeof(T);
            return GetInstance(type, null, null) as T;
        }

        public T GetInstance<T>(ResolveConstructorArgumentDelegate argResolver) where T : class
        {
            Type type = typeof(T);
            return GetInstance(type, null, argResolver) as T;
        }

        public T GetInstance<T>(string key) where T : class
        {
            Type type = typeof(T);
            return GetInstance(type, key, null) as T;
        }

        public T GetInstance<T>(string key, ResolveConstructorArgumentDelegate argResolver) where T : class
        {
            Type type = typeof(T);
            return GetInstance(type, key, argResolver) as T;
        }


        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <returns>True if instance was found</returns>
        public bool TryGetInstance(Type contract, out object instance)
        {
            return TryGetInstance(contract, null, null, out instance);
        }

        public bool TryGetInstance(Type contract, string key, out object instance)
        {
            return TryGetInstance(contract, key, null, out instance);
        }

        public bool TryGetInstance(Type contract, ResolveConstructorArgumentDelegate argResolver, out object instance)
        {
            return TryGetInstance(contract, null, argResolver, out instance);
        }

        public bool TryGetInstance(Type contract, string key, ResolveConstructorArgumentDelegate argResolver, out object instance)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));

            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    ObjPairCollection objPairs;
                    if (m_objectKeyMap.TryGetValue(contract, out objPairs))
                    {
                        instance = objPairs.Where(p => p.Item1.Equals(key)).Select(p => p.Item2).FirstOrDefault();
                        if (instance != null)
                        {
                            return true;
                        }
                    }

                    instance = m_ioCConfiguration.GetInstance(contract, argResolver, true, null);
                    if (instance != null)
                    {
                        if (objPairs != null)
                        {
                            objPairs.Add(new ObjPair(key, instance));
                        }
                        else
                        {
                            m_objectKeyMap = m_objectKeyMap.Add(contract, new ObjPairCollectionImpl() { new ObjPair(key, instance) });
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    instance = m_ioCConfiguration.GetInstance(contract, argResolver, true, null);
                    return instance != null;
                }
            }
            catch
            {
                instance = null;
                return false;
            }
        }


        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>True if instance was found</returns>
        public bool TryGetInstance<T>(out T instance) where T : class
        {
            return TryGetInstance(null, null, out instance);
        }

        public bool TryGetInstance<T>(ResolveConstructorArgumentDelegate argResolver, out T instance) where T : class
        {
            return TryGetInstance(null, argResolver, out instance);
        }

        public bool TryGetInstance<T>(string key, out T instance) where T : class
        {
            return TryGetInstance(key, null, out instance);
        }

        public bool TryGetInstance<T>(string key, ResolveConstructorArgumentDelegate argResolver, out T instance) where T : class
        {
            Type contract = typeof(T);

            object tmpInstance;
            if (TryGetInstance(contract, key, argResolver, out tmpInstance))
            {
                instance = tmpInstance as T;
                return instance != null;
            }
            else
            {
                instance = null;
                return false;
            }
        }

        /// <summary>
        /// Returns a list of all mapped (by key) instances for contract
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public IReadOnlyList<object> GetAllInstances(Type contract)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));

            ObjPairCollection mappedItems;
            if (m_objectKeyMap.TryGetValue(contract, out mappedItems))
            {
                return mappedItems.Select(o => o.Item2).ToArray();
            }
            else
            {
                return new object[0];
            }
        }

        /// <summary>
        /// Returns a list of all mapped (by key) instances for contract
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IReadOnlyList<TContract> GetAllInstances<TContract>()
        {
            Type contract = typeof(TContract);
            return GetAllInstances(contract).OfType<TContract>().ToArray();
        }

        #endregion GET

        #region Remove Mapped

        public bool RemoveCurrentSingleInstance(Type contractType)
        {
            return m_ioCConfiguration.RemoveSingleInstance(contractType);
        }

        public bool RemoveCurrentSingleInstance<TContract>() where TContract : class
        {
            Type contract = typeof(TContract);
            return RemoveCurrentSingleInstance(contract);
        }

        public void ClearAllMappedInstances()
        {
            m_objectKeyMap = m_objectKeyMap.Clear();
        }

        public void ClearAllMappedInstancesForContract(Type contract)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));

            ObjPairCollection mappedItems;
            if (m_objectKeyMap.TryGetValue(contract, out mappedItems))
            {
                mappedItems.Clear();
            }
        }

        public void ClearAllMappedInstancesForContract<TContract>()
        {
            var contract = typeof(TContract);
            ClearAllMappedInstancesForContract(contract);
        }

        #endregion;

        private IEnumerable GetManyInstances(Type type, ResolveConstructorArgumentDelegate argResolver)
        {
            var castMethod = typeof(Enumerable).GetTypeInfo().GetDeclaredMethod("Cast");
            var caxtGenericMethod = castMethod.MakeGenericMethod(new Type[] { type });

            var instances = m_ioCConfiguration.GetManyInstances(type, argResolver);
            return caxtGenericMethod.Invoke(null, new object[] { instances }) as IEnumerable;

        }

        internal IEnumerable<T> GetManyInstances<T>()
        {
            var type = typeof(T);
            return GetManyInstances(type, null).OfType<T>();
        }

        public void Fill<T>(T instance) where T : class
        {
            Fill(instance, null);
        }

        public void Fill<T>(T instance, ResolveConstructorArgumentDelegate argResolver) where T : class
        {
            ArgumentValidator.IsNotNull(instance, nameof(instance));

            var type = typeof(T);
            var fields = type.GetRuntimeFields();
            foreach (var field in fields.Where(f => !f.IsStatic))
            {
                if (field.IsDefined(typeof(IoCResolveManyAttribute)))
                {
                    var fieldType = field.FieldType.GetTypeInfo();
                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var genericTypeArgs = fieldType.GenericTypeArguments;
                        var genericTypeArg = genericTypeArgs.First();

                        var collection = GetManyInstances(genericTypeArg, argResolver);
                        field.SetValue(instance, collection);

                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Field '{0}' has to be of type 'IEnumerable<>'", field.Name));
                    }
                }
                else if (field.IsDefined(typeof(IoCResolveAttribute)))
                {
                    var fieldType = field.FieldType.GetTypeInfo();
                    if (fieldType.IsClass || fieldType.IsInterface)
                    {
                        object resolvedInstance = this.GetInstance(field.FieldType, argResolver);
                        field.SetValue(instance, resolvedInstance);
                    }
                }
            }

            var properties = type.GetRuntimeProperties();
            foreach (var property in properties.Where(p => p.CanWrite && !p.SetMethod.IsStatic))
            {
                if (property.IsDefined(typeof(IoCResolveManyAttribute)))
                {
                    var propertyType = property.PropertyType.GetTypeInfo();
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var genericTypeArgs = propertyType.GenericTypeArguments;
                        var genericTypeArg = genericTypeArgs.First();

                        var collection = GetManyInstances(genericTypeArg, argResolver);
                        property.SetValue(instance, collection);

                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Property '{0}' has to be of type 'IEnumerable<>'", property.Name));
                    }
                }
                else if (property.IsDefined(typeof(IoCResolveAttribute)))
                {
                    var propertyType = property.PropertyType.GetTypeInfo();
                    if (propertyType.IsClass || propertyType.IsInterface)
                    {
                        object resolvedInstance = this.GetInstance(property.PropertyType, argResolver);
                        property.SetValue(instance, resolvedInstance);
                    }
                }
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
                    m_ioCConfiguration.Dispose();
                    m_objectKeyMap = m_objectKeyMap.Clear();
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