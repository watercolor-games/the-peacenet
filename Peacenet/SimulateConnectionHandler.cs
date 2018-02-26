using Peacenet.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;

namespace Peacenet
{
    public class SimulateConnectionHandler : IMessageHandler
    {
        [Dependency]
        private IPBackend _ip = null;

        [Dependency]
        private SystemEntityBackend _entityManager = null;

        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.SP_SIMULATE_CONNECTION_TO_PLAYER;
            }
        }

        public ServerResponseType HandleMessage(Backend.Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            if (backend.IsMultiplayer)
                return ServerResponseType.REQ_ERROR;
            uint ipaddr = datareader.ReadUInt32();
            if (_ip.GrabEntity(ipaddr) == null)
                return ServerResponseType.REQ_ERROR;
            string entity = _entityManager.GetPlayerEntityId(session);
            var playerIp = _ip.FetchAllIPs(entity);
            if(playerIp.Length==0)
                return ServerResponseType.REQ_ERROR;
            try
            {
                _ip.MakeConnection(ipaddr, playerIp[0].Address);
                return ServerResponseType.REQ_SUCCESS;
            }
            catch
            {
                return ServerResponseType.REQ_ERROR;
            }
        }
    }
}
