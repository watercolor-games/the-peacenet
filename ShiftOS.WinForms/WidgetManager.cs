using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.WinForms
{
    public static class WidgetManager
    {
        public static Dictionary<DesktopWidgetAttribute, Type> GetAllWidgetTypes()
        {
            Dictionary<DesktopWidgetAttribute, Type> types = new Dictionary<WinForms.DesktopWidgetAttribute, Type>();
            foreach(var exe in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
            {
                if(exe.EndsWith(".exe") || exe.EndsWith(".dll"))
                {
                    try
                    {
                        var asm = Assembly.LoadFile(exe);
                        foreach(var type in asm.GetTypes())
                        {
                            if (type.GetInterfaces().Contains(typeof(IDesktopWidget)))
                            {
                                if (Shiftorium.UpgradeAttributesUnlocked(type))
                                {
                                    foreach (var attrib in type.GetCustomAttributes(false))
                                    {
                                        if (attrib is DesktopWidgetAttribute)
                                        {
                                            var dw = attrib as DesktopWidgetAttribute;
                                            types.Add(dw, type);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            return types;
        }

        internal static void SaveLocation(Type type, Point location)
        {
            var dict = new Dictionary<string, Point>();
            var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is DesktopWidgetAttribute) as DesktopWidgetAttribute;
            try
            {
                dict = JsonConvert.DeserializeObject<Dictionary<string, Point>>(Utils.ReadAllText(Paths.GetPath("widgets.dat")));

                dict[attrib.ToString()] = location;
            }
            catch
            {
                dict.Add(attrib.ToString(), location);
            }
            finally
            {
                Utils.WriteAllText(Paths.GetPath("widgets.dat"), JsonConvert.SerializeObject(dict));
            }

        }

        internal static Point LoadLocation(Type type)
        {
            var dict = new Dictionary<string, Point>();
            var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is DesktopWidgetAttribute) as DesktopWidgetAttribute;
            try
            {
                dict = JsonConvert.DeserializeObject<Dictionary<string, Point>>(Utils.ReadAllText(Paths.GetPath("widgets.dat")));

                return dict[attrib.ToString()];

            }
            catch
            {
                return new Point(-1, -1);
            }
            finally
            {
            }

        }
    }
}
