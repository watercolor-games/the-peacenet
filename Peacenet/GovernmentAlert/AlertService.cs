using Peacenet.Server;
using Plex.Engine;
using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.GovernmentAlert
{
    public class AlertService : IEngineComponent
    {
        [Dependency]
        private AsyncServerManager _server = null;

        private int _alertLevel = 0;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private ItchOAuthClient _itch = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public void Initiate()
        {
            _server.BroadcastReceived += _server_BroadcastReceived;
            _plexgate.GetLayer(LayerType.Foreground).AddEntity(_plexgate.New<AlertBgmEntity>());
        }

        private bool _dissipating = false;

        public bool Dissipating
        {
            get
            {
                return _dissipating;
            }
        }

        public int AlertLevel
        {
            get
            {
                return _alertLevel;
            }
        }

        private void _server_BroadcastReceived(Plex.Objects.ServerBroadcastType type, System.IO.BinaryReader reader)
        {
            if(type == Plex.Objects.ServerBroadcastType.GovernmentAlert)
            {
                string playerid = reader.ReadString();
                if (_server.IsMultiplayer)
                    if (_itch.User.id.ToString() != playerid)
                        return;
                double alertLevel = reader.ReadDouble();
                _alertLevel = (int)Math.Ceiling(alertLevel * 5);
                _dissipating = reader.ReadBoolean();
            }
        }
    }
}
