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
using System.IO;
using Plex.Objects.PlexFS;
using Plex.Objects.ShiftFS;
using Plex.Engine;
using Plex.Objects.Streams;

namespace Peacenet.Filesystem
{
    /// <summary>
    /// Provides a simple asynchronous API with alternative synchronous methods for interacting with a virtual file system.
    /// </summary>
    public class FSManager : IEngineComponent, IDisposable
    {
        private IAsyncFSBackend _backend = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public event Action<string> WriteOperation;

        public Stream OpenRead(string file)
        {
            return new ReadOnlyStream(_backend.Open(file, OpenMode.Open));
        }

        public Stream OpenWrite(string file)
        {
            var ret = new WriteOnlyStream(_backend.Open(file, OpenMode.OpenOrCreate));
            WriteOperation?.Invoke(file); // Won't fire if the stream is written to afterwards... FIXME?
            return ret;
        }

        public Stream Open(string file, OpenMode mode)
        {
            var ret = _backend.Open(file, mode);
            WriteOperation?.Invoke(file);
            return ret;
        }

        /// <summary>
        /// Read all text (in UTF8) of a file.
        /// </summary>
        /// <param name="file">The path to a file to read.</param>
        /// <returns>The text of the file.</returns>
        public string ReadAllText(string file)
        {
            return Encoding.UTF8.GetString(ReadAllBytes(file));
        }

        /// <inheritdoc cref="ReadAllText(string)"/>
        public async Task<string> ReadAllTextAsync(string file)
        {
            byte[] data = await ReadAllBytesAsync(file);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Writes Unicode text to a file.
        /// </summary>
        /// <param name="path">The file path to write to</param>
        /// <param name="text">The text to write</param>
        public void WriteAllText(string path, string text)
        {
            WriteAllBytes(path, Encoding.UTF8.GetBytes(text));
        }

        /// <inheritdoc cref="WriteAllText(String, String)"/>
        public async Task WriteAllTextAsync(string path, string text)
        {
            await WriteAllBytesAsync(path, Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Set the virtual FS backend.
        /// </summary>
        /// <param name="backend">An <see cref="IAsyncFSBackend"/> object implementing the necessary methods for interacting with a VFS.</param>
        public void SetBackend(IAsyncFSBackend backend)
        {
            if (backend == null)
                throw new ArgumentNullException("Backend cannot be null.");
            _plexgate.Inject(backend);
            _backend = backend;
            _backend.Initialize();
        }

        /// <summary>
        /// Create a directory.
        /// </summary>
        /// <param name="path">The path for the new directory.</param>
        public void CreateDirectory(string path)
        {
            _backend.CreateDirectory(path);
            WriteOperation?.Invoke(path);
        }

        /// <summary>
        /// Check if a directory exists.
        /// </summary>
        /// <param name="path">The directory to look for</param>
        /// <returns>Whether the specified directory exists</returns>
        public bool DirectoryExists(string path)
        {
            return _backend.DirectoryExists(path);
        }

        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="path">The file path to check</param>
        /// <returns>Whether the file exists</returns>
        public bool FileExists(string path)
        {
            return _backend.FileExists(path);
        }

        /// <summary>
        /// Get file or directory record information.
        /// </summary>
        /// <param name="path">The path to a file or directory</param>
        /// <returns>The record information for the file/directory at the given path.</returns>
        public FileRecord GetFileRecord(string path)
        {
            return _backend.GetFileRecord(path);
        }

        /// <summary>
        /// Get all files inside a directory
        /// </summary>
        /// <param name="path">The directory to search</param>
        /// <returns>All files found in the directory</returns>
        public string[] GetFiles(string path)
        {
            return _backend.GetFiles(path).OrderBy(x=>x).ToArray();
        }

        /// <summary>
        /// Get all directories inside a directory
        /// </summary>
        /// <param name="path">The directory to search</param>
        /// <returns>All directories within the directory</returns>
        public string[] GetDirectories(string path)
        {
            return _backend.GetDirectories(path).OrderBy(x=>x).ToArray();
        }

        /// <summary>
        /// Write binary data to a file
        /// </summary>
        /// <param name="path">The path to a file to write to</param>
        /// <param name="data">The binary data to write</param>
        public void WriteAllBytes(string path, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            _backend.WriteAllBytes(path, data);
            WriteOperation?.Invoke(path);
        }

        /// <summary>
        /// Delete a file or directory from the VFS
        /// </summary>
        /// <param name="path">A path to the file or directory to delete</param>
        public void Delete(string path)
        {
            _backend.Delete(path);
            WriteOperation?.Invoke(path);
        }

        /// <summary>
        /// Read binary data from a file.
        /// </summary>
        /// <param name="path">The path to a file to read</param>
        /// <returns>The binary data of the file</returns>
        public byte[] ReadAllBytes(string path)
        {
            return _backend.ReadAllBytes(path);
        }
        /// <inheritdoc cref="CreateDirectory(string)"/>
        public async Task CreateDirectoryAsync(string path)
        {
            await Task.Run(() =>
            {
                _backend.CreateDirectory(path);
                WriteOperation?.Invoke(path);
            });
        }

        /// <inheritdoc cref="DirectoryExists(string)"/>
        public async Task<bool> DirectoryExistsAsync(string path)
        {
            bool result = false;
            await Task.Run(() =>
            {
                result = _backend.DirectoryExists(path);
            });
            return result;
        }

        /// <inheritdoc cref="FileExists(string)"/>
        public async Task<bool> FileExistsAsync(string path)
        {
            bool result = false;
            await Task.Run(() =>
            {
                result = _backend.FileExists(path);
            });
            return result;
        }

        /// <inheritdoc cref="GetFileRecord(string)"/>
        public async Task<FileRecord> GetFileRecordAsync(string path)
        {
            FileRecord result = null;
            await Task.Run(() =>
            {
                result = _backend.GetFileRecord(path);
            });
            return result;
        }

        /// <inheritdoc cref="GetFiles(string)"/>
        public async Task<string[]> GetFilesAsync(string path)
        {
            string[] result = null;
            await Task.Run(() =>
            {
                result = _backend.GetFiles(path);
            });
            return result;
        }

        /// <inheritdoc cref="GetDirectories(string)"/>
        public async Task<string[]> GetDirectoriesAsync(string path)
        {
            string[] result = null;
            await Task.Run(() =>
            {
                result = _backend.GetDirectories(path);
            });
            return result;
        }

        /// <inheritdoc cref="WriteAllBytes(string, byte[])"/>
        public async Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            await Task.Run(() =>
            {
                _backend.WriteAllBytes(path, bytes);
                WriteOperation?.Invoke(path);
            });
        }

        /// <inheritdoc cref="Delete(string)"/>
        public async Task DeleteAsync(string path)
        {
            await Task.Run(() =>
            {
                _backend.Delete(path);
                WriteOperation?.Invoke(path);
            });
        }

        /// <inheritdoc cref="ReadAllBytes(string)"/>
        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            byte[] data = null;
            await Task.Run(() =>
            {
                data = _backend.ReadAllBytes(path);
            });
            return data;
        }

        /// <inheritdoc/>
        public void Initiate()
        {

        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _backend?.Unload();
        }
    }

