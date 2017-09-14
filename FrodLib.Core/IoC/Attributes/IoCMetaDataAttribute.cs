using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    class IoCMetadataAttribute : Attribute
    {
        public IoCMetadataAttribute(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; private set;  }

        public object Value { get; private set; }
    }
}
