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

        private LiteCollection<Entity> _entities = null;
        private LiteCollection<PlayerEntityMap> _players = null;

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
            Logger.Log("PEACENET WORLD ENTITY SYSTEM");
            Logger.Log("-------------------------------------");
            Logger.Log("");
            foreach(var line in _asciiArt.Replace("\r", "").Split('\n'))
            {
                Logger.Log(line.TrimEnd());
            }
            Logger.Log("Looking up entity information in the database...");
            _entities = _database.Database.GetCollection<Entity>("world_entities");
            Logger.Log("Now looking up playerid->entityid map...");
            _players = _database.Database.GetCollection<PlayerEntityMap>("world_player_entity_map");
            _entities.EnsureIndex(x => x.Id);
            _players.EnsureIndex(x => x.Id);
            Logger.Log("Looking for non-existent player entities...");
            int count = _players.Delete(y => _entities.FindOne(x => x.Id == y.EntityId) == null);
            Logger.Log($"{count} row(s) deleted from player->entityid map due to non-existent entities.");
            Logger.Log($"{_entities.Find(x => x.IsNPC == true).Count()} NPC(s) loaded. {_entities.Find(x => x.IsNPC == false).Count()} player(s) loaded.");
            _backend.PlayerJoined += (playerid, user) =>
            {
                string entityid = GetPlayerEntityId(playerid);
                if (entityid == null)
                    entityid = SpawnPlayerEntity(playerid);
                var entity = GetEntity(entityid);
                entity.DisplayName = user.display_name;
                entity.Description = "";
                Logger.Log("Updating display data for " + user.display_name);
            };
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
            Logger.Log("Spawned new player entity for " + playerid);
            return entity.Id;
        }

        public void SafetyCheck()
        {
            _players.Delete(x => x == null);
            int count = _players.Delete(y => _entities.FindOne(x => x.Id == y.EntityId) == null);
            Logger.Log($"{count} row(s) deleted from player->entityid map due to non-existent entities.");
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

    public class PlayerEntityMap
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public string ItchUserId { get; set; }
    }
}
