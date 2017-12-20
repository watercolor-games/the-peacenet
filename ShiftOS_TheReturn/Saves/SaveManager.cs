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


    public interface ISaveBackend
    {
        T GetValue<T>(string key, T defaultValue);
        void SetValue<T>(string key, T value);
    }
}
