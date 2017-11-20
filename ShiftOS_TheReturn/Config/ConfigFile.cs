using System;
namespace Plex.Engine.Config
{
    public struct ConfigFile
    {
        public int ResolutionIndex { get; set; }
        public bool Fullscreen { get; set; }
        public bool EnableDiscordRichPresence { get; set; }
        public string TextRendererClass { get; set; }

        public static ConfigFile Default
        {
            get
            {
                return new ConfigFile
                {
                    ResolutionIndex = -1,
                    Fullscreen = true,
                    EnableDiscordRichPresence = false,
                    TextRendererClass = getDefaultRendererName()
                };
            }
        }

        internal static string getDefaultRendererName()
        {
            var dRenderer = TextRenderer.GetDefaultRenderer();
            if (dRenderer == null)
            {
                var fRenderer = TextRenderer.GetFallbackRenderer();
                if (fRenderer == null)
                    throw new Exception("Something has gone horribly, horribly wrong. We could not find a valid default or fallback text renderer.");
                return fRenderer.GetType().Name;
            }
            return dRenderer.GetType().Name;
        }
    }
}
