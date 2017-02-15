using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ShiftOS.Objects;
using static ShiftOS.Server.Program;
using NetSockets;

namespace ShiftOS.Server
{
    public static class LegionManager
    {
        [MudRequest("legion_createnew", typeof(Legion))]
        public static void CreateLegion(string guid, object contents)
        {
            List<Legion> legions = new List<Legion>();
            if (File.Exists("legions.json"))
                legions = JsonConvert.DeserializeObject<List<Legion>>(File.ReadAllText("legions.json"));

            var l = contents as Legion;
            bool legionExists = false;

            foreach (var legion in legions)
            {
                if (legion.ShortName == l.ShortName)
                    legionExists = true;
            }

            if (legionExists == false)
            {
                legions.Add(l);
                server.DispatchTo(new Guid(guid), new NetObject("test", new ServerMessage
                {
                    Name = "legion_create_ok",
                    GUID = "server"
                }));

            }
            else
            {
                server.DispatchTo(new Guid(guid), new NetObject("test", new ServerMessage
                {
                    Name = "legion_alreadyexists",
                    GUID = "server"
                }));
            }

            File.WriteAllText("legions.json", JsonConvert.SerializeObject(legions, Formatting.Indented));

        }

        [MudRequest("legion_get_all", null)]
        public static void GetAllLegions(string guid, object contents)
        {
            List<Legion> allLegions = new List<Legion>();

            if (File.Exists("legions.json"))
                allLegions = JsonConvert.DeserializeObject<List<Legion>>(File.ReadAllText("legions.json"));

            server.DispatchTo(new Guid(guid), new NetObject("alllegions", new ServerMessage
            {
                Name = "legion_all",
                GUID = "server",
                Contents = JsonConvert.SerializeObject(allLegions)
            }));

        }
        
        [MudRequest("legion_get_users", typeof(Legion))]
        public static void GetLegionUsers(string guid, object contents)
        {
            var lgn = contents as Legion;

            List<string> userIDs = new List<string>();

            foreach (var savfile in Directory.GetFiles("saves"))
            {
                try
                {
                    var savefilecontents = JsonConvert.DeserializeObject<Save>(File.ReadAllText(savfile));
                    if (savefilecontents.CurrentLegions.Contains(lgn.ShortName))
                    {
                        userIDs.Add($"{savefilecontents.Username}@{savefilecontents.SystemName}");
                    }
                }
                catch { }
            }

            server.DispatchTo(new Guid(guid), new NetObject("userlist", new ServerMessage
            {
                Name = "legion_users_found",
                GUID = "server",
                Contents = JsonConvert.SerializeObject(userIDs)
            }));

        }

        [MudRequest("user_get_legion", typeof(Save))]
        public static void GetUserLegion(string guid, object contents)
        {
            var userSave = contents as Save;

            if (File.Exists("legions.json"))
            {
                var legionList = JsonConvert.DeserializeObject<List<Legion>>(File.ReadAllText("legions.json"));
                foreach (var legion in legionList)
                {
                    if (userSave.CurrentLegions.Contains(legion.ShortName))
                    {
                        server.DispatchTo(new Guid(guid), new NetObject("reply", new ServerMessage
                        {
                            Name = "user_legion",
                            GUID = "server",
                            Contents = JsonConvert.SerializeObject(legion)
                        }));
                        return;
                    }
                }
            }

            server.DispatchTo(new Guid(guid), new NetObject("fuck", new ServerMessage
            {
                Name = "user_not_found_in_legion",
                GUID = "server"
            }));

        }
    }
}
