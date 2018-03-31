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
using Peacenet.Backend;

namespace Plex.Server
{
    class Program
    {
        private static Backend _backend = null;

        internal static bool IsMultiplayerServer = true;

        private static int _port = 3251;

        static void Main(string[] args)
        {
            _backend = new Backend(_port, IsMultiplayerServer);
            _backend.Listen();
            if (IsMultiplayerServer)
            {
                Console.ReadKey();
                _backend.Shutdown();
            }
        }
    }
}
