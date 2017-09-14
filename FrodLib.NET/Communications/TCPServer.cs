using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Communications
{
    [ExcludeFromCodeCoverage]
    internal class TCPServer : IDisposable
    {
        private TcpListener listener;

        public event EventHandler HasPendingConnection;
        public event EventHandler ClosningTCPServerListener;

        private Thread backgroundThread;
        private bool listenerIsRunning;
        private readonly object lockObject = new object();
        private IPAddress address;
        private int port;

        public bool IsRunning
        {
            get
            {
                return listenerIsRunning;
            }
        }

        public TCPServer(IPAddress address, int port)
        {
            this.address = address;
            this.port = port;
        }

        /// <summary>
        /// Starts the server if it isn't already running
        /// </summary>
        public void StartServer()
        {
            lock (lockObject)
            {
                if (!listenerIsRunning)
                {
                    
                    listener = new TcpListener(address, port);
                    listener.Start();
                    listenerIsRunning = true;
                    backgroundThread = new Thread(() =>
                    {
                        while (listenerIsRunning)
                        {
                            if (listener.Pending())
                            {
                                var handler = HasPendingConnection;
                                if (handler != null)
                                {
                                    handler(this, EventArgs.Empty);
                                }
                            }
                            Thread.Sleep(100);
                        }
                    });
                    backgroundThread.IsBackground = true;
                    backgroundThread.Start();
                }
                else
                {
                    throw new InvalidOperationException("Server is already running");
                }
            }
        }

        /// <summary>
        /// Returns the pending connection or null if no connection is pending
        /// </summary>
        /// <returns></returns>
        public TCPConnection AccecptConnection()
        {
            lock(lockObject)
            {
                if (listener.Pending())
                {
                    Socket socket = listener.AcceptSocket();
                    return new TCPConnection(socket);
                }
                else
                {
                    return null;
                }
            }

        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void StopServer()
        {
            lock (lockObject)
            {
                if (listenerIsRunning)
                {
                    listenerIsRunning = false;
                    listener.Stop();
                    var handler = ClosningTCPServerListener;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                    backgroundThread = null;
                }
            }
        }

        /// <summary>
        /// Restarts the server
        /// </summary>
        public void RestartServer()
        {
            lock (lockObject)
            {
                StopServer();
                StartServer();
            }
        }

        void IDisposable.Dispose()
        {
            StopServer();
        }
    }
}
