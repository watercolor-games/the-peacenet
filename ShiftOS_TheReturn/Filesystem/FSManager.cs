using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Config;
using Plex.Engine.Server;
using System.IO;
using Plex.Objects.PlexFS;
using Plex.Objects.ShiftFS;

namespace Plex.Engine.Filesystem
{
    public class FSManager : IEngineComponent
    {
        [Dependency]
        private AppDataManager _appdata = null;

        private IAsyncFSBackend _backend = null;

        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public void SetBackend(IAsyncFSBackend backend)
        {
            if (backend == null)
                throw new ArgumentNullException("Backend cannot be null.");
            _plexgate.Inject(backend);
            _backend = backend;
            _backend.Initialize();
        }

        public void CreateDirectory(string path)
        {
            _backend.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return _backend.DirectoryExists(path);
        }

        public bool FileExists(string path)
        {
            return _backend.FileExists(path);
        }

        public FileRecord GetFileRecord(string path)
        {
            return _backend.GetFileRecord(path);
        }

        public string[] GetFiles(string path)
        {
            return _backend.GetFiles(path).OrderBy(x=>x).ToArray();
        }

        public string[] GetDirectories(string path)
        {
            return _backend.GetDirectories(path).OrderBy(x=>x).ToArray();
        }

        public void WriteAllBytes(string path, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            _backend.WriteAllBytes(path, data);
        }

        public void Delete(string path)
        {
            _backend.Delete(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return _backend.ReadAllBytes(path);
        }

        public async Task CreateDirectoryAsync(string path)
        {
            await Task.Run(() =>
            {
                _backend.CreateDirectory(path);
            });
        }

        public async Task<bool> DirectoryExistsAsync(string path)
        {
            bool result = false;
            await Task.Run(() =>
            {
                result = _backend.DirectoryExists(path);
            });
            return result;
        }

        public async Task<bool> FileExistsAsync(string path)
        {
            bool result = false;
            await Task.Run(() =>
            {
                result = _backend.FileExists(path);
            });
            return result;
        }

        public async Task<FileRecord> GetFileRecordAsync(string path)
        {
            FileRecord result = null;
            await Task.Run(() =>
            {
                result = _backend.GetFileRecord(path);
            });
            return result;
        }

        public async Task<string[]> GetFilesAsync(string path)
        {
            string[] result = null;
            await Task.Run(() =>
            {
                result = _backend.GetFiles(path);
            });
            return result;
        }

        public async Task<string[]> GetDirectoriesAsync(string path)
        {
            string[] result = null;
            await Task.Run(() =>
            {
                result = _backend.GetDirectories(path);
            });
            return result;
        }

        public async Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            await Task.Run(() =>
            {
                _backend.WriteAllBytes(path, bytes);
            });
        }

        public async Task DeleteAsync(string path)
        {
            await Task.Run(() =>
            {
                _backend.Delete(path);
            });
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            byte[] data = null;
            await Task.Run(() =>
            {
                data = _backend.ReadAllBytes(path);
            });
            return data;
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
            _backend?.Unload();
        }
    }

    public interface IAsyncFSBackend
    {
        void Initialize();
        void Unload();

        bool DirectoryExists(string path);
        bool FileExists(string path);

        string[] GetDirectories(string path);
        string[] GetFiles(string path);

        byte[] ReadAllBytes(string path);
        void WriteAllBytes(string path, byte[] data);

        void CreateDirectory(string path);
        void Delete(string path);

        FileRecord GetFileRecord(string path);
    }
}
