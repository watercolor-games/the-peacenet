using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.WinForms
{
    public static class SkinCommands
    {
        [Command("reset")]
        [RequiresUpgrade("shifter")]
        public static bool ResetSkin()
        {
            Utils.WriteAllText(Paths.GetPath("skin.json"), JsonConvert.SerializeObject(new Skin()));
            SkinEngine.LoadSkin();
            return true;
        }
    }
}
