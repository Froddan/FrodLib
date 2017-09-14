using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    public static class IoCExtensions
    {
        public static TContract CreateInstance<TContract>(this IIoCRegistry registry) where TContract : class
        {
            return (TContract)registry.CreateInstance(typeof(TContract));
        }
        
    }
}
