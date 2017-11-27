using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using System.IO;
using Newtonsoft.Json;

namespace Plex.Engine.Config
{
    public class ConfigManager : IEngineComponent
    {
        [Dependency]
        private Plexgate _plexgate = null;

        private CoreConfig _config = null;

        public void Initiate()
        {
            Logger.Log("Starting configuration manager...");
            if (!File.Exists("config.json"))
            {
                Logger.Log("Config file doesn't exist. Creating a new one.", LogType.Warning);
                _config = new CoreConfig
                {
                    Fullscreen = true,
                    Resolution = _plexgate.GetSystemResolution(),
                    TextRendererClass = TextRenderer.GetDefaultRenderer().GetType().Name
                };
                SaveConfig();
            }
            else
            {
                LoadConfig();
            }
            ApplyConfig();
            Logger.Log("Done.");
        }

        public void LoadConfig()
        {
            _config = JsonConvert.DeserializeObject<CoreConfig>(File.ReadAllText("config.json"));
            Logger.Log("Config loaded from disk.");
        }

        public void ApplyConfig()
        {
            Logger.Log("Applying config...");
            Logger.Log($"Fullscreen: {_config.Fullscreen}");
            _plexgate.graphicsDevice.IsFullScreen = _config.Fullscreen;
            string resolution = _config.Resolution;
            string[] available = _plexgate.GetAvailableResolutions();
            if (!available.Contains(resolution))
            {
                Logger.Log($"Resolution {resolution} not supported by this display or GPU. Using system native resolution instead!", LogType.Warning);
                _config.Resolution = _plexgate.GetSystemResolution();
                resolution = _config.Resolution;
                SaveConfig();
            }
            _plexgate.ApplyResolution(resolution);
            _plexgate.graphicsDevice.ApplyChanges();
            Logger.Log("Done.");
        }

        public void SaveConfig()
        {
            Logger.Log("Saving config to disk.");
            File.WriteAllText("config.json", JsonConvert.SerializeObject(_config, Formatting.Indented));
            Logger.Log("Done.");
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            SaveConfig();
            _config = null;
            Logger.Log("Config manager's shut down.");
        }
    }

    public class CoreConfig
    {
        public string TextRendererClass { get; set; }
        public string Resolution { get; set; }
        public bool Fullscreen { get; set; }
    }

}
