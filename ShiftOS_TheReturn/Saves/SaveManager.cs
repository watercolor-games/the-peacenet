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
    }
}
