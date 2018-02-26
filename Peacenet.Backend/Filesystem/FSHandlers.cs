using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Peacenet.Backend.Filesystem;
using Plex.Objects;

namespace Peacenet.Backend
{
    /// <summary>
    /// Handler for checking if a directory exists
    /// </summary>
    [RequiresSession]
    public class DirectoryExistsHandler : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;
        
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_DIREXISTS;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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

    /// <summary>
    /// Handler for checking if a file exists.
    /// </summary>
    [RequiresSession]
    public class FileExistsHandler : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;


        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_FILEEXISTS;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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

    /// <summary>
    /// Handler for creating a directory.
    /// </summary>
    [RequiresSession]
    public class DirectoryCreator : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;


        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_CREATEDIR;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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

    /// <summary>
    /// Handler for retrieving a file list.
    /// </summary>
    [RequiresSession]
    public class FileListRetriever : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_GETFILES;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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

    /// <summary>
    /// Handler for retrieving a mount list.
    /// </summary>
    [RequiresSession]
    public class MountListRetriever : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_GETMOUNTS;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var username = _entityBackend.GetPlayerEntityId(session);
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

    /// <summary>
    /// Handler for retrieving a directory list.
    /// </summary>
    [RequiresSession]
    public class DirListRetriever : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_GETDIRS;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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

    /// <summary>
    /// Handler for retrieving a file record.
    /// </summary>
    [RequiresSession]
    public class FileRecordRetriever : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_RECORDINFO;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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
        }
    }

    /// <summary>
    /// Handler for deleting a file or directory.
    /// </summary>
    [RequiresSession]
    public class FileDeletor : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_DELETE;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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

    /// <summary>
    /// Handler for reading a file.
    /// </summary>
    [RequiresSession]
    public class FileReader : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_READFROMFILE;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    /// <summary>
    /// Handler for writing to a file.
    /// </summary>
    [RequiresSession]
    public class FileWriter : IMessageHandler
    {
        [Dependency]
        private FSManager drivemgr = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.FS_WRITETOFILE;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string content = datareader.ReadString();
            string path;
            var username = _entityBackend.GetPlayerEntityId(session);
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
