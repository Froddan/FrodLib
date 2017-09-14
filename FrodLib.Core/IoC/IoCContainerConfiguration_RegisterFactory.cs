using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections;

namespace FrodLib.IoC
{
    partial class IoCContainerConfiguration
    {

      
        public IIoCContainerMapResult Register(Type contract, Delegate implFactory, params object[] args)
        {
            return m_defaultRegistry.Register(contract, implFactory, args);
        }

        public IIoCContainerMapResult Register<TType>(Delegate implFactory, params object[] args)
        {
            var type = typeof(TType);
            return Register(type, implFactory, args);
        }

    }

    public static class IoCContainerConfigurationExtensions
    {
        public static IIoCContainerMapResult Register(this IIoCContainerConfiguration containerConfig, Type contract, Func<object, object> implFactory, params object[] args)
        {
            Delegate factoryDelegate = implFactory;
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register(this IIoCContainerConfiguration containerConfig, Type contract, Func<object[], object> implFactory, params object[] args)
        {
            Delegate factoryDelegate = implFactory;
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract>(this IIoCContainerConfiguration containerConfig, Func<object[], TContract> implFactory, params object[] args)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract>(this IIoCContainerConfiguration containerConfig, Func<TContract> implFactory)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            object[] args = { };
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract, P1>(this IIoCContainerConfiguration containerConfig, Func<P1, TContract> implFactory, P1 arg1)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            object[] args = { arg1 };
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract, P1, P2>(this IIoCContainerConfiguration containerConfig, Func<P1, P2, TContract> implFactory, P1 arg1, P2 arg2)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            object[] args = { arg1, arg2 };
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract, P1, P2, P3>(this IIoCContainerConfiguration containerConfig, Func<P1, P2, P3, TContract> implFactory, P1 arg1, P2 arg2, P3 arg3)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            object[] args = { arg1, arg2, arg3 };
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract, P1, P2, P3, P4>(this IIoCContainerConfiguration containerConfig, Func<P1, P2, P3, P4, TContract> implFactory, P1 arg1, P2 arg2, P3 arg3, P4 arg4)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            object[] args = { arg1, arg2, arg3, arg4 };
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract, P1, P2, P3, P4, P5>(this IIoCContainerConfiguration containerConfig, Func<P1, P2, P3, P4, P5, TContract> implFactory, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            object[] args = { arg1, arg2, arg3, arg4, arg5 };
            return containerConfig.Register(contract, factoryDelegate, args);
        }

        public static IIoCContainerMapResult Register<TContract, P1, P2, P3, P4, P5, P6>(this IIoCContainerConfiguration containerConfig, Func<P1, P2, P3, P4, P5, P6, TContract> implFactory, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6)
            where TContract : class
        {
            Type contract = typeof(TContract);
            Delegate factoryDelegate = implFactory;
            object[] args = { arg1, arg2, arg3, arg4, arg5, arg6 };
            return containerConfig.Register(contract, factoryDelegate, args);
        }

    }
}
