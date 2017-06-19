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

        public static UserConfig Get()
        {
            var conf = new UserConfig
            {
                Language  = "english",
                DigitalSocietyAddress = "michaeltheshifter.me",
                DigitalSocietyPort = 13370
            };

            if (!File.Exists("servers.json"))
            {
                File.WriteAllText("servers.json", JsonConvert.SerializeObject(conf, Formatting.Indented));
            }
            else
            {
                conf = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText("servers.json"));
            }
            return conf;
        }
    }
}
