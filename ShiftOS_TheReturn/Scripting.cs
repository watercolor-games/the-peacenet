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

namespace ShiftOS.Engine.Scripting
{
    public class LuaInterpreter
    {
        dynamic Lua = new DynamicLua.DynamicLua();
        public bool Running = true;

        public LuaInterpreter()
        {
            SetupAPIs();
            Application.ApplicationExit += (o, a) =>
            {
                Running = false;
            };
        }

        public void SetupAPIs()
        {

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
        }

        
        public void ExecuteFile(string file)
        {
            if (Utils.FileExists(file))
            {
                Execute(Utils.ReadAllText(file));
            }
            else
            {
                throw new Exception("The file \"" + file + "\" was not found on the system.");
            }
        }

        public void Execute(string lua)
        {
                Console.WriteLine("");
                Lua(lua);
                Console.WriteLine($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
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
        public int getCodepoints() { return SaveSystem.CurrentSave.Codepoints; }


        public bool runCommand(string cmd)
        {
            var args = TerminalBackend.GetArgs(ref cmd);

            return TerminalBackend.RunClient(cmd, args);
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
