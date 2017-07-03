/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ShiftOS.Objects.ShiftFS.Utils;
using ShiftOS.Objects.ShiftFS;
using Newtonsoft.Json;
using System.Threading;

namespace ShiftOS.Engine
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

            AddPath("root", "system");

            AddPath("root", "home");
            AddPath("home", "documents");
            AddPath("home", "desktop");
            AddPath("home", "pictures");


            AddPath("system", "local");
            AddPath("local", "english.local");
            AddPath("local", "deutsch.local");
            AddPath("local", "verbose.local");
            AddPath("system", "data");
            AddPath("system", "applauncher");
            AddPath("data", "save.json");
            AddPath("data", "user.dat");
            AddPath("data", "notifications.dat");
            AddPath("data", "skin");
            AddPath("skin", "widgets.dat");
            AddPath("system", "programs");
            AddPath("system", "kernel.sft");
            AddPath("system", "conf.sft");
            AddPath("skin", "current");
            AddPath("current", "skin.json");
            AddPath("current", "images");

            CheckPathExistence();

            CreateAndMountSharedFolder();
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
                return System.IO.Path.Combine(appdata, "ShiftOS", "saves");
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
                if (!path.Value.Contains(".") && path.Key != "classic")
                {
                    if (!DirectoryExists(path.Value))
                    {
                        Console.WriteLine($"Writing directory: {path.Value.Replace(Locations["root"], "\\")}");
                        CreateDirectory(path.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{string, string}"/> representing all paths in the system. 
        /// </summary>
        private static Dictionary<string, string> Locations { get; set; }

        /// <summary>
        /// Mounts the ShiftOS shared directory to 1:/, creating the directory if it does not exist.
        /// </summary>
        public static void CreateAndMountSharedFolder()
        {
            if (!System.IO.Directory.Exists(SharedFolder))
            {
                System.IO.Directory.CreateDirectory(SharedFolder);
            }

            var mount = new Directory();
            mount.Name = "Shared";
            Utils.Mount(JsonConvert.SerializeObject(mount));
            ScanForDirectories(SharedFolder, 1);
            //This event-based system allows us to sync the ramdisk from ShiftOS to the host OS.
            Utils.DirectoryCreated += (dir) =>
            {
                try
                {
                    if (dir.StartsWith("1:/"))
                    {
                        string real = dir.Replace('/', System.IO.Path.DirectorySeparatorChar).Replace("1:", SharedFolder);
                        if (!System.IO.Directory.Exists(real))
                            System.IO.Directory.CreateDirectory(real);
                    }
                }
                catch { }
            };

            Utils.DirectoryDeleted += (dir) =>
            {
                try
                {
                    if (dir.StartsWith("1:/"))
                    {
                        string real = dir.Replace('/', System.IO.Path.DirectorySeparatorChar).Replace("1:", SharedFolder);
                        
                        if (System.IO.Directory.Exists(real))
                            System.IO.Directory.Delete(real, true);
                    }
                }
                catch { }
            };

            Utils.FileWritten += (dir) =>
            {
                try
                {
                    if (dir.StartsWith("1:/"))
                    {
                        string real = dir.Replace('/', System.IO.Path.DirectorySeparatorChar).Replace("1:", SharedFolder);
                        System.IO.File.WriteAllBytes(real, ReadAllBytes(dir));
                    }
                }
                catch { }
            };

            Utils.FileDeleted += (dir) =>
            {
                try
                {
                    if (dir.StartsWith("1:/"))
                    {
                        string real = dir.Replace('/', System.IO.Path.DirectorySeparatorChar).Replace("1:", SharedFolder);
                        if (System.IO.File.Exists(real))
                            System.IO.File.Delete(real);
                    }
                }
                catch { }
            };

            //This thread will sync the ramdisk from the host OS to ShiftOS.
            var t = new Thread(() =>
            {
                while (!SaveSystem.ShuttingDown)
                {
                    Thread.Sleep(15000);
                    ScanForDirectories(SharedFolder, 1);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        private static void ScanForDirectories(string folder, int mount)
        {
            foreach (var file in System.IO.Directory.GetFiles(folder))
            {
                string mfsDir = file.Replace(SharedFolder, $"{mount}:").Replace("\\", "/");
                if (!FileExists(mfsDir))
                    WriteAllBytes(mfsDir, System.IO.File.ReadAllBytes(file));
            }
            foreach (var directory in System.IO.Directory.GetDirectories(folder))
            {
                string mfsDir = directory.Replace(SharedFolder, $"{mount}:").Replace("\\", "/");
                if(!DirectoryExists(mfsDir))
                    CreateDirectory(mfsDir);
                ScanForDirectories(directory, mount);
            }
        }

        /// <summary>
        /// Gets the ShiftOS shared folder.
        /// </summary>
        public static string SharedFolder { get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ShiftOS_Shared"); } }

        /// <summary>
        /// Gets the location of the ShiftOS.mfs file.
        /// </summary>
        public static string SaveFile { get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ShiftOS.mfs"); } }

        /// <summary>
        /// Gets the path of the inner save file.
        /// </summary>
        [Obsolete("Not used.")]
        public static string SaveFileInner { get { return Locations["save.json"]; } }

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
