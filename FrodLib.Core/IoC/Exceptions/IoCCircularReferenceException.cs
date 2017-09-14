using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    public class IoCCircularReferenceException : IoCException
    {
        public IoCCircularReferenceException(Type contract, string message) : base(contract, message)
        {
        }

        public IoCCircularReferenceException(Type contract, string message, Exception innerException) : base(contract, message, innerException)
        {
        }
    }
}
