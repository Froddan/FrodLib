using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    class IoCLazy<T, TMetadata> : Lazy<T>
    {
        public IoCLazy(TMetadata metadata, Func<T> factory): base(factory)
        {
            this.Metadata = metadata;
        }

        public TMetadata Metadata { get; private set; }
    }
}
