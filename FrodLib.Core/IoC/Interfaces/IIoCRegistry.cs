using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.IoC
{
    public interface IIoCRegistry : IDisposable
    {
        bool HasImplementationForType(Type contract);

        //TContract CreateInstance<TContract>() where TContract : class;

        object CreateInstance(Type contract);
        
    }

    internal interface IDefaultIoCContainer : IIoCRegistry
    {
       
        object CreateInstance(Type contract, IIoCArgumentResolver argResolver, bool resolveMethodInject, IDictionary<Type, object> resovedObjects);

        IIoCContainerMapResult Register(Type contract, Type implementation, object[] args);
    }

    internal interface IIntenalIoCRegistry : IIoCRegistry
    {
        IEnumerable<object> CreateManyInstance(Type contract);
    }
}
