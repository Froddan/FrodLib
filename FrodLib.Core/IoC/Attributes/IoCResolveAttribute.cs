using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{

    /// <summary>
    /// Marks field and properties that should be resolved when an instance is sent into the Fill method of the IoCContainer
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IoCResolveAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class IoCResolveManyAttribute : IoCResolveAttribute
    {

    }

    /// <summary>
    /// Marks methods that are used to inject objects, that are not injected in constructor during object creation
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal class IoCInjectAttribute : Attribute
    {

    }
}
