/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShiftOS.Objects.ShiftFS.Utils;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Provides basic high-level access to the ShiftOS filesystem engine (ShiftFS) and File Skimmer.
    /// </summary>
    public static class FileSkimmerBackend
    {
        private static IFileSkimmer _fs = null;

        /// <summary>
        /// Opens a file from the specified ShiftFS path.
        /// </summary>
        /// <param name="path">The path to open.</param>
        public static void OpenFile(string path)
        {
            if (!Objects.ShiftFS.Utils.FileExists(path))
                throw new System.IO.FileNotFoundException("ShiftFS could not find the file specified.", path);
            foreach (var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IFileHandler)) && Shiftorium.UpgradeAttributesUnlocked(x)))
            {
                foreach(FileHandlerAttribute attrib in type.GetCustomAttributes(false).Where(x => x is FileHandlerAttribute))
                {
                    if (path.ToLower().EndsWith(attrib.Extension))
                    {
                        var obj = (IFileHandler)Activator.CreateInstance(type);
                        obj.OpenFile(path);
                    }
                }
            }
        }
        public static FileType GetFileType(string path)
        {
            
            if (path == "__upone")
                return FileType.UpOne;

            if (DirectoryExists(path))
            {
                if (Mounts.Contains(GetDirectoryInfo(path)))
                    return FileType.Mount;
                else
                    return FileType.Directory;
            }


            string ext = path.Split('.')[path.Split('.').Length - 1];
            switch (ext)
            {
                case "txt":
                    return FileType.TextFile;
                case "pic":
                case "png":
                case "jpg":
                case "bmp":
                case "gif":
                    return FileType.Image;
                case "py":
                    return FileType.Python;
                case "mfs":
                    return FileType.Filesystem;
                case "lua":
                    return FileType.Lua;
                case "skn":
                    return FileType.Skin;
                case "json":
                    return FileType.JSON;
                case "sft":
                //No, not "sex" - ShiftOS EXecutable. xD
                case "sex":
                    return FileType.Executable;
                case "cf":
                    return FileType.CommandFormat;
                default:
                    return FileType.Unknown;
            }
        }

        /// <summary>
        /// Opens the specified directory path inside a new File Skimmer frontend.
        /// </summary>
        /// <param name="path">The path to open</param>
        public static void OpenDirectory(string path)
        {
            _fs.OpenDirectory(path);
        }

        /// <summary>
        /// Allows you to prompt the user to select a file, either to open or save, and filter the types of files they can select.
        /// </summary>
        /// <param name="types">An array of file extensions that the user may select.</param>
        /// <param name="style">The UI style of the new file select frontend.</param>
        /// <param name="callback">The Action that is called when the user selects a file. The string argument provided by this call is the path of the file they selected.</param>
        public static void GetFile(string[] types, FileOpenerStyle style, Action<string> callback)
        {
            _fs.GetPath(types, style, callback);
        }

        /// <summary>
        /// Initiates the file skimmer backend with a new middle-end layer.
        /// </summary>
        /// <param name="fs">The middle-end IFileSkimmer that'll do all the work.</param>
        /// <remarks>Without a middle-end, the File Skimmer will not function properly.</remarks>
        public static void Init(IFileSkimmer fs)
        {
            _fs = fs;
        }

        public static System.Drawing.Image GetImage(string filepath)
        {
            return _fs.GetImage(filepath);
        }

        public static string GetFileExtension(FileType fileType)
        {
            return _fs.GetFileExtension(fileType);
        }
    }

    /// <summary>
    /// Provides primary middle-end functions allowing the File Skimmer API to talk with your frontend.
    /// </summary>
    public interface IFileSkimmer
    {
        void OpenFile(string filepath);
        void GetPath(string[] filetypes, FileOpenerStyle style, Action<string> callback);
        void OpenDirectory(string path);
        Image GetImage(string path);
        string GetFileExtension(FileType fileType);
    }


    /// <summary>
    /// Different types of UI styles for File Openers.
    /// </summary>
    public enum FileOpenerStyle
    {
        Open,
        Save
    }

    /// <summary>
    /// Recognized file types within the ShiftFS engine.
    /// </summary>
    public enum FileType
    {
        TextFile,
        Directory,
        Mount,
        UpOne,
        Image,
        Skin,
        JSON,
        Executable,
        Lua,
        Python,
        Filesystem,
        CommandFormat,
        Unknown
    }
}
