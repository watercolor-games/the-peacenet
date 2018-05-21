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
    [Package("ppm", "Peacenet Package Manager", "Provides access to the Peacenet package repositories.", "Base")]
    [BasePackage]
    public class PackageManager : IEngineComponent
    {
        private List<PackageData> _all = new List<PackageData>();

        private PackageData[] _available = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private GameManager _game = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public IEnumerable<PackageData> AvailablePackages
        {
            get
            {
                return _all.Where(x => x.Metadata.MinimumSkillLevel <= _game.State.SkillLevel);
            }
        }

        public PackageData[] BasePackages => _all.Where(x => x.IsBasePackage).ToArray();

        public bool ArePackagesInstalled(Type type)
        {
            bool metaInstalled = true;
            var meta = type.GetCustomAttributes(false).FirstOrDefault(x => x is PackageAttribute) as PackageAttribute;
            if (meta == null)
                metaInstalled = true;
            if (meta != null && !_game.State.IsPackageInstalled(meta.Id))
                metaInstalled = false;

            if (type.GetCustomAttributes(false).Any(x => x is BasePackageAttribute))
                return true;

            if (type.GetCustomAttributes(false).Any(x => x is PackageDependencyAttribute && !_game.State.IsPackageInstalled((x as PackageDependencyAttribute).Id)))
                return false;

            if (_plexgate.GetDependencyTypes(type).Any(x => !ArePackagesInstalled(x)))
                return false;

            return metaInstalled;
        }

        public void Initiate()
        {
            Logger.Log("Peacegate is looking for PPM packages...");

            foreach (var type in ReflectMan.Types)
            {
                var packageData = type.GetCustomAttributes(false).FirstOrDefault(x => x is PackageAttribute) as PackageAttribute;
                if(packageData != null)
                {
                    bool isBase = type.GetCustomAttributes(false).Any(x => x is BasePackageAttribute);
                    List<string> dependencies = type.GetCustomAttributes(false).Where(x => x is PackageDependencyAttribute).Select(x => (x as PackageDependencyAttribute).Id).ToList();
                    
                    foreach(var subtype in _plexgate.GetDependencyTypes(type))
                    {
                        var subData = subtype.GetCustomAttributes(false).FirstOrDefault(x => x is PackageAttribute) as PackageAttribute;
                        if(subData != null)
                        {
                            if (!dependencies.Contains(subData.Id))
                                dependencies.Add(subData.Id);
                        }
                    }
                    var data = new PackageData(packageData, dependencies.ToArray(), isBase);
                    Logger.Log($"Found {data.Metadata.Id}");
                    _all.Add(data);
                }
            }
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
