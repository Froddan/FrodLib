using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    /// <summary>
    /// Marks a constructor as the primary constructor to use.
    /// If more then one is marked as primary constructor then the first one ecountered will be used
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class PrimaryConstructorAttribute : Attribute
    {
    }
}
