using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.Backend
{
    public class connections : ITerminalCommand
    {
        [Dependency]
        private IPBackend _ipbackend = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        public string Description
        {
            get
            {
                return "Shows all connections to and from your system.";
            }
        }

        public string Name
        {
            get
            {
                return "connections";
            }
        }

        public IEnumerable<string> UsageStrings
        {
            get
            {
                return null;
            }
        }

        public void Run(Backend backend, ConsoleContext console, string sessionid, Dictionary<string, object> args)
        {
            var ips = _ipbackend.FetchAllIPs(_entityBackend.GetPlayerEntityId(sessionid));
            if(ips.Length == 0)
            {
                console.WriteLine("Error: you are not connected to The Peacenet.");
                return;
            }
            foreach (var ip in ips)
            {
                console.WriteLine($"Connections for {_ipbackend.GetIPString(ip.Address)}:\n\n");
                foreach(var connection in _ipbackend.GetConnectionsTo(ip.Address))
                {
                    console.WriteLine($"From {_ipbackend.GetIPString(connection.From)} to you.");
                }
                foreach (var connection in _ipbackend.GetConnectionsFrom(ip.Address))
                {
                    console.WriteLine($"From you to {_ipbackend.GetIPString(connection.To)}.");
                }
            }
        }
    }
}
