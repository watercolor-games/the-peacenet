using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.WinForms
{
    public static class VirtualEnvironments
    {
        private static List<ShiftOSEnvironment> _environments = new List<ShiftOSEnvironment>();

        public static void Create(string sysname, List<ShiftOS.Objects.ClientSave> users, ulong cp, ShiftOS.Objects.ShiftFS.Directory fs)
        {
            var env = new ShiftOSEnvironment
            {
                SystemName = sysname,
                Users = users,
                Codepoints = cp,
                Filesystem = fs
            };
            _environments.Add(env);
        }

        public static void Clear()
        {
            _environments.Clear();
        }

        public static ShiftOSEnvironment Get(string sysname)
        {
            return _environments.FirstOrDefault(x => x.SystemName == sysname);
        }
    }

    public class ShiftOSEnvironment
    {
        public string SystemName { get; set; }
        public ulong Codepoints { get; set; }
        public ShiftOS.Objects.ShiftFS.Directory Filesystem { get; set; }
        public List<ShiftOS.Objects.ClientSave> Users { get; set; }
    }
}
