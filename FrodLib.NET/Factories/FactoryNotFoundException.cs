using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Factories
{
    [ExcludeFromCodeCoverage]
    public class FactoryNotFoundException : Exception, ISerializable
    {
        public FactoryNotFoundException()
            : base()
        {

        }

        public FactoryNotFoundException(string message)
            : base(message)
        {

        }

        public FactoryNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public FactoryNotFoundException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}
