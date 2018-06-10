using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Peacenet.Filesystem;
using Plex.Engine;
using Peacenet.Applications;
using Plex.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;

namespace Peacenet.CoreUtils
{
    /// <summary>
    /// Provides an engine component for retrieving client-side information about a given file or directory path.
    /// </summary>
    public class FileUtils : IEngineComponent
    {

        

        [Dependency]
        private GameLoop _GameLoop = null;

        [Dependency]
        private FSManager _fs = null;

        /// <summary>
        /// Parses a path string and retrieves the destination file name.
        /// </summary>
        /// <param name="path">The file or directory path to parse.</param>
        /// <returns>The file or directory name that the path points to.</returns>
        public string GetNameFromPath(string path)
        {
            while (path.EndsWith("/"))
                path = path.Remove(path.Length - 1, 1);
            return path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        /// <summary>
        /// Resolve any "up-one" and "current" directory names in the specified path to an absolute path. I.E: /home/alkaline/../Documents/./.. -> /home/Documents
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns>The resolved path</returns>
        public string Resolve(string path)
        {
            Stack<string> pathParts = new Stack<string>();
            string[] split = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in split)
            {
                if (part == ".")
                    continue;
                if (part == "..")
                {
                    if (pathParts.Count > 0)
                        pathParts.Pop(); //remove the parent directory entry from the path.
                    continue;
                }
                if (part == "~")
                {
                    pathParts.Push("home");
                    continue;
                }
                pathParts.Push(part);
            }
            string[] parts = new string[pathParts.Count];
            for (int i = parts.Length-1; i >= 0; i--)
            {
                parts[i] = pathParts.Pop();
            }
            string absolute = "";
            foreach (var part in parts)
                absolute += "/" + part;
            if (string.IsNullOrWhiteSpace(absolute))
                absolute = "/";
            return absolute;
        }

        /// <summary>
        /// Gets the MIME type of a specified file name via its extension.
        /// </summary>
        /// <param name="filename">The file name to look up</param>
        /// <returns>The MIME type for the file</returns>
        public string GetMimeType(string filename)
        {
            if (!filename.Contains("."))
                return "unknown";
            int last = filename.LastIndexOf(".");
            int len = filename.Length - last;
            string ext = filename.Substring(last, len);
            var types = MimeTypeMap.List.MimeTypeMap.GetMimeType(ext);
            return (types.Count == 0) ? "unknown" : types.First();
        }

        /// <summary>
        /// Retrieves an icon for a given MIME type.
        /// </summary>
        /// <param name="mimetype">The MIME type to look up.</param>
        /// <returns>A <see cref="Texture2D"/> containing the icon for the MIME type.</returns>
        public Texture2D GetMimeIcon(string mimetype)
        {
            switch (mimetype)
            {
                case "text/plain":
                    return _GameLoop.Content.Load<Texture2D>("UIIcons/FileIcons/file-text");
                default:
                    return _GameLoop.Content.Load<Texture2D>("UIIcons/FileIcons/unknown");
            }
        }

        /// <inheritdoc/>
        public void Initiate()
        {
        }
    }

    /// <summary>
    /// Provides an engine component allowing the creation of dialogs for retrieving file paths from the player.
    /// </summary>
    public class GUIUtils : IEngineComponent
    {
        [Dependency]
        private WindowSystem _winsys = null;

        /// <inheritdoc/>
        public void Initiate()
        {
        }

        /// <summary>
        /// Ask the user for a file.
        /// </summary>
        /// <param name="saving">Whether the player is saving to a file or opening an existing file.</param>
        /// <param name="callback">A callback to be run when the file is selected.</param>
        /// <param name="filter">An array of MIME types that represents the types of files that should be shown in the file manager's file list. Empty or <see langword="null"/> for no filter.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception> 
        public void AskForFile(bool saving, string[] filter, Action<string> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var fs = new FileManager(_winsys);
            fs.Mode = (saving) ? FileManagerMode.SaveFile : FileManagerMode.OpenFile;
            fs.FileFilter = filter;
            fs.FileSelected += (p) =>
            {
                callback(p);
                fs.Close();
            };
            fs.Show();
        }
    }
}
