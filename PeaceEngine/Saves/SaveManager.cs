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
using Microsoft.Xna.Framework.Content;
using Plex.Engine.Config;

namespace Plex.Engine.Saves
{
    /// <summary>
    /// Provides a simple API for interacting with a save file.
    /// </summary>
    public class SaveManager : IEngineComponent
    {
        private ISaveBackend _backend = null;

        [Dependency]
        private Plexgate _plexgate = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            _backend = _plexgate.New<DefaultSaveBackend>();
        }

        /// <summary>
        /// Create a snapshot of the current save file
        /// </summary>
        /// <returns>The ID of the new snapshot</returns>
        public string CreateSnapshot()
        {
            return _backend.CreateSnapshot();
        }

        /// <summary>
        /// Restore the save to a previous snapshot
        /// </summary>
        /// <param name="id">The ID of the snapshot to restore</param>
        public void RestoreSnapshot(string id)
        {
            _backend.RestoreSnapshot(id);
        }

        /// <summary>
        /// Set the backend for the save manager to use.
        /// </summary>
        /// <param name="backend">An <see cref="ISaveBackend"/> instance implementing methods required to fetch and modify values of a save file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="backend"/> was null.</exception> 
        public void SetBackend(ISaveBackend backend)
        {
            if (backend == null)
                throw new ArgumentNullException(nameof(backend));
            _backend = backend;
            _plexgate.Inject(_backend);
        }

        /// <summary>
        /// Asynchronously retrieve a save value.
        /// </summary>
        /// <typeparam name="T">The type of value you'd like to be given</typeparam>
        /// <param name="key">THe key for the entry you'd like to look up</param>
        /// <param name="defaultValue">The default value to be set for the entry if an entry with the specified key wasn't found.</param>
        /// <param name="result">A callback to be run when the entry was read successfully.</param>
        /// <param name="error">A callback to be run when an error occurred.</param>
        /// <returns>A <see cref="Task"/> you can use to await the method.</returns>
        public async Task GetValueAsync<T>(string key, T defaultValue, Action<T> result, Action<Exception> error)
        {
            await Task.Run(() =>
            {
                try
                {
                    result?.Invoke(_backend.GetValue(key, defaultValue));
                }
                catch(Exception ex)
                {
                    error?.Invoke(ex);
                }
            });
        }
        
        /// <summary>
        /// Retrieve the value of a specified save entry.
        /// </summary>
        /// <typeparam name="T">The type of value you'd like to receive.</typeparam>
        /// <param name="key">The key of the entry to look up.</param>
        /// <param name="defaultValue">The default value for the entry if no entry was found.</param>
        /// <returns>The value of the entry.</returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            object result = null;
            Exception error = null;
            GetValueAsync(key, defaultValue, (res) => { result = res; }, (err) => { error = err; }).Wait();
            if (error != null)
                throw error;
            return (T)result;
        }

        /// <summary>
        /// Asynchronously set the value of a save entry.
        /// </summary>
        /// <typeparam name="T">The type of value that will be set</typeparam>
        /// <param name="key">The key of the entry to modify</param>
        /// <param name="value">The value to be set in the entry</param>
        /// <param name="done">A callback to be run when the save modification was completed.</param>
        /// <param name="error">A callback to be run when an error has occurred.</param>
        /// <returns>A <see cref="Task"/> you can use to await the method.</returns>
        public async Task SetValueAsync<T>(string key, T value, Action done, Action<Exception> error)
        {
            await Task.Run(() =>
            {
                try
                {
                    _backend.SetValue(key, value);
                    done?.Invoke();
                }
                catch(Exception ex)
                {
                    error?.Invoke(ex);
                }
            });
        }

        /// <summary>
        /// Set the value of a save entry.
        /// </summary>
        /// <typeparam name="T">The type of value to be set</typeparam>
        /// <param name="key">The key of the entry to be set</param>
        /// <param name="value">The new value for the entry</param>
        public void SetValue<T>(string key, T value)
        {
            Exception error = null;
            SetValueAsync(key, value, null, (err) => { error = err; }).Wait();
            if (error != null)
                throw error;
        }
    }

    /// <summary>
    /// Provides an API for interacting with a save file.
    /// </summary>
    public interface ISaveBackend
    {
        /// <summary>
        /// Retrieve a value from the save file.
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve</typeparam>
        /// <param name="key">The key of the entry to search for. If an entry is not found, the default value is added as this entry.</param>
        /// <param name="defaultValue">The default value of the entry if it is not found</param>
        /// <returns>The value of the save entry</returns>
        T GetValue<T>(string key, T defaultValue);
        /// <summary>
        /// Set a value in a save file.
        /// </summary>
        /// <typeparam name="T">The type of entry to save</typeparam>
        /// <param name="key">The key of the entry to set. If it doesn't exist, it will be created.</param>
        /// <param name="value">The value of the entry.</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// Create a snapshot of the current save file.
        /// </summary>
        /// <returns>The ID of the snapshot</returns>
        string CreateSnapshot();

        /// <summary>
        /// Restore the save file to a specified snapshot.
        /// </summary>
        /// <param name="id">The ID of the snapshot to restore to.</param>
        void RestoreSnapshot(string id);
    }

    public class DefaultSaveBackend : ISaveBackend,ILoadable, IDisposable
    {
        private class Snapshot
        {
            public string Id { get; set; }
            public DateTime Timestamp { get; set; }
            public string Data { get; set; }
        }

        [Dependency]
        private AppDataManager _appdata = null;

        private string _filepath = null;

        private Dictionary<string, object> _values = null;

        private List<Snapshot> _snapshots = new List<Snapshot>();

        public string CreateSnapshot()
        {
            var snapshot = new Snapshot
            {
                Id = _snapshots.Count.ToString(),
                Data = JsonConvert.SerializeObject(_values),
                Timestamp = DateTime.UtcNow
            };
            _snapshots.Add(snapshot);
            return snapshot.Id;
        }

        public void Dispose()
        {
            string json = null;
            if(_snapshots.Count > 0)
            {
                json = _snapshots.OrderBy(x => x.Timestamp).First().Data;
            }
            else
            {
                json = JsonConvert.SerializeObject(_values);
            }
            File.WriteAllText(_filepath, json);
            _snapshots = null;
            _values = null;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            //"Borrowed" from the ConfigManager.GetValue<T> method.
            if (_values.ContainsKey(key))
            {
                if (typeof(T).IsEnum)
                {
                    return (T)Enum.Parse(typeof(T), _values[key].ToString());
                }
                return (T)Convert.ChangeType(_values[key], typeof(T));
            }
            else
                SetValue(key, defaultValue);
            return defaultValue;

        }

        public void Load(ContentManager content)
        {
            //Set up file path
            _filepath = Path.Combine(_appdata.GamePath, "save.json");

            //Load save if file exists
            if (File.Exists(_filepath))
                _values = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(_filepath));
            else //just create an empty dictionary.
            {
                _values = new Dictionary<string, object>();
            }

            //Create snapshots list
            _snapshots = new List<Snapshot>();
        }

        public void RestoreSnapshot(string id)
        {
            var snapshot = _snapshots.FirstOrDefault(x => x.Id == id);
            if (snapshot == null)
                return;
            _values = JsonConvert.DeserializeObject<Dictionary<string, object>>(snapshot.Data);
            _snapshots.Remove(snapshot);
            Save();
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(_values);
            File.WriteAllText(_filepath, json);
        }

    public void SetValue<T>(string key, T value)
        {
            if (_values.ContainsKey(key))
                _values[key] = value;
            else
                _values.Add(key, value);
            Save();
        }
    }
}
