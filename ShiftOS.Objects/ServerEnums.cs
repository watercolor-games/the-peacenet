using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public enum ServerMessageType : byte
    {
        U_CONF = 0x00,
        SERVERINFO = 0x01,
        SERVERRULES = 0x02,
        WORLD = 0x03,

        FS_CREATEMOUNT = 0x10,
        FS_GETMOUNTS = 0x11,
        FS_CREATEDIR = 0x12,
        FS_GETDIRS = 0x13,
        FS_GETFILES = 0x14,
        FS_WRITETOFILE = 0x15,
        FS_READFROMFILE = 0x16,
        FS_DELETE = 0x17,
        FS_RECORDINFO = 0x18,
        FS_DIREXISTS = 0x19,
        FS_FILEEXISTS = 0x1A,

        TRM_INVOKE = 0x20,
        TRM_GETCMDS = 0x21,
        TRM_MANPAGE = 0x22,


        UPG_GETUPGRADES = 0x30,
        UPG_ISINSTALLED = 0x31,
        UPG_ISLOADED = 0x32,
        UPG_SETINSTALLED = 0x33,
        UPG_LOAD = 0x34,
        UPG_UNLOAD = 0x35,
        UPG_GETCOUNT = 0x36,

        USR_VALIDATEKEY = 0x40,
        USR_LOGIN = 0x41,
        USR_REGISTER = 0x42,
        USR_GETUSERNAME = 0x43,
        USR_GETSYSNAME = 0x44,
        USR_GETXP = 0x45,
        USR_GETCASH = 0x46,
        USR_GETRANK = 0x47,
        USR_GETNETNAME = 0x48,
        USR_GETAVATAR = 0x49,
        USR_SETAVATAR = 0x4A,
        USR_SETUSERNAME = 0x4B,
        USR_ADDXP = 0x4C,

        CHAT_JOIN = 0x50,
        CHAT_LEAVE = 0x51,
        CHAT_SENDTEXT = 0x52,
        CHAT_SENDACTION = 0x53,
        CHAT_GETUSERS = 0x54,

        ACL_GETUSERACCESSLEVEL = 0x60,
        ACL_BANUSER = 0x61,
        ACL_BANIP = 0x62,
        ACL_UNBANUSER = 0x63,
        ACL_UNBANIP = 0x64,
        ACL_GETBANNEDIPS = 0x65,
        ACL_GETBANNEDUSERS = 0x66,

        CASH_DEDUCT = 0x70,
        CASH_DEPOSIT = 0x71,

        SP_COMPLETESTORY = 0x80,
        SP_SETPICKUP = 0x81,

        STREAM_OP = 0x90,
    }
    
    public enum StreamOp : byte
    {
        get_CanRead,
        get_CanSeek,
        get_CanWrite,
        get_Length,
        get_Position,
        set_Position,
        Flush,
        Read,
        Seek,
        SetLength,
        Write,
        Close,
    }

    public enum ServerResponseType : byte
    {
        REQ_SUCCESS = 0x00,
        REQ_ERROR = 0x01,
        
        REQ_LOGINREQUIRED = 0x10,
        REQ_BANNED = 0x11,

    }

    public enum BroadcastType : byte
    {
        SRV_SHUTDOWN = 0xFF,

        SRV_WORLDSAVE = 0x00,
        SRV_ANNOUNCEMENT = 0x01,

        CHAT_USERJOINED = 0x10,
        CHAT_USERLEFT = 0x11,
        CHAT_MESSAGESENT = 0x12,
        CHAT_ACTIONSENT = 0x13,


    }

    public class ServerConfiguration
    {
        public string ServerName { get; set; }
        public string DiscordPayloadURL { get; set; }
        public int MaxPlayers = 25;
    }

    public enum ACLPermission : byte
    {
        User = 0,
        Moderator = 1,
        Admin = 2
    }

    public enum UpgradeResult : byte
    {
        UNCAUGHT_ERROR = 0xFF,
        MISSING_UPGRADE = 0x01,
        ALREADY_LOADED = 0x02,
        NO_SLOTS = 0x03,
        LOADED = 0x04,
        ALREADY_UNLOADED = 0x05,
        UNLOADED = 0x06
    }

}
