using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.Frontend.Apps
{
    [FileHandler("Skin Loader", ".skn", "")]
    public class SkinLoader : IFileHandler
    {
        public void OpenFile(string file)
        {
            string skn = Utils.ReadAllText(file);
            Utils.WriteAllText(Paths.GetPath("skin.json"), skn);
            SkinEngine.LoadSkin();
            
        }
    }
}
