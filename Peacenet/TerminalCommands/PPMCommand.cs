using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Objects;

namespace Peacenet.TerminalCommands
{
    public class PPMCommand : ITerminalCommand
    {
        public string Name => "ppm";

        public string Description => "Access the Peacenet Package Manager";

        [Dependency]
        private PackageManager _ppm = null;

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "available";
                yield return "search <term>";
                yield return "install [-y] <package>";
                yield return "remove [-y] <package>";
                yield return "dependencies <package>";
                yield return "man <package>";
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string searchTerm = arguments["<term>"]?.ToString();
            string package = arguments["<package>"]?.ToString();
            bool autoYes = (bool)arguments["-y"];

            if((bool)arguments["available"])
            {
                foreach(var pkg in _ppm.AvailablePackages)
                {
                    console.WriteLine($" - {pkg.Metadata.Id}");
                }
            }
            else if((bool)arguments["man"])
            {
                var pkg = _ppm.AvailablePackages.FirstOrDefault(x => x.Metadata.Id == package);
                if(pkg==null)
                {
                    console.WriteLine("ppm: error: package not found.");
                    return;
                }
                console.WriteLine($@"{pkg.Metadata.Id}

Name:
    {pkg.Metadata.Name}

Author:
    {pkg.Metadata.Author}

Minimum skill level:
    {pkg.Metadata.MinimumSkillLevel}

Category:
    {pkg.Metadata.Category}

Summary:
    {pkg.Metadata.Description}

Dependencies:");
                if (pkg.Dependencies == null || pkg.Dependencies.Length == 0)
                    console.WriteLine("    None");
                else
                    foreach(var dep in pkg.Dependencies)
                    {
                        console.WriteLine("     - " + dep);
                    }
            }
        }
    }
}
