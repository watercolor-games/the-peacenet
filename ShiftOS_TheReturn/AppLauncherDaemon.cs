using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ShiftOS.Engine
{
    public static class AppLauncherDaemon
    {
        public static bool Contains(this AssemblyName[] asms, string name)
        {
            foreach(var asm in asms)
            {
                if (asm.FullName.Contains(name))
                    return true;
            }
            return false;
        }

        public static List<LauncherItem> Available()
        {
            List<LauncherItem> win = new List<LauncherItem>();
            
            foreach (var asmExec in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (asmExec.EndsWith(".dll") | asmExec.EndsWith(".exe"))
                {
                    var asm = Assembly.LoadFrom(asmExec);

                    if (asm.GetReferencedAssemblies().Contains("ShiftOS.Engine") || asm.FullName.Contains("ShiftOS.Engine"))
                    {
                            foreach (var type in asm.GetTypes())
                            {
                                if (type.GetInterfaces().Contains(typeof(IShiftOSWindow)))
                                {
                                    foreach (var attr in type.GetCustomAttributes(false))
                                    {
                                        if (attr is LauncherAttribute)
                                        {
                                            var launch = attr as LauncherAttribute;
                                            if (launch.UpgradeInstalled)
                                            {
                                                win.Add(new LauncherItem { DisplayData = launch, LaunchType = type });
                                            }
                                        }
                                    }
                                }
                            }
                    }
                }
            }
            return win;
        }

    }

    public class LauncherItem
    {
        public LauncherAttribute DisplayData { get; internal set; }
        public Type LaunchType { get; internal set; }

    }
}
