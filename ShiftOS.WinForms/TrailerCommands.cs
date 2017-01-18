#define TRAILER
#if TRAILER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    [Namespace("trailer")]
    public static class TrailerCommands
    {
        [Command("init")]
        public static bool TrailerInit()
        {
            var oobe = new Oobe();
            oobe.StartTrailer();
            return true;
        }
    }
}
#endif