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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ShiftOS.Objects.ShiftFS;
using DynamicLua;
using System.Dynamic;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading;
using System.Net;

namespace ShiftOS.Engine.Scripting
{
    /// <summary>
    /// Brings some C# goodies to the Lua system.
    /// </summary>
    [Exposed("strutils")]
    public class StringUtils
    {
        /// <summary>
        /// Checks if a string ends with a specified string.
        /// </summary>
        /// <param name="operand">The string to operate on</param>
        /// <param name="value">The string to check for</param>
        /// <returns>Whether <paramref name="operand"/> ends with <paramref name="value"/>.</returns>
        public bool endswith(string operand, string value)
        {
            return operand.EndsWith(value);
        }


        /// <summary>
        /// Checks if a string starts with a specified string.
        /// </summary>
        /// <param name="operand">The string to operate on</param>
        /// <param name="value">The string to check for</param>
        /// <returns>Whether <paramref name="operand"/> starts with <paramref name="value"/>.</returns>
        public bool startswith(string operand, string value)
        {
            return operand.StartsWith(value);
        }

        /// <summary>
        /// Checks if a string contains a specified string.
        /// </summary>
        /// <param name="operand">The string to operate on</param>
        /// <param name="value">The string to check for</param>
        /// <returns>Whether <paramref name="operand"/> contains <paramref name="value"/>.</returns>
        public bool contains(string operand, string value)
        {
            return operand.Contains(value);
        }
    }

    /// <summary>
    /// DynamicLua wrapper for the ShiftOS engine.
    /// </summary>
    public class LuaInterpreter
    {
        /// <summary>
        /// The DynamicLua backend.
        /// </summary>
        public dynamic Lua = new DynamicLua.DynamicLua();

        /// <summary>
        /// Boolean representing whether the script is running.
        /// </summary>
        public bool Running = true;

