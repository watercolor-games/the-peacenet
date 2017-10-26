using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;
using Plex.Objects.ShiftFS;

namespace Plex.Engine
{
    public static class FSUtils
    { 
        private static MountInformation[] _mountinfo = null;
        private static byte[] _readbytes_data = null;
        private static string _fs_result = null;
        private static bool? _fs_exists_result = null;
        private static string[] _filelist_result = null;
        private static FileRecord _fr_result = null;
        private static string _wstrguid = null;

        private static PathData _createPathData(string path)
        {
            int drivenum = -1;
            if(!int.TryParse(path.Substring(0, path.IndexOf(":")), out drivenum))
            {
                throw new System.IO.IOException($"Invalid pathspec {path}. Plex paths follow the pathspec 'drivenum:/path/to/directory/or/file.txt'.");
            }
            string dpath = path.Substring(path.IndexOf(":"));
            return new PathData
            {
                DriveNumber = drivenum,
                Path = dpath
            };
        }

        public static void CreateDirectory(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_CREATEDIR))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                if (result.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        throw new IOException(reader.ReadString());
                    }
                }

            }
        }

        public static FileRecord GetFileInfo(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_RECORDINFO))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    if (result.Message == 0x00)
                    {
                        return JsonConvert.DeserializeObject<FileRecord>(reader.ReadString());
                    }
                    else
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }
        }

        public static void CreateMountIfNotExists()
        {
            using(var sstr = new ServerStream(ServerMessageType.FS_CREATEMOUNT))
            {
                sstr.Write(JsonConvert.SerializeObject(new
                {
                    volume = 0,
                    label = "System",
                }));
                var result = sstr.Send();
                if(result.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using(var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }
        }

        public static byte[] ReadAllBytes(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_READFROMFILE))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    if (result.Message == 0x00)
                    {
                        int len = reader.ReadInt32();
                        return reader.ReadBytes(len);
                    }
                    else
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }
        }

        public static bool IsMountpoint(string path)
        {
            return path.Contains("/") == false && path.EndsWith(":");
        }

        public static void WriteAllText(string path, string contents)
        {
            WriteAllBytes(path, Encoding.UTF8.GetBytes(contents));
        }

        public static MountInformation[] GetMounts()
        {
            using (var w = new ServerStream(ServerMessageType.FS_GETMOUNTS))
            {
                var result = w.Send();
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    if (result.Message == 0x00)
                    {
                        int len = reader.ReadInt32();
                        MountInformation[] mounts = new MountInformation[len];
                        for (int i = 0; i < len; i++)
                        {
                            string label = reader.ReadString();
                            int num = reader.ReadInt32();
                            mounts[i] = new MountInformation
                            {
                                DriveNumber = num,
                                VolumeLabel = label
                            };
                        }
                        return mounts;
                    }
                    else
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }
        }

        public static void Delete(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_DELETE))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                if (result.Message != 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }


        public static void WriteAllBytes(string path, byte[] contents)
        {
            if (contents == null)
                contents = new byte[] { };
            using (var w = new ServerStream(ServerMessageType.FS_WRITETOFILE))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                w.Write(contents.Length);
                w.Write(contents);
                var result = w.Send();
                if (result.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }

        }

        public static bool DirectoryExists(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_DIREXISTS))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    if (result.Message == 0x00)
                    {
                        return reader.ReadByte() == 1;
                    }
                    return false;
                }
            }

        }

        public static bool FileExists(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_FILEEXISTS))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    if (result.Message == 0x00)
                    {
                        return reader.ReadByte() == 1;
                    }
                    return false;
                }
            }

        }

        public static string ReadAllText(string path)
        {
            return Encoding.UTF8.GetString(ReadAllBytes(path));
        }

        public static string[] GetDirectories(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_GETDIRS))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    if (result.Message == 0x00)
                    {
                        int len = reader.ReadInt32();
                        string[] dirs = new string[len];
                        for (int i = 0; i < len; i++)
                        {
                            dirs[i] = reader.ReadString();
                        }
                        return dirs;

                    }
                    else
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }
        }

        public static string[] GetFiles(string path)
        {
            using (var w = new ServerStream(ServerMessageType.FS_GETFILES))
            {
                w.Write(JsonConvert.SerializeObject(_createPathData(path)));
                var result = w.Send();
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    if (result.Message == 0x00)
                    {
                        int len = reader.ReadInt32();
                        string[] dirs = new string[len];
                        for (int i = 0; i < len; i++)
                        {
                            dirs[i] = reader.ReadString();
                        }
                        return dirs;

                    }
                    else
                    {
                        throw new IOException(reader.ReadString());
                    }
                }
            }
        }
    }
}
