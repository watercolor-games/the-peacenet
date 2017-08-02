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
        public byte[] FindLootBytes(string id)
        {
            foreach(var res in typeof(Properties.Resources).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if(res.Name == id)
                {
                    var obj = res.GetValue(null);
                    if(obj is System.Drawing.Image)
                    {
                        var img = (obj as System.Drawing.Bitmap);
                        using(var memstr = new System.IO.MemoryStream())
                        {
                            img.Save(memstr, System.Drawing.Imaging.ImageFormat.Png);
                            return memstr.ToArray();
                        }
                    }
                    else if(obj is System.IO.UnmanagedMemoryStream)
                    {
                        var ms = obj as System.IO.MemoryStream;
                        return ms.ToArray();
                    }
                    else if(obj is string)
                    {
                        var bytes = Encoding.UTF8.GetBytes(obj.ToString());
                        return bytes;
                    }
                }
            }
            return null;
        }

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

        public Port[] GetPorts()
        {
            return JsonConvert.DeserializeObject<Port[]>(Properties.Resources.Ports);
        }

        public byte[] GetLootFromResource(string resId)
        {
            return new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }; //nyi
        }

        public Loot[] GetLoot()
        {
            return JsonConvert.DeserializeObject<Loot[]>(Properties.Resources.LootInfo);
        }
    }
}
