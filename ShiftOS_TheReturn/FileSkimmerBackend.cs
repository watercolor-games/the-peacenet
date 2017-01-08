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
            _fs.OpenFile(path);
        }

        /// <summary>
        /// Gets the file type of a given path.
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>The FileType of the path</returns>
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
            return new Bitmap(42, 42);
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
        Unknown
    }
}
