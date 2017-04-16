using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    public static class WidgetManager
    {
        public static Dictionary<DesktopWidgetAttribute, Type> GetAllWidgetTypes()
        {
            Dictionary<DesktopWidgetAttribute, Type> types = new Dictionary<WinForms.DesktopWidgetAttribute, Type>();
            foreach(var exe in Directory.GetFiles(Environment.CurrentDirectory))
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


    }
}
