using FrodLib.Interfaces;
using FrodLib.Resources;
using FrodLib.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    internal sealed partial class DefaultIoCRegistry : IDefaultIoCContainer, IDisposable
    {
        private readonly Dictionary<Type, IImmutableList<IFrodIoCContainerItem>> _map;
        private readonly IoCContainer m_container;

        public DefaultIoCRegistry(IoCContainer container)
        {
            m_container = container;
            _map = new Dictionary<Type, IImmutableList<IFrodIoCContainerItem>>();
        }

        public object CreateInstance(Type contract)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));
            return _GetInstance(contract, null, true, null, true, new Dictionary<Type, object>(), true);
        }


        public TInterface CreateInstance<TInterface>() where TInterface : class
        {
            return _GetInstance(typeof(TInterface), null, true, null, true, new Dictionary<Type, object>(), true) as TInterface;
        }

        public object CreateInstance(Type contract, ResolveConstructorArgumentDelegate argResolver, bool resolveMyMethodInject, IDictionary<Type, object> resolvedObjects)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));
            resolvedObjects = resolvedObjects ?? new Dictionary<Type, object>();
            object instance = _GetInstance(contract, null, true, argResolver, resolveMyMethodInject, resolvedObjects, true);
            if (resolveMyMethodInject)
            {
                bool @continue = false;
                HashSet<object> methodInjectedForObject = new HashSet<object>();
                do
                {
                    @continue = false;
                    foreach (var val in resolvedObjects.Values.ToArray())
                    {
                        if (methodInjectedForObject.Contains(val))
                        {
                            continue;
                        }
                        else
                        {
                            ResolveObjectsByMethodInjection(val, argResolver, resolvedObjects);
                            methodInjectedForObject.Add(val);
                            @continue = true;
                        }
                    }
                } while (@continue);
            }
            return instance;
        }


        public bool HasImplementationForType(Type type)
        {
            if (type == null) return false;

            IImmutableList<IFrodIoCContainerItem> implementations;
            if (_map.TryGetValue(type, out implementations)) return implementations.Count > 0;
            else return false;
        }


        public IIoCContainerMapResult Register(Type contract, Type implementation, object[] args)
        {

            ValidateContractImplementationMatch(contract, implementation);

            IFrodIoCContainerItem implItem = new IoCContainerItem(contract, this, implementation, args);
            implItem = new IoCCircularReferenceCheckerContainerItem(implItem);

            IList<Type> typesMappedToContract = AddContainerItemToMap(contract, implItem);
            return new IoCContainerMapResult(this, contract, implItem, typesMappedToContract);
        }

        public IIoCContainerMapResult Register(Type contract, Delegate implFactory, object[] args)
        {

            ValidateContractFactory(contract, implFactory);

            IFrodIoCContainerItem implItem;

            if (IsAsyncMethod(implFactory.GetMethodInfo()))
            {
                implItem = new IoCAsyncFactoryContainerItem(contract, this, implFactory, args);
                implItem = new IoCCircularReferenceCheckerContainerItem(implItem);
            }
            else
            {
                implItem = new IoCFactoryContainerItem(contract, this, implFactory, args);
                implItem = new IoCCircularReferenceCheckerContainerItem(implItem);
            }
            //implItem = new IoCFactoryContainerItem(implFactory, args);

            IList<Type> typesMappedToContract = AddContainerItemToMap(contract, implItem);

            return new IoCContainerMapResult(this, contract, implItem, typesMappedToContract);
        }

        internal IIoCContainerMapResult RegisterDecorator(Type decoratorContract, Type decoratorImplementation, object[] args)
        {
            throw new NotImplementedException();

            ValidateContractImplementationMatch(decoratorContract, decoratorImplementation);
        }

        private IList<Type> AddContainerItemToMap(Type contract, IFrodIoCContainerItem implItem)
        {
            IList<Type> typesMappedToContract = null;

            IImmutableList<IFrodIoCContainerItem> implementations;
            if (_map.TryGetValue(contract, out implementations))
            {
                if (implementations.Count > 0 && implementations[0] is IoCRegistredInstanceContainerItem)
                {
                    implementations = implementations.Insert(1, implItem);
                }
                else
                {
                    implementations = implementations.Insert(0, implItem);
                }
                typesMappedToContract = implementations.Select(ci => ci.ImplementationType).ToArray();
                _map[contract] = implementations;
            }
            else
            {
                _map.Add(contract, ImmutableList.Create(implItem));
                typesMappedToContract = new Type[] { implItem.ImplementationType };
            }

            return typesMappedToContract;
        }

        public void RegisterInstance(Type contract, object instance)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));
            ArgumentValidator.IsNotNull(instance, nameof(instance));
            ValidateContractImplementationMatch(contract, instance.GetType());

            var registredInstanceItem = new IoCRegistredInstanceContainerItem(contract, instance);

            IImmutableList<IFrodIoCContainerItem> implementations;
            if (_map.TryGetValue(contract, out implementations))
            {
                if (implementations.Count > 0 && implementations[0] is IoCRegistredInstanceContainerItem)
                {
                    //Replace old instance
                    implementations = implementations.SetItem(0, registredInstanceItem);
                }
                else
                {
                    implementations = implementations.Insert(0, registredInstanceItem);
                }

                _map[contract] = implementations;
            }
            else
            {
                _map.Add(contract, ImmutableList.Create<IFrodIoCContainerItem>(registredInstanceItem));
            }
        }

        internal IEnumerable<object> CreateManyInstance(Type contract, ResolveConstructorArgumentDelegate argResolver)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));
            var resolvedObjects = new Dictionary<Type, object>();
            var instances = _GetManyInstances(contract, argResolver, resolvedObjects);
            //bool @continue = false;
            //HashSet<object> methodInjectedForObject = new HashSet<object>();
            //do
            //{
            //    @continue = false;
            //    foreach (var val in resolvedObjects.Values.ToArray())
            //    {
            //        if (methodInjectedForObject.Contains(val))
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            ResolveObjectsByMethodInjection(val, argResolver, resolvedObjects);
            //            methodInjectedForObject.Add(val);
            //            @continue = true;
            //        }
            //    }
            //} while (@continue);

            return instances;
        }

        public bool RemoveSingleInstance(Type contract)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));

            IImmutableList<IFrodIoCContainerItem> implementations;
            if (_map.TryGetValue(contract, out implementations))
            {
                if (implementations.Count > 0)
                {
                    IoCSingleInstanceContainerItem singleInstanceItem = null;
                    var item = implementations[0];
                    if (item is IoCRegistredInstanceContainerItem)
                    {
                        //Remove instance
                        _map[contract] = implementations.RemoveAt(0);
                        return true;
                    }
                    else if ((singleInstanceItem = item as IoCSingleInstanceContainerItem) != null)
                    {
                        singleInstanceItem.ClearCurrentInstance();
                        return true;
                    }
                }

            }

            return false;
        }

        public bool Unregister(Type type)
        {
            ArgumentValidator.IsNotNull(type, nameof(type));

            bool removed = false;

            if (_map.ContainsKey(type))
            {
                removed |= _map.Remove(type);
            }

            return removed;
        }

        internal object _GetInstance(Type contract, Type initialContract, bool isFirstCallForContract, ResolveConstructorArgumentDelegate argResolver, bool resolveInjectByMethod, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));

            var contractTypeInfo = contract.GetTypeInfo();
            IImmutableList<IFrodIoCContainerItem> implementations;

            if (initialContract == null) initialContract = contract;

            object obj = null;
            if (useResolvedObjects && resolvedObjects.TryGetValue(initialContract, out obj))
            {
            }
            else if (isFirstCallForContract && _map.TryGetValue(contract, out implementations) && implementations.Any())
            {
                obj = implementations[0].CreateInstance(contract, argResolver, resolvedObjects, true);
            }
            else if (contractTypeInfo.IsInterface || contractTypeInfo.IsAbstract)
            {
                return null;
            }
            else
            {
                ConstructorInfo selectedConstructor = GetDefaultConstructorForType(contract);

                if (selectedConstructor == null)
                {
                    if (m_container.m_ioCConfiguration.DefaultConstructorSelectionRule == ConstructorSelectorRule.RequirePrimary)
                    {
                        throw new IoCConstructorMissingException(contract, string.Format(StringResources.NoRequiredPrimaryConstructorCouldBeFoundForTypeFormat, contract.Name));
                    }
                    else
                    {
                        throw new IoCConstructorMissingException(contract, StringResources.NoDefaultConstructorCouldBeFound);
                    }
                }

                var constParams = selectedConstructor.GetParameters();
                var args = GetArguments(contract, constParams, argResolver, resolveInjectByMethod, resolvedObjects);
                obj = selectedConstructor.Invoke(args);

                resolvedObjects[initialContract] = obj;

            }

            return obj;
        }

        private class GenericItemCreator<T>
        {
            private IFrodIoCContainerItem containerItem;
            private Type contract;
            private ResolveConstructorArgumentDelegate argResolver;
            private IDictionary<Type, object> resolvedObjects;


            public GenericItemCreator(IFrodIoCContainerItem containerItem, Type contract, ResolveConstructorArgumentDelegate argResolver, IDictionary<Type, object> resolvedObjects)
            {
                this.containerItem = containerItem;
                this.contract = contract;
                this.argResolver = argResolver;
                this.resolvedObjects = resolvedObjects;
            }

            public T CreateInstance()
            {
                var obj = containerItem.CreateInstance(contract, argResolver, resolvedObjects, false);
                return (T)obj;
            }
        }

        internal IEnumerable<object> _GetManyInstances(Type contract, ResolveConstructorArgumentDelegate argResolver, IDictionary<Type, object> resolvedObjects)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));

            var contractTypeInfo = contract.GetTypeInfo();
            IImmutableList<IFrodIoCContainerItem> implementations;



            if (contractTypeInfo.IsGenericType && contract.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                var genericArgumentContract = contractTypeInfo.GenericTypeArguments.First();
                if (_map.TryGetValue(genericArgumentContract, out implementations) && implementations.Any())
                {
                    foreach (var implementation in implementations)
                    {
                        var factory = CreatelazyFactoryDelegate(genericArgumentContract, implementation, argResolver, resolvedObjects);
                        yield return Activator.CreateInstance(contract, factory);
                    }
                }
            }
            else
            {
                if (_map.TryGetValue(contract, out implementations) && implementations.Any())
                {
                    foreach (var implementation in implementations)
                    {
                        var obj = implementation.CreateInstance(contract, argResolver, resolvedObjects, false);
                        // ResolveObjectsByMethodInjection(obj, argResolver);
                        yield return obj;
                    }
                }
            }
        }

        private Delegate CreatelazyFactoryDelegate(Type contract, IFrodIoCContainerItem implementation, ResolveConstructorArgumentDelegate argResolver, IDictionary<Type, object> resolvedObjects)
        {
            var creatorType = typeof(GenericItemCreator<>);

            Delegate lambda;
            var typeGenericCreator = creatorType.MakeGenericType(contract);
            var typeGenericCreatorMethod = typeGenericCreator.GetRuntimeMethod(nameof(CreateInstance), new Type[0]);

            var creatorInstance = Activator.CreateInstance(typeGenericCreator, implementation, contract, argResolver, resolvedObjects);
            var instance = Expression.Constant(creatorInstance);
            var call = Expression.Call(instance, typeGenericCreatorMethod);
            lambda = Expression.Lambda(Expression.Convert(call, contract)).Compile();
            return lambda;
        }

        private void ResolveObjectsByMethodInjection(object obj, ResolveConstructorArgumentDelegate argResolver, IDictionary<Type, object> resolvedObjects)
        {
            var objTypeInfo = obj.GetType().GetTypeInfo();
            var methods = objTypeInfo.DeclaredMethods;
            foreach (var method in methods)
            {
                var iocInjectAttr = method.GetCustomAttribute<IoCInjectAttribute>();
                if (iocInjectAttr != null)
                {
                    var methodParamTypes = method.GetParameters();
                    var args = GetArguments(null, methodParamTypes, null, false, resolvedObjects);
                    method.Invoke(obj, args);
                }
            }
        }

        private object[] GetArguments(Type contract, ParameterInfo[] parameters, ResolveConstructorArgumentDelegate argResolver, bool resolveInjectByMethod, IDictionary<Type, object> resolvedObjects)
        {
            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramsType = parameters[i].ParameterType;
                var paramsTypeInfo = paramsType.GetTypeInfo();

                object argValue = null;
                if (paramsType.Equals(typeof(IIoCContainer)))
                {
                    args[i] = m_container;
                }
                else if (resolvedObjects.TryGetValue(paramsType, out argValue))
                {
                    args[i] = argValue;
                }
                else if (argResolver != null && argResolver(i, parameters[i].Name, paramsType, out argValue))
                {
                    args[i] = argValue;
                }
                else if (paramsType == typeof(string))
                {
                    args[i] = default(string);
                }
                else if ((paramsTypeInfo.IsClass || paramsTypeInfo.IsInterface || paramsTypeInfo.IsAbstract) && !paramsType.Equals(contract))
                {
                    args[i] = m_container.GetInstance(paramsType, argResolver, resolveInjectByMethod, resolvedObjects);
                }
                else if (paramsTypeInfo.IsValueType)
                {
                    args[i] = Activator.CreateInstance(paramsType);
                }
                else
                {
                    args[i] = null;
                }
            }
            return args;
        }

        private ConstructorInfo GetDefaultConstructorForType(Type contract)
        {
            var contractTypeInfo = contract.GetTypeInfo();
            ConstructorInfo selectedConstructor = null;
            var constructors = contractTypeInfo.DeclaredConstructors.ToArray();

            var selectionRule = m_container.m_ioCConfiguration.DefaultConstructorSelectionRule;
            int selectedConstructorArgs;
            if (selectionRule == ConstructorSelectorRule.LeastArgs)
            {
                selectedConstructorArgs = int.MaxValue;
            }
            else
            {
                selectedConstructorArgs = -1;
            }

            if (constructors.Length <= 1)
            {
                return constructors.FirstOrDefault();
            }

            foreach (var constructor in constructors)
            {
                if (constructor.GetCustomAttributes(false).Any(a => a is PrimaryConstructorAttribute))
                {
                    selectedConstructor = constructor;
                    selectedConstructorArgs = constructor.GetParameters().Length;
                    break;
                }
                else if (selectionRule != ConstructorSelectorRule.RequirePrimary)
                {
                    int constParamLength = constructor.GetParameters().Length;
                    if ((selectionRule == ConstructorSelectorRule.MostArgs && selectedConstructorArgs < constructor.GetParameters().Length) ||
                        (selectionRule == ConstructorSelectorRule.LeastArgs && selectedConstructorArgs > constructor.GetParameters().Length))
                    {
                        selectedConstructor = constructor;
                        selectedConstructorArgs = constParamLength;
                    }
                }
            }

            return selectedConstructor;
        }

        private bool IsAsyncMethod(MethodInfo method)
        {

            Type attType = typeof(AsyncStateMachineAttribute);

            // Obtain the custom attribute for the method. 
            // The value returned contains the StateMachineType property. 
            // Null is returned if the attribute isn't present for the method. 
            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);

            return (attrib != null);
        }

        private void ValidateContractFactory(Type contract, Delegate implFactory)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));
            ArgumentValidator.IsNotNull(implFactory, nameof(implFactory));

            var typeinfo = contract.GetTypeInfo();
            Type returnType = implFactory.GetMethodInfo().ReturnType;
            if ((typeinfo.IsClass || typeinfo.IsInterface) == false)
            {
                throw new ArgumentException("Contract is not of type Interface or Class");
            }
            else if (returnType == typeof(void))
            {
                throw new ArgumentException("The factory method return type cannot be null");
            }
            else if (!contract.GetTypeInfo().IsAssignableFrom(returnType.GetTypeInfo()))
            {
                throw new ArgumentException("The return type: " + returnType + " is not assignable to contract: " + contract);
            }
        }
        private void ValidateContractImplementationMatch(Type contract, Type implementation)
        {
            ArgumentValidator.IsNotNull(contract, nameof(contract));
            ArgumentValidator.IsNotNull(implementation, nameof(implementation));
            var typeinfo = contract.GetTypeInfo();
            if ((typeinfo.IsClass || typeinfo.IsInterface) == false)
            {
                throw new ArgumentException("Contract is not of type Interface or Class");
            }
            else if (!contract.GetTypeInfo().IsAssignableFrom(implementation.GetTypeInfo()))
            {
                throw new ArgumentException("The implementation type: " + implementation + " is not assignable to contract: " + contract);
            }
        }
        #region SUPPORT CLASSES

        private void UpdateContainerItem(Type contract, IFrodIoCContainerItem mappedItem, IFrodIoCContainerItem singleInstanceContainerItem)
        {
            IImmutableList<IFrodIoCContainerItem> implementations;
            if (_map.TryGetValue(contract, out implementations))
            {
                _map[contract] = implementations.Replace(mappedItem, singleInstanceContainerItem);
            }
        }

        private class IoCContainerMapResult : IIoCContainerMapResult
        {

            private IList<Type> m_allTypesForContract;
            private Type m_contract;
            private IFrodIoCContainerItem m_mappedItem;
            private DefaultIoCRegistry m_registry;

            internal IoCContainerMapResult(DefaultIoCRegistry registry, Type contract, IFrodIoCContainerItem mappedItem, IList<Type> typesMappedToContract)
            {
                m_registry = registry;
                m_contract = contract;
                m_mappedItem = mappedItem;
                m_allTypesForContract = typesMappedToContract;
            }

            public IList<Type> AllImplementationsOfContract
            {
                get { return m_allTypesForContract; }
            }

            public Type ContractType
            {
                get { return m_contract; }
            }

            public Type NewImplementationType
            {
                get { return m_mappedItem.ImplementationType; }
            }
            /// <summary>
            /// Marks this implementation as singleton so the same instance will be returned every time
            /// </summary>
            public IIoCContainerMapResult AsSingleInstance()
            {
                m_registry.UpdateContainerItem(m_contract, m_mappedItem, new IoCSingleInstanceContainerItem(m_mappedItem, m_registry));
                return this;
            }
        }
        #endregion SUPPORT CLASSES

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _map.Clear();
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
        #endregion

    }

}
