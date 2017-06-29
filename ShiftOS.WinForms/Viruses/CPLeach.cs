using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Viruses
{
    [Virus("cpleach", "Codepoint Leach", "Every 6 seconds, a certain amount of Codepoints is taken from the user until they're left with nothing. The amount taken away each time is determined by the threat level.")]
    public class CPLeach : IVirus
    {
        public System.Windows.Forms.Timer Timer = null;

        public void Infect(int threatlevel)
        {
            Timer = new System.Windows.Forms.Timer();
            Timer.Interval = 6000;
            Timer.Tick += (o, a) =>
            {
                ulong codepointDecrease = (ulong)threatlevel * 4;
                if (SaveSystem.CurrentSave.Codepoints > codepointDecrease)
                    SaveSystem.CurrentSave.Codepoints -= codepointDecrease;
                else
                    SaveSystem.CurrentSave.Codepoints = 0;
            };
            Timer.Start();
        }

        public void Disinfect()
        {
            Timer.Stop();
        }
    }
}
