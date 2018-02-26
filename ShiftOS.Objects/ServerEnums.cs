using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    /// <summary>
    /// Represents a Peacenet server request type.
    /// </summary>
    public enum ServerMessageType : byte
    {
        /// <summary>
        /// Retrieve the current server configuration (singleplayer or multiplayer).
        /// </summary>
        U_CONF = 0x00,

        /// <summary>
        /// Create a user FS if it doesn't exist.
        /// </summary>
        FS_CREATEMOUNT = 0x10,
        /// <summary>
        /// Retrieve a list of FSes for the current user.
        /// </summary>
        FS_GETMOUNTS = 0x11,
        /// <summary>
        /// Create a directory on a user FS.
        /// </summary>
        FS_CREATEDIR = 0x12,
        /// <summary>
        /// Retrieve a list of subdirectories in a directory on a user FS.
        /// </summary>
        FS_GETDIRS = 0x13,
        /// <summary>
        /// Retrieve a list of files in a directory on a user FS.
        /// </summary>
        FS_GETFILES = 0x14,
        /// <summary>
        /// Write binary data to a file on a user FS. Overwrites if the file already exists.
        /// </summary>
        FS_WRITETOFILE = 0x15,
        /// <summary>
        /// Read binary data from a file on a user FS.
        /// </summary>
        FS_READFROMFILE = 0x16,
        /// <summary>
        /// Delete a file or directory from a user FS.
        /// </summary>
        FS_DELETE = 0x17,
        /// <summary>
        /// Retrieve file/directory record info (such as name, size, etc) from a user FS.
        /// </summary>
        FS_RECORDINFO = 0x18,
        /// <summary>
        /// Retrieve whether a specified directory exists on a user FS.
        /// </summary>
        FS_DIREXISTS = 0x19,
        /// <summary>
        /// Retrieve whether a specified file exists on a user FS.
        /// </summary>
        FS_FILEEXISTS = 0x1A,

        /// <summary>
        /// Invoke a server-side Terminal Command and retrieve the command's output.
        /// </summary>
        TRM_INVOKE = 0x20,
        /// <summary>
        /// Retrieve a list of server-side Terminal Commands.
        /// </summary>
        TRM_GETCMDS = 0x21,
        /// <summary>
        /// Retrieve the manual page of a server-side Terminal Command.
        /// </summary>
        TRM_MANPAGE = 0x22,
        
        /// <summary>
        /// Log into the server chat.
        /// </summary>
        CHAT_JOIN = 0x50,
        /// <summary>
        /// Log out of the server chat.
        /// </summary>
        CHAT_LEAVE = 0x51,
        /// <summary>
        /// Send text to the chat.
        /// </summary>
        CHAT_SENDTEXT = 0x52,
        /// <summary>
        /// Send a third-person action to the chat (example: "Alkaline loves coding").
        /// </summary>
        CHAT_SENDACTION = 0x53,
        /// <summary>
        /// Retrieve the usernames of all people in chat.
        /// </summary>
        CHAT_GETUSERS = 0x54,

        /// <summary>
        /// Perform a remote stream operation.
        /// </summary>
        STREAM_OP = 0x90,

        /// <summary>
        /// Retrieve a value from a user's save file.
        /// </summary>
        SAVE_GETVAL = 0xA0,
        /// <summary>
        /// Set a value in a user's save file.
        /// </summary>
        SAVE_SETVAL = 0xA1,

        /// <summary>
        /// Create a snapshot of the current save file for the user. Only works in Single-player.
        /// </summary>
        SAVE_TAKESNAPSHOT = 0xA2,

        /// <summary>
        /// Restore the current user's save file to an existing snapshot. Only works in Single-player.
        /// </summary>
        SAVE_RESTORESNAPSHOT = 0xA3,

        SP_SIMULATE_CONNECTION_TO_PLAYER = 0xA4
    }
    
    /// <summary>
    /// Represents a server broadcast message type.
    /// </summary>
    public enum ServerBroadcastType : byte
    {
        /// <summary>
        /// The server is shutting down. The client must disconnect NOW.
        /// </summary>
        Shutdown = 0x00,

        /// <summary>
        /// A user has joined the chat.
        /// </summary>
        Chat_UserJoin = 0x10,
        /// <summary>
        /// A user has left the chat.
        /// </summary>
        Chat_UserLeave = 0x11,
        /// <summary>
        /// A user has sent a third-person action to the chat.
        /// </summary>
        Chat_Action = 0x12,
        /// <summary>
        /// A user has sent text to the chat.
        /// </summary>
        Chat_Message = 0x13,

        SYSTEM_CONNECTED = 0x20
    }

    /// <summary>
    /// Encapsulates a Peacenet broadcast datagram.
    /// </summary>
    public class PlexBroadcast
    {
        private byte[] _data = null;

        /// <summary>
        /// Creates a new instance of the <see cref="PlexBroadcast"/> class. 
        /// </summary>
        /// <param name="type">The type of the broadcast</param>
        /// <param name="content">The content of the broadcast's body.</param>
        public PlexBroadcast(ServerBroadcastType type, byte[] content)
        {
            if (content.Length == 0)
                content = new byte[0];
            _data = content;
            Type = type;
        }

        /// <summary>
        /// Retrieves the type of the broadcast.
        /// </summary>
        public ServerBroadcastType Type { get; private set; }

        /// <summary>
        /// Opens a binary reader that reads the message's content.
        /// </summary>
        /// <returns>A <see cref="System.IO.BinaryReader"/> capable of reading the content of the message.</returns>
        public System.IO.BinaryReader OpenStream()
        {
            return new System.IO.BinaryReader(new System.IO.MemoryStream(_data), Encoding.UTF8, false);
        }
    }

    /// <summary>
    /// Represents a remote stream operation.
    /// </summary>
    public enum StreamOp : byte
    {
        /// <summary>
        /// Can the stream be read from?
        /// </summary>
        get_CanRead,
        /// <summary>
        /// Can you seek the stream?
        /// </summary>
        get_CanSeek,
        /// <summary>
        /// Can you write to the stream?
        /// </summary>
        get_CanWrite,
        /// <summary>
        /// Retrieves the length of the stream.
        /// </summary>
        get_Length,
        /// <summary>
        /// Retrieves the read position of the stream.
        /// </summary>
        get_Position,
        /// <summary>
        /// Sets the read position of the stream.
        /// </summary>
        set_Position,
        /// <summary>
        /// Flushes the stream.
        /// </summary>
        Flush,
        /// <summary>
        /// Reads from the stream.
        /// </summary>
        Read,
        /// <summary>
        /// Seeks the stream.
        /// </summary>
        Seek,
        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        SetLength,
        /// <summary>
        /// Writes to the stream.
        /// </summary>
        Write,
        /// <summary>
        /// Closes the stream.
        /// </summary>
        Close,
    }

    /// <summary>
    /// Represents a server response type.
    /// </summary>
    public enum ServerResponseType : byte
    {
        /// <summary>
        /// The request was successfully handled. Carry on.
        /// </summary>
        REQ_SUCCESS = 0x00,
        /// <summary>
        /// An error occurred with the request.
        /// </summary>
        REQ_ERROR = 0x01,
        
        /// <summary>
        /// The request was made anonymously but requires a Watercolor Games log in.
        /// </summary>
        REQ_LOGINREQUIRED = 0x10,
        /// <summary>
        /// The user who made the request is banned from the server or Watercolor community.
        /// </summary>
        REQ_BANNED = 0x11,

    }
}
