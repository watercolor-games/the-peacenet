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
using Microsoft.Xna.Framework.Input;

namespace Plex.Engine.Config
{
    /// <summary>
    /// Provides an engine component that allows other components to have user-configurable values.
    /// </summary>
    public class ConfigManager : IEngineComponent, IDisposable
    {

        private Dictionary<string, object> _config = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private AppDataManager _appdata = null;

        private string _path = "";

        private class configEntity : IEntity
        {
            /// <inheritdoc/>
            public void OnGameExit()
            {
                _config.SaveToDisk();
            }

            [Dependency]
            private ConfigManager _config = null;
            public void Draw(GameTime time, GraphicsContext gfx)
            {
            }

            public void OnKeyEvent(KeyboardEventArgs e)
            {
            }

            public void OnMouseUpdate(MouseState mouse)
            {
            }

            public void Update(GameTime time)
            {
                float sfx = _config.GetValue("audioSfxVolume", 1.0F);
                float sfxClamped = MathHelper.Clamp(sfx, 0, 1);
                if (sfxClamped != sfx)
                {
                    _config.SetValue("audioSfxVolume", sfxClamped);
                }
                SoundEffect.MasterVolume = sfxClamped;
            }
        }

        /// <inheritdoc/>
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

            var entity = _plexgate.New<configEntity>();
            _plexgate.GetLayer(LayerType.NoDraw).AddEntity(entity);
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
            {
                if (typeof(T).IsEnum)
                {
                    return (T)Enum.Parse(typeof(T), _config[name].ToString());
                }
                return (T)Convert.ChangeType(_config[name], typeof(T));
            }
            else
                _config.Add(name, defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Sets the value of the config entry with the specified name. If the entry doesn't exist, a new one will be created with this value.
        /// </summary>
        /// <param name="name">The entry name to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(string name, object value)
        {
            if (_config.ContainsKey(name))
                _config[name] = value;
            else
                _config.Add(name, value);
        }

        /// <summary>
        /// Applies the loaded configuration dictionary to the engine and all components.
        /// </summary>
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

            foreach (var component in _plexgate.GetAllComponents())
            {
                if (component.GetType().GetInterfaces().Contains(typeof(IConfigurable)))
                {
                    (component as IConfigurable).ApplyConfig();
                }
            }
            Logger.Log("Done.");
        }

        /// <summary>
        /// Loads the configuration dictionary from disk, causing any unsaved changes to be lost.
        /// </summary>
        public void LoadFromDisk()
        {
            _config = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(_path));
        }

        /// <summary>
        /// Saves the current configuration dictionary to disk.
        /// </summary>
        public void SaveToDisk()
        {
            Logger.Log("Saving config to disk...");
            File.WriteAllText(_path, JsonConvert.SerializeObject(_config, Formatting.Indented));
            Logger.Log("Done.");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            SaveToDisk();
            _config = null;
        }
    }

    /// <summary>
    /// Provides an API for a configurable <see cref="IEngineComponent"/>. 
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// Occurs when <see cref="ConfigManager"/> applies its configuration dictionary.
        /// </summary>
        void ApplyConfig();
    }

    /// <summary>
    /// Provides a cross-platform path where game data may be stored.
    /// </summary>
    public class AppDataManager : IEngineComponent
    {
        private string _path = "";

        /// <inheritdoc/>
        public void Initiate()
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Watercolor Games", "Peacenet");
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        /// <summary>
        /// Retrieves the game data path.
        /// </summary>
        public string GamePath
        {
            get
            {
                return _path;
            }
        }
    }
}
