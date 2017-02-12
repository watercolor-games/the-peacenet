using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using Newtonsoft.Json;
using System.IO;
using static ShiftOS.Server.Program;
using NetSockets;

namespace ShiftOS.Server
{
    public static class PongHighscores
    {
        [MudRequest("pong_gethighscores")]
        public static void GetHighScores(string guid, object contents)
        {
            if (File.Exists("pong_highscores.json"))
            {
                server.DispatchTo(new Guid(guid), new NetObject("pongstuff", new ServerMessage
                {
                    Name = "pong_highscores",
                    GUID = "server",
                    Contents = File.ReadAllText("pong_highscores.json")
                }));
            }

        }

        [MudRequest("pong_sethighscores")]
        public static void PostHighscores(string guid, object contents)
        {
            var hs = new List<PongHighscore>();
            if (File.Exists("pong_highscores.json"))
                hs = JsonConvert.DeserializeObject<List<PongHighscore>>(File.ReadAllText("pong_highscores.json"));

            var newHS = JsonConvert.DeserializeObject<PongHighscore>(contents as string);
            for (int i = 0; i <= hs.Count; i++)
            {
                try
                {
                    if (hs[i].UserName == newHS.UserName)
                    {
                        if (newHS.HighestLevel > hs[i].HighestLevel)
                            hs[i].HighestLevel = newHS.HighestLevel;
                        if (newHS.HighestCodepoints > hs[i].HighestCodepoints)
                            hs[i].HighestCodepoints = newHS.HighestCodepoints;
                        File.WriteAllText("pong_highscores.json", JsonConvert.SerializeObject(hs));
                        return;

                    }
                }
                catch
                {

                }
            }
            hs.Add(newHS);
            File.WriteAllText("pong_highscores.json", JsonConvert.SerializeObject(hs));

        }
    }
}
