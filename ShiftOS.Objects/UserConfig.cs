using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShiftOS.Objects
{
    public class UserConfig
    {
        public string Language { get; set; }
        public string DigitalSocietyAddress { get; set; }
        public int DigitalSocietyPort { get; set; }

        private static UserConfig def = new UserConfig
            {
                Language  = "english",
                DigitalSocietyAddress = "michaeltheshifter.me",
                DigitalSocietyPort = 13370
            };

        public static UserConfig current = null;

    public static UserConfig Get()
        {
            if (current != null)
                return current;
            if (File.Exists("servers.json"))
                current = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText("servers.json"));
            else
            {
                File.WriteAllText("servers.json", JsonConvert.SerializeObject(def, Formatting.Indented));
                current = def;
            }
            return current;
        }
    }
}
