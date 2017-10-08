using System;
using System.Collections.Generic;
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
        [ClientMessageHandler("fs_streamid"), AsyncExecution]
        public static void StreamID(string content, string ip)
        {
            Dictionary<string, string> res = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            _fs_result = res["result"];
            _wstrguid = res["streamid"];
        }

        [ClientMessageHandler("fs_mounts"), AsyncExecution]
        public static void MountsHandler(string content, string ip)
        {
            _mountinfo = JsonConvert.DeserializeObject<MountInformation[]>(content);
            _fs_result = "success";
        }

        [ClientMessageHandler("fs_rbytes"), AsyncExecution]
        public static void BytesHandler(string content, string ip)
        {
            if (_rbwriter == null)
                return;
            byte[] data = Convert.FromBase64String(content);
            _rbwriter.Write(data);
        }

        [ClientMessageHandler("fs_readdone"), AsyncExecution]
        public static void ReadDone(string content, string ip)
        {
            if (_rbwriter == null)
                return;
            _readbytes_data = _rbstream.ToArray();
            _fs_result = "success";
        }

        private static System.IO.MemoryStream _rbstream = null;
        private static System.IO.BinaryWriter _rbwriter = null;

        [ClientMessageHandler("fs_result"), AsyncExecution]
        public static void Handler_FSResult(string content, string ip)
        {
            _fs_result = content;
        }

        [ClientMessageHandler("fs_fileinfo"), AsyncExecution]
        public static void Handler_FileInfo(string content, string ip)
        {
            _fr_result = JsonConvert.DeserializeObject<FileRecord>(content);
            _fs_result = "success";
        }


        [ClientMessageHandler("fs_filelist"), AsyncExecution]
        public static void Handler_FSFileList(string content, string ip)
        {
            _filelist_result = JsonConvert.DeserializeObject<string[]>(content);
            _fs_result = "success";
        }


        [ClientMessageHandler("fs_exists"), AsyncExecution]
        public static void Handler_ExistsResult(string content, string ip)
        {
            _fs_exists_result = (content == "1") ? true : false;
            _fs_result = "success";
        }


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
            _fs_result = null;
            var split = _createPathData(path);
            ServerManager.SendMessage("fs_createdirectory", JsonConvert.SerializeObject(split));
            while (_fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
        }

        public static FileRecord GetFileInfo(string path)
        {
            _fr_result = null;
            _fs_result = null;
            ServerManager.SendMessage("fs_getfileinfo", JsonConvert.SerializeObject(_createPathData(path)));
            while (_fr_result == null && _fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
            return _fr_result;
        }

        public static void CreateMountIfNotExists()
        {
            _fs_result = null;
            ServerManager.SendMessage("fs_createifnotexist", JsonConvert.SerializeObject(new
            {
                volume = 0,
                label = "System",
            }));
            while (_fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
        }

        public static byte[] ReadAllBytes(string path)
        {
            _fs_result = null;
            var pdata = _createPathData(path);
            var tcpClient = new TcpClient();
            tcpClient.Connect(ServerManager.SessionInfo.ServerIP, ServerManager.SessionInfo.ServerPort);
            var tcpStream = tcpClient.GetStream();
            var writer = new System.IO.BinaryWriter(tcpStream);
            var reader = new System.IO.BinaryReader(tcpStream);

            writer.Write(JsonConvert.SerializeObject(new PlexServerHeader
            {
                Message = "fs_readbytes",
                IPForwardedBy = "0.0.0.0",
                Content = JsonConvert.SerializeObject(pdata),
                SessionID = ServerManager.SessionInfo.SessionID
            }));
            string result = reader.ReadString();
            byte[] data = null;
            if (result == "success")
            {
                int count = reader.ReadInt32();
                data = reader.ReadBytes(count);
            }
            
            writer.Close();
            reader.Close();
            tcpClient.Close();

            if (data == null)
                throw new System.IO.IOException(result);
            return data;
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
            _mountinfo = null;
            ServerManager.SendMessage("fs_getmounts", "");
            while (_mountinfo == null)
                Thread.Sleep(10);
            return _mountinfo;
        }

        public static void Delete(string path)
        {
            _fs_result = null;
            var split = _createPathData(path);
            ServerManager.SendMessage("fs_delete", JsonConvert.SerializeObject(split));
            while (_fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);

        }

        private static async Task<string> _startUpload(string path, byte[] contents)
        {
            if (_wstrguid != null)
                return "An upload operation is currently running. You can't start a new one.";
            _fs_result = null;
            var pdata = _createPathData(path);
            pdata.AdditionalData = contents.Length.ToString();
            await Task.Run(() =>
            {
                ServerManager.SendMessage("fs_allocwrite", JsonConvert.SerializeObject(pdata));
            });
            while (_fs_result == null || _wstrguid == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                return _fs_result;
            var tcpClient = new TcpClient();
            tcpClient.Connect(ServerManager.SessionInfo.ServerIP, ServerManager.SessionInfo.ServerPort);
            var tcpStream = tcpClient.GetStream();
            var writer = new System.IO.BinaryWriter(tcpStream);
            var reader = new System.IO.BinaryReader(tcpStream);
            //The upload stream has been started on the server so we can start pushing data to it.
            string json = JsonConvert.SerializeObject(new
            {
                StreamID = _wstrguid,
                Buffer = contents
            });
            writer.Write(JsonConvert.SerializeObject(new PlexServerHeader
            {
                Content = json,
                IPForwardedBy = "0.0.0.0",
                Message = "fs_write",
                SessionID = ServerManager.SessionInfo.SessionID
            }));
                _fs_result = reader.ReadString();
            if (_fs_result != "success")
            {
                writer.Close();
                reader.Close();
                tcpClient.Close();
                return _fs_result;
            }
            
            writer.Close();
            reader.Close();
            tcpClient.Close();

            _fs_result = null;
            await Task.Run(() =>
            {
                ServerManager.SendMessage("fs_close", _wstrguid);
            });
            while (_fs_result == null)
                Thread.Sleep(10);
            _wstrguid = null;
            if (_fs_result != "success")
                return _fs_result;
            return "success";
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
            var task = _startUpload(path, contents);
            task.Wait();
            string result = task.Result;
            if (result != "success")
                throw new System.IO.IOException(result);
            Thread.Sleep(500);
        }

        public static bool DirectoryExists(string path)
        {
            _fs_exists_result = null;
            _fs_result = null;
            ServerManager.SendMessage("fs_direxists", JsonConvert.SerializeObject(_createPathData(path)));
            while (_fs_exists_result == null && _fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
            return (bool)_fs_exists_result;

        }

        public static bool FileExists(string path)
        {
            _fs_exists_result = null;
            _fs_result = null;
            ServerManager.SendMessage("fs_fileexists", JsonConvert.SerializeObject(_createPathData(path)));
            while (_fs_exists_result == null && _fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
            return (bool)_fs_exists_result;

        }

        public static string ReadAllText(string path)
        {
            return Encoding.UTF8.GetString(ReadAllBytes(path));
        }

        public static string[] GetDirectories(string path)
        {
            _filelist_result = null;
            _fs_result = null;
            ServerManager.SendMessage("fs_getdirs", JsonConvert.SerializeObject(_createPathData(path)));
            while (_filelist_result == null && _fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
            return _filelist_result;
                                                                                          }

        /// <summary>
        /// Copies a file or directory from one path to another, deleting the original.
        /// </summary>
        /// <param name="path">THe input path, must be a valid directory or file.</param>
        /// <param name="target">The output path.</param>
        public static void Move(string path, string target)
        {
            _fs_result = null;
            var split = _createPathData(path);
            split.AdditionalData = target;
            ServerManager.SendMessage("fs_move", JsonConvert.SerializeObject(split));
            while (_fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
            
        }


        /// <summary>
        /// Copies a file or directory from one path to another.
        /// </summary>
        /// <param name="path">The input path, must be a valid directory or file.</param>
        /// <param name="target">The output path.</param>
        public static void Copy(string path, string target)
        {
            _fs_result = null;
            var split = _createPathData(path);
            split.AdditionalData = target;
            ServerManager.SendMessage("fs_copy", JsonConvert.SerializeObject(split));
            while (_fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);

        }

        public static string[] GetFiles(string path)
        {
            _filelist_result = null;
            _fs_result = null;
            ServerManager.SendMessage("fs_getfiles", JsonConvert.SerializeObject(_createPathData(path)));
            while (_filelist_result == null && _fs_result == null)
                Thread.Sleep(10);
            if (_fs_result != "success")
                throw new System.IO.IOException(_fs_result);
            return _filelist_result;
        }
    }
}
