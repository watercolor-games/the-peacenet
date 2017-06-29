using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
#if DEBUG
    public static class VirusTestCommands
    {
        [Command("infect", description = "DEBUG: Infect the system with a virus.")]
        [RequiresArgument("id")]
        [RequiresArgument("threatlevel")]
        public static void Infect(Dictionary<string, object> args)
        {
            var id = args["id"].ToString();
            var threatlevel = Convert.ToInt32(args["threatlevel"].ToString());

            VirusManager.Infect(id, threatlevel);
        }
        [Command("disinfect", description = "DEBUG: Disinfect the system, removing the specified virus.")]
        [RequiresArgument("id")]
        public static void Disinfect(Dictionary<string, object> args)
        {
            var id = args["id"].ToString();

            VirusManager.Disinfect(id);
        }
    }
#endif
}