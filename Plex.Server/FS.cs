using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;
using Plex.Objects.ShiftFS;

namespace Plex.Server
{
    public static class FS
    {
        private static List<ADriveMount> Mounts = new List<ADriveMount>();
        private static List<FSStreamData> _writers = new List<FSStreamData>();
        
        public class BufferData
        {
            public string StreamID { get; set; }
            public byte[] Buffer { get; set; }
        }

        public class FSStreamData
        {
            private System.IO.MemoryStream _stream = null;

            public FSStreamData()
            {
                _stream = new System.IO.MemoryStream();
                Stream = new System.IO.BinaryWriter(_stream);
            }

            public string SessionID { get; set; }
            public PathData PathData { get; set; }
            public string StreamID { get; set; }
            public ADriveMount Mount { get; set; }
            public int ContentLength { get; set; }

            public System.IO.BinaryWriter Stream { get; private set; }

            public void Close(string ip, int port)
            {
                var pdata = PathData;
                var mount = Mount;
                if (mount == null)
                {
                    DispatchFSResult("Mountpoint not found.", SessionID, ip, port);
                    return;
                }
                try
                {
                    var bytes = _stream.ToArray();
                    if (bytes.Length != ContentLength)
                    {
                        DispatchFSResult("The uploaded data does not match the expected content length.", SessionID, ip, port);
                    }
                    else
                    {
                        mount.WriteAllBytes(pdata.Path, bytes);
                        DispatchFSResult("success", SessionID, ip, port);
                    }
                }
                catch (Exception ex)
                {
                    DispatchFSResult(ex.Message, SessionID, ip, port);
                }
                Stream.Close();
                _stream.Close();
            }
        }

        [ServerMessageHandler("fs_close"), SessionRequired]
        public static void CloseWrite(string session, string content, string ip, int port)
        {
            var writer = _writers.FirstOrDefault(x => x.StreamID == content && x.SessionID == session);
            if (writer == null)
            {
                DispatchFSResult("A stream with the specified stream ID and session ID was not found.", session, ip, port);
                return;
            }
            try
            {
                writer.Close(ip, port);
                DispatchFSResult("success", session, ip, port);
            }
            catch (Exception ex)
            {
                DispatchFSResult(ex.Message, session, ip, port);

            }

        }


        [ServerMessageHandler("fs_write"), SessionRequired, Tcp]
        public static void FSWrite(string session, string content, System.IO.BinaryWriter _tcp)
        {
            var bufferdata = JsonConvert.DeserializeObject<BufferData>(content);
            var writer = _writers.FirstOrDefault(x => x.StreamID == bufferdata.StreamID && x.SessionID == session);
            if(writer == null)
            {
                _tcp.Write("A stream with the specified stream and session IDs was not found.");                
                return;
            }
            writer.Stream.Write(bufferdata.Buffer);
            _tcp.Write("success");
        }

        [ServerMessageHandler("fs_allocwrite")]
        [SessionRequired]
        public static void AllocWrite(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if (mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            int length = -1;
            if(int.TryParse(pdata.AdditionalData, out length) == false)
            {
                DispatchStreamID("Incorrect content length specified.", "", session, ip, port);
                return;
            }
            var str = new FSStreamData
            {
                ContentLength = length,
                Mount = mount,
                PathData = pdata,
                SessionID = session,
                StreamID = Guid.NewGuid().ToString()
            };
            _writers.Add(str);
            DispatchStreamID("success", str.StreamID, session, ip, port);
        }

        public static void DispatchStreamID(string result, string stream, string session, string ip, int port)
        {
            Program.SendMessage(new PlexServerHeader
            {
                Content = JsonConvert.SerializeObject(new
                {
                    result = result,
                    streamid = stream
                }),
                IPForwardedBy = ip,
                Message = "fs_streamid",
                SessionID = session
            }, port);
        }


        public static MountInformation CreateVolume(HackableSystem sys, int num, string label, string sessionid)
        {
            //TODO: plexfat support.
            if (!System.IO.Directory.Exists("userdrives"))
            {
                System.IO.Directory.CreateDirectory("userdrives");
            }
            var mountinfo = new MountInformation
            {
                DriveNumber = num,
                ImageFilePath = $"userdrives/{sys.SystemDescriptor.SystemName}_{num}_{label}.pfs",
                Specification = DriveSpec.PlexFAT,
                VolumeLabel = label
            };
            return mountinfo;
        }

        public static void MountFS(MountInformation mi, string sessionid)
        {
            foreach(var type in ReflectMan.Types.Where(x=>x.BaseType == typeof(ADriveMount)))
            {
                var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is FSHandler) as FSHandler;
                if(attrib != null)
                {
                    if(attrib.Specification == mi.Specification)
                    {
                        var mount = (ADriveMount)Activator.CreateInstance(type, new object[] { mi, sessionid });
                        Mounts.Add(mount);
                        return;
                    }
                }
            }
        }

