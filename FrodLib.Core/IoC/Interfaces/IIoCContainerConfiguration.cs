using FrodLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.IoC
{
    public interface IIoCContainerConfiguration
    {
        ConstructorSelectorRule DefaultConstructorSelectionRule { get; set; }

        void AddRegistry(IIoCRegistry registry);

        bool RemoveRegistry(IIoCRegistry registry);

        void Scan(Action<IIoCScanner> scanAction);

#region REGISTER TYPE

        IIoCContainerMapResult Register<TContract, TImplementation>(params object[] args) where TContract : class
            where TImplementation : TContract;
        IIoCContainerMapResult Register(Type contract, Type implementation, params object[] args);

#endregion

#region REGISTER INSTANCE

        void RegisterInstance(Type contract, object instance);

        void RegisterInstance<TContract>(TContract instance) where TContract : class;

#endregion REGISTER INSTANCE

#region REGISTER FACTORY

        IIoCContainerMapResult Register(Type contract, Delegate implFactory, params object[] args);
        IIoCContainerMapResult Register<TType>(Delegate implFactory, params object[] args);

#endregion

#region Unregister

        /// <summary>
        /// Removes an mapped contract implementation
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Unregister(Type type);

        /// <summary>
        /// Removes an mapped contract implementation
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <returns></returns>
        bool Unregister<TIn>() where TIn : class;

#endregion Unregister

    }
}
