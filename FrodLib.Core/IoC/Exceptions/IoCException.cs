using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    public class IoCException : InvalidOperationException
    {
        public Type Contract { get; private set; }

      
        public IoCException(Type contract, string message) : base(message)
        {
            Contract = contract;
        }

        public IoCException(Type contract, string message, Exception innerException) : base(message, innerException)
        {
            Contract = contract;
        }
    }
}
