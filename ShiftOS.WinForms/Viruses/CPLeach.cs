using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Viruses
{
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
                if (SaveSystem.CurrentSave.Codepoints <= codepointDecrease)
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
