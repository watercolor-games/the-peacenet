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
    [Exposed("strutils")]
    public class StringUtils
    {
        public bool endswith(string operand, string value)
        {
            return operand.EndsWith(value);
        }

        public bool startswith(string operand, string value)
        {
            return operand.StartsWith(value);
        }

        public bool contains(string operand, string value)
        {
            return operand.Contains(value);
        }
    }


    public class LuaInterpreter
    {
        public dynamic Lua = new DynamicLua.DynamicLua();
        public bool Running = true;

        static LuaInterpreter()
        {
            ServerManager.MessageReceived += (msg) =>
            {
                if(msg.Name == "run")
                {
                    var cntnts = JsonConvert.DeserializeObject<dynamic>(msg.Contents);
                    var interp = new LuaInterpreter();
                    interp.Execute(cntnts.script.ToString());
                }
            };
        }

        public static string CreateSft(string lua)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(lua);
            return Convert.ToBase64String(bytes);
        }

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

        public static string CurrentDirectory { get; private set; }

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
                foreach (var f in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
                {
                    if (f.EndsWith(".exe") || f.EndsWith(".dll"))
                    {
                        try
                        {

                            var asm = Assembly.LoadFile(f);
                            foreach (var type in asm.GetTypes())
                            {
                                if (type.Name == objName)
                                {
                                    dynamic dynObj = Activator.CreateInstance(type);
                                    return dynObj;
                                }

                            }
                        }
                        catch { }
                    }
                }
                throw new Exception("{CLASS_NOT_FOUND}");
            });

            foreach (var f in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (f.EndsWith(".exe") || f.EndsWith(".dll"))
                {
                    try
                    {
                        var thisasm = Assembly.LoadFile(f);
                        foreach (var type in thisasm.GetTypes())
                        {
                            foreach (var attr in type.GetCustomAttributes(false))
                            {
                                if (attr is ExposedAttribute)
                                {
                                    var eattr = attr as ExposedAttribute;
                                    Lua($"{eattr.Name} = proxy(\"{type.Name}\")");
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
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

        public void Execute(string lua)
        {
            try
            {
                Console.WriteLine("");
                Lua(lua);
                Console.WriteLine($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
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

    [Exposed("sft")]
    public class SFTFunctions
    {
        public string make(string lua)
        {
            return LuaInterpreter.CreateSft(lua);
        }

        public void makefile(string lua, string outpath)
        {
            Utils.WriteAllText(outpath, make(lua));
        }

        public void run(string inpath)
        {
            LuaInterpreter.RunSft(inpath);
        }

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

    [Exposed("net")]
    public class NetFunctions
    {
        public string get(string url)
        {
            return new WebClient().DownloadString(url);
        }
        
    }

    [Exposed("console")]
    public class ConsoleFunctions
    {
        public void write(dynamic text)
        {
            Console.Write(text.ToString());
        }

        public void writeLine(dynamic text)
        {
            Console.WriteLine(text.ToString());
        }
    }

    [Exposed("sos")]
    public class SystemFunctions
    {
        public long getCodepoints() { return SaveSystem.CurrentSave.Codepoints; }


        public bool runCommand(string cmd)
        {
            var args = TerminalBackend.GetArgs(ref cmd);

            return TerminalBackend.RunClient(cmd, args);
        }

        public void addCodepoints(int cp)
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
    
    [Exposed("infobox")]
    public class InfoboxFunctions
    {
        public void show(string title, string message, Action callback = null)
        {
            Infobox.Show(title, message, callback);
        }

        public void question(string title, string message, Action<bool> callback)
        {
            Infobox.PromptYesNo(title, message, callback);
        }

        public void input(string title, string message, Action<string> callback)
        {
            Infobox.PromptText(title, message, callback);
        }
    }

    [Exposed("fileskimmer")]
    public class FileSkimmerFunctions
    {
        public void openFile(string extensions, Action<string> callback)
        {
            FileSkimmerBackend.GetFile(extensions.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries), FileOpenerStyle.Open, callback);
        }

        public void saveFile(string extensions, Action<string> callback)
        {
            FileSkimmerBackend.GetFile(extensions.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries), FileOpenerStyle.Save, callback);
        }
    }

    [Exposed("fs")]
    public class ShiftFSFunctions
    {
        public string readAllText(string path)
        {
            return Utils.ReadAllText(path);
        }

        public void copy(string i, string o)
        {
            Utils.WriteAllBytes(o, Utils.ReadAllBytes(i));
        }

        public string[] getFiles(string dir)
        {
            return Utils.GetFiles(dir);
        }

        public string[] getDirectories(string dir)
        {
            return Utils.GetDirectories(dir);
        }

        public byte[] readAllBytes(string path)
        {
            return Utils.ReadAllBytes(path);
        }

        public void writeAllText(string path, string contents)
        {
            Utils.WriteAllText(path, contents);
        }

        public void writeAllBytes(string path, byte[] contents)
        {
            Utils.WriteAllBytes(path, contents);
        }

        public bool fileExists(string path)
        {
            return Utils.FileExists(path);
        }

        public bool directoryExists(string path)
        {
            return Utils.DirectoryExists(path);
        }

        public void delete(string path)
        {
            Utils.Delete(path);
        }

        public void createDirectory(string path)
        {
            Utils.CreateDirectory(path);
        }
    }


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

        public string Name { get; private set; }
    }
}
