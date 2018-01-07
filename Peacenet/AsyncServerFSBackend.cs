using Plex.Engine.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects.ShiftFS;
using Plex.Engine.Server;
using System.IO;
using Newtonsoft.Json;
using Plex.Engine;

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

            //Because we're dealing with async shit in a non-async method, and I hate AggregateExceptions, let's create an Exception variable for async error reporting.
            Exception err = null;
            //Make a call to the server to grab all the user's mounts.
            _server.SendMessage(Plex.Objects.ServerMessageType.FS_GETMOUNTS, null, (res, reader) =>
            {
                try
                {
                    //We should get back REQ_SUCCESS if we got a mountlist - even if it's empty. If we didn't get a success response, report an error.
                    if (res != Plex.Objects.ServerResponseType.REQ_SUCCESS)
                        err = new Exception($"Server returned error {res} while loading mountpoint information.");
                    else
                    {
                        //Read an int32 from the response stream so we know how many mountpoints to read
                        int mountCount = reader.ReadInt32();
                        //Read that many mounts
                        for (int i = 0; i < mountCount; i++)
                        {
                            //Each mount is stored as a string (drive label) and int (drive number) in sequence.
                            //We'll use the drive label as the mountpoint's path.
                            string mPath = reader.ReadString();
                            int sNumber = reader.ReadInt32();
                            //If two mounts share the same path, there's a serious problem.
                            if (fstab.ContainsKey(mPath))
                            {
                                err = new InvalidOperationException("Two mountpoints share the same path. This is a serious bug. Tell a developer or server admin immediately. You should never ever see this.");
                                break;
                            }
                            fstab.Add(mPath, sNumber);
                        }
                    }
                }
                catch(Exception ex)
                {
                    err = ex;
                }
            }).Wait();
            //Check for errors.
            if (err != null)
                throw err;
            //Now we check to make sure we have a mount at /. If not, create it.
            if(!fstab.ContainsKey("/"))
            {
                byte[] body = null;
                //Another server call, this time "FS_CREATEMOUNT", which has a body.
                using(var memstr = new MemoryStream())
                {
                    //The memory stream stores the binary body data, this BinaryWriter allows us to write the JSON for this request.
                    using (var writer = new BinaryWriter(memstr, Encoding.UTF8))
                    {
                        writer.Write(JsonConvert.SerializeObject(new
                        {
                            //Volume is an integer and is the remote drive number
                            volume = 0,
                            //Label is a string, that's our mountpoint.
                            label = "/"
                        }));
                        //Data written. Let's retrieve the body byte[]
                        body = memstr.ToArray();
                        //Done with that crap.
                    }
                }
                //Now make the actual call.
                _server.SendMessage(Plex.Objects.ServerMessageType.FS_CREATEMOUNT, body, (res, reader) =>
                {
                    //Same as above, if no "REQ_SUCCESS", we had an error.
                    if(res != Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    {
                        err = new Exception($"Server returned error code {res} while creating mountpoint at /.");
                    }
                    //If we got this far there was no error.
                }).Wait();
                //Check for error.
                if (err != null)
                    throw err;
                //Add mountpoint.
                fstab.Add("/", 0);
                //And we're done.
            }
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
    }
}
