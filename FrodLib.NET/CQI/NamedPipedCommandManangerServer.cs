using FrodLib.Interfaces;
using FrodLib.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    public class NamedPipedCommandManangerServer : CommandManager, ICommandManagerOutput, IDisposable
    {
        private readonly string m_pipeName;
        private readonly int m_maxPipeServers;
        private readonly Thread[] m_servers;
        private readonly NamedPipeServerStream[] m_pipeServers;
        private bool isRunning = true;
        [ThreadStatic]
        private static PipeStreamString s_streamString;

        public NamedPipedCommandManangerServer()
            : this(Assembly.GetEntryAssembly().GetName().Name, 1)
        {

        }

        public NamedPipedCommandManangerServer(string pipeName) : this(pipeName, 1)
        {

        }

        public NamedPipedCommandManangerServer(string pipeName, int maxNofServers)
        {
            m_pipeName = pipeName;
            m_maxPipeServers = maxNofServers;
            m_servers = new Thread[maxNofServers];
            m_pipeServers = new NamedPipeServerStream[maxNofServers];

            InitializeCommandHistory();

            CommandRegistry.Instance.RegisterCommandContainer(Assembly.GetAssembly(typeof(CommandManager)));
            CommandRegistry.Instance.RegisterCommandContainer(Assembly.GetAssembly(typeof(NamedPipedCommandManangerServer)));

            SetupPipeServer();
        }

        private void SetupPipeServer()
        {
            for (int i = 0; i < m_maxPipeServers; i++)
            {
                m_servers[i] = new Thread(ServerThread);
                m_servers[i].IsBackground = true;
                m_servers[i].Start(i);
            }
        }

        private void ServerThread(object data)
        {
            int threadIndex = (int)data;
            do
            {
                using (m_pipeServers[threadIndex] = new NamedPipeServerStream(m_pipeName, PipeDirection.InOut, m_maxPipeServers, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
                {
                    bool synchronized = false;
                    var pipeServer = m_pipeServers[threadIndex];
                    System.Diagnostics.Trace.TraceInformation("Awaiting client connection for pipe '" + m_pipeName + "::" + threadIndex + "'");
                    while (isRunning && !pipeServer.IsConnected)
                    {
                        var async = pipeServer.BeginWaitForConnection(null, null);
                        if (async.AsyncWaitHandle.WaitOne(5000))
                        {
                            pipeServer.EndWaitForConnection(async);
                            break;
                        }
                        else
                        {
                            try
                            {
                                pipeServer.EndWaitForConnection(async);
                            }
                            catch (IOException)
                            {}
                        }
                    }

                    System.Diagnostics.Trace.TraceInformation("Client connected");
                    InitializeCommandHistory();

                    try
                    {
                        s_streamString = new PipeStreamString(pipeServer);
                        string input = s_streamString.ReadString();
                        if (input == "SYNC")
                        {
                            s_streamString.WriteString("SYNC");
                            input = s_streamString.ReadString();
                            if (input == "OK")
                            {
                                synchronized = true;
                            }
                        }

                        System.Diagnostics.Trace.TraceInformation("Client syncronized: " + synchronized);
                        while (synchronized && isRunning && pipeServer.IsConnected)
                        {
                            input = s_streamString.ReadString();
                            if (input == null) synchronized = false;
                            else if (input == string.Empty) continue;
                            else
                            {
                                ExecuteCommand(input);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.TraceError("Error occured: " + e.Message);
                    }
                    finally
                    {
                        System.Diagnostics.Trace.TraceInformation("Closing connection connection");
                        ((ICommandManager)this).ClearCommandHistory();
                        s_streamString = null;
                        m_pipeServers[threadIndex] = null;
                    }

                }
            } while (isRunning);
        }

        public void WriteLine(string message)
        {
            var stream = s_streamString;
            if (stream != null)
            {
                stream.WriteString(message);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    isRunning = false;
                    for (int i = 0; i < m_pipeServers.Length; i++)
                    {
                        var pipeServer = m_pipeServers[i];
                        if (pipeServer != null)
                        {
                            if (pipeServer.IsConnected)
                            {
                                pipeServer.Disconnect();
                            }
                            pipeServer.Close();
                            pipeServer.Dispose();
                        }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NamedPipedCommandManangerServer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
