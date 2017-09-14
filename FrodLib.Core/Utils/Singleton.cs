using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{

    /// <summary>
    /// Creates a singleton of the specified generic type
    /// <para>This class can be used instead of implementing the singleton pattern inside the target class</para>
    /// <para>Note that it is still possible to create a separate instace of the tagret class when using this approach</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Singleton<T> where T : new()
    {
        private Singleton()
        {

        }

        public static T Instance
        {
            get
            {
                return Nested<T>.NestedInstance;
            }
        }

        private class Nested<K> where K : new()
        {
            public static readonly K NestedInstance = new K();
        }
    }
}
