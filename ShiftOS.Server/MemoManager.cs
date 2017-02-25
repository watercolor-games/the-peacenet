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
using NetSockets;
using Newtonsoft.Json;
using System.IO;
using static ShiftOS.Server.Program;

namespace ShiftOS.Server
{
    public static class MemoManager
    {
        [MudRequest("get_memos_for_user", typeof(Dictionary<string, object>))]
        public static void GetMemosForUser(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null)
            {
                string usrnme = args["username"].ToString();

                List<MUDMemo> mmos = new List<MUDMemo>();

                if (File.Exists("memos.json"))
                {
                    foreach (var mmo in JsonConvert.DeserializeObject<MUDMemo[]>(File.ReadAllText("memos.json")))
                    {
                        if (mmo.UserTo == usrnme)
                        {
                            mmos.Add(mmo);
                        }
                    }
                }

                server.DispatchTo(new Guid(guid), new NetObject("mud_memos", new ServerMessage
                {
                    Name = "mud_usermemos",
                    GUID = "server",
                    Contents = JsonConvert.SerializeObject(mmos)
                }));
            }

        }

        [MudRequest("mud_postmemo", typeof(MUDMemo))]
        public static void PostMemo(string guid, object contents)
        {
            MUDMemo memo = contents as MUDMemo;
            List<MUDMemo> memos = new List<MUDMemo>();

            if (File.Exists("memos.json"))
                memos = JsonConvert.DeserializeObject<List<MUDMemo>>(File.ReadAllText("memos.json"));

            memos.Add(memo);
            File.WriteAllText("memos.txt", JsonConvert.SerializeObject(memos));
        }
    }
}
