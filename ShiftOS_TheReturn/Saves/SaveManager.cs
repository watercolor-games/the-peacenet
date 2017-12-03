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

namespace Plex.Engine.Saves
{
    public class SaveManager : IEngineComponent
    {
        private ISaveBackend _backend = null;

        private List<string> _localPaths = new List<string>();

        private string savePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Watercolor Games", "Peacenet", "Saves"); ;
            }
        }

        public string[] GetSavePaths()
        {
            return _localPaths.ToArray();
        }

        public void CreateSinglePlayerSave()
        {
            string name = $"peacenet{_localPaths.Count}.save";
            string path = Path.Combine(savePath, name);
            _backend = new SinglePlayerSaveBackend(path);
            ReadSavePaths();
        }

        public bool StartSinglePlayerSession(string path)
        {
            if (_localPaths.Contains(path))
            {
                _backend = new SinglePlayerSaveBackend(path);
                return true;
            }
            return false;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            return _backend.GetValue(key, defaultValue);
        }

        public void SetValue<T>(string key, T value)
        {
            _backend.SetValue(key, value);
        }

        public void Initiate()
        {
            Logger.Log("Starting save manager...");
            if (!Directory.Exists(savePath))
            {
                Logger.Log($"Creating save directory: {savePath}");
                Directory.CreateDirectory(savePath);
                Logger.Log("Done.");
            }
            ReadSavePaths();
        }

        public void EndSession()
        {
            _backend?.OnSessionEnd();
            _backend = null;

        }

        public void ReadSavePaths()
        {
            _localPaths = Directory.GetFiles(savePath).Where(x => x.EndsWith(".save")).ToList();
            Logger.Log($"{_localPaths.Count} saves found.");
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
            _backend = null;
        }
    }

    public interface ISaveBackend
    {
        T GetValue<T>(string key, T defaultValue);
        void SetValue<T>(string key, T value);
        void OnSessionEnd();
    }

    public class SinglePlayerSaveBackend : ISaveBackend
    {
        private Dictionary<string, object> _save = null;
        private string _path = null;

        public SinglePlayerSaveBackend(string path)
        {
            _path = path;
            if (File.Exists(_path))
            {
                _save = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(_path));

            }
            else
            {
                _save = new Dictionary<string, object>();
                File.WriteAllText(_path, JsonConvert.SerializeObject(_save));
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            if (_save.ContainsKey(key))
                return (T)Convert.ChangeType(_save[key], typeof(T));
            SetValue(key, defaultValue);
            return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
            if (_save.ContainsKey(key))
                _save[key] = value;
            else
                _save.Add(key, value);
            File.WriteAllText(_path, JsonConvert.SerializeObject(_save));
        }

        public void OnSessionEnd() { }
    }
}
