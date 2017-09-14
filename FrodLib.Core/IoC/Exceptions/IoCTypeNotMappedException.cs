using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    public class IoCTypeNotMappedException : IoCException
    {
       
        public IoCTypeNotMappedException(Type contract, string message) : base(contract, message)
        {

        }

        public IoCTypeNotMappedException(Type contract, string message, Exception innerException) : base(contract, message, innerException)
        {

        }
    }
}
