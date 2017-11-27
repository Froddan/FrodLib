using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    partial class DefaultIoCRegistry
    {
        #region CONTAINER ITEMS

        private interface IFrodIoCContainerItem
        {
            object CreateInstance(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects);

            Type Contract { get; }
            Type ImplementationType { get; }
            
        }

        private interface IIoCAsyncContainerItem : IFrodIoCContainerItem
        {
            Task<object> CreateInstanceAsync(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects);
        }

        private abstract class IoCContainerItemBase : IFrodIoCContainerItem
        {
            public virtual Type Contract { get; protected set; }

            public virtual Type ImplementationType { get; protected set; }


            public abstract object CreateInstance(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects);
           
        }

        private class IoCAsyncFactoryContainerItem : IoCFactoryContainerItem, IIoCAsyncContainerItem
        {

            public IoCAsyncFactoryContainerItem(Type contract, DefaultIoCRegistry registry, Delegate factoryFunc, object[] args) : base(contract, registry, factoryFunc, args)
            {

            }

            public async Task<object> CreateInstanceAsync(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects)
            {
                var methodParams = m_factoryFunc.GetMethodInfo().GetParameters();
                var args = Args;

                object instance = null;

                if (methodParams.Length == args.Length)
                {
                    //The inserted arguments should hopefully match the method signature here
                    instance = await (dynamic)m_factoryFunc.DynamicInvoke(args);
                }
                else
                {
                    //Lookup the method arguments
                    args = m_registry.GetArguments(contract, methodParams, argResolver, false, resolvedObjects);
                    instance = await (dynamic)m_factoryFunc.DynamicInvoke(args);
                }

                return instance;
            }

        }

        private class IoCCircularReferenceCheckerContainerItem : IoCContainerItemBase, IFrodIoCContainerItem
        {
            public override Type ImplementationType { get { return m_instanceCreator.ImplementationType; } }
            public override Type Contract { get { return m_instanceCreator.Contract; } }
         
            private readonly IFrodIoCContainerItem m_instanceCreator;

            [ThreadStatic]
            private static HashSet<Type> m_circularDependecyDetector;

            protected void CheckCircularReference(Type type)
            {
                if (m_circularDependecyDetector == null) m_circularDependecyDetector = new HashSet<Type>();

                if (m_circularDependecyDetector.Contains(type))
                {
                    throw new IoCCircularReferenceException(type, string.Format(StringResources.IoCCircularReferenceDetected, type));
                }

            }

            public IoCCircularReferenceCheckerContainerItem(IFrodIoCContainerItem instanceCreator)
            {
                m_instanceCreator = instanceCreator;
            }

            public override object CreateInstance(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects)
            {
                CheckCircularReference(ImplementationType);

                try
                {
                    m_circularDependecyDetector.Add(ImplementationType);

                    object instance = m_instanceCreator.CreateInstance(contract, argResolver, resolvedObjects, useResolvedObjects);
                    return instance;
                }
                finally
                {
                    m_circularDependecyDetector.Remove(ImplementationType);
                }
            }
        }

        private class IoCSingleInstanceContainerItem : IoCContainerItemBase, IFrodIoCContainerItem
        {
            public override Type ImplementationType { get { return m_instanceCreator.ImplementationType; } }
            public override Type Contract { get { return m_instanceCreator.Contract; } }

            private object m_instance;
            private readonly IFrodIoCContainerItem m_instanceCreator;
            private readonly DefaultIoCRegistry m_registry;

            public IoCSingleInstanceContainerItem(IFrodIoCContainerItem instanceCreator, DefaultIoCRegistry registry)
            {
                m_instanceCreator = instanceCreator;
                m_registry = registry;
            }

            public override object CreateInstance(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects)
            {
                if (m_instance == null)
                {
                    m_instance = m_instanceCreator.CreateInstance(contract, argResolver, resolvedObjects, useResolvedObjects);
                }
                return m_instance;
            }

            internal void ClearCurrentInstance()
            {
                m_instance = null;
            }
        }

        private class IoCRegistredInstanceContainerItem : IoCContainerItemBase, IFrodIoCContainerItem
        {

            public override Type ImplementationType { get { return m_instance.GetType(); } }
            private object m_instance;

            public IoCRegistredInstanceContainerItem(Type contract, object instance)
            {
                Contract = contract;
                m_instance = instance;
            }

            public override object CreateInstance(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects)
            {
                return m_instance;
            }
        }

        private class IoCFactoryContainerItem : IoCContainerItemBase, IFrodIoCContainerItem
        {
            protected DefaultIoCRegistry m_registry;
           
            protected Delegate m_factoryFunc;
            public object[] Args { get; private set; }

            public IoCFactoryContainerItem(Type contract, DefaultIoCRegistry registry, Delegate factoryFunc, object[] args)
            {
                Contract = contract;
                m_registry = registry;
                m_factoryFunc = factoryFunc;
                Args = args ?? new object[0];
                ImplementationType = factoryFunc.GetType();
            }

            public override object CreateInstance(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects)
            {
                var methodParams = m_factoryFunc.GetMethodInfo().GetParameters();
                var args = Args;

                object instance = null;

                if (methodParams.Length == args.Length)
                {
                    //The inserted arguments should hopefully match the method signature here
                    instance = m_factoryFunc.DynamicInvoke(args);
                }
                else
                {
                    //Lookup the method arguments
                    args = m_registry.GetArguments(contract, methodParams, argResolver, false, resolvedObjects);
                    instance = m_factoryFunc.DynamicInvoke(args);
                }

                return instance;
            }

        }

        private class IoCContainerItem : IoCContainerItemBase, IFrodIoCContainerItem
        {
            private DefaultIoCRegistry m_registry;


            public IoCContainerItem(Type contract, DefaultIoCRegistry registry, Type implementationType, object[] args)
            {

                var implementationTypeInfo = implementationType.GetTypeInfo();
                if (implementationTypeInfo.IsClass && !implementationTypeInfo.IsAbstract)
                {
                    m_registry = registry;
                    Contract = contract;
                    ImplementationType = implementationType;
                    Args = args;
                }
                else
                {
                    throw new InvalidOperationException(StringResources.ImplementationTypeIsNotAnInstantiatableClass);
                }
            }

            public object[] Args { get; private set; }
           
            public override object CreateInstance(Type contract, IIoCArgumentResolver argResolver, IDictionary<Type, object> resolvedObjects, bool useResolvedObjects)
            {
                try
                {
                    if (Args != null && Args.Any())
                    {
                        return Activator.CreateInstance(ImplementationType, Args);
                    }
                    else
                    {
                        object instance = instance = m_registry._GetInstance(ImplementationType,Contract, !ImplementationType.Equals(contract), argResolver, false, resolvedObjects, useResolvedObjects);

                        if (instance != null)
                        {
                            return instance;
                        }
                        else
                        {
                            throw new IoCException(contract, StringResources.UnableToCreateInstanceOfType);
                        }
                    }
                }
                catch (MissingMemberException)
                {
                    object instance = instance = m_registry._GetInstance(ImplementationType, Contract, !ImplementationType.Equals(contract), argResolver, false, resolvedObjects,useResolvedObjects);
                    if (instance != null)
                    {
                        return instance;
                    }
                    else
                    {
                        throw;
                    }
                }
            }


        }

        #endregion CONTAINER ITEMS


    }
}
