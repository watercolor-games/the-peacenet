using Plex.Engine;
using Plex.Engine.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.CoreUtils
{
    public class cd : ITerminalCommand
    {
        [Dependency]
        private FileUtils _futils = null;

        [Dependency]
        private FSManager _fs = null;

        public string Name
        {
            get
            {
                return "cd";
            }
        }

        public string Description
        {
            get
            {
                return "Change to a new working directory.";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<directory>";
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string directory = arguments["<directory>"].ToString();
            if (!directory.StartsWith("/"))
                directory = console.WorkingDirectory + "/" + directory;
            string resolved = _futils.Resolve(directory);
            if(!_fs.DirectoryExists(resolved))
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Red);
                console.WriteLine($"cd: {resolved}: Directory not found");
                return;
            }
            console.WorkingDirectory = resolved;
        }
    }
}
