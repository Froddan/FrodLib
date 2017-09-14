using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Communications
{
    [ExcludeFromCodeCoverage]
    internal class TCPConnection : IDisposable
    {

        private Socket socket;
        private TcpClient client;

        private NetworkStream stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private const string Starting = "STARTING_SENDING_OBJECT";
        private const string Finished = "FINISHED_SENDING_OBJECT";

        private readonly object writeLockObject = new object();
        private readonly object readLockObject = new object();
        private static readonly object cacheLockObject = new object();
        private static readonly Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

        public TCPConnection(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
            InitializeReaderAndWriter(client.GetStream());
        }

        public TCPConnection(Socket socket)
        {
            InitializeReaderAndWriter(new NetworkStream(socket));
            this.socket = socket;
        }

        protected StreamReader StreamReader
        {
            get
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("StreamReader has not been initalized. Please call 'InitializeReaderAndWriter' method from sub class");
                }
                return _reader;
            }
            private set
            {
                _reader = value;
            }
        }

        protected StreamWriter StreamWriter
        {
            get
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("StreamReader has not been initalized. Please call 'InitializeReaderAndWriter' method from sub class");
                }
                return _writer;
            }
            private set
            {
                _writer = value;
            }
        }

        protected bool IsInitialized { get; private set; }

        protected void InitializeReaderAndWriter(NetworkStream stream)
        {
            this.stream = stream;
            StreamReader = new StreamReader(stream);
            StreamWriter = new StreamWriter(stream);
            IsInitialized = true;

            StreamWriter.AutoFlush = true;

        }

        public void Send()
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine();
            }

        }

        public void Send(bool value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(char value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(char[] buffer)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(buffer);
            }
        }

        public void Send(decimal value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(double value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(float value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(int value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(long value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public virtual void Send(object value)
        {
            if (value == null) return;
            lock (writeLockObject)
            {
                Type valueType = value.GetType();
                XmlSerializer serializer = new XmlSerializer(valueType);
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, value);
                string xmlObject = writer.ToString();

                StreamWriter.WriteLine(Starting);
                StreamWriter.WriteLine(valueType.FullName);
                StreamWriter.WriteLine(xmlObject);
                StreamWriter.WriteLine(Finished);
            }
        }

        public void Send(string value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(uint value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }

        public void Send(ulong value)
        {
            lock (writeLockObject)
            {
                StreamWriter.WriteLine(value);
            }
        }


        public string Receive()
        {
            lock (readLockObject)
            {
                return StreamReader.ReadLine();
            }
        }

        public bool IsClient
        {
            get
            {
                return client != null;
            }
        }

        public bool IsServer
        {
            get
            {
                return socket != null;
            }
        }

        public TcpClient Client
        {
            get
            {
                return client;
            }
        }

        public Socket Socket
        {
            get
            {
                return this.socket;
            }
        }

        public NetworkStream Stream
        {
            get
            {
                return stream;
            }
        }

        public virtual object ReceiveObject()
        {
            lock (readLockObject)
            {
                string recvString = StreamReader.ReadLine();
                if (recvString != Starting)
                {
                    return null;
                }
                var typeName = StreamReader.ReadLine();
                var type = FindType(typeName);

                if (type != null)
                {

                    StringBuilder sb = new StringBuilder();
                    while ((recvString = StreamReader.ReadLine()) != Finished)
                    {
                        recvString = recvString.Trim();
                        recvString.Replace("\0", "");
                        sb.Append(recvString);
                    }
                    string objectXML = sb.ToString();
                    XmlSerializer serializer = new XmlSerializer(type);
                    StringReader stringReader = new StringReader(objectXML);
                    XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
                    var obj = serializer.Deserialize(xmlTextReader);
                    return obj;
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual T Receive<T>()
        {
            object recivedObject = ReceiveObject();
            if (recivedObject != null)
            {
                return (T)recivedObject;
            }
            else
            {
                return default(T);
            }
        }

        public bool HasDataToRead
        {
            get
            {
                try
                {
                    return stream.DataAvailable;
                }
                catch
                {
                    return false;
                }
            }
        }

        public virtual void Close()
        {
            StreamReader.Close();
            StreamWriter.Close();

            if (socket != null)
            {
                socket.Close();
            }
            if (client != null)
            {
                client.Close();
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        private static Type FindType(string typename)
        {
            lock (cacheLockObject)
            {
                if (_cachedTypes.ContainsKey(typename))
                {
                    return _cachedTypes[typename];
                }
            }
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var foundType = asm.GetType(typename);
                if (foundType != null)
                {
                    lock (cacheLockObject)
                    {
                        if (!_cachedTypes.ContainsKey(typename))
                        {
                            _cachedTypes.Add(typename, foundType);
                        }
                    }
                    return foundType;
                }
            }
            return null;
        }

        private bool isDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {

                if (disposing)
                {
                    _reader.Close();
                    _reader.Dispose();

                    _writer.Close();
                    _writer.Dispose();

                    if (socket != null)
                    {
                        socket.Close();
                        socket.Dispose();
                    }

                    if (client != null)
                    {
                        client.Close();
                    }
                }
            }
            // Code to dispose unmanaged resources 
            // held by the class
            isDisposed = true;
            Dispose(disposing);
        }
        ~TCPConnection()
        {
            Dispose(false);
        }

    }
}
