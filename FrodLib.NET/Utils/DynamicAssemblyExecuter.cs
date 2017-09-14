using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.Utils
{
    /// <summary>
    /// Loads an assembly in it's own appdomain.
    /// <para>Loaded assembly can be hot swapped by putting a new version of the assembly in the monitored folder</para
    /// </summary>
    public class DynamicAssemblyExecuter : IDisposable
    {
        private Isolated<AssemblyLoader> m_loader;
        private readonly string m_assemblyName;
        private FileSystemWatcher m_fileWatcher;
        private readonly object m_lockObject = new object();
        bool isDisposed = false;

        /// <summary>
        /// Loads assembly into it's own AppDomain and creates a monitor on folder '.\\NewVersions' to allow Hot swapping of the assembly
        /// </summary>
        /// <param name="assemblyName">The assembly that can be hot swapped</param>
        public DynamicAssemblyExecuter(string assemblyName) : this(assemblyName, ".\\NewVersions")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName">The assembly that can be hot swapped</param>
        /// <param name="monitorFolder">The folder to monitor for new versions of assembly</param>
        public DynamicAssemblyExecuter(string assemblyName, string monitorFolder)
        {
            m_assemblyName = Path.GetFileName(assemblyName);
            ReloadAssembly();
            m_fileWatcher = new FileSystemWatcher(monitorFolder, m_assemblyName);
            m_fileWatcher.IncludeSubdirectories = false;
            m_fileWatcher.Created += M_fileWatcher_Created;
            m_fileWatcher.EnableRaisingEvents = true;
        }

        private void M_fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (FileUtilities.IsFileClosed(e.FullPath, true))
            {
                lock (m_lockObject)
                {
                    System.Diagnostics.Trace.TraceInformation("Hot Swapping Assembly: " + m_assemblyName);
                    m_loader.Dispose();
                    try
                    {
                        var entryAsm = Assembly.GetEntryAssembly();
                        var directory = Path.GetDirectoryName(entryAsm.Location);
                        string orgAsmPath = Path.Combine(directory, m_assemblyName);
                        File.Delete(orgAsmPath);
                        File.Move(e.FullPath, orgAsmPath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError(ex.Message);
                    }
                    ReloadAssembly();
                }
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("File lock exists on file: " + e.FullPath + ". Hot swap of assembly '"+ m_assemblyName + "' not possible at this moment");
            }
        }


        private void ReloadAssembly()
        {
            m_loader = new Isolated<AssemblyLoader>();
            m_loader.Value.LoadAssembly(m_assemblyName);
        }

        /// <summary>
        /// Creates instance of class named by instanceTypeName and executes the method named by methodName
        /// </summary>
        /// <param name="instanceTypeName">The full name of class that contains the method to execute</param>
        /// <param name="methodName">The method to execute</param>
        /// <param name="parameters">Method arguments</param>
        /// <returns></returns>
        public object ExecuteMethod(string instanceTypeName, string methodName, params object[] parameters)
        {
            lock (m_lockObject)
            {
                return m_loader.Value.ExecuteMethod(instanceTypeName, methodName, parameters);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                m_fileWatcher.Dispose();
                m_loader.Dispose();
            }

            // Free any unmanaged objects here. 
            isDisposed = true;
        }

        ~DynamicAssemblyExecuter()
        {
            Dispose(false);
        }


        private class AssemblyLoader : MarshalByRefObject
        {
            private Assembly _assembly;
            private readonly Dictionary<Type, object> m_instances = new Dictionary<Type, object>();

            public override object InitializeLifetimeService()
            {
                return null;
            }

            public void LoadAssembly(string path)
            {
                _assembly = Assembly.Load(AssemblyName.GetAssemblyName(path));
            }

            public object ExecuteMethod(string instanceTypeName, string methodName, params object[] parameters)
            {
                object instance;
                var type = _assembly.GetType(instanceTypeName);
                if (type == null) throw new ArgumentException(instanceTypeName + " is not a type in Assembly: " + _assembly);
                if (m_instances.TryGetValue(type, out instance) == false)
                {
                    instance = _assembly.CreateInstance(type.FullName);
                }

                MethodInfo MyMethod = type.GetMethod(methodName);
                return MyMethod.Invoke(instance, BindingFlags.InvokeMethod, null, parameters, null);
            }
        }

    }
}