        [ServerMessageHandler("fs_createifnotexist")]
        [SessionRequired]
        public static void CreateFSIfNotExisting(string session_id, string content, string ip, int port)
        {
            var session = SessionManager.GrabAccount(session_id);
            var hackable = Program.GetSaveFromPrl(session.SaveID);
            var volumeinfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var volume = hackable.Filesystems.FirstOrDefault(x => x.DriveNumber == Convert.ToInt32(volumeinfo["volume"].ToString()) && x.VolumeLabel == volumeinfo["label"].ToString());
            if(volume == null)
            {
                volume = CreateVolume(hackable, Convert.ToInt32(volumeinfo["volume"].ToString()), volumeinfo["label"].ToString(), session_id);
                hackable.Filesystems.Add(volume);
                Program.SaveWorld();
            }
            if(Mounts.FirstOrDefault(x=>x.SessionID == session_id && x.DriveNumber == volume.DriveNumber) == null)
                MountFS(volume, session_id);
            DispatchFSResult("success", session_id, ip, port);
        }

        public static void DispatchFSResult(string result, string session, string ip, int port)
        {
            Program.SendMessage(new PlexServerHeader
            {
                Content = result,
                IPForwardedBy = ip,
                Message = "fs_result",
                SessionID = session
            }, port);
        }

        public static void DispatchFileList(string[] result, string session, string ip, int port)
        {
            Program.SendMessage(new PlexServerHeader
            {
                Content = JsonConvert.SerializeObject(result),
                IPForwardedBy = ip,
                Message = "fs_filelist",
                SessionID = session
            }, port);
        }


        public static void DispatchExistsResult(bool result, string session, string ip, int port)
        {
            Program.SendMessage(new PlexServerHeader
            {
                Content = (result) ? "1" : "0",
                IPForwardedBy = ip,
                Message = "fs_exists",
                SessionID = session
            }, port);
        }

        public static PathData GetPathData(string data)
        {
            return JsonConvert.DeserializeObject<PathData>(data);
        }

        public static ADriveMount GetDriveMount(string pdata, string session)
        {
            var dataparsed = GetPathData(pdata);
            return Mounts.FirstOrDefault(x => x.DriveNumber == dataparsed.DriveNumber && x.SessionID == session);
        }

        [ServerMessageHandler("fs_direxists"), SessionRequired]
        public static void DirectoryExists(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            bool result = mount != null;
            if (!pdata.Path.Contains("/")) //obvi. root dir
            {
                result = true;
            }
            else
            {
                if (result)
                    result = mount.DirectoryExists(pdata.Path);
            }
            DispatchExistsResult(result, session, ip, port);
        }

