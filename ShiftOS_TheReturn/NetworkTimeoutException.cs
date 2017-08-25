using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Plex.Engine
{
    public class NetworkTimeoutException : Exception
    {
        public NetworkTimeoutException(IPAddress address, int port) : base($"Could not establish a proper connection with {address.ToString()}:{port} - connection timed out.")
        {
            Address = address;
            Port = port;
        }

        public IPAddress Address { get; private set; }
        public int Port { get; private set; }
    }
}
