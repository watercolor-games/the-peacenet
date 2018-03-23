using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Backend
{
    public class ReputationManager : IBackendComponent
    {
        [Dependency]
        private DatabaseHolder _database = null;

        private LiteCollection<GovernmentAlertState> _gvtAlertStates = null;
        
        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        [Dependency]
        private Backend _backend = null;

        public void Initiate()
        {
            Logger.Log("The Government is reviewing its database on targeted sentiences...");
            _gvtAlertStates = _database.Database.GetCollection<GovernmentAlertState>("world_gvtalert");
            _gvtAlertStates.EnsureIndex(x => x.Id);
            _entityBackend.EntitySpawned += (id, entity) =>
            {
                var alert = _gvtAlertStates.FindOne(x => x.EntityId == id);
                if (alert == null)
                {
                    alert = new GovernmentAlertState
                    {
                        Id = Guid.NewGuid().ToString(),
                        EntityId = id,
                        AlertLevel = 0
                    };
                    _gvtAlertStates.Insert(alert);
                }
            };
            _backend.PlayerJoined += (id, player) =>
            {
                var entity = _entityBackend.GetPlayerEntityId(id);
                var alert = _gvtAlertStates.FindOne(x => x.EntityId == entity);
                if (alert == null)
                {
                    alert = new GovernmentAlertState
                    {
                        Id = Guid.NewGuid().ToString(),
                        EntityId = entity,
                        AlertLevel = 0
                    };
                    _gvtAlertStates.Insert(alert);
                }


            };
        }

        private void dispatchAlertLevel(string id, float alert)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    writer.Write(id);
                    writer.Write(alert);
                    writer.Flush();
                    _backend.BroadcastToPlayer(Plex.Objects.ServerBroadcastType.GovernmentAlert, ms.ToArray(), id);
                }
            }
        }

        public void SafetyCheck()
        {
        }

        public void Unload()
        {
        }
    }

    public class GovernmentAlertState
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public float AlertLevel { get; set; }
    }
}
