using System;
using System.Collections.Generic;
using System.IO;
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

        [ServerMessageHandler( ServerMessageType.FS_CREATEMOUNT)]
        [SessionRequired]
        public static void CreateFSIfNotExisting(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
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
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
        }

        public static PathData GetPathData(string data)
        {
            return JsonConvert.DeserializeObject<PathData>(data);
        }

        public static ADriveMount GetDriveMount(int dnum, string session)
        {
            return Mounts.FirstOrDefault(x => x.DriveNumber == dnum && x.SessionID == session);
        }

        [ServerMessageHandler( ServerMessageType.FS_DIREXISTS), SessionRequired]
        public static void DirectoryExists(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
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
            byte real = result ? (byte)1 : (byte)0;
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);
            writer.Write(real);
        }

        [ServerMessageHandler( ServerMessageType.FS_DELETE), SessionRequired]
        public static void FSDelete(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            if(mount == null)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Mountpoint not found.");
                return;
            }
            if (mount.FileExists(pdata.Path))
            {
                mount.DeleteFile(pdata.Path);
                writer.Write((byte)ServerResponseType.REQ_SUCCESS);
                writer.Write(session);
                return;
            }
            else if (mount.DirectoryExists(pdata.Path))
            {
                mount.DeleteDirectory(pdata.Path);
                writer.Write((byte)ServerResponseType.REQ_SUCCESS);
                writer.Write(session);
                return;
            }
            writer.Write((byte)ServerResponseType.REQ_ERROR);
            writer.Write(session);
            writer.Write("File or directory does not exist.");
        }

        [ServerMessageHandler( ServerMessageType.FS_FILEEXISTS), SessionRequired]
        public static void FileExists(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            bool result = mount != null;
            if (result)
                result = mount.FileExists(pdata.Path);
            byte real = result ? (byte)1 : (byte)0;
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);
            writer.Write(real);
        }

        [ServerMessageHandler( ServerMessageType.FS_GETFILES), SessionRequired]
        public static void GetFiles(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            if(mount == null)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Mountpoint not found.");

                return;
            }
            if (!mount.DirectoryExists(pdata.Path))
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Directory not found.");
                return;
            }
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);

            var dirs = mount.GetFiles(pdata.Path);
            writer.Write(dirs.Length);
            foreach(var dir in dirs)
            {
                writer.Write(dir);
            }
        }

        [ServerMessageHandler( ServerMessageType.FS_CREATEDIR), SessionRequired]
        public static void CreateDirectory(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            if (mount == null)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Mountpoint not found.");
                return;
            }
            if (mount.DirectoryExists(pdata.Path))
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Directory already exists.");
                return;
            }
            mount.CreateDirectory(pdata.Path);
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);
        }

        [ServerMessageHandler( ServerMessageType.FS_RECORDINFO), SessionRequired]
        public static void GetFileInfo(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            if (mount == null)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Mountpoint not found.");
                return;
            }
            var finf = mount.GetFileInfo(pdata.Path);
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);
            writer.Write(JsonConvert.SerializeObject(finf));

        }

        [ServerMessageHandler( ServerMessageType.FS_READFROMFILE), SessionRequired]
        public static void ReadBytes(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            if (mount == null)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Mountpoint not found.");
                return;
            }
            if(!mount.FileExists(pdata.Path))
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("File not found.");
                return;

            }
            byte[] data = mount.ReadAllBytes(pdata.Path);
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);
            writer.Write(data.Length);
            writer.Write(data);
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

        [ServerMessageHandler( ServerMessageType.FS_GETDIRS), SessionRequired]
        public static void GetDirectories(string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            if (mount == null)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Mountpoint not found.");

                return;
            }
            if (!mount.DirectoryExists(pdata.Path))
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Directory not found.");
                return;
            }
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);

            var dirs = mount.GetDirectories(pdata.Path);
            writer.Write(dirs.Length);
            foreach (var dir in dirs)
            {
                writer.Write(dir);
            }
        }

        [ServerMessageHandler( ServerMessageType.FS_WRITETOFILE), SessionRequired]
        public static void WriteBytes   (string session, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var pdata = GetPathData(content);
            var mount = GetDriveMount(pdata.DriveNumber, session);
            if (mount == null)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write("Mountpoint not found.");
                return;
            }
            try
            {
                mount.WriteAllBytes(pdata.Path, Convert.FromBase64String(pdata.AdditionalData));
                writer.Write((byte)ServerResponseType.REQ_SUCCESS);
                writer.Write(session);
            }
            catch (Exception ex)
            {
                writer.Write((byte)ServerResponseType.REQ_ERROR);
                writer.Write(session);
                writer.Write(ex.Message);
            }
        }

        [ServerMessageHandler( ServerMessageType.FS_GETMOUNTS), SessionRequired]
        public static void GetMounts(string session, BinaryReader reader, BinaryWriter writer)
        {
            var sessiondata = SessionManager.GrabAccount(session);
            var hackable = Program.GetSaveFromPrl(sessiondata.SaveID);
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session);
            writer.Write(hackable.Filesystems.Count);
            foreach(var fs in hackable.Filesystems)
            {
                writer.Write(fs.VolumeLabel);
                writer.Write(fs.DriveNumber);
            }
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
