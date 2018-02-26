using Peacenet.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Plex.Engine;
using Newtonsoft.Json;

namespace Peacenet
{
    public class NPCSpawner : IBackendComponent
    {
        [Peacenet.Backend.Dependency]
        private SystemEntityBackend _entityBackend = null;

        [Peacenet.Backend.Dependency]
        private IPBackend _ip = null;

        private string _gameContentDirectory = "";

        public void Initiate()
        {
            _gameContentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Hackables");
            if (!Directory.Exists(_gameContentDirectory))
                Directory.CreateDirectory(_gameContentDirectory);
            foreach(var file in Directory.GetFiles(_gameContentDirectory))
            {
                if(file.ToLower().EndsWith(".json"))
                {
                    var json = File.ReadAllText(file);
                    try
                    {
                        var entityInfo = JsonConvert.DeserializeObject<ProcgenEntity>(json);
                        var entity = _entityBackend.SpawnNPCEntity(entityInfo.Name, entityInfo.Description);
                        uint ipaddr = _ip.GetIPFromString(entityInfo.IPAddress);
                        if (_ip.GrabEntity(ipaddr) == null)
                            _ip.AllocateIPv4Address(ipaddr, entity);

                    }
                    catch(Exception ex)
                    {
                        Logger.Log("Cannot create NPC entity from description file. " + ex.ToString());
                    }
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

    public class ProcgenEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IPAddress { get; set; }

        public string[] LootableProgramIDs { get; set; }
    }
}
