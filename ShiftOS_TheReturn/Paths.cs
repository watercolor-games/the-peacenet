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
    public static class Paths
    {
        public static void Init()
        {
            Locations = new Dictionary<string, string>();
            Locations.Add("classic", "C:\\ShiftOS");
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
            AddPath("data", "save.json");
            AddPath("data", "user.dat");
            AddPath("data", "skin");
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

        public static string[] GetAllWithoutKey()
        {
            List<string> strings = new List<string>();
            foreach (var str in Locations)
            {
                strings.Add(str.Value);
            }
            return strings.ToArray();

        }

        public static string GetPath(string id)
        {
            return Locations[id];
        }

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

        private static Dictionary<string, string> Locations { get; set; }

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
        }



        public static void ScanForDirectories(string folder, int mount)
        {
            foreach (var file in System.IO.Directory.GetFiles(folder))
            {
                string mfsDir = file.Replace(SharedFolder, $"{mount}:").Replace("\\", "/");
                WriteAllBytes(mfsDir, System.IO.File.ReadAllBytes(file));
            }
            foreach (var directory in System.IO.Directory.GetDirectories(folder))
            {
                string mfsDir = directory.Replace(SharedFolder, $"{mount}:").Replace("\\", "/");
                CreateDirectory(mfsDir);
                ScanForDirectories(directory, mount);
            }
        }

        public static string SharedFolder { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ShiftOS_Shared"; } }
        public static string SaveFile { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ShiftOS.mfs"; } }
        public static string SaveFileInner { get { return Locations["save.json"]; } }

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
