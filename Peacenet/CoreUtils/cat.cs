using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Engine.Filesystem;

namespace Peacenet.CoreUtils
{
    public class cat : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "Print the contents of a file to the console.";
            }
        }

        public string Name
        {
            get
            {
                return "cat";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<file>";
            }
        }

        [Dependency]
        private FileUtils _fsutils = null;

        [Dependency]
        private FSManager _fs = null;

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string file = arguments["<file>"].ToString();
            if (!file.StartsWith("/"))
                file = console.WorkingDirectory + "/" + file;
            string resolved = _fsutils.Resolve(file);
            if(!_fs.FileExists(resolved))
            {
                console.WriteLine("cat: File not found.");
                return;
            }
            console.WriteLine(_fs.ReadAllText(resolved));
        }
    }
}
