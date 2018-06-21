using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Engine;
using Peacenet.Applications;

namespace Peacenet.TerminalCommands
{
    public class connect : ITerminalCommand
    {
        public string Name => "connect";

        public string Description => "Connect to a remote system within The Peacenet.";

        [Dependency]
        private GameManager _game = null;

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<host>";
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string host = arguments["<host>"].ToString();

            if(!_game.State.StartConnection(host))
            {
                console.WriteLine("connect: Hostname or IP address could not be resolved.");
                return;
            }

            var connection = _game.State.ActiveConnections.FirstOrDefault(x => x.Sentience.Hostname == host || x.Sentience.IPAddress.ToIPv4String() == host);

            int tries = 4;
            string user = "";
            string pass = "";
            while (tries > 0 && !connection.Authenticated)
            {
                console.Write("login as: ");
                user = console.ReadLine();
                console.Write("password: ");
                pass = console.ReadLine();
                if(!connection.Authenticate(user, pass))
                {
                    tries--;
                    console.WriteLine("Permission denied.");
                    if (tries > 0)
                        console.WriteLine($"Try again. [tries left: {tries}]");
                }
            }

            if(!connection.Authenticated)
            {
                _game.State.EndConnection(host);
                console.WriteLine("Disconnected.");
                return;
            }
        }
    }
}
