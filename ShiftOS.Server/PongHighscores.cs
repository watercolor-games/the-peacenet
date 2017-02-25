/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
        [MudRequest("pong_gethighscores", null)]
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

        [MudRequest("pong_sethighscores", typeof(PongHighscore))]
        public static void PostHighscores(string guid, object contents)
        {
            var hs = new List<PongHighscore>();
            if (File.Exists("pong_highscores.json"))
                hs = JsonConvert.DeserializeObject<List<PongHighscore>>(File.ReadAllText("pong_highscores.json"));

            var newHS = contents as PongHighscore;
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
