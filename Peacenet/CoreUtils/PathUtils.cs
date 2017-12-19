using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Filesystem;

namespace Peacenet.CoreUtils
{
    public class FileUtils : IEngineComponent
    {
        private FSManager _fs = null;

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
    }
}
