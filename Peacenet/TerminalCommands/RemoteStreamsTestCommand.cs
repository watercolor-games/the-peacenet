using System;
using System.Collections.Generic;
using Plex.Engine;
using Plex.Objects;
using Peacenet.Server;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Peacenet
{
    /// <summary>
    /// Client side part of the remote streams test
    /// </summary>
    public class RemoteStreamsTestCommand : ITerminalCommand
    {
        public string Name => "rstest";
        public string Description => "Should output some epic death grips lyrics :sunglasses:";
        public IEnumerable<string> Usages => null;

        [Dependency]
        AsyncServerManager man;

        [Dependency]
        RemoteStreams rstreams;

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            int id = -1;
            Task.WaitAll(man.SendMessage(ServerMessageType.STREAM_TEST, new byte[] { }, (resp, read) => id = read.ReadInt32()));
            console.WriteLine("That's the message that I sent.  (DAVID BOWIE IS NOT MC RIDE)");
            using (var fobj = rstreams.Open(id))
            {
                console.WriteLine("Remote Stream opened.");
                console.WriteLine($"CanRead = {fobj.CanRead}");
                console.WriteLine($"CanWrite = {fobj.CanWrite}");
                console.WriteLine($"CanSeek = {fobj.CanSeek}");
                console.WriteLine($"Buffered = {fobj is BufferedStream}");
                console.WriteLine($"Length = {fobj.Length}");
                using (var read = new BinaryReader(fobj, Encoding.UTF8, true))
                {
                    console.WriteLine(Encoding.UTF8.GetString(read.ReadBytes((int)fobj.Length)));
                    fobj.Seek(0, SeekOrigin.Begin);
                    console.WriteLine(Encoding.UTF8.GetString(read.ReadBytes((int)fobj.Length).Reverse().ToArray()));
                }
            }
        }
    }
}
