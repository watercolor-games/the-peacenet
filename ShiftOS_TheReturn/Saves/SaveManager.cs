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

        [Dependency]
        private Plexgate _plexgate = null;

        public int DrawIndex
        {
            get
            {
                return -1;
            }
        }

        public void Initiate()
        {
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

        public void SetBackend(ISaveBackend backend)
        {
            if (backend == null)
                throw new ArgumentNullException(nameof(backend));
            _backend = backend;
            _plexgate.Inject(_backend);
        }

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

        public T GetValue<T>(string key, T defaultValue)
        {
            object result = null;
            Exception error = null;
            GetValueAsync(key, defaultValue, (res) => { result = res; }, (err) => { error = err; }).Wait();
            if (error != null)
                throw error;
            return (T)result;
        }

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
    }
}