        /// <summary>
        /// Static constructor for the <see cref="LuaInterpreter"/> class. 
        /// </summary>
        static LuaInterpreter()
        {
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "run")
                {
                    TerminalBackend.PrefixEnabled = false;
                    var cntnts = JsonConvert.DeserializeObject<dynamic>(msg.Contents);
                    var interp = new LuaInterpreter();
                    Desktop.InvokeOnWorkerThread(() =>
                    {
                        interp.Execute(cntnts.script.ToString());

                    });
                    TerminalBackend.PrefixEnabled = true;
                    TerminalBackend.PrintPrompt();
                }
            };
        }

        /// <summary>
        /// Create a .SFT representation of a Lua script.
        /// </summary>
        /// <param name="lua">The Lua code to convert</param>
        /// <returns>Base64 SFT representation.</returns>
        public static string CreateSft(string lua)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(lua);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Run a compressed .SFT file as a lua script.
        /// </summary>
        /// <param name="sft">The .sft file to run.</param>
        public static void RunSft(string sft)
        {
            if (Utils.FileExists(sft))
            {
                try
                {
                    string b64 = Utils.ReadAllText(sft);
                    byte[] bytes = Convert.FromBase64String(b64);
                    string lua = Encoding.UTF8.GetString(bytes);
                    CurrentDirectory = sft.Replace(Utils.GetFileInfo(sft).Name, "");
                    new LuaInterpreter().Execute(lua);
                }
                catch
                {
                    Infobox.Show("Invalid binary.", "This file is not a valid ShiftOS binary executable.");
                }
            }
        }

        /// <summary>
        /// Get the current working directory of the script.
        /// </summary>
        public static string CurrentDirectory { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="LuaInterpreter"/> class. 
        /// </summary>
        public LuaInterpreter()
        {
            Lua(@"function totable(clrlist)
    local t = {}
    local it = clrlist:GetEnumerator()
    while it:MoveNext() do
        t[#t+1] = it.Current
    end
    return t
end");

            SetupAPIs();
            Application.ApplicationExit += (o, a) =>
            {
                Running = false;
            };
        }

        /// <summary>
        /// Scans the engine, frontend, and all mods for Lua-exposed classes and functions.
        /// </summary>
        public void SetupAPIs()
        {
            Lua.currentdir = (string.IsNullOrWhiteSpace(CurrentDirectory)) ? "0:" : CurrentDirectory;
            Lua.random = new Func<int, int, int>((min, max) =>
            {
                return new Random().Next(min, max);
            });
            Lua.registerEvent = new Action<string, Action<object>>((eventName, callback) =>
            {
                LuaEvent += (e, s) =>
                {
                    if (e == eventName)
                        try
                        {
                            callback?.Invoke(s);
                        }
                        catch(Exception ex)
                        {
                            Infobox.Show("Event propagation error.", "An error occurred while propagating the " + eventName + " event. " + ex.Message);
                        }
                };
            });
            //This temporary proxy() method will be used by the API prober.
            Lua.proxy = new Func<string, dynamic>((objName) =>
            {
                dynamic dynObj = ReflectMan.Types.FirstOrDefault(t => t.Name == objName);
                if (dynObj != null)
                    return dynObj;
                throw new Exception("{CLASS_NOT_FOUND}");
            });
            foreach (var type in ReflectMan.Types)
                foreach (var attr in Array.FindAll(type.GetCustomAttributes(false), a => a is ExposedAttribute))
                    Lua($"{(attr as ExposedAttribute).Name} = proxy(\"{type.Name}\")");
            //Now we can null out the proxy() method as it can cause security risks.
            Lua.isRunning = new Func<bool>(() => { return this.Running; });
            Lua.proxy = null;
            Lua.invokeOnWorkerThread = new Action<string>((func) =>
            {
                Desktop.InvokeOnWorkerThread(new Action(() =>
                {
                    Lua(func + "()");
                }));
            });
            Lua.invokeOnNewThread = new Action<string>((func) =>
            {
                var t = new Thread(new ThreadStart(() =>
                {
                    Lua(func + "()");
                }));
                t.IsBackground = true;
                t.Start();
            });
            Lua.includeScript = new Action<string>((file) =>
            {
                if (!Utils.FileExists(file))
                    throw new ArgumentException("File does not exist.");

                if (!file.EndsWith(".lua"))
                    throw new ArgumentException("File is not a lua file.");

                Lua(Utils.ReadAllText(file));
            });
        }

        /// <summary>
        /// Executes the specified file as an uncompressed Lua script.
        /// </summary>
        /// <param name="file">The file to execute.</param>
        public void ExecuteFile(string file)
        {
            if (Utils.FileExists(file))
            {
                CurrentDirectory = file.Replace(Utils.GetFileInfo(file).Name, "");
                Execute(Utils.ReadAllText(file));
            }
            else
            {
                throw new Exception("The file \"" + file + "\" was not found on the system.");
            }
        }

        /// <summary>
        /// Executes the specified string as a Lua script.
        /// </summary>
        /// <param name="lua">The Lua code to execute.</param>
        public void Execute(string lua)
        {
            try
            {
                Console.WriteLine("");
                Lua(lua);
                Console.WriteLine($"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
            }
            catch (Exception e)
            {
                Infobox.Show("Script error", $@"Script threw {e.GetType().Name}:

{e.Message}");
            }
        }

        /// <summary>
        /// Occurs when a Lua event is fired by C#.
        /// </summary>
        private static event Action<string, object> LuaEvent;

        /// <summary>
        /// Raises a Lua event with the specified name and caller object.
        /// </summary>
        /// <param name="eventName">The name of the event. Scripts use this to check what type of event occurred.</param>
        /// <param name="caller">The caller of the event. Scripts can use this to check if they should handle this event.</param>
        public static void RaiseEvent(string eventName, object caller)
        {
            LuaEvent?.Invoke(eventName, caller);
        }
    }

    /// <summary>
    /// Lua functions for .sft files.
    /// </summary>
    [Exposed("sft")]
    public class SFTFunctions
    {
        /// <summary>
        /// Make a .sft file from a lua code string
        /// </summary>
        /// <param name="lua">The Lua code</param>
        /// <returns>The resulting .sft string</returns>
        public string make(string lua)
        {
            return LuaInterpreter.CreateSft(lua);
        }

        /// <summary>
        /// Make a .sft string and save to a specified file.
        /// </summary>
        /// <param name="lua">The Lua code to compress</param>
        /// <param name="outpath">The path to save the compressed .sft file to.</param>
        public void makefile(string lua, string outpath)
        {
            Utils.WriteAllText(outpath, make(lua));
        }

        /// <summary>
        /// Run a compressed .sft file in the <see cref="LuaInterpreter"/>. 
        /// </summary>
        /// <param name="inpath">The .sft file to run.</param>
        public void run(string inpath)
        {
            LuaInterpreter.RunSft(inpath);
        }

        /// <summary>
        /// Reads the specified .sft file and decompresses to it's Lua form.
        /// </summary>
        /// <param name="sft">The .sft file to uncompress</param>
        /// <returns>The resulting Lua code.</returns>
        public string unmake(string sft)
        {
            if (Utils.FileExists(sft))
            {
                string b64 = Utils.ReadAllText(sft);
                byte[] bytes = Convert.FromBase64String(b64);
                string lua = Encoding.UTF8.GetString(bytes);
                return lua;
            }
            return "";
        }
    }

    /// <summary>
    /// Network functions for Lua.
    /// </summary>
    [Exposed("net")]
    public class NetFunctions
    {
        /// <summary>
        /// Submit a GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to open</param>
        /// <returns>The result from the server</returns>
        public string get(string url)
        {
            return new WebClient().DownloadString(url);
        }
        
    }

    /// <summary>
    /// Console functions for Lua.
    /// </summary>
    [Exposed("console")]
    public class ConsoleFunctions
    {
        /// <summary>
        /// Write text to the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void write(dynamic text)
        {
            Console.Write(text.ToString());
        }

        /// <summary>
        /// Write text to the console, followed by a new line.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void writeLine(dynamic text)
        {
            Console.WriteLine(text.ToString());
        }
    }

    /// <summary>
    /// The main ShiftOS API.
    /// </summary>
    [Exposed("sos")]
    public class SystemFunctions
    {
        /// <summary>
        /// Retrieves the user's Codepoints from the save file.
        /// </summary>
        /// <returns>The user's Codepoints.</returns>
        public ulong getCodepoints() { return SaveSystem.CurrentSave.Codepoints; }

        /// <summary>
        /// Run a command in the Terminal.
        /// </summary>
        /// <param name="cmd">The command to run, using regular ShiftOS syntax.</param>
        /// <returns>Whether the command was found and ran.</returns>
        public bool runCommand(string cmd)
        {
            var args = TerminalBackend.GetArgs(ref cmd);

            return TerminalBackend.RunClient(cmd, args);
        }

        /// <summary>
        /// Adds the specified amount of Codepoints to the save flie.
        /// </summary>
        /// <param name="cp">The codepoints to add.</param>
        public void addCodepoints(uint cp)
        {
            if (cp > 100 || cp <= 0)
            {
                throw new Exception("Value 'cp' must be at or below 100, and above 0.");
            }
            else
            {
                SaveSystem.CurrentSave.Codepoints += cp;
                SaveSystem.SaveGame();
            }
        }
    }
    
    /// <summary>
    /// User information API.
    /// </summary>
    [Exposed("userinfo")]
    public class UserInfoFunctions
    {
        /// <summary>
        /// Gets the user name of the currently logged in user.
        /// </summary>
        /// <returns>The user's username.</returns>
        public string getUsername()
        {
            return SaveSystem.CurrentUser.Username;
        }

        /// <summary>
        /// Retrieves the user's system name.
        /// </summary>
        /// <returns>The user's system name.</returns>
        public string getSysname()
        {
            return SaveSystem.CurrentSave.SystemName;
        }

        /// <summary>
        /// Gets the user's ShiftOS email (username@sysname).
        /// </summary>
        /// <returns>The user's email.</returns>
        public string getEmail()
        {
            return getUsername() + "@" + getSysname();
        }
    }

    /// <summary>
    /// Infobox API for Lua.
    /// </summary>
    [Exposed("infobox")]
    public class InfoboxFunctions
    {
        /// <summary>
        /// Show a message to the user in an Infobox.
        /// </summary>
        /// <param name="title">The title of the Infobox</param>
        /// <param name="message">The infobox's message</param>
        /// <param name="callback">A function to run when the user clicks "OK"</param>
        public void show(string title, string message, Action callback = null)
        {
            Infobox.Show(title, message, callback);
        }

        /// <summary>
        /// Ask a simple yes/no question to the user using an Infobox.
        /// </summary>
        /// <param name="title">The title of the Infobox</param>
        /// <param name="message">The infobox's message</param>
        /// <param name="callback">A function to run when they choose an option. The boolean argument will be true if the user clicks Yes, and false if they click No.</param>
        public void question(string title, string message, Action<bool> callback)
        {
            Infobox.PromptYesNo(title, message, callback);
        }

        /// <summary>
        /// Prompt the user for text using an Infobox.
        /// </summary>
        /// <param name="title">The infobox's title</param>
        /// <param name="message">The infobox's message</param>
        /// <param name="callback">A function to run when the user clicks "OK". The string value is the text entered by the user.</param>
        /// <param name="isPassword">Whether the text box should hide its characters as if it were a password box.</param>
        public void input(string title, string message, Action<string> callback, bool isPassword = false)
        {
            Infobox.PromptText(title, message, callback, isPassword);
        }
    }

    /// <summary>
    /// File Skimmer API for Lua.
    /// </summary>
    [Exposed("fileskimmer")]
    public class FileSkimmerFunctions
    {
        /// <summary>
        /// Opens a File Skimmer "Open File" dialog.
        /// </summary>
        /// <param name="extensions">Semicolon-separated list of file extensions that the opener should let through the filter.</param>
        /// <param name="callback">Function to be called when the user chooses a file. The string value is the file's path.</param>
        public void openFile(string extensions, Action<string> callback)
        {
            FileSkimmerBackend.GetFile(extensions.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries), FileOpenerStyle.Open, callback);
        }

        /// <summary>
        /// Opens a File Skimmer "Save File" dialog.
        /// </summary>
        /// <param name="extensions">Semicolon-separated list of file extensions that the opener should let through the filter.</param>
        /// <param name="callback">Function to be called when the user chooses a file. The string value is the file's path.</param>
        public void saveFile(string extensions, Action<string> callback)
        {
            FileSkimmerBackend.GetFile(extensions.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries), FileOpenerStyle.Save, callback);
        }
    }

    /// <summary>
    /// ShiftFS API for Lua.
    /// </summary>
    [Exposed("fs")]
    public class ShiftFSFunctions
    {
        /// <summary>
        /// Read all text in a file to a string.
        /// </summary>
        /// <param name="path">The file path to read</param>
        /// <returns>The string containing the file's contents.</returns>
        public string readAllText(string path)
        {
            return Utils.ReadAllText(path);
        }

        /// <summary>
        /// Copy a file from one place to another.
        /// </summary>
        /// <param name="i">The source file</param>
        /// <param name="o">The destination path</param>
        public void copy(string i, string o)
        {
            Utils.WriteAllBytes(o, Utils.ReadAllBytes(i));
        }

        /// <summary>
        /// Gets all files in the specified directory.
        /// </summary>
        /// <param name="dir">The directory to search</param>
        /// <returns>A string array containing all file paths in the directory.</returns>
        public string[] getFiles(string dir)
        {
            return Utils.GetFiles(dir);
        }

        /// <summary>
        /// Gets all directories inside a directory.
        /// </summary>
        /// <param name="dir">The directory to search</param>
        /// <returns>A string array containing all directory paths in the directory.</returns>
        public string[] getDirectories(string dir)
        {
            return Utils.GetDirectories(dir);
        }

        /// <summary>
        /// Read the binary contents of a file to a <see cref="byte"/> array. 
        /// </summary>
        /// <param name="path">The file path to read.</param>
        /// <returns>The resulting byte array.</returns>
        public byte[] readAllBytes(string path)
        {
            return Utils.ReadAllBytes(path);
        }

        /// <summary>
        /// Writes the specified text to a file.
        /// </summary>
        /// <param name="path">The file path</param>
        /// <param name="contents">The text to write</param>
        public void writeAllText(string path, string contents)
        {
            Utils.WriteAllText(path, contents);
        }

        /// <summary>
        /// Writes the specified binary data to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="contents">The binary data</param>
        public void writeAllBytes(string path, byte[] contents)
        {
            Utils.WriteAllBytes(path, contents);
        }

        /// <summary>
        /// Determines whether the specified path exists and is a file.
        /// </summary>
        /// <param name="path">The path to search</param>
        /// <returns>The result of the search.</returns>
        public bool fileExists(string path)
        {
            return Utils.FileExists(path);
        }

        /// <summary>
        /// Determines whether the specified path exists and is a directory.
        /// </summary>
        /// <param name="path">The path to search</param>
        /// <returns>The result of the search.</returns>
        public bool directoryExists(string path)
        {
            return Utils.DirectoryExists(path);
        }

        /// <summary>
        /// Deletes the file/directory at the specified path.
        /// </summary>
        /// <param name="path">The path to delete</param>
        public void delete(string path)
        {
            Utils.Delete(path);
        }

        /// <summary>
        /// Creates a new directory at the specified path.
        /// </summary>
        /// <param name="path">The path to create</param>
        public void createDirectory(string path)
        {
            Utils.CreateDirectory(path);
        }
    }

    /// <summary>
    /// Marks the specified class as a Lua API object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExposedAttribute : Attribute
    {
        /// <summary>
        /// If applied to a non-static class, the ShiftOS engine will see this class as a Lua object of the specified name.
        /// </summary>
        /// <param name="name">The name of the Lua object</param>
        public ExposedAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The API object's name
        /// </summary>
        public string Name { get; private set; }
    }
}
