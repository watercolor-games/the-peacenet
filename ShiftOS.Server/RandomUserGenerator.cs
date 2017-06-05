using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ShiftOS.Objects;
using static ShiftOS.Objects.ShiftFS.Utils;

namespace ShiftOS.Server
{
    public static class RandomUserGenerator
    {
        const string USERPREFIXES = "culled;purged;anon;fatal;unaccounted;netban;killed;old";
        const string SYSNAMES = "unknown;error;dead;system;mud";
        const string PASSCHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_";

        public static void StartThread()
        {
            var t = new Thread(() =>
            {
                var rnd = new Random();
                while (Program.server.IsOnline)
                {
                    if (!Directory.Exists("deadsaves"))
                    {
                        Directory.CreateDirectory("deadsaves");
                    }

                    var sve = new Save();

                    int passLength = 0;
                    int securityGrade = rnd.Next(1, 5);
                    switch (securityGrade)
                    {
                        default:
                            passLength = 4;
                            break;
                        case 2:
                            passLength = 6;
                            break;
                        case 3:
                            passLength = 10;
                            break;
                        case 4:
                            passLength = 15;
                            break;
                    }

                    string pass = "";
                    char lastChar = '\0';
                    for (int i = 0; i < passLength; i++)
                    {
                        char c = PASSCHARS[rnd.Next(0, PASSCHARS.Length)];
                        while(c == lastChar)
                        {
                            c = PASSCHARS[rnd.Next(0, PASSCHARS.Length)];
                        }
                        pass += c;
                        lastChar = c; //this ensures no repeated sequences.
                    }

                    sve.Password = pass;

                    int id_length = rnd.Next(3, 10);
                    string id = "";

                    lastChar = '\0';
                    for (int i = 0; i < id_length; i++)
                    {
                        char c = PASSCHARS[rnd.Next(0, PASSCHARS.Length)];
                        while (c == lastChar)
                        {
                            c = PASSCHARS[rnd.Next(0, PASSCHARS.Length)];
                        }
                        id += c;
                        lastChar = c; //this ensures no repeated sequences.
                    }

                    string[] names = USERPREFIXES.Split(';');
                    string name = names[rnd.Next(0, names.Length)];
                    sve.Username = $"{id}_{name}";

                    names = SYSNAMES.Split(';');
                    name = names[rnd.Next(0, names.Length)];

                    sve.SystemName = name;

                    //Codepoint generation.
                    int startCP = 0;
                    int maxAmt = 0;

                    switch (securityGrade)
                    {
                        default:
                            startCP = 1000;
                            maxAmt = 12500;
                            break;
                        case 2:
                            startCP = 25000;
                            maxAmt = 50000; 
                            break;
                        case 3:
                            startCP = 75000;
                            maxAmt = 150000;
                            break;
                        case 4:
                            startCP = 500000;
                            maxAmt = 1500000;
                            break;
                    }

                    sve.Codepoints = (ulong)rnd.Next(startCP, maxAmt);

                    //FS treasure generation.
                    /*
                    //create a ramdisk dir
                    var dir = new ShiftOS.Objects.ShiftFS.Directory();
                    //name the directory after the user
                    dir.Name = sve.Username;
                    dir.permissions = Objects.ShiftFS.Permissions.All;
                    //json the object and mount
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dir);
                    //mount it to the MUD
                    ShiftOS.Objects.ShiftFS.Utils.Mount(json);
                    //get the mount id
                    int mountid = ShiftOS.Objects.ShiftFS.Utils.Mounts.Count - 1;

                    bool leakShiftnetDataRandomly = (rnd.Next(0, 10) > 5);

                    //create home directory
                    CreateDirectory($"{mountid}:/home");
                    //create downloads directory
                    CreateDirectory($"{mountid}:/home/downloads");
                    //if we're leaking shiftnet data...
                    CreateDirectory($"{mountid}:/home/documents");

                    //alright, let's leak some shop items.
                    foreach(var shop in Newtonsoft.Json.JsonConvert.DeserializeObject<Shop[]>(File.ReadAllText("shops.json")))
                    {
                        if(shop != null)
                        {
                            try
                            {
                                foreach(var item in shop.Items)
                                {
                                    if(rnd.Next(0,10) > 5)
                                    {
                                        if (sve.Codepoints >= item.Cost)
                                        {
                                            //deduct item's codepoints.
                                            sve.Codepoints -= item.Cost;

                                            //create a new file in user's downloads folder...with the item's name and binary contents inside.
                                            WriteAllBytes($"{mountid}:/home/downloads/{item.Name}.{GetFileExt(item.FileType)}", item.MUDFile);
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    //shiftnetData<ntfsFile, sfsFile>
                    Dictionary<string, string> shiftnetData = new Dictionary<string, string>(); 

                    if(leakShiftnetDataRandomly == true)
                    {
                        //And maybe let's leak some shiftnet sites.
                        LeakDirectories($"{mountid}:/home/documents", out shiftnetData);
                        
                        //Now start saving the directories.
                        foreach(var kv in shiftnetData)
                        {
                            if(!DirectoryExists(kv.Value))
                                CreateDirectory(kv.Value);

                            foreach(var file in Directory.GetFiles(kv.Key))
                            {
                                WriteAllBytes(kv.Value, File.ReadAllBytes(file));
                            }
                        }
                    }*/

                    //save the save file to disk.
                    File.WriteAllText("deadsaves/" + sve.Username + ".save", Newtonsoft.Json.JsonConvert.SerializeObject(sve, Newtonsoft.Json.Formatting.Indented));
                    //We don't care about the encryption algorithm because these saves can't be logged into as regular users.

                    /*
                    //Now we export the mount.
                    string exportedMount = ExportMount(mountid);
                    //And save it to disk.
                    File.WriteAllText("deadsaves/" + sve.Username + ".mfs", exportedMount);
                    */


                    Thread.Sleep((60 * 60) * 1000); //approx. 1 hour.

                }
            });
            t.IsBackground = true;
            t.Start();
        }

