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
        [MudRequest("get_memos_for_user")]
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

        [MudRequest("mud_postmemo")]
        public static void PostMemo(string guid, object contents)
        {
            MUDMemo memo = JsonConvert.DeserializeObject<MUDMemo>(contents as string);
            List<MUDMemo> memos = new List<MUDMemo>();

            if (File.Exists("memos.json"))
                memos = JsonConvert.DeserializeObject<List<MUDMemo>>(File.ReadAllText("memos.json"));

            memos.Add(memo);
            File.WriteAllText("memos.txt", JsonConvert.SerializeObject(memos));
        }
    }
}
