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
    public class ls : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "Lists the files and directories within the current working directory.";
            }
        }

        public string Name
        {
            get
            {
                return "ls";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "[-a]";
            }
        }
        
        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private FileUtils _futils = null;

        private string shorten(string path)
        {
            return _futils.GetNameFromPath(path);
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            bool doHidden = (bool)arguments["-a"];
            if(!_fs.DirectoryExists(console.WorkingDirectory))
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Red);
                console.WriteLine($"ls: Directory not found.");
                return;
            }
            foreach(var dir in _fs.GetDirectories(console.WorkingDirectory))
            {
                if (shorten(dir).StartsWith(".") && (doHidden == false))
                    continue;
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Green);
                console.Write(shorten(dir) + "    ");
            }
            foreach (var file in _fs.GetFiles(console.WorkingDirectory))
            {
                if (shorten(file).StartsWith(".") && (doHidden == false))
                    continue;
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                console.Write(shorten(file) + "    ");
            }
            console.WriteLine("");
        }
    }
}
