using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Peacenet.Filesystem;

namespace Peacenet.CoreUtils
{
    /// <summary>
    /// Contains core command-line utilities for the in-game Terminal.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc { }

    /// <summary>
    /// Provides a command capable of reading text from a file and displaying it in the console.
    /// </summary>
    public class cat : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Print the contents of a file to the console.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "cat";
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
