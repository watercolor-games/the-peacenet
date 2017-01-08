using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
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
                var asm = Assembly.GetExecutingAssembly();
                foreach (var type in asm.GetTypes())
                {
                    if (type.Name == objName)
                    {
                        dynamic dynObj = Activator.CreateInstance(type);
                        return dynObj;
                    }

                }
                throw new Exception("{CLASS_NOT_FOUND}");
            });

            var thisasm = Assembly.GetExecutingAssembly();
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

    public class LuaVirus : Virus
    {
        public override string Signature
        {
            get
            {
                return signature;
            }
        }

        public override int ThreatLevel
        {
            get
            {
                return _threatlevel;
            }
        }

        public override string Type
        {
            get
            {
                return "lua";
            }
        }

        private LuaInterpreter interpreter = null;
        private string signature = "unknown";

        private int _threatlevel = 0;

        public LuaVirus(string script, int threatLevel) : base()
        {
            _threatlevel = threatLevel;

            interpreter = new LuaInterpreter();

            Action<string> mud_message = new Action<string>((sig) =>
            {
                signature = sig;
            });

            ServerManager.MessageReceived += (msg) =>
            {
                if(msg.Name == "mud_virus_signature")
                {
                    mud_message?.Invoke(msg.Contents);
                    mud_message = null;
                }
            };

            ServerManager.SendMessage("mud_scanvirus", script);

            _script = script;
        }


        private string _script = "";
        
        public override void Activate()
        {
            interpreter.Execute(_script);
        }

        public override void Deactivate()
        {
            interpreter.Running = false;
            interpreter = null;
        }
    }

    [Exposed("consts")]
    public class Constants
    {
        public readonly DockStyle dock_none = DockStyle.None;
        public readonly DockStyle dock_top = DockStyle.Top;
        public readonly DockStyle dock_bottom = DockStyle.Bottom;
        public readonly DockStyle dock_left = DockStyle.Left;
        public readonly DockStyle dock_right = DockStyle.Right;
        public readonly DockStyle dock_fill = DockStyle.Fill;

        public readonly AnchorStyles anchor_none = 0x00;
        public readonly AnchorStyles anchor_top = AnchorStyles.Top;
        public readonly AnchorStyles anchor_bottom = AnchorStyles.Bottom;
        public readonly AnchorStyles anchor_left = AnchorStyles.Left;
        public readonly AnchorStyles anchor_right = AnchorStyles.Right;

    }

    [Exposed("shiftos")]
    public class CoreAPI
    {
        public void sleep(int ms) { Thread.Sleep(ms); }

        public bool isRunning() { return !SaveSystem.ShuttingDown; }

        public int random(int min, int max)
        {
            return new Random().Next(min, max);
        }

        public void info(string title, string message)
        {
            Infobox.Show(title, message);
        }

        public void executeCommand(string cmd)
        {
            TerminalBackend.InvokeCommand(cmd);
        }

        public void shutdown()
        {
            TerminalBackend.InvokeCommand("sys.shutdown");
        }
    }
    [Exposed("shiftorium")]
    public class ShiftoriumAPI
    {
        public dynamic[] getAvailable()
        {
            return Shiftorium.GetAvailable();
        }

        public dynamic[] getAll()
        {
            return Shiftorium.GetDefaults().ToArray();
        }

        public bool upgradeInstalled(string id)
        {
            return Shiftorium.UpgradeInstalled(id);
        }

        public bool buy(ShiftoriumUpgrade upgrade)
        {
            foreach (var upg in getAvailable())
            {
                if (upg.ID == upgrade && SaveSystem.CurrentSave.Codepoints >= upg.Cost)
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Exposed("gui")]
    public class GuiAPI
    {
        public void addControl(dynamic parent, dynamic child)
        {
            parent.Controls.Add(child);
        }

        public dynamic size(int width, int height)
        {
            return new System.Drawing.Size(width, height);
        }

        public dynamic point(int x, int y)
        {
            return new System.Drawing.Point(x, y);
        }

        public dynamic rect(dynamic p, dynamic s)
        {
            return new System.Drawing.Rectangle(p, s);
        }

        public dynamic rect(int x, int y, int w, int h)
        {
            return new System.Drawing.Rectangle(x, y, w, h);
        }

        public dynamic panel()
        {
            return new Panel();
        }

        public dynamic label()
        {
            return new Label();
        }

        public dynamic button()
        {
            return new Button();
        }

        public dynamic listbox()
        {
            return new ListBox();
        }
        
        public dynamic combobox()
        {
            return new ComboBox();
        }

        public dynamic textbox()
        {
            return new TextBox();
        }

        public dynamic window()
        {
            return new Form();
        }

    }

    [Exposed("color")]
    public class ColorAPI
    {
        public dynamic fromRgb(int r, int g, int b)
        {
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        public dynamic fromArgb(int a, int r, int g, int b)
        {
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        public dynamic fromName(string name)
        {
            return System.Drawing.Color.FromName(name);
        }
    }

    [Exposed("fonts")]
    public class FontsAPI
    {
        public readonly int style_regular = 0x00;
        public readonly int style_bold = 0x01;
        public readonly int style_italic = 0x02;
        public readonly int style_underline = 0x04;
        public readonly int style_strike = 0x08;

        public dynamic create(string name, float size, int style)
        {
            return new System.Drawing.Font(name, size, (System.Drawing.FontStyle)style);
        }

    }

    [Exposed("json")]
    public class JsonAPI
    {
        public string serialize(dynamic dynObj)
        {
            return JsonConvert.SerializeObject(dynObj);
        }

        public dynamic deserialize(string json)
        {
            return JsonConvert.DeserializeObject<dynamic>(json);
        }
    }

    [Exposed("fs")]
    public class FilesystemAPI
    {
        public void writeAllText(string target, string contents)
        {
            Utils.WriteAllText(target, contents);
        }

        public string readAllText(string file)
        {
            return Utils.ReadAllText(file);
        }

        public bool fileExists(string file)
        {
            return Utils.FileExists(file);
        }

        public bool directoryExists(string dir)
        {
            return Utils.DirectoryExists(dir);
        }

        public void createDirectory(string dir)
        {
            Utils.CreateDirectory(dir);
        }

        //experimental - no idea how arrays will be handled in Lua...
        public string[] getFiles(string dir)
        {
            return Utils.GetFiles(dir);
        }

        public string[] getDirectories(string dir)
        {
            return Utils.GetDirectories(dir);
        }

        public string mount(string mfsFile)
        {
            Utils.MountPersistent(mfsFile);
            return $"{Utils.Mounts.Count - 1}:";
        }

        public void unmount(int driveNum)
        {
            Utils.Mounts.RemoveAt(driveNum);
        }

    }


    public class PythonInterpreter
    {
        ScriptEngine python;
        ScriptScope pyScope;
        ScriptSource pySource;

        public PythonInterpreter()
        {
            python = Python.CreateEngine();
            pyScope = python.CreateScope();
            AddExposedObjects();
        }

        private void AddExposedObjects()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach(var type in asm.GetTypes())
            {
                foreach(var attr in type.GetCustomAttributes(false))
                {
                    if(attr is ExposedAttribute)
                    {
                        var eattr = attr as ExposedAttribute;
                        dynamic dynObj = Activator.CreateInstance(type);
                        pyScope.SetVariable(eattr.Name, dynObj);
                    }
                }
            }
        }

        public void ExecuteFile(string file)
        {
            if(Utils.FileExists(file))
            {
                Execute(Utils.ReadAllText(file));
            }
            else
            {
                throw new Exception("The file \"" + file + "\" was not found on the system.");
            }
        }

        public void Execute(string py)
        {
            Console.WriteLine("");
            pySource = python.CreateScriptSourceFromString(py, Microsoft.Scripting.SourceCodeKind.AutoDetect);
            pySource.Execute(pyScope);
            Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
        }
    }

    public class ExposedAttribute : Attribute
    {
        /// <summary>
        /// Marks the following class as exposed to the ShiftOS Scripting System.
        /// </summary>
        /// <param name="objName">The object name that the Scripting System should use as the variable name for scripts.</param>
        public ExposedAttribute(string objName)
        {
            Name = objName;
        }

        public string Name { get; set; }
    }
}