        [ServerMessageHandler("fs_delete"), SessionRequired]
        public static void FSDelete(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if(mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            if (mount.FileExists(pdata.Path))
            {
                mount.DeleteFile(pdata.Path);
                DispatchFSResult("success", session, ip, port);
                return;
            }
            if (mount.DirectoryExists(pdata.Path))
            {
                mount.DeleteDirectory(pdata.Path);
                DispatchFSResult("success", session, ip, port);
                return;
            }
            DispatchFSResult("File or directory not found.", session, ip, port);

        }

        [ServerMessageHandler("fs_fileexists"), SessionRequired]
        public static void FileExists(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            bool result = mount != null;
            if (result)
                result = mount.FileExists(pdata.Path);
            DispatchExistsResult(result, session, ip, port);
        }

        [ServerMessageHandler("fs_getfiles"), SessionRequired]
        public static void GetFiles(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if(mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            if (!mount.DirectoryExists(pdata.Path))
            {
                DispatchFSResult("Directory not found.", session, ip, port);
                return;
            }
            DispatchFileList(mount.GetFiles(pdata.Path), session, ip, port);
        }

        [ServerMessageHandler("fs_createdirectory"), SessionRequired]
        public static void CreateDirectory(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if (mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            if (mount.DirectoryExists(pdata.Path))
            {
                DispatchFSResult("Directory already exists.", session, ip, port);
                return;
            }
            mount.CreateDirectory(pdata.Path);
            DispatchFileList(mount.GetFiles(pdata.Path), session, ip, port);
        }

        public static void DispatchFileInfo(FileRecord record, string session, string ip, int port)
        {
            if (record == null)
            {
                DispatchFSResult("File or directory not found.", session, ip, port);
                return;
            }
            Program.SendMessage(new PlexServerHeader
            {
                Content = JsonConvert.SerializeObject(record),
                Message = "fs_fileinfo",
                IPForwardedBy = ip,
                SessionID = session
            }, port);
        }

        [ServerMessageHandler("fs_getfileinfo"), SessionRequired]
        public static void GetFileInfo(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if (mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            DispatchFileInfo(mount.GetFileInfo(pdata.Path), session, ip, port);
        }

        [ServerMessageHandler("fs_readbytes"), SessionRequired, Tcp]
        public static void ReadBytes(string session, string content, System.IO.BinaryWriter _tcp)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if (mount == null)
            {
                _tcp.Write("Mountpoint not found.");
                return;
            }
            if(!mount.FileExists(pdata.Path))
            {
                _tcp.Write("File not found.");
                return;

            }
            _tcp.Write("success");
            byte[] data = mount.ReadAllBytes(pdata.Path);
            _tcp.Write(data.Length);
            _tcp.Write(data);
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

        public static void DispatchReadDone(string session, string ip, int port)
        {
            Program.SendMessage(new PlexServerHeader
            {
                Content = "",
                IPForwardedBy = ip,
                Message = "fs_readdone",
                SessionID = session
            }, port);

        }

        public static void DispatchBytes(byte[] bytes, string session, string ip, int port)
        {
            Program.SendMessage(new PlexServerHeader
            {
                Content = Convert.ToBase64String(bytes),
                IPForwardedBy = ip,
                Message = "fs_rbytes",
                SessionID = session
            }, port);
        }

        [ServerMessageHandler("fs_getdirs"), SessionRequired]
        public static void GetDirectories(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if (mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            if (!mount.DirectoryExists(pdata.Path))
            {
                DispatchFSResult("Directory not found.", session, ip, port);
                return;
            }
            DispatchFileList(mount.GetDirectories(pdata.Path), session, ip, port);
        }

        [ServerMessageHandler("fs_writetext"), SessionRequired]
        public static void WriteText(string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if (mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            try
            {
                mount.WriteAllText(pdata.Path, pdata.AdditionalData);
                DispatchFSResult("success", session, ip, port);
            }
            catch (Exception ex)
            {
                DispatchFSResult(ex.Message, session, ip, port);
            }
        }


        [ServerMessageHandler("fs_writebytes"), SessionRequired]
        public static void WriteBytes   (string session, string content, string ip, int port)
        {
            var pdata = GetPathData(content);
            var mount = GetDriveMount(content, session);
            if (mount == null)
            {
                DispatchFSResult("Mountpoint not found.", session, ip, port);
                return;
            }
            try
            {
                mount.WriteAllBytes(pdata.Path, Convert.FromBase64String(pdata.AdditionalData));
                DispatchFSResult("success", session, ip, port);
            }
            catch(Exception ex)
            {
                DispatchFSResult(ex.Message, session, ip, port);
            }
        }

        [ServerMessageHandler("fs_getmounts"), SessionRequired]
        public static void GetMounts(string session, string content, string ip, int port)
        {
            var sessiondata = SessionManager.GrabAccount(session);
            var hackable = Program.GetSaveFromPrl(sessiondata.SaveID);
            Program.SendMessage(new PlexServerHeader
            {
                Message = "fs_mounts",
                Content = JsonConvert.SerializeObject(hackable.Filesystems),
                IPForwardedBy = ip,
                SessionID = session
            }, port);
        }
    }

    public abstract class ADriveMount
    {
        private int _volumenum = 0;
        private string _diskLoc = "";
        private string _usersession = "";

        public ADriveMount(MountInformation mountdata, string usersession)
        {
            _volumenum = mountdata.DriveNumber;
            _diskLoc = mountdata.ImageFilePath;
            _usersession = usersession;
            EnsureDriveExistence();
        }

        /// <summary>
        /// Gets the drive number associated with this volume.
        /// </summary>
        public int DriveNumber
        {
            get
            {
                return _volumenum;
            }
        }

        /// <summary>
        /// Gets the volume's disk image filepath for storage on the system HDD/SSD.
        /// </summary>
        public string ImageLocation
        {
            get
            {
                return _diskLoc;
            }
        }

        /// <summary>
        /// Gets the usersession ID for this drive.
        /// </summary>
        public string SessionID
        {
            get
            {
                return _usersession;
            }
        }

        /// <summary>
        /// Ensure proper existence of the drive. Use this to create any required data if it doesn't already exist.
        /// </summary>
        public abstract void EnsureDriveExistence();

        /// <summary>
        /// Write binary data to a file.
        /// </summary>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="bytes">The data to write</param>
        public abstract void WriteAllBytes(string path, byte[] bytes);

        /// <summary>
        /// Get information about a file or directory such as the name, size, etc.
        /// </summary>
        /// <param name="path">The path to retrieve info about.</param>
        /// <returns>The file record data.</returns>
        public abstract FileRecord GetFileInfo(string path);

        /// <summary>
        /// Write text to a file.
        /// </summary>
        /// <param name="path">The file path to write to</param>
        /// <param name="text">The text to write</param>
        public abstract void WriteAllText(string path, string text);

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="path">The file to delete.</param>
        public abstract void DeleteFile(string path);

        /// <summary>
        /// Delete a directory.
        /// </summary>
        /// <param name="path">The directory to delete.</param>
        public abstract void DeleteDirectory(string path);

        /// <summary>
        /// Check if a file exists at this path.
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>Whether the file exists</returns>
        public abstract bool FileExists(string path);

        /// <summary>
        /// Check if a directory exists at this path
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>Whether the directory exists</returns>
        public abstract bool DirectoryExists(string path);

        /// <summary>
        /// Create a directory.
        /// </summary>
        /// <param name="path">The path to create the directory at</param>
        public abstract void CreateDirectory(string path);

        /// <summary>
        /// Read the text of a file
        /// </summary>
        /// <param name="path">The file to read</param>
        /// <returns>The text to read</returns>
        public abstract string ReadAllText(string path);

        /// <summary>
        /// Read the binary data of a file
        /// </summary>
        /// <param name="path">The file to read</param>
        /// <returns>The binary data</returns>
        public abstract byte[] ReadAllBytes(string path);

        /// <summary>
        /// Get a list of files in a directory.
        /// </summary>
        /// <param name="path">The directory to search</param>
        /// <returns>All files found in the directory.</returns>
        public abstract string[] GetFiles(string path);

        /// <summary>
        /// Get all subdirectories in a directory.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <returns>A list of all found subdirectories.</returns>
        public abstract string[] GetDirectories(string path);


    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FSHandler : Attribute
    {
        public FSHandler(DriveSpec spec)
        {
            Specification = spec;
        }

        public DriveSpec Specification { get; private set; }
    }
}
