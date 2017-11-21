using FrodLib.IoC;
using FrodLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    public sealed class CommandRegistry : IIntenalIoCRegistry
    {
       
        public static CommandRegistry Instance
        {
            get
            {
                return Singleton<CommandRegistry>.Instance;
            }
        }

        object IIoCRegistry.CreateInstance(Type contract)
        {
            throw new NotImplementedException();
        }

        //TContract IIoCRegistry.CreateInstance<TContract>()
        //{
        //    throw new NotImplementedException();
        //}

        IEnumerable<object> IIntenalIoCRegistry.CreateManyInstance(Type contract)
        {
            var commandType = typeof(ICQICommand).GetTypeInfo();
            foreach (var asm in m_catalogs)
            {
                foreach (TypeInfo typeInfo in asm.DefinedTypes)
                {
                    if (!typeInfo.IsAbstract && !typeInfo.IsInterface && commandType.IsAssignableFrom(typeInfo))
                    {
                        Type type = typeInfo.AsType();
                        if (contract == typeof(ICQICommand))
                        {
                            yield return Activator.CreateInstance(type);
                        }
                        else if (contract == typeof(Lazy<ICQICommand>))
                        {
                            yield return new Lazy<ICQICommand>(() =>
                            {
                                return (ICQICommand)Activator.CreateInstance(type);
                            });
                        }
                        else if (contract == typeof(IoCLazy<ICQICommand, ICQICommandData>))
                        {
                            PromptCommandMetadata commandMetaData = new PromptCommandMetadata();
                            var metadataAttributes = typeInfo.GetCustomAttributes<MetadataAttribute>();
                            foreach (var attr in metadataAttributes)
                            {
                                if(attr.Name.Equals(nameof(PromptCommandMetadata.Command), StringComparison.CurrentCultureIgnoreCase) && attr.Value != null)
                                {
                                    commandMetaData.Command = attr.Value.ToString();
                                }
                                else if (attr.Name.Equals(nameof(PromptCommandMetadata.Description), StringComparison.CurrentCultureIgnoreCase) && attr.Value != null)
                                {
                                    commandMetaData.Description = attr.Value.ToString();
                                }
                            }

                            yield return new CQILazy(type, commandMetaData);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            
        }

        bool IIoCRegistry.HasImplementationForType(Type contract)
        {
            
            if (contract == typeof(ICQICommand))
            {
                return true;
            }
            else if (contract == typeof(Lazy<ICQICommand>))
            {
                return true;
            }
            else if (contract == typeof(IoCLazy<ICQICommand, ICQICommandData>))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<Assembly> m_catalogs = new List<Assembly>();

        internal event EventHandler CommandCatalogsChanged;

        public void RegisterCommandContainer(Assembly asm)
        {
            //AssemblyCatalog assemblyCatalog = new AssemblyCatalog(asm);
            m_catalogs.Add(asm);
            RaiseCommandCatalogsChanged();
        }

        //public void RegisterCommandContainer(string directoryPath)
        //{
        //    //DirectoryCatalog directoryCatalog = new DirectoryCatalog(directoryPath);
        //    //m_catalogs.Add(directoryCatalog);
        //    RaiseCommandCatalogsChanged();
        //}

        public  void UnregisterCommandContainer(Assembly asm)
        {
            bool catalogRemoved = false;
            for (int i = 0; i < m_catalogs.Count; i++)
            {
                if (m_catalogs[i] == asm)
                {
                    m_catalogs.RemoveAt(i);
                    catalogRemoved = true;
                    break;
                }
            }

            if (catalogRemoved)
            {
                RaiseCommandCatalogsChanged();
            }
        }

        //public void UnregisterCommandContainer(string directoryPath)
        //{
        //    bool catalogRemoved = false;
        //    //for (int i = 0; i < m_catalogs.Count; i++)
        //    //{
        //    //    if (m_catalogs[i] is DirectoryCatalog)
        //    //    {
        //    //        if (((DirectoryCatalog)m_catalogs[i]).Path == directoryPath)
        //    //        {
        //    //            m_catalogs.RemoveAt(i);
        //    //            catalogRemoved = true;
        //    //            break;
        //    //        }
        //    //    }
        //    //}

        //    if (catalogRemoved)
        //    {
        //        RaiseCommandCatalogsChanged();
        //    }
        //}

        private void RaiseCommandCatalogsChanged()
        {
            var handler = CommandCatalogsChanged;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }

       

        private class PromptCommandMetadata : ICQICommandData
        {
            private string m_command;
            private string m_Description;

            public string Command
            {
                get { return m_command ?? string.Empty; } set { m_command = value; }
            }

            public string Description
            {
                get { return m_Description ?? string.Empty; }
                set { m_Description = value; }
            }
        }

        private class CQILazy : IoCLazy<ICQICommand, ICQICommandData>
        {
          
            public CQILazy(Type implementation, ICQICommandData metadata ) :base(metadata, () => {
                return (ICQICommand)Activator.CreateInstance(implementation);
            })
            {

            }
        }
    }
}
