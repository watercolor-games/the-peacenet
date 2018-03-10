using System;
using System.Collections.Generic;
using Plex.Objects;
using System.IO;
using Plex.Engine;
using Peacenet.Filesystem;

namespace Peacenet.TerminalCommands
{
#if DEBUG
    public class ImportCommand : ITerminalCommand
    {
        [Dependency]
        FSManager fs;

        public string Description => "Debug only: Imports a file from the real FS to the current directory";
        public string Name => "import";

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<path>";
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            var path = arguments["<path>"] as string;
            using (var ifobj = File.OpenRead(path))
            using (var ofobj = fs.OpenWrite(console.WorkingDirectory + path.Substring(path.LastIndexOf('/'))))
                ifobj.CopyTo(ofobj);
        }
    }
#endif
}
