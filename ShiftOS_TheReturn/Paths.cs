using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plex.Objects.ShiftFS;
using Newtonsoft.Json;
using System.Threading;

namespace Plex.Engine
{
    /// <summary>
    /// Management class for ShiftFS path variables.
    /// </summary>
    public static class Paths
    {
        /// <summary>
        /// Initiate the path system.
        /// </summary>
        public static void Init()
        {
                Locations = new Dictionary<string, string>();
                Locations.Add("root", "0:");

            AddPath("root", "etc");
            AddPath("root", "usr");

            AddPath("root", "home");
            AddPath("home", "documents");
            AddPath("home", "desktop");
            AddPath("home", "pictures");


            AddPath("usr", "local");
            AddPath("local", "english.local");
            AddPath("local", "deutsch.local");
            AddPath("local", "verbose.local");
            AddPath("etc", "plexgate");
            AddPath("plexgate", "launcheritems");
            AddPath("plexgate", "sentience.plex");
            AddPath("home", "user.dat");
            AddPath("plexgate", "notifications.plex");
            AddPath("plexgate", "ui");
            AddPath("ui", "widgets.dat");
            AddPath("root", "bin");
            AddPath("root", "boot");
            AddPath("boot", "plexkrnl.so");
            AddPath("etc", "conf.plex");
            AddPath("ui", "current");
            AddPath("current", "themedata.plex");
            AddPath("current", "images");

            CheckPathExistence();

        }

        /// <summary>
        /// Returns all paths in an array of strings.
        /// </summary>
        /// <returns>The array</returns>
        public static string[] GetAll()
        {
            List<string> strings = new List<string>();
            foreach(var str in Locations)
            {
                strings.Add(str.Key + " = " + str.Value);
            }
            return strings.ToArray();

        }

        public static string SaveDirectory
        {
            get
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return System.IO.Path.Combine(appdata, "Plex", "saves");
            }
        }

        /// <summary>
        /// Gets all full paths without their keynames.
        /// </summary>
        /// <returns>A string array representing all paths.</returns>
        public static string[] GetAllWithoutKey()
        {
            List<string> strings = new List<string>();
            foreach (var str in Locations)
            {
                strings.Add(str.Value);
            }
            return strings.ToArray();

        }

        /// <summary>
        /// Get the full path using a path key.
        /// </summary>
        /// <param name="id">The path key (folder/filename) for the path.</param>
        /// <returns>The full path.</returns>
        public static string GetPath(string id)
        {
            return Locations[id];
        }

        /// <summary>
        /// Checks all directories in the path system to see if they exist, and if not, creates them.
        /// </summary>
        private static void CheckPathExistence()
        {
            foreach(var path in Locations)
            {
                if (!path.Value.Contains(".") && path.Key != "classic" && !path.Value.EndsWith(":"))
                {
                    if (!FSUtils.DirectoryExists(path.Value))
                    {
                        Console.WriteLine($"Writing directory: {path.Value.Replace(Locations["root"], "\\")}");
                        FSUtils.CreateDirectory(path.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{string, string}"/> representing all paths in the system. 
        /// </summary>
        private static Dictionary<string, string> Locations { get; set; }

        /// <summary>
        /// Add a path to the system.
        /// </summary>
        /// <param name="parent">The path's parent directory.</param>
        /// <param name="path">The filename for the path.</param>
        public static void AddPath(string parent, string path)
        {
            Locations.Add(path, Locations[parent] + "/" + path);
        }

        public static string Translate(string path)
        {
            return Locations["root"] + path.Replace("\\", "/");
        }
    }
}
