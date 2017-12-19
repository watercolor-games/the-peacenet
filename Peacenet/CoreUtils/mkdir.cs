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
    public class mkdir : ITerminalCommand
    {
        [Dependency]
        private FileUtils _futils = null;

        [Dependency]
        private FSManager _fs = null;

        public string Name
        {
            get
            {
                return "mkdir";
            }
        }

        public string Description
        {
            get
            {
                return "Create a directory.";
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
            string dir = arguments["<directory>"].ToString();
            if (!dir.StartsWith("/"))
                dir = console.WorkingDirectory + "/" + dir;
            string abs = _futils.Resolve(dir);

            if (_fs.DirectoryExists(abs))
            {
                console.WriteLine($"mkdir: Directory already exists: {abs}");
                return;
            }
            _fs.CreateDirectory(abs);
        }
    }
}
