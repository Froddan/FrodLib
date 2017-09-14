using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class MetadataAttribute : Attribute
    {
        public MetadataAttribute(string name, object value)
        {
            this.Name = name ?? string.Empty;
            this.Value = value;
        }

        public string Name { get; private set; }

        public object Value { get; private set; }
    }
}
