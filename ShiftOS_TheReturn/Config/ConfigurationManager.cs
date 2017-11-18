using System;
using System.IO;
using Newtonsoft.Json;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Plex.Engine.Config
{
    public static class ConfigurationManager
    {
        private static ConfigFile _config = ConfigFile.Default;

        public static Resolution GetSystemResolution()
        {
            int w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            return new Resolution(w, h);

        }

        public static void SetFullscreen(bool value)
        {
            _config.Fullscreen = value;
        }

        public static bool GetFullscreen()
        {
            return _config.Fullscreen;
        }

        public static void SetResolution(int index)
        {
            _config.ResolutionIndex = index;
        }

        public static Resolution[] GetSupportedResolutions()
        {
            List<Resolution> resolutions = new List<Resolution>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) 
            {
                int w = mode.Width;
                int h = mode.Height;
                if (w < 800 && h < 600)
                    continue;
                if(resolutions.FirstOrDefault(x=>x.Width == w && x.Height == h) == null)
                {
                    resolutions.Add(new Resolution(w, h));
                }
            }
            return resolutions.OrderByDescending(x=>x.Width * x.Height).ToArray();
        }

        public static void SetRPCEnable(bool value)
        {
            _config.EnableDiscordRichPresence = value;
        }

        public static bool GetRPCEnable()
        {
            return _config.EnableDiscordRichPresence;
        }

        public static Resolution GetResolution()
        {
            var resolutions = GetSupportedResolutions();

            int index = _config.ResolutionIndex;

            if(index < 0 || index >= resolutions.Length)
                return resolutions.Last();
            return resolutions[index];
        }

        public static void SaveConfig()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(_config, Formatting.Indented));
        }

        public static void ApplyConfig()
        {
            SaveConfig();
            UIManager.Game.ApplyConfig();
        }

        public static void LoadConfig()
        {
            if(!File.Exists("config.json"))
                _config = ConfigFile.Default;
            else
                _config = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText("config.json"));
        }
    }

    public class Resolution
    {
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
