using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using System.Windows.Forms;

namespace ShiftOS.WinForms.Viruses
{
    [Virus("shifticide", "Shifticide",  "Extremely deadly virus. This will delete your Home directory, and cannot be recovered.")]
    public class Shifticide : IVirus
    {
        private Timer _virusTimer = null;
        void IVirus.Disinfect()
        {
            _virusTimer.Stop();
            _virusTimer = null;
            if (ShiftOS.Objects.ShiftFS.Utils.DirectoryExists("0:/home"))
                ShiftOS.Objects.ShiftFS.Utils.CreateDirectory("0:/home");
            
        }

        void IVirus.Infect(int threatlevel)
        {
            _virusTimer = new Timer();
            _virusTimer.Interval = 5000 / threatlevel;
            if (ShiftOS.Objects.ShiftFS.Utils.DirectoryExists("0:/home"))
            ShiftOS.Objects.ShiftFS.Utils.Delete("0:/home");
        }
    }
}
