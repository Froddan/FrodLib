using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FrodLib.Utils;

namespace FrodLib.IoC
{
    class IoCScanner : IIoCScanner
    {
        private List<Assembly> m_assembliesToScan = new List<Assembly>();
        private List<string> m_includedNamespaces = new List<string>();
        private List<string> m_excludedNamespaces = new List<string>();
        private bool m_scanForRegistries;
        private bool m_scanOnlyInterfacesWithASingleImplementation;
        private Predicate<Type> m_excludeFilter;
        private bool m_registerAsSingleInstance;

        public void Exclude(Predicate<Type> filter)
        {
            ArgumentValidator.IsNotNull(filter, nameof(filter));
            m_excludeFilter = filter;
        }

        public void ExcludeNamespace(string excludeNamespace)
        {
            ArgumentValidator.IsNotNull(excludeNamespace, nameof(excludeNamespace));
            if (!m_excludedNamespaces.Contains(excludeNamespace))
            {
                m_excludedNamespaces.Add(excludeNamespace);
            }
        }

        public void IncludeNamespace(string includeNamespace)
        {
            ArgumentValidator.IsNotNull(includeNamespace, nameof(includeNamespace));
            if (!m_includedNamespaces.Contains(includeNamespace))
            {
                m_includedNamespaces.Add(includeNamespace);
            }
        }


        public void ScanAssembly(Assembly asm)
        {
            ArgumentValidator.IsNotNull(asm, nameof(asm));
            if (!m_assembliesToScan.Contains(asm))
            {
                m_assembliesToScan.Add(asm);
            }
        }

        public void ScanForRegistires()
        {
            m_scanForRegistries = true;
        }

        public void SingleImplementationOfInterface()
        {
            m_scanOnlyInterfacesWithASingleImplementation = true;
        }

        internal void AddScannedTypesToRegistry(IoCContainerConfiguration config)
        {
            if(m_scanForRegistries)
            {
                var registries = GetTypesImplementingInterface(m_assembliesToScan, typeof(IIoCRegistry));
                foreach (var registry in registries)
                {
                    if (registry.Equals(typeof(DefaultIoCRegistry))) continue;

                    IIoCRegistry registryInstance = Activator.CreateInstance(registry) as IIoCRegistry;
                    if(registryInstance != null) config.AddRegistry(registryInstance);
                }

            }


            var interfaces = GetAllInterfaces(m_assembliesToScan);
            foreach (var @interface in interfaces)
            {
                var implementations = GetTypesImplementingInterface(m_assembliesToScan, @interface);
                var implementationsArr = implementations.Where(t => IsTypeInIncludedNamespaces(t) &&
                                                            !IsTypeInExcludedNamespaces(t) &&
                                                            (m_excludeFilter == null || !m_excludeFilter(t))).ToArray();

                if (implementationsArr.Length == 0) continue;
                else if (m_scanOnlyInterfacesWithASingleImplementation && implementationsArr.Length > 1) continue;
                else
                {
                    foreach (var implementation in implementationsArr)
                    {
                        var result = config.Register(@interface, implementation, null);
                        if(m_registerAsSingleInstance)
                        {
                            result.AsSingleInstance();
                        }
                    }
                }
            }
        }

        private IEnumerable<Type> GetAllInterfaces(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(asm => asm.ExportedTypes).Where(t => t.GetTypeInfo().IsInterface);
        }

        private IEnumerable<Type> GetTypesImplementingInterface(IEnumerable<Assembly> assemblies, Type @interface)
        {
            var interfaceTypeInfo = @interface.GetTypeInfo();
            return assemblies.SelectMany(asm => asm.DefinedTypes).Where(ti =>
            {
                TypeInfo typeInfo = ti;

                return typeInfo.IsClass && !typeInfo.IsAbstract && interfaceTypeInfo.IsAssignableFrom(typeInfo);
            }).Select(ti => ti.AsType());
        }

        private bool IsTypeInIncludedNamespaces(Type type)
        {
            if (m_includedNamespaces == null || m_includedNamespaces.Count == 0) return true;

            string typeNamespace = type.Namespace;
            foreach (var ns in m_includedNamespaces)
            {
                if (typeNamespace.StartsWith(ns, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsTypeInExcludedNamespaces(Type type)
        {
            if (m_excludedNamespaces == null || m_excludedNamespaces.Count == 0) return false;

            string typeNamespace = type.Namespace;
            foreach (var ns in m_excludedNamespaces)
            {
                if (typeNamespace.StartsWith(ns, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public void RegisterAsSingleInstance()
        {
            m_registerAsSingleInstance = true;
        }

        public void ScanAssemblyContainingType<TType>()
        {
            var type = typeof(TType);
            var asm = type.GetTypeInfo().Assembly;
            ScanAssembly(asm);
        }
    }
}
