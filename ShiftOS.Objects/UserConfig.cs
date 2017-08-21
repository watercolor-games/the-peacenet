using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plex.Objects
{
    //enums
    public enum BloomPresets
    {
        [FriendlyDescription("Absolutely no bloom whatsoever in the UI. Almost as if you're running an actual operating system and not a game. Good on resources as there are no shaders to use, but man.... so... boring...")]
        None,
        [FriendlyDescription("It's a step up from no bloom at all as you get a pretty decent bloom effect with very little performance impact. I mean, the fuck else did you expect? It's called CHEAP for a REASON.")]
        Cheap,
        [FriendlyDescription("This one's there so you can have glowy effects without fucking up the UI readability. It looks cool, performs well, and let's you stay focused on the fucking game.")]
        Focused,
        [FriendlyDescription("MOTHER OF GOD THAT'S FUCKING BLINDINGLY BRIGHT. It's like that damn solar eclipse on August 21st, 2017. I swear too fucking much.")]
        One,
        [FriendlyDescription("Small amount of glow. The fuck else did you expect?")]
        Small,
        [FriendlyDescription("No funny descriptions for this.")]
        Wide,
        [FriendlyDescription("ERHMERGERD SO PURDY")]
        SuperWide,
    };

    public class FriendlyDescription : Attribute
    {
        public FriendlyDescription(string desc)
        {
            Description = desc;
        }

        public string Description { get; private set; }
    }

    public class UserConfig
    {
        public string Language { get; set; }
        public string DigitalSocietyAddress { get; set; }
        public int DigitalSocietyPort { get; set; }
        public int ScreenWidth = 1920;
        public int ScreenHeight = 1080;
        public bool Fullscreen = true;
        public BloomPresets BloomPreset = BloomPresets.SuperWide;

        private static UserConfig def = new UserConfig
        {
            Language = "english",
            DigitalSocietyAddress = "getPlex.net",
            DigitalSocietyPort = 13370,
            Fullscreen = true,
            ScreenWidth = 1920,
            ScreenHeight = 1080,
            BloomPreset = BloomPresets.SuperWide
            };

        public static UserConfig current = null;

    public static UserConfig Get()
        {
            if (current != null)
                return current;
            if (File.Exists("config.json"))
                current = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText("config.json"));
            else
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(def, Formatting.Indented));
                current = def;
            }
            return current;
        }
    }
}
