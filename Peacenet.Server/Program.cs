#define MILESTONE4

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections;
using WatercolorGames.CommandLine;

namespace Plex.Server
{
    class Program
    {
        private static Backend _backend = null;

        internal static bool IsMultiplayerServer = true;

        private static int _port = 3251;

        static void Main(string[] args)
        {
#if MILESTONE4
            using(var server = new Peacenet.Backend.Server(3251, 10))
            {
                server.Listen();
            }
#else
            _backend = new Backend(_port, IsMultiplayerServer);
            _backend.Listen();
            if (IsMultiplayerServer)
            {
                Console.ReadKey();
                _backend.Shutdown();
            }
#endif
        }
    }
}