        public static void LeakDirectories(string output, out Dictionary<string,string> targets)
        {
            var rnd = new Random();
            List<string> dirs = new List<string>();
            foreach(var pth in getDirectories("shiftnet"))
            {
                if(rnd.Next(0,10) > 5)
                {
                    dirs.Add(pth);
                }
            }

            targets = new Dictionary<string, string>();
            foreach(var dir in dirs)
            {
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    string sDir = dir.Replace("\\", "/");
                    if (sDir.Contains("shiftnet"))
                    {
                        while (!sDir.StartsWith("shiftnet"))
                        {
                            sDir = sDir.Remove(0, 1);
                        }
                        targets.Add(dir, output + "/" + sDir);
                    }
                }
            }
              
        }

        private static List<string> getDirectories(string path)
        {
            List<string> paths = new List<string>();
            foreach(var pth in Directory.GetDirectories(path))
            {
                paths.AddRange(getDirectories(pth).ToArray());
            }
            paths.Add(path);
            return paths;
        }

        public static string GetFileExt(int fType)
        {
            switch((FileType)fType)
            {
                default:
                    return "bin";
                case FileType.Executable:
                    return "sft";
                case FileType.Filesystem:
                    return "mfs";
                case FileType.Image:
                    return "pic";
                case FileType.JSON:
                    return "json";
                case FileType.Lua:
                    return "lua";
                case FileType.Skin:
                    return "skn";
                case FileType.TextFile:
                    return ".txt";
            }
        }

        public enum FileType
        {
            TextFile,
            Directory,
            Mount,
            UpOne,
            Image,
            Skin,
            JSON,
            Executable,
            Lua,
            Python,
            Filesystem,
            Unknown
        }
    }
}
