using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace Plex.Server
{
    public class Program
    {
        private static UdpClient _server = null;

        public static void Main(string[] args)
        {
            _server = new UdpClient();
            var _ipEP = new IPEndPoint(IPAddress.Any, 62252);
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Bind(new IPEndPoint(IPAddress.Loopback, 62252));
            _server.Client = sock;
            while (true)
            {
                _ipEP = new IPEndPoint(IPAddress.Any, 62252);
                var receive = _server.Receive(ref _ipEP);
                string data = Encoding.UTF8.GetString(receive);
                if(data == "heart")
                {
                    var beat = Encoding.UTF8.GetBytes("beat");
                    _server.Send(beat, beat.Length, new IPEndPoint(_ipEP.Address, _ipEP.Port));
                }
            }
        }
    }
}
