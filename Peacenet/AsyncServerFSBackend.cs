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

        private List<IClientMount> _clientMounts = new List<IClientMount>();

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
            var cMount = _clientMounts.FirstOrDefault(x => path.StartsWith(x.Path));
            if (cMount != null)
                throw new IOException("Read-only filesystem.");
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
            var cMount = _clientMounts.FirstOrDefault(x => path.StartsWith(x.Path));
            if (cMount != null)
                throw new IOException("Read-only filesystem.");
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
            var cMount = _clientMounts.FirstOrDefault(x => x.Path == path);
            if (cMount != null)
                return true;
            cMount = _clientMounts.FirstOrDefault(x => path.StartsWith(x.Path));
            if (cMount != null)
                return false;
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
            
            var cMount = _clientMounts.FirstOrDefault(x => x.Path == path);
            if (cMount != null)
                return false;
            string name = path.Split('/').Last();
            string dir = path.Substring(0, path.LastIndexOf('/'));
            cMount = _clientMounts.FirstOrDefault(x => x.Path == dir);
            if (cMount != null)
                return cMount.GetFiles().Contains(name);


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
            var cMount = _clientMounts.FirstOrDefault(x => x.Path == path);
            if (cMount != null)
                return new[] { ".", ".." };

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
            var cMount = _clientMounts.FirstOrDefault(x => path.StartsWith(x.Path));
            if (cMount != null)
            {
                string mPath = path.Substring(path.LastIndexOf('/') + 1);
                if (!cMount.GetFiles().Contains(mPath))
                    throw new IOException("File not found.");
                return new FileRecord
                {
                    IsDirectory = false,
                    Name = mPath,
                    SizeBytes = cMount.GetFileContents(mPath).Length
                };
            }
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
            var cMount = _clientMounts.FirstOrDefault(x => x.Path == path);
            if (cMount != null)
                return cMount.GetFiles();
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

        [Dependency]
        private Plexgate _plexgate = null;

        /// <inheritdoc/>
        public void Initialize()
        {
            _clientMounts.Clear();

            foreach(var t in ReflectMan.Types.Where(x=>x.GetInterfaces().Contains(typeof(IClientMount))))
            {
                var m = (IClientMount)_plexgate.New(t);
                if (_clientMounts.FirstOrDefault(x => x.Path == m.Path) != null)
                    throw new IOException($"Cannot mount {m.Path} - directory already mounted.");
                _clientMounts.Add(m);
            }

            //What we want to do is we want to first get a list of all mounts (partitions) from the server.

            //Create the fstab dictionary, that maps client-side Unix-like mountpoints to server-side COSMOS-like drive numbers.
            fstab = new Dictionary<string, int>();
            //Mount the system drive.
            fstab.Add("/", 0);

            //Create any client-side mountpoints as directories on the VFS
            foreach (var cfs in _clientMounts)
                if (!DirectoryExists(cfs.Path))
                    CreateDirectory(cfs.Path);

        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string path)
        {
            var cMount = _clientMounts.FirstOrDefault(x => path.StartsWith(x.Path));
            if (cMount != null)
            {
                string mPath = path.Substring(path.LastIndexOf('/') + 1);
                if (!cMount.GetFiles().Contains(mPath))
                    throw new IOException("File not found.");
                return cMount.GetFileContents(mPath);
            }
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
            var cMount = _clientMounts.FirstOrDefault(x => path.StartsWith(x.Path));
            if (cMount != null)
            {
                throw new IOException("Read-only filesystem.");
            }
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
            var cMount = _clientMounts.FirstOrDefault(x => x.Path.StartsWith(path));
            if (cMount != null)
            {
                string mPath = path.Remove(0, cMount.Path.Length);
                if (!cMount.GetFiles().Contains(mPath))
                    if (mode == OpenMode.OpenOrCreate)
                        throw new IOException("Read-only filesystem.");
                    else
                        throw new IOException("File not found.");
                return new MemoryStream(cMount.GetFileContents(mPath));
                
            }
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
