using Peacenet.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects.ShiftFS;
using System.IO;
using Newtonsoft.Json;
using Plex.Engine;
using Peacenet.Server;
using Plex.Objects.PlexFS;
using Plex.Objects;

namespace Peacenet
{
    /// <summary>
    /// Provides a server-based file system backend for the Peacenet.
    /// </summary>
    public class AsyncServerFSBackend : IAsyncFSBackend
    {
        private Dictionary<string, int> fstab = null;

        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private RemoteStreams _rstreams = null;

        private string unresolve(string path)
        {
            string[] split = path.Split(new[] { ":" }, StringSplitOptions.None);
            if (split.Length != 2)
                throw new FormatException("Path was not in the correct format for unresolving.");
            int dnum = -1;
            if (!int.TryParse(split[0], out dnum))
                throw new FormatException("Invalid drive number. Can't resolve to mount point.");
            string client = null;
            try
            {
                client = fstab.First(x => x.Value == dnum).Key + split[1];
            }
            catch
            {
                client = split[1];
            }
            while (client.Contains("//"))
                client = client.Replace("//", "/");
            return client;
        }

        private int getNumberFromPath(string path, out string local)
        {
            try
            {
                var first = fstab.OrderByDescending(x => x.Key.Length).First(x => path.StartsWith(x.Key));
                local = ":" + path.Remove(0, first.Key.Length-1);
                if (local.EndsWith("/"))
                    local = local.Remove(local.Length - 1, 1);
                return first.Value;
            }
            catch
            {
                local = ":" + path;
                if (local.EndsWith("/"))
                    local = local.Remove(local.Length - 1, 1);
                return fstab["/"];
            }
        }

