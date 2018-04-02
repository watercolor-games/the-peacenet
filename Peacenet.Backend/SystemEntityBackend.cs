using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Backend
{
    public class SystemEntityBackend : IBackendComponent
    {
        [Dependency]
        private DatabaseHolder _database = null;

        [Dependency]
        private Backend _backend = null;

        private LiteCollection<ProtectedPort> _protectedPorts = null;
        private LiteCollection<Entity> _entities = null;
        private LiteCollection<PlayerEntityMap> _players = null;

        public event Action<string, Entity> EntitySpawned;

        private const string _asciiArt = @"               :shmMMMMMMmy+-                                          -+ymMMMMMMmhs:               
            `oNMMMMMMMMMMMMMMm+                                      +mMMMMMMMMMMMMMMNo`            
           +NMMMMMMMMMMMMMMMMMMm:                                  :mMMMMMMMMMMMMMMMMMMN+           
          yMMMMMMMMMMMMMMMMMMMMMM/                                /MMMMMMMMMMMMMMMMMMMMMMy          
         /MMMMMMMMMMMMMMMMMMMMMMMN.                              .NMMMMMMMMMMMMMMMMMMMMMMM/         
         yMMMMMMMMMMMMMMMMMMMMMMMM/                              /MMMMMMMMMMMMMMMMMMMMMMMMy         
         yMMMMMMMMMMMMMMMMMMMMMMMM/       ./oyddddddddyo/.       /MMMMMMMMMMMMMMMMMMMMMMMMy         
         oMMMMMMMMMMMMMMMMMMMMMMMM:   `/yNMMMMMMMMMMMMMMMMNy/`   :MMMMMMMMMMMMMMMMMMMMMMMMo         
         `mMMMMMMMMMMMMMMMMMMMMMMy  -yNMMMMMMMMMMMMMMMMMMMMMMNy-  yMMMMMMMMMMMMMMMMMMMMMMm`         
          .dMMMMMMMMMMMMMMMMMMMMy `sMMMMMMMMMMMMMMMMMMMMMMMMMMMMs``yMMMMMMMMMMMMMMMMMMMMd.          
            +mMMMMMMMMMMMMMMMMd: -mMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm- :dMMMMMMMMMMMMMMMMm+            
              :yNMMMMMMMMMMms-  -NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN-  -smMMMMMMMMMMNy:              
      .oyy+-     `-//////-`    `mMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm`    `-//////-`     -+yyo.      
     yMMMMMMNho:.          ..  sMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMs  ..          .:ohNMMMMMMy     
    sMMMMMMMMMMMMNmhhhhhhmNMo  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh  oMNmhhhhhhmNMMMMMMMMMMMMs    
   .NMMMMMMMMMMMMMMMMMMMMMMMo  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh  oMMMMMMMMMMMMMMMMMMMMMMMN.   
   oMMMMMMMMMMMMMMMMMMMMMMMMo  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh  oMMMMMMMMMMMMMMMMMMMMMMMMo   
   hMMMMMMMMMMMMMMMMMMMMMMMMh  oMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMo  hMMMMMMMMMMMMMMMMMMMMMMMMh   
   mMMMMMMMMMMMMMMMMMMMMMMMMM: `mMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm` :MMMMMMMMMMMMMMMMMMMMMMMMMm   
   mMMMMMMMMMMMMMMMMMMMMMMMMMm` -NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN- `mMMMMMMMMMMMMMMMMMMMMMMMMMm   
   mMMMMMMMMMMMMMMMMMMMMMMMMMMm` -mMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm- `mMMMMMMMMMMMMMMMMMMMMMMMMMMm   
   mMMMMMMMMMMMMMMMMMMMMMMMMMmmh` `sMMMMMMMMMMMMMMMMMMMMMMMMMMMMs` `hmmMMMMMMMMMMMMMMMMMMMMMMMMMm   
   oMMMMMMMMMMMMMMMMMMMNh+:.        .smMMMMMMMMMMMMMMMMMMMMMMms.        .:+hNMMMMMMMMMMMMMMMMMMMo   
    :dMMMMMMMMMMMMMMMd/` `/oyddddh+.   :ymMMMMMMMMMMMMMMMMmy:   .+hddddyo/` `/dMMMMMMMMMMMMMMMd:    
      ./shhhmNNNNNNd. `+dMMMMMMMMMMMdo.   .:+shhhddhhhs+:.   .odMMMMMMMMMMMd+` .dNNNNNNmhhhs/.      
                     +NMMMMMMMMMMMMMMMMms/-`            `-/smMMMMMMMMMMMMMMMMN+                     
                   `dMMMMMMMMMMMMMMMMMMMMMMMmdhsooooshdmMMMMMMMMMMMMMMMMMMMMMMMd`                   
                  `dMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMd`                  
                 `dMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMd`                 
                 sMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMs                 
                .MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM.                
                oMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMo                
                NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN                
               .MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM.               
               /MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM/               
               /MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM/               
               /MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM/               
               /MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM/               
               /MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM/               
                hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh                
                `dMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMd`                
                  /mMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm/                  
                    :ymMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmy:                    
                       `.:::+oooooooooooooooooooooooooooooooooooooooooo+:::.`                       

Bringing you perfectly awesome co-existing NPCs and players to Peacenet multiplayer servers since 2025.
";

        public void Initiate()
        {
            Plex.Objects.Logger.Log("PEACENET WORLD ENTITY SYSTEM");
            Plex.Objects.Logger.Log("-------------------------------------");
            Plex.Objects.Logger.Log("");
            foreach(var line in _asciiArt.Replace("\r", "").Split('\n'))
            {
                Plex.Objects.Logger.Log(line.TrimEnd());
            }
            Plex.Objects.Logger.Log("Looking up port/service info...");
            _protectedPorts = _database.Database.GetCollection<ProtectedPort>("world_protected_ports");
            _protectedPorts.EnsureIndex(x => x.Id);
            var services = Enum.GetValues(typeof(Service)).Cast<int>().ToArray();
            int deletedCount = _protectedPorts.Delete(x => !services.Contains(x.Port));
            Plex.Objects.Logger.Log($"{deletedCount} open ports deleted from database due to no known services.");
            Plex.Objects.Logger.Log($"{_protectedPorts.Count()} ports still in the database.");

            Plex.Objects.Logger.Log("Looking up entity information in the database...");
            _entities = _database.Database.GetCollection<Entity>("world_entities");
            Plex.Objects.Logger.Log("Now looking up playerid->entityid map...");
            _players = _database.Database.GetCollection<PlayerEntityMap>("world_player_entity_map");
            _entities.EnsureIndex(x => x.Id);
            _players.EnsureIndex(x => x.Id);
            Plex.Objects.Logger.Log("Looking for non-existent player entities...");
            int count = _players.Delete(y => _entities.FindOne(x => x.Id == y.EntityId) == null);
            Plex.Objects.Logger.Log($"{count} row(s) deleted from player->entityid map due to non-existent entities.");
            Plex.Objects.Logger.Log($"{_entities.Find(x => x.IsNPC == true).Count()} NPC(s) loaded. {_entities.Find(x => x.IsNPC == false).Count()} player(s) loaded.");
            _backend.PlayerJoined += (playerid, user) =>
            {
                string entityid = GetPlayerEntityId(playerid);
                if (entityid == null)
                    entityid = SpawnPlayerEntity(playerid);
                var entity = GetEntity(entityid);
                entity.DisplayName = user.display_name;
                entity.Description = "";
                _entities.Update(entity);
                Plex.Objects.Logger.Log("Updating display data for " + user.display_name);
                UpdatePorts(entityid);
            };

            int deletedPortsNoEntity = _protectedPorts.Delete(x => GetEntity(x.EntityId) == null);
            Plex.Objects.Logger.Log($"Deleted {deletedPortsNoEntity} ports from database because they're orphaned and have no entities.");
            foreach (var entity in _entities.FindAll())
                UpdatePorts(entity.Id);
        }

        public ProtectedPort[] GetPorts(string entityid)
        {
            return _protectedPorts.Find(x => x.EntityId == entityid).ToArray();
        }

        public void UpdatePorts(string entityid)
        {
            var services = Enum.GetValues(typeof(Service)).Cast<int>().ToArray();
            var entity = GetEntity(entityid);
            if (entity == null)
                return;
            if (entity.IsNPC)
                return;
            foreach (var service in services)
            {
                if (_protectedPorts.FindOne(x => x.EntityId == entityid && x.Port == service) == null)
                {
                    _protectedPorts.Insert(new ProtectedPort
                    {
                        Id = Guid.NewGuid().ToString(),
                        EntityId = entityid,
                        Port = (ushort)service,
                        SecurityLevel = 1
                    });
                    Plex.Objects.Logger.Log($"Enabled service {service} with security level 1 on {entityid}.");
                }
            }
        }

        public string GetPlayerId(string entityid)
        {
            var player = _players.FindOne(x => x.EntityId == entityid);
            if (player == null)
                return null;
            return player.ItchUserId;
        }

        public void SetupPortForNPC(string entityid, Service service, int securityLevel)
        {
            var entity = GetEntity(entityid);
            if (entity == null)
                return;
            if (!entity.IsNPC)
                return;
            var port = _protectedPorts.FindOne(x => x.EntityId == entityid && x.Port == (int)service);
            if (port == null)
            {
                _protectedPorts.Insert(new ProtectedPort
                {
                    Id = Guid.NewGuid().ToString(),
                    EntityId = entityid,
                    Port = (ushort)service,
                    SecurityLevel = securityLevel
                });
            }
            else
            {
                port.SecurityLevel = securityLevel;
                _protectedPorts.Update(port);
            }
        }

        public string SpawnNPCEntity(string name, string description)
        {
            var existing = _entities.FindOne(x => x.DisplayName == name && x.Description == description);
            if (existing != null)
                return existing.Id;
            var entity = new Entity
            {
                Id = Guid.NewGuid().ToString(),
                IsNPC = true,
                DisplayName = name,
                Description = description
            };
            _entities.Insert(entity);
            EntitySpawned?.Invoke(entity.Id, entity);
            return entity.Id;
        }

        public Entity GetEntity(string id)
        {
            return _entities.FindOne(x => x.Id == id);
        }

        public string GetPlayerEntityId(string playerid)
        {
            var player = _players.FindOne(x => x.ItchUserId == playerid);
            if (player == null)
                return null;
            return player.EntityId;
        }

        public string SpawnPlayerEntity(string playerid)
        {
            var player = _players.FindOne(x => x.ItchUserId == playerid);
            if (player != null)
                return null;
            var entity = new Entity
            {
                Id = Guid.NewGuid().ToString(),
                IsNPC = false,
                DisplayName = "",
                Description = ""
            };
            _entities.Insert(entity);
            _players.Insert(new PlayerEntityMap
            {
                Id = Guid.NewGuid().ToString(),
                EntityId = entity.Id,
                ItchUserId = playerid
            });
            Plex.Objects.Logger.Log("Spawned new player entity for " + playerid);
            EntitySpawned?.Invoke(entity.Id, entity);
            return entity.Id;
        }

        public void SafetyCheck()
        {
            _players.Delete(x => x == null);
            int count = _players.Delete(y => _entities.FindOne(x => x.Id == y.EntityId) == null);
            Plex.Objects.Logger.Log($"{count} row(s) deleted from player->entityid map due to non-existent entities.");
            int deletedPortsNoEntity = _protectedPorts.Delete(x => GetEntity(x.EntityId) == null);
            Plex.Objects.Logger.Log($"Deleted {deletedPortsNoEntity} ports from database because they're orphaned and have no entities.");
        }

        public void Unload()
        {
        }
    }

    public class Entity
    {
        public string Id { get; set; }
        public bool IsNPC { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public class PublicPort
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public ushort Port { get; set; }
    }

    public class ProtectedPort
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public ushort Port { get; set; }
        public int SecurityLevel { get; set; }
    }

    public class PlayerEntityMap
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public string ItchUserId { get; set; }
    }

    public enum Service
    {
        ftp = 21,
        ssh = 22,
    }
}