    /// <summary>
    /// Provides a client-side API for interacting with a virtual file system.
    /// </summary>
    public interface IAsyncFSBackend
    {
        /// <summary>
        /// Initialize the VFS and load any required resources.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Unload any required resources.
        /// </summary>
        void Unload();
        
        /// <summary>
        /// Retrieves whether a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>Whether the path exists as a directory.</returns>
        bool DirectoryExists(string path);
        /// <summary>
        /// Retrieves whether a file exists at a given path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>Whether the path exists as a file.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Retrieves a list of subdirectories in a directory.
        /// </summary>
        /// <param name="path">A path to a directory to check.</param>
        /// <returns>The subdirectories found inside the directory.</returns>
        string[] GetDirectories(string path);
        /// <summary>
        /// Retrieves a list of files in a given directory.
        /// </summary>
        /// <param name="path">The path to a directory to check.</param>
        /// <returns>The files found in the directory.</returns>
        string[] GetFiles(string path);

        /// <summary>
        /// Reads the binary content of a file.
        /// </summary>
        /// <param name="path">The path to a file to read.</param>
        /// <returns>The contents of the file.</returns>
        byte[] ReadAllBytes(string path);
        /// <summary>
        /// Writes binary data to a file. Overwrites if the file exists already.
        /// </summary>
        /// <param name="path">The path to a file to write data to.</param>
        /// <param name="data">The data to write.</param>
        void WriteAllBytes(string path, byte[] data);

        /// <summary>
        /// Create a directory if it doesn't exist.
        /// </summary>
        /// <param name="path">The path to a directory to create.</param>
        void CreateDirectory(string path);
        /// <summary>
        /// Delete a file or directory at a given path.
        /// </summary>
        /// <param name="path">The path to a file or directory to delete.</param>
        void Delete(string path);

        /// <summary>
        /// Retrieve information about a file.
        /// </summary>
        /// <param name="path">The path to a file to check.</param>
        /// <returns>A <see cref="FileRecord"/> containing information about the file.</returns>
        FileRecord GetFileRecord(string path);

        /// <summary>
        /// Open a file for streaming.
        /// </summary>
        /// <returns>An open stream.</returns>
        /// <param name="path">The path to a file.</param>
        /// <param name="mode">The open mode.</param>
        Stream Open(string path, OpenMode mode);
    }
}