        private byte[] writePathData(string dpath, string additional)
        {
            string localpath;
            int dnum = getNumberFromPath(dpath, out localpath);
            var pdata = new PathData
            {
                DriveNumber = dnum,
                Path = localpath,
                AdditionalData = additional
            };
            using (var memstr = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memstr, Encoding.UTF8))
                {
                    writer.Write(JsonConvert.SerializeObject(pdata));
                    return memstr.ToArray();
                }
            }
        }

        /// <inheritdoc/>
        public void CreateDirectory(string path)
        {
            Exception err = null;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_CREATEDIR, writePathData(path, null), (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    return;
                if (res == Plex.Objects.ServerResponseType.REQ_ERROR)
                    if (reader.BaseStream.Length > 0)
                    {
                        err = new IOException(reader.ReadString());
                        return;
                    }
                err = new Exception("An I/O error has occurred.");
            }).Wait();
            if (err != null)
                throw err;
        }

        /// <inheritdoc/>
        public void Delete(string path)
        {
            Exception err = null;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_DELETE, writePathData(path, null), (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    return;
                if (res == Plex.Objects.ServerResponseType.REQ_ERROR)
                    if (reader.BaseStream.Length > 0)
                    {
                        err = new IOException(reader.ReadString());
                        return;
                    }
                err = new Exception("An I/O error has occurred.");
            }).Wait();
            if (err != null)
                throw err;
        }

        /// <inheritdoc/>
        public bool DirectoryExists(string path)
        {
            Exception err = null;
            bool result = false;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_DIREXISTS, writePathData(path, null), (res, reader) =>
            {
                if(res != Plex.Objects.ServerResponseType.REQ_SUCCESS)
                {
                    err = new Exception("An I/O error has occurred.");
                    return;
                }
                result = reader.ReadBoolean();
            }).Wait();
            if (err != null)
                throw err;
            return result;
        }

        /// <inheritdoc/>
        public bool FileExists(string path)
        {
            Exception err = null;
            bool result = false;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_FILEEXISTS, writePathData(path, null), (res, reader) =>
            {
                if (res != Plex.Objects.ServerResponseType.REQ_SUCCESS)
                {
                    err = new Exception("An I/O error has occurred.");
                    return;
                }
                result = reader.ReadBoolean();
            }).Wait();
            if (err != null)
                throw err;
            return result;
        }

        /// <inheritdoc/>
        public string[] GetDirectories(string path)
        {
            List<string> result = new List<string>();
            Exception err = null;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_GETDIRS, writePathData(path, null), (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                {
                    int length = reader.ReadInt32();
                    for (int i = 0; i < length; i++)
                    {
                        result.Add(unresolve(reader.ReadString()));
                    }
                    return;
                }
                if (res == Plex.Objects.ServerResponseType.REQ_ERROR)
                    if (reader.BaseStream.Length > 0)
                    {
                        err = new IOException(reader.ReadString());
                        return;
                    }
                err = new Exception("An I/O error has occurred.");
            }).Wait();
            if (err != null)
                throw err;
            return result.ToArray();

        }

        /// <inheritdoc/>
        public FileRecord GetFileRecord(string path)
        {
            FileRecord result = null;
            Exception err = null;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_RECORDINFO, writePathData(path, null), (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                {
                    result = JsonConvert.DeserializeObject<FileRecord>(reader.ReadString());
                    return;
                }
                if (res == Plex.Objects.ServerResponseType.REQ_ERROR)
                    if (reader.BaseStream.Length > 0)
                    {
                        err = new IOException(reader.ReadString());
                        return;
                    }
                err = new Exception("An I/O error has occurred.");
            }).Wait();
            if (err != null)
                throw err;
            return result;

        }

        /// <inheritdoc/>
        public string[] GetFiles(string path)
        {
            List<string> result = new List<string>();
            Exception err = null;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_GETFILES, writePathData(path, null), (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                {
                    int length = reader.ReadInt32();
                    for (int i = 0; i < length; i++)
                    {
                        result.Add(unresolve(reader.ReadString()));
                    }
                    return;
                }
                if (res == Plex.Objects.ServerResponseType.REQ_ERROR)
                    if (reader.BaseStream.Length > 0)
                    {
                        err = new IOException(reader.ReadString());
                        return;
                    }
                err = new Exception("An I/O error has occurred.");
            }).Wait();
            if (err != null)
                throw err;
            return result.ToArray();
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            //What we want to do is we want to first get a list of all mounts (partitions) from the server.

            //Create the fstab dictionary, that maps client-side Unix-like mountpoints to server-side COSMOS-like drive numbers.
            fstab = new Dictionary<string, int>();
            //Mount the system drive.
            fstab.Add("/", 0);


        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string path)
        {
            byte[] result = new byte[0];
            Exception err = null;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_READFROMFILE, writePathData(path, null), (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                {
                    int length = reader.ReadInt32();
                    result = reader.ReadBytes(length);
                    return;
                }
                if (res == Plex.Objects.ServerResponseType.REQ_ERROR)
                    if (reader.BaseStream.Length > 0)
                    {
                        err = new IOException(reader.ReadString());
                        return;
                    }
                err = new Exception("An I/O error has occurred.");
            }).Wait();
            if (err != null)
                throw err;
            return result;

        }

        /// <inheritdoc/>
        public void Unload()
        {
            fstab.Clear();
            fstab = null;
        }

        /// <inheritdoc/>
        public void WriteAllBytes(string path, byte[] data)
        {
            byte[] bdata = null;
            string localpath;
            int dnum = getNumberFromPath(path, out localpath);
            var pdata = new PathData
            {
                DriveNumber = dnum,
                Path = localpath,
            };
            using (var memstr = new MemoryStream())
            {
                using(var writer = new BinaryWriter(memstr, Encoding.UTF8))
                {
                    writer.Write(JsonConvert.SerializeObject(pdata));
                    writer.Write(data.Length);
                    writer.Write(data);
                    bdata = memstr.ToArray();
                }
            }

            Exception err = null;
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_WRITETOFILE, bdata, (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    return;
                if (res == Plex.Objects.ServerResponseType.REQ_ERROR)
                    if (reader.BaseStream.Length > 0)
                    {
                        err = new IOException(reader.ReadString());
                        return;
                    }
                err = new Exception("An I/O error has occurred.");
            }).Wait();
            if (err != null)
                throw err;

        }

        public Stream Open(string path, OpenMode mode)
        {
            var err = new Exception("Unknown error.");
            var ret = -1;
            _server.SendMessage(ServerMessageType.FS_OPENSTREAM, writePathData(path, null).Concat(BitConverter.GetBytes((int)mode)).ToArray(), (res, reader) =>
            {
                if (res == ServerResponseType.REQ_SUCCESS)
                {
                    ret = reader.ReadInt32();
                    return;
                }
                if (res == ServerResponseType.REQ_ERROR && reader.BaseStream.Length > 0)
                {
                    err = new Exception(reader.ReadString());
                    return;
                }
            }).Wait();
            if (ret < 0)
                throw err;
            return _rstreams.Open(ret);
        }
    }
}
