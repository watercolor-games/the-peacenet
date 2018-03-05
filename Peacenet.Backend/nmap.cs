using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.Backend
{
    public class nmap : ITerminalCommand
    {
        [Dependency]
        private IPBackend _ip = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        public string Description
        {
            get
            {
                return "Scan the specified IP address for open and exploitable ports.";
            }
        }

        public string Name
        {
            get
            {
                return "nmap";
            }
        }

        public IEnumerable<string> UsageStrings
        {
            get
            {
                yield return "<ipaddr>";
            }
        }

        public void Run(Backend backend, ConsoleContext console, string sessionid, Dictionary<string, object> args)
        {
            string ip = args["<ipaddr>"].ToString();

            var entity = _ip.GrabEntity(_ip.GetIPFromString(ip));
            if (entity == null)
            {
                console.WriteLine("nmap: error: IP address non-existent.");
                return;
            }

            console.WriteLine($"Port information for {ip}:");
            foreach(var port in _entityBackend.GetPorts(entity))
            {
                console.WriteLine($"{port.Port}: {Enum.GetName(typeof(Service), (int)port.Port)}, security level: {port.SecurityLevel}");
            }
        }
    }
}
