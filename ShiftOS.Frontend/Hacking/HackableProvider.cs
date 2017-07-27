using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using ShiftOS.Engine;
using Newtonsoft.Json;

namespace ShiftOS.Frontend
{
    public class HackableProvider : IHackableProvider
    {
        public Hackable[] GetHackables()
        {
            return JsonConvert.DeserializeObject<Hackable[]>(Properties.Resources.Hackables);
        }

        public Exploit[] GetExploits()
        {
            return JsonConvert.DeserializeObject<Exploit[]>(Properties.Resources.Exploits);
        }

        public Payload[] GetPayloads()
        {
            return JsonConvert.DeserializeObject<Payload[]>(Properties.Resources.Payloads);
        }

        public byte[] GetLootFromResource(string resId)
        {
            return new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }; //nyi
        }

        public LootInfo[] GetLootInfo()
        {
            return JsonConvert.DeserializeObject<LootInfo[]>(Properties.Resources.LootInfo);
        }
    }
}
