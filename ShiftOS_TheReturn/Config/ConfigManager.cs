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
using Microsoft.Xna.Framework.Audio;

namespace Plex.Engine.Config
{
    public class ConfigManager : IEngineComponent
    {
        private Dictionary<string, object> _config = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private AppDataManager _appdata = null;

        private string _path = "";

        public void Initiate()
        {
            _path = Path.Combine(_appdata.GamePath, "config.json");
            Logger.Log("Loading configuration file...");
            if (!File.Exists(_path))
            {
                Logger.Log("Config file not found. Making new one.");
                _config = new Dictionary<string, object>();
                SaveToDisk();
            }
            else
            {
                LoadFromDisk();
            }
            Apply();
        }

        /// <summary>
        /// Get the value of a setting in the config file. If the setting doesn't exist, the default value you supply will be added.
        /// </summary>
        /// <param name="defaultValue">The default value for the setting if it doesn't exist.</param>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The setting's value.</returns>
        public T GetValue<T>(string name, T defaultValue)
        {
            if (_config.ContainsKey(name))
                return (T)Convert.ChangeType(_config[name], typeof(T));
            else
                _config.Add(name, defaultValue);
            return defaultValue;
        }

        public void SetValue(string name, object value)
        {
            if (_config.ContainsKey(name))
                _config[name] = value;
            else
                _config.Add(name, value);
        }

        public void Apply()
        {
            Logger.Log("Config file is now being applied.");
            string defaultResolution = _plexgate.GetSystemResolution();
            string resolution = GetValue("screenResolution", defaultResolution).ToString();

            string[] available = _plexgate.GetAvailableResolutions();
            if (!available.Contains(resolution))
            {
                resolution = defaultResolution;
                SetValue("screenResolution", resolution);
            }
            _plexgate.ApplyResolution(resolution);

            foreach(var component in _plexgate.GetAllComponents())
            {
                if(_plexgate.DependsOn(component, this))
                {
                    if (component.GetType().GetInterfaces().Contains(typeof(IConfigurable)))
                    {
                        (component as IConfigurable).ApplyConfig();
                    }
                }
            }

            Logger.Log("Done.");
        }

        public void LoadFromDisk()
        {
            _config = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(_path));
        }

        public void SaveToDisk()
        {
            Logger.Log("Saving config to disk...");
            File.WriteAllText(_path, JsonConvert.SerializeObject(_config, Formatting.Indented));
            Logger.Log("Done.");
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
            float sfx = GetValue("audioSfxVolume", 1.0F);
            float sfxClamped = MathHelper.Clamp(sfx, 0, 1);
            if (sfxClamped != sfx)
                SetValue("audioSfxVolume", sfxClamped);
            SoundEffect.MasterVolume = sfxClamped;

        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            SaveToDisk();
            _config = null;
        }
    }

    public interface IConfigurable
    {
        void ApplyConfig();
    }

    public class AppDataManager : IEngineComponent
    {
        private string _path = "";

        public void Initiate()
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Watercolor Games", "Peacenet");
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public string GamePath
        {
            get
            {
                return _path;
            }
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
        }
    }
}
