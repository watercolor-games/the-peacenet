using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Peacenet.Backend.Filesystem;
using Peacenet.Backend.Sessions;
using Plex.Objects;

namespace Peacenet.Backend
{
    [RequiresSession]
    public class DriveCreator : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_CREATEMOUNT;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var sessionmgr = backend.GetBackendComponent<SessionManager>();
            var drivemgr = backend.GetBackendComponent<FSManager>();

            string content = datareader.ReadString();
            var volumeinfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            int dnum = Convert.ToInt32(volumeinfo["volume"]);
            string label = volumeinfo["label"].ToString();

            drivemgr.CreateFS(session, dnum, label);

            return 0x00;

        }
    }

    [RequiresSession]
    public class DirectoryExistsHandler : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_DIREXISTS;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            bool result = false;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount != null)
            {
                if (!path.Contains("/"))
                    result = true;
                else
                {
                    result = mount.DirectoryExists(path);
                }
            }
            datawriter.Write(result);
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    [RequiresSession]
    public class FileExistsHandler : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_FILEEXISTS;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            bool result = false;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount != null)
            {
                if (!path.Contains("/"))
                    result = true;
                else
                {
                    result = mount.FileExists(path);
                }
            }
            datawriter.Write(result);
            return ServerResponseType.REQ_SUCCESS;
        }
    }


    [RequiresSession]
    public class DirectoryCreator : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_CREATEDIR;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount == null)
            {
                datawriter.Write("Mountpoint not found.");
                return ServerResponseType.REQ_ERROR;
            }
            if (!path.Contains("/"))
            {
                datawriter.Write("Invalid pathspec.");
                return ServerResponseType.REQ_ERROR;
            }
            try
            {
                mount.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                datawriter.Write(ex.Message);
                return ServerResponseType.REQ_ERROR;
            }
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    [RequiresSession]
    public class FileListRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_GETFILES;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount == null)
            {
                datawriter.Write("Mountpoint not found.");
                return ServerResponseType.REQ_ERROR;
            }
            if (!mount.DirectoryExists(path))
            {
                datawriter.Write("Directory not found..");
                return ServerResponseType.REQ_ERROR;
            }
            try
            {
                var files = mount.GetFiles(path);
                datawriter.Write(files.Length);
                foreach (var file in files)
                    datawriter.Write(file);
            }
            catch (Exception ex)
            {
                datawriter.Write(ex.Message);
                return ServerResponseType.REQ_ERROR;
            }
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    [RequiresSession]
    public class MountListRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_GETMOUNTS;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            var username = session;
            var mounts = drivemgr.GetDrivesForUser(username);
            datawriter.Write(mounts.Count);
            foreach (var mount in mounts)
            {
                datawriter.Write(mount.Value);
                datawriter.Write(mount.Key);
            }
            return ServerResponseType.REQ_SUCCESS;
        }
    }


    [RequiresSession]
    public class DirListRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_GETDIRS;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount == null)
            {
                datawriter.Write("Mountpoint not found.");
                return ServerResponseType.REQ_ERROR;
            }
            if (!mount.DirectoryExists(path))
            {
                datawriter.Write("Directory not found..");
                return ServerResponseType.REQ_ERROR;
            }
            try
            {
                var files = mount.GetDirectories(path);
                datawriter.Write(files.Length);
                foreach (var file in files)
                    datawriter.Write(file);
            }
            catch (Exception ex)
            {
                datawriter.Write(ex.Message);
                return ServerResponseType.REQ_ERROR;
            }
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    [RequiresSession]
    public class FileRecordRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_RECORDINFO;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount == null)
            {
                datawriter.Write("Mountpoint not found.");
                return ServerResponseType.REQ_ERROR;
            }
            if (!mount.FileExists(path) && !mount.DirectoryExists(path))
            {
                datawriter.Write("File or directory not found..");
                return ServerResponseType.REQ_ERROR;
            }
            try
            {
                var finf = mount.GetFileInfo(path);
                datawriter.Write(JsonConvert.SerializeObject(finf));
                return ServerResponseType.REQ_SUCCESS;
            }
            catch (Exception ex)
            {
                datawriter.Write(ex.Message);
                return ServerResponseType.REQ_ERROR;
            }
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    [RequiresSession]
    public class FileDeleter : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_DELETE;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount == null)
            {
                datawriter.Write("Mountpoint not found.");
                return ServerResponseType.REQ_ERROR;
            }
            if (mount.FileExists(path))
            {
                mount.DeleteFile(path);
                return ServerResponseType.REQ_SUCCESS;
            }
            if (mount.DirectoryExists(path))
            {
                mount.DeleteDirectory(path);
                return ServerResponseType.REQ_SUCCESS;
            }
            datawriter.Write("File or directory not found.");
            return ServerResponseType.REQ_ERROR;
        }
    }

    [RequiresSession]
    public class FileReader : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_READFROMFILE;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount == null)
            {
                datawriter.Write("Mountpoint not found.");
                return ServerResponseType.REQ_ERROR;
            }
            if (!mount.FileExists(path))
            {
                datawriter.Write("File not found.");
                return ServerResponseType.REQ_ERROR;
            }
            var bytes = mount.ReadAllBytes(path);
            datawriter.Write(bytes.Length);
            datawriter.Write(bytes);
            return ServerResponseType.REQ_ERROR;
        }
    }

    [RequiresSession]
    public class FileWriter : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_WRITETOFILE;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var drivemgr = backend.GetBackendComponent<FSManager>();
            var sessionmgr = backend.GetBackendComponent<SessionManager>();

            string content = datareader.ReadString();
            string path;
            var username = session;
            var mount = drivemgr.GetDriveFromPathData(username, content, out path);
            if (mount == null)
            {
                datawriter.Write("Mountpoint not found.");
                return ServerResponseType.REQ_ERROR;
            }
            if (mount.DirectoryExists(path))
            {
                datawriter.Write("Cannot write file data to a directory.");
                return ServerResponseType.REQ_ERROR;
            }
            try
            {
                int len = datareader.ReadInt32();
                byte[] bytes = datareader.ReadBytes(len);
                mount.WriteAllBytes(path, bytes);
                return ServerResponseType.REQ_SUCCESS;
            }
            catch (Exception ex)
            {
                datawriter.Write(ex.Message);
                return ServerResponseType.REQ_ERROR;
            }
        }
    }


}
