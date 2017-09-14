using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    public interface IIoCContainerMapResult
    {
        Type ContractType { get; }
        Type NewImplementationType { get; }
        IList<Type> AllImplementationsOfContract { get; }

        IIoCContainerMapResult AsSingleInstance();
    }
}
