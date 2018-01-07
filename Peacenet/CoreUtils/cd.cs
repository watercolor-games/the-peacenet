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
    /// <summary>
    /// A command which changes the working directory of its <see cref="ConsoleContext"/>. 
    /// </summary>
    public class cd : ITerminalCommand
    {
        [Dependency]
        private FileUtils _futils = null;

        [Dependency]
        private FSManager _fs = null;

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "cd";
            }
        }

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Change to a new working directory.";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<directory>";
            }
        }

        /// <inheritdoc/>
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
