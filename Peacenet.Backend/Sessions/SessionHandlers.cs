using System;
using System.IO;
using Peacenet.Backend.Saves;
using Plex.Objects;

namespace Peacenet.Backend.Sessions
{
    public class LoginHandler : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.USR_LOGIN;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var savemgr = backend.GetBackendComponent<SaveManager>();

            string username = datareader.ReadString();
            string password = datareader.ReadString();
            string sessionid;
            var sessionmanager = backend.GetBackendComponent<SessionManager>();
            if (backend.IsMultiplayer)
            {
                if (sessionmanager.Login(username, password, out sessionid) != LoginStatus.Success)
                {
                    return ServerResponseType.REQ_ERROR;
                }
            }
            else
            {
                sessionid = sessionmanager.CreateSinglePlayerSession();
                username = "user";
            }
            if (savemgr.GetSave(username) == null)
            {
                savemgr.AddSave(username);
            }

            datawriter.Write(sessionid);
            return ServerResponseType.REQ_SUCCESS;
        
        }
    }

    public class UsernameRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.USR_GETUSERNAME;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var sessionmgr = backend.GetBackendComponent<SessionManager>();
            datawriter.Write(sessionmgr.GetUserFromSession(session).Username);
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    public class SysnameRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.USR_GETSYSNAME;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var sessionmgr = backend.GetBackendComponent<SessionManager>();
            var savemgr = backend.GetBackendComponent<SaveManager>();
            var username = sessionmgr.GetUserFromSession(session).Username;
            datawriter.Write(savemgr.GetSave(username).SystemName);
            return ServerResponseType.REQ_SUCCESS;
        }
    }

}
