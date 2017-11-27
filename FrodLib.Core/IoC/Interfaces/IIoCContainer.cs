using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.IoC
{
    public interface IIoCContainer : IDisposable
    {
        /// <summary>
        /// Called to configure the IoC container
        /// </summary>
        /// <param name="configureAction"></param>
        void Configure(Action<IIoCContainerConfiguration> configureAction);

        /// <summary>
        /// Returns an instance mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        object GetInstance(Type contract);

        /// <summary>
        /// Returns an instance mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="argResolver"></param>
        /// <returns></returns>
        object GetInstance(Type contract, ResolveConstructorArgumentDelegate argResolver);

        /// <summary>
        /// Returns an instance mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="argResolver"></param>
        /// <returns></returns>
        object GetInstance(Type contract, IIoCArgumentResolver argResolver);

        /// <summary>
        /// Returns an instance of the mapped contract and maps it to a key, to allow the container to return it again without recreating it
        /// <para>This key mapping is unneccesary if the implementation is already mapped as a single instance using AsSingleInstance() during registration ie. the same instance is already returned (Singleton)</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetInstance(Type contract, string key);

        /// <summary>
        /// Returns an instance mapped to the contract and maps it to a key, to allow the container to return it again without recreating it
        /// <para>This key mapping is unneccesary if the implementation is already mapped as a single instance using AsSingleInstance() during registration ie. the same instance is already returned (Singleton)</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="key"></param>
        /// <param name="argResolver"></param>
        /// <returns></returns>
        object GetInstance(Type contract, string key, ResolveConstructorArgumentDelegate argResolver);

        /// <summary>
        /// Returns an instance mapped to the contract and maps it to a key, to allow the container to return it again without recreating it
        /// <para>This key mapping is unneccesary if the implementation is already mapped as a single instance using AsSingleInstance() during registration ie. the same instance is already returned (Singleton)</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="key"></param>
        /// <param name="argResolver"></param>
        /// <returns></returns>
        object GetInstance(Type contract, string key, IIoCArgumentResolver argResolver);

        /// <summary>
        /// Returns an instance mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        TContract GetInstance<TContract>() where TContract : class;

        /// <summary>
        /// Returns an instance mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        TContract GetInstance<TContract>(ResolveConstructorArgumentDelegate argResolver) where TContract : class;

        /// <summary>
        /// Returns an instance mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        TContract GetInstance<TContract>(IIoCArgumentResolver argResolver) where TContract : class;

        /// <summary>
        /// Returns an instance mapped to the contract and maps it to a key, to allow the container to return it again without recreating it
        /// <para>This key mapping is unneccesary if the implementation is already mapped as a single instance using AsSingleInstance() during registration ie. the same instance is already returned (Singleton)</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TContract GetInstance<TContract>(string key) where TContract : class;

        /// <summary>
        /// Returns an instance mapped to the contract and maps it to a key, to allow the container to return it again without recreating it
        /// <para>This key mapping is unneccesary if the implementation is already mapped as a single instance using AsSingleInstance() during registration ie. the same instance is already returned (Singleton)</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="key"></param>
        /// <param name="argResolver"></param>
        /// <returns></returns>
        TContract GetInstance<TContract>(string key, ResolveConstructorArgumentDelegate argResolver) where TContract : class;

        /// <summary>
        /// Returns an instance mapped to the contract and maps it to a key, to allow the container to return it again without recreating it
        /// <para>This key mapping is unneccesary if the implementation is already mapped as a single instance using AsSingleInstance() during registration ie. the same instance is already returned (Singleton)</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="key"></param>
        /// <param name="argResolver"></param>
        /// <returns></returns>
        TContract GetInstance<TContract>(string key, IIoCArgumentResolver argResolver) where TContract : class;

        /// <summary>
        /// Tries to return an instance that is mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance(Type contract, out object instance);

        /// <summary>
        /// Tries to return an instance that is mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="argResolver"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool TryGetInstance(Type contract, ResolveConstructorArgumentDelegate argResolver, out object instance);

        /// <summary>
        /// Tries to return an instance that is mapped to the contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="argResolver"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool TryGetInstance(Type contract, IIoCArgumentResolver argResolver, out object instance);

        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance(Type contract, string key, out object instance);


        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance(Type contract, string key, ResolveConstructorArgumentDelegate argResolver, out object instance);

        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance(Type contract, string key, IIoCArgumentResolver argResolver, out object instance);


        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance<TContract>(out TContract instance) where TContract : class;

        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance<TContract>(ResolveConstructorArgumentDelegate argResolverout,out TContract instance) where TContract : class;

        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance<TContract>(IIoCArgumentResolver argResolverout, out TContract instance) where TContract : class;

        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance<TContract>(string key, out TContract instance) where TContract : class;

        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance<TContract>(string key, ResolveConstructorArgumentDelegate argResolver, out TContract instance) where TContract : class;

        /// <summary>
        /// Tries to return an instance of the mapped contract
        /// <para>Instance can be either a new instance or a previous mapped one</para>
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>True if instance was found</returns>
        bool TryGetInstance<TContract>(string key, IIoCArgumentResolver argResolver, out TContract instance) where TContract : class;


        #region Remove Mapped

        /// <summary>
        /// Tries to resolve all none static fields and properties marked with <see cref="FrodLib.IoC.IoCResolveAttribute" /> attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void Fill<T>(T instance) where T : class;

        /// <summary>
        /// Tries to resolve all none static fields and properties marked with <see cref="FrodLib.IoC.IoCResolveAttribute" /> attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="argResolver"></param>
        void Fill<T>(T instance, ResolveConstructorArgumentDelegate argResolver) where T : class;

        /// <summary>
        /// Tries to resolve all none static fields and properties marked with <see cref="FrodLib.IoC.IoCResolveAttribute" /> attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="argResolver"></param>
        void Fill<T>(T instance, IIoCArgumentResolver argResolver) where T : class;

        /// <summary>
        /// Returns a list of all mapped (by key) instances for contract
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        IReadOnlyList<object> GetAllInstances(Type contract);

        /// <summary>
        /// Returns a list of all mapped (by key) instances for contract
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IReadOnlyList<TContract> GetAllInstances<TContract>();

        /// <summary>
        /// Clears all instances mapped to a key
        /// </summary>
        void ClearAllMappedInstances();

        /// <summary>
        /// Clears all instances mapped to a key for the specific contract
        /// </summary>
        void ClearAllMappedInstancesForContract(Type contract);

        /// <summary>
        /// Clears all instances mapped to a key for the specific contract
        /// </summary>
        void ClearAllMappedInstancesForContract<TContract>();

        /// <summary>
        /// Removes the current mapped single instance of a contract in order to allow it to be recreated
        /// </summary>
        /// <param name="contractType">The contract for which the single instance should be removed</param>
        /// <returns></returns>
        bool RemoveCurrentSingleInstance(Type contractType);

        /// <summary>
        /// Removes the current mapped single instance of a contract in order to allow it to be recreated
        /// </summary>
        /// <typeparam name="TContract">The contract for which the single instance should be removed</typeparam>
        /// <returns></returns>
        bool RemoveCurrentSingleInstance<TContract>() where TContract : class;

        #endregion;
    }
}
