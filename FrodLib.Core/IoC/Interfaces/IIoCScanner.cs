using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FrodLib.IoC
{
    public interface IIoCScanner
    {
       

        /// <summary>
        /// Scan the assembly for types that should be mapped
        /// <para>Call for each assembly that should be included</para>
        /// </summary>
        /// <param name="asm"></param>
        void ScanAssembly(Assembly asm);

        void ScanAssemblyContainingType<TType>();

        /// <summary>
        /// Scan the specified assemblies to register custom registries
        /// </summary>
        void ScanForRegistires();

        /// <summary>
        /// A filter so the caller can exclude objects per type if they should be mapped or not
        /// </summary>
        /// <param name="filter"></param>
        void Exclude(Predicate<Type> filter);

        /// <summary>
        /// The scanner will exclude types that are included in the following namespace
        /// <para>Call multiple times for each namespace that should be excluded</para>
        /// </summary>
        /// <param name="excludeNamespace"></param>
        void ExcludeNamespace(string excludeNamespace);

        /// <summary>
        /// The scanner will only look for types that are included in the following namespace
        /// <para>Call multiple times for each namespace that should be included</para>
        /// </summary>
        /// <param name="includeNamespace"></param>
        void IncludeNamespace(string includeNamespace);

        /// <summary>
        /// Register only interfaces that has only one implementation
        /// </summary>
        void SingleImplementationOfInterface();

        /// <summary>
        /// All found implementations will be registered as single instance ie. The same instance will always be returned and not a new one each time
        /// </summary>
        void RegisterAsSingleInstance();
    }
}
