using System;
namespace Plex.Engine.Config
{
    public struct ConfigFile
    {
        public int ResolutionIndex { get; set; }
        public bool Fullscreen { get; set; }
        public bool EnableDiscordRichPresence { get; set; }

        public static ConfigFile Default
        {
            get
            {
                return new ConfigFile
                {
                    ResolutionIndex = -1,
                    Fullscreen = true,
                    EnableDiscordRichPresence = false
                };
            }
        }
    }
}
