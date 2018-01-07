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
    /// <summary>
    /// Deletes a specified file or directory from the filesystem.
    /// </summary>
    public class rm : ITerminalCommand
    {
        [Dependency]
        private FileUtils _futils = null;

        [Dependency]
        private FSManager _fs = null;

        public string Description
        {
            get
            {
                return "Removes the specified file or directory from your computer.";
            }
        }

        public string Name
        {
            get
            {
                return "rm";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "[-r -f] <path>";
            }
        }

        private bool deleteRecursive(ConsoleContext console, string directory, bool force)
        {
            foreach(var dir in _fs.GetDirectories(directory))
            {
                if(_futils.GetNameFromPath(dir) == ".." || _futils.GetNameFromPath(dir) == ".")
                {
                    continue;
                }
                if (!deleteRecursive(console, dir, force))
                    return false;
                if(!force)
                {
                    console.Write($"Are you sure you want to remove {dir}? [y/N]");
                    if (console.ReadLine().ToLower() != "y")
                    {
                        console.WriteLine("rm: Operation cancelled by user.");
                        return false;
                    }
                }
            }
            foreach(var file in _fs.GetFiles(directory))
            {
                if (!force)
                {
                    console.Write($"Are you sure you want to remove {file}? [y/N]");
                    if (console.ReadLine().ToLower() != "y")
                    {
                        console.WriteLine("rm: Operation cancelled by user.");
                        return false;
                    }
                }
                _fs.Delete(file);

            }
            if (!force)
            {
                console.Write($"Are you sure you want to remove {directory}? [y/N]");
                if (console.ReadLine().ToLower() != "y")
                {
                    console.WriteLine("rm: Operation cancelled by user.");
                    return false;
                }
            }

            _fs.Delete(directory);
            return true;
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string path = arguments["<path>"].ToString();
            bool force = (bool)arguments["-f"];
            bool recursive = (bool)arguments["-r"];

            if (!path.StartsWith("/"))
                path = console.WorkingDirectory + "/" + path;

            string absolute = _futils.Resolve(path);

            if (_fs.FileExists(absolute))
            {
                if (!force)
                {
                    console.Write($"Are you sure you want to remove {path}? [y/N]");
                    if(console.ReadLine().ToLower() != "y")
                    {
                        console.WriteLine("rm: Operation cancelled by user.");
                        return;
                    }
                }
                _fs.Delete(absolute);
            }
            else if (_fs.DirectoryExists(absolute))
            {
                var dirs = _fs.GetDirectories(absolute).Where(x => x != "..").ToArray();
                var files = _fs.GetFiles(absolute);
                if(dirs.Length + files.Length > 0)
                {
                    if(!recursive)
                    {
                        console.WriteLine($"rm: omitting non-empty directory {path}");
                        return;
                    }
                }
                deleteRecursive(console, absolute, force);
            }
            else
            {
                if (!force)
                {
                    console.WriteLine($"rm: File or directory not found: {path}");
                    return;
                }
            }
        }
    }
}
