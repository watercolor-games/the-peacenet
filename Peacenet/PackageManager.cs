using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.Interfaces;
using Plex.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet
{
    public class PackageManager : IEngineComponent
    {
        private List<PackageData> _all = new List<PackageData>();

        private PackageData[] _available = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private GameManager _game = null;

        public void Initiate()
        {
            Logger.Log("Peacegate is looking for PPM packages...");

            foreach (var type in ReflectMan.Types.Where(x => x.Inherits(typeof(Window)) || x.GetInterfaces().Contains(typeof(ITerminalCommand))))
            {
                var packageData = type.GetCustomAttributes(false).FirstOrDefault(x => x is PackageAttribute) as PackageAttribute;
                if(packageData != null)
                {
                    bool isBase = type.GetCustomAttributes(false).Any(x => x is BasePackageAttribute);
                    string[] dependencies = type.GetCustomAttributes(false).Where(x => x is PackageDependencyAttribute).Select(x => (x as PackageDependencyAttribute).Id).ToArray();
                    var data = new PackageData(packageData, dependencies, isBase);
                    Logger.Log($"Found {data.Metadata.Id}");
                    _all.Add(data);
                }
            }

            _os.SessionStart += () =>
            {
                resetAvailable();
            };
        }

        private void resetAvailable()
        {
            _available = _all.Where(x => !_game.State.IsPackageInstalled(x.Metadata.Id) && !x.Dependencies.Any(y => !_game.State.IsPackageInstalled(y)) && (x.IsBasePackage || _game.State.SkillLevel >= x.Metadata.MinimumSkillLevel)).ToArray();
        }
    }

    public class PackageData
    {
        public PackageAttribute Metadata { get; private set; }
        public bool IsBasePackage { get; private set; }
        public string[] Dependencies { get; private set; }

        public PackageData(PackageAttribute metadata, string[] dependencies = null, bool isBase = false)
        {
            Metadata = metadata;
            Dependencies = dependencies;
            IsBasePackage = isBase;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PackageAttribute : Attribute
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MinimumSkillLevel { get; private set; }
        public string Category { get; private set; }
        public string Author { get; private set; }

        public PackageAttribute(string id, string name, string description, string category = "Other", int level = 0, string author = "Peacegate Development Team")
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            MinimumSkillLevel = level;
            Author = author;
        }
    }

    /// <summary>
    /// Marks a Peacegate package as a base package, making it and all dependencies automatically installed during the Tutorial's installation screen regardless of the minimum skill level.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BasePackageAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PackageDependencyAttribute : Attribute
    {
        public string Id { get; private set; }

        public PackageDependencyAttribute(string id)
        {
            Id = id;
        }
    }
}
