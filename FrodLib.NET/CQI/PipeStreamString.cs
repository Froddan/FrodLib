using FrodLib.Streams;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    class PipeStreamString : StreamString
    {
        private readonly PipeStream m_stream;

        public PipeStreamString(PipeStream stream) : base(stream)
        {
            m_stream = stream;
        }
    }
}
