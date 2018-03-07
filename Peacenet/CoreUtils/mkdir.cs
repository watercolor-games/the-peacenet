using Plex.Engine;
using Peacenet.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.CoreUtils
{
    /// <summary>
    /// Creates a directory with the specified name in the current working directory - if it doesn't already exist.
    /// </summary>
    public class mkdir : ITerminalCommand
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
                return "mkdir";
            }
        }

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Create a directory.";
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
