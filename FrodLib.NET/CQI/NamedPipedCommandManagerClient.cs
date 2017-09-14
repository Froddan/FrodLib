using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    public class NamedPipedCommandManagerClient : IDisposable
    {
        private readonly ICommandManagerOutput m_output;
        private NamedPipeClientStream pipeClient;
        private readonly string m_pipeName;
        private readonly string m_serverName;
        private bool m_isRunning;

        public NamedPipedCommandManagerClient(string pipeName, ICommandManagerOutput output) : this(pipeName, "localhost", output)
        {
           
        }

        public NamedPipedCommandManagerClient(string pipeName, string serverName, ICommandManagerOutput output)
        {
            m_pipeName = pipeName;
            m_serverName = serverName;
            m_output = output;
        }

        public bool Connect()
        {
            return Connect(5000);
        }

        public bool Connect(int timeout)
        {
            try
            {
                pipeClient = new NamedPipeClientStream(m_serverName, m_pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                pipeClient.Connect(timeout);
                System.Diagnostics.Trace.TraceInformation("Connected. Awaiting synchronization");
                m_pipeStreamString = new PipeStreamString(pipeClient);
                m_pipeStreamString.WriteString("SYNC");
                string syncInput = m_pipeStreamString.ReadString();
                if (syncInput == "SYNC")
                {
                    m_pipeStreamString.WriteString("OK");
                    m_isRunning = true;
                    Thread readThread = new Thread(ReaderThread);
                    readThread.IsBackground = true;
                    readThread.Start(m_pipeStreamString);
                    System.Diagnostics.Trace.TraceInformation("Synchronized");
                    return true;
                }
                else
                {
                    System.Diagnostics.Trace.TraceWarning("Unable to synchronize");
                    Disconnect();
                    return false;
                }
            }
            catch (TimeoutException)
            {
                System.Diagnostics.Trace.TraceError("Connection timeout");
                return false;
            }
        }

        private void ReaderThread(object state)
        {
            try
            {
                while (m_isRunning)
                {
                    var message = m_pipeStreamString.ReadString();
                    if (message == null)
                    {
                        Disconnect();
                        return;
                    }
                    else if (message == string.Empty) continue;
                    else
                    {
                        m_output.WriteLine(message);
                    }
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
           
        }

        public void Disconnect()
        {
            System.Diagnostics.Trace.TraceInformation("Pipe Command manager disconnected");
            m_isRunning = false;
            pipeClient.Close();
            pipeClient.Dispose();
        }

        public void ExecuteCommand(string command)
        {
            m_pipeStreamString.WriteString(command);
        }

        public void ExecuteCommand(string command, params object[] args)
        {
            StringBuilder sb = new StringBuilder(command);
            foreach(var arg in args)
            {
                sb.Append(" ");
                if(arg is string)
                {
                    sb.Append("\"" + arg + "\"");
                }
                else
                {
                    sb.Append(arg);
                }
            }
            ExecuteCommand(sb.ToString());
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private PipeStreamString m_pipeStreamString;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Disconnect();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NamedPipedCommandManagerClient() {
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
