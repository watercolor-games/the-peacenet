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
using Newtonsoft.Json;
using System.Collections.Generic;
using static ShiftOS.Objects.ShiftFS.Utils;
using System.Text;
using System.Threading;

namespace ShiftOS.Objects.ShiftFS
{

    public class File
    {
        public string Name;
        public byte[] Data;
        public byte[] HeaderData;
        public bool ReadAccessToLowUsers;
        public UserPermissions permissions;
        public System.IO.Stream GetStream()
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                return new System.IO.MemoryStream(Data);
            }
            else if (ReadAccessToLowUsers == true)
            {
                return new System.IO.MemoryStream(Data, false);
            }
            return null;
        }

        public File(string name, byte[] data, bool ReadAccess_to_low_users, UserPermissions perm)
        {
            Name = name;
            Data = data;
            permissions = perm;
            ReadAccessToLowUsers = ReadAccess_to_low_users;
        }
    }
    public class Directory
    {
        public string Name;
        public List<File> Files = new List<File>();
        public List<Directory> Subdirectories = new List<Directory>();
        public bool ReadAccessToLowUsers;
        public UserPermissions permissions;
        public void AddFile(File file)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                Files.Add(file);
            }
        }
        public void RemoveFile(string name)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                Files.Remove(Files.Find(x => x.Name == name));
            }
        }
        public void RemoveFile(File file)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                Files.Remove(file);
            }
        }
        public File FindFileByName(string name)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                return Files.Find(x => x.Name == name);
            }
            return null;
        }
        public void AddDirectory(Directory dir)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                Subdirectories.Add(dir);
            }
        }
        public void RemoveDirectory(string name)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                Subdirectories.Remove(Subdirectories.Find(x => x.Name == name));
            }
        }
        public void RemoveDirectory(Directory dir)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                Subdirectories.Remove(dir);
            }
        }
        public Directory FindDirectoryByName(string name)
        {
            if ((int)CurrentUser <= (int)permissions)
            {
                return Subdirectories.Find(x => x.Name == name);
            }
            return null;
        }
    }

    public static class Utils
    {
        public static UserPermissions CurrentUser { get; set; }

        public static List<Directory> Mounts { get; set; }

        static Utils()
        {
            if (Mounts == null)
                Mounts = new List<Directory>();

        }

        public static void Mount(string json)
        {
            var dir = JsonConvert.DeserializeObject<Directory>(json);
            Mounts.Add(dir);
        }

        public static void MountPersistent(string mfsFile)
        {
            var dir = JsonConvert.DeserializeObject<Directory>(ReadAllText(mfsFile));
            Mounts.Add(dir);
            string oldJson = JsonConvert.SerializeObject(dir);
            var t = new Thread(new ThreadStart(() =>
            {
                while (Mounts != null)
                {
                    if (oldJson != JsonConvert.SerializeObject(dir))
                    {
                        oldJson = JsonConvert.SerializeObject(dir);
                        WriteAllText(mfsFile, oldJson);
                    }
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        public static event Action<string> DirectoryCreated;
        public static event Action<string> DirectoryDeleted;
        public static event Action<string> FileWritten;
        public static event Action<string> FileDeleted;
        public static event Action<string> FileRead;

        public static void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
            {
                string[] pathlist = path.Split('/');
                int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
                var dir = Mounts[vol];
                for (int i = 1; i <= pathlist.Length - 2; i++)
                {
                    dir = dir.FindDirectoryByName(pathlist[i]);
                }
                dir.AddDirectory(new Directory
                {
                    Name = pathlist[pathlist.Length - 1],
                    permissions = CurrentUser,
                });
                DirectoryCreated?.Invoke(path);
            }
            else
            {
                throw new Exception("The directory \"" + path + "\" already exists.");
            }
        }

        public static byte[] ReadAllBytes(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            var file = dir.FindFileByName(pathlist[pathlist.Length - 1]);
            FileRead?.Invoke(path);
            return file.Data;

        }

        public static void WriteAllText(string path, string contents)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }

            if (!FileExists(path))
            {
                try
                {
                    dir.AddFile(new File(pathlist[pathlist.Length - 1], Encoding.UTF8.GetBytes(contents), false, CurrentUser));
                }
                catch { }
            }
            else
            {
                var f = dir.FindFileByName(pathlist[pathlist.Length - 1]);
                f.Data = Encoding.UTF8.GetBytes(contents);
            }
            FileWritten?.Invoke(path);
        }


        public static void Delete(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }

            if (FileExists(path))
            {
                dir.RemoveFile(pathlist[pathlist.Length - 1]);
                FileDeleted?.Invoke(path);
            }
            else
            {
                dir.RemoveDirectory(pathlist[pathlist.Length - 1]);
                DirectoryDeleted?.Invoke(path);
            }

        }


        public static void WriteAllBytes(string path, byte[] contents)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }

            if (!FileExists(path))
            {
                dir.AddFile(new File(pathlist[pathlist.Length - 1], contents, false, CurrentUser));
            }
            else
            {
                var f = dir.FindFileByName(pathlist[pathlist.Length - 1]);
                f.Data = contents;
            }
            FileWritten?.Invoke(path);
        }



        public static string ExportMount(int index)
        {
            var dir = Mounts[index];
            return JsonConvert.SerializeObject(dir, Formatting.Indented);
        }

        public static bool DirectoryExists(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            if (Mounts[vol] == null)
                Mounts[vol] = new Directory();
            var dir = Mounts[vol];

            for (int i = 1; i <= pathlist.Length - 1; i++)
            {
                dir = dir?.FindDirectoryByName(pathlist[i]);
            }
            return dir != null;

        }

        public static bool FileExists(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            return dir.FindFileByName(pathlist[pathlist.Length - 1]) != null;

        }

        public static Directory GetDirectoryInfo(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 1; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1, 1);
            return dir;
        }

        public static string ReadAllText(string path)
        {
            return Encoding.UTF8.GetString(ReadAllBytes(path));
        }

    

    public static File GetFileInfo(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            return dir.FindFileByName(pathlist[pathlist.Length - 1]);

        }

        public static byte[] GetHeaderData(string filePath)
        {
            return GetFileInfo(filePath).HeaderData;
        }

        public static string GetHeaderText(string filePath)
        {
            byte[] header = GetHeaderData(filePath);
            return (header == null) ? "" : Encoding.UTF8.GetString(header);
        }

        public static void SetHeaderData(string filePath, byte[] data)
        {
            GetFileInfo(filePath).HeaderData = data;
        }

        public static void SetHeaderText(string filePath, string text)
        {
            SetHeaderData(filePath, Encoding.UTF8.GetBytes(text));
        }

        public static string[] GetDirectories(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for(int i = 1; i <= pathlist.Length - 1; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1, 1);

            List<string> paths = new List<string>();

            foreach(var subdir in dir.Subdirectories)
            {
                paths.Add(path + "/" + subdir.Name);
            }
            paths.Sort();
            return paths.ToArray();
        }

        /// <summary>
        /// Copies a file or directory from one path to another, deleting the original.
        /// </summary>
        /// <param name="path">THe input path, must be a valid directory or file.</param>
        /// <param name="target">The output path.</param>
        public static void Move(string path, string target)
        {
            if (FileExists(path))
            {
                WriteAllBytes(target, ReadAllBytes(path));
                Delete(path);
            }
            else if (DirectoryExists(path))
            {
                if (!DirectoryExists(target))
                    CreateDirectory(target);
                foreach (var file in GetFiles(path))
                {
                    var name = GetFileInfo(file).Name;
                    Copy(file, target + "/" + name);
                }
                foreach (var dir in GetDirectories(path))
                {
                    string name = GetDirectoryInfo(dir).Name;
                    Copy(dir, target + "/" + name);
                }
                Delete(path);
            }
        }


        /// <summary>
        /// Copies a file or directory from one path to another.
        /// </summary>
        /// <param name="path">The input path, must be a valid directory or file.</param>
        /// <param name="target">The output path.</param>
        public static void Copy(string path, string target)
        {
            if (FileExists(path))
                WriteAllBytes(target, ReadAllBytes(path));
            else if (DirectoryExists(path))
            {
                if (!DirectoryExists(target))
                    CreateDirectory(target);
                foreach(var file in GetFiles(path))
                {
                    var name = GetFileInfo(file).Name;
                    Copy(file, target + "/" + name);
                }
                foreach(var dir in GetDirectories(path))
                {
                    string name = GetDirectoryInfo(dir).Name;
                    Copy(dir, target + "/" + name);
                }
            }
        }

        public static string[] GetFiles(string path)
        {
            string[] pathlist = path.Split('/');
            int vol = Convert.ToInt32(pathlist[0].Replace(":", ""));
            var dir = Mounts[vol];
            for (int i = 1; i <= pathlist.Length - 1; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1, 1);

            List<string> paths = new List<string>();

            foreach (var subdir in dir.Files)
            {
                paths.Add(path + "/" + subdir.Name);
            }
            paths.Sort();
            return paths.ToArray();
        }

        public static void WriteAllText(string v, object p)
        {
            throw new NotImplementedException();
        }
    }
}
