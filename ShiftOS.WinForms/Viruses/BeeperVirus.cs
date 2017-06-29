using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using System.Windows.Forms;

namespace ShiftOS.WinForms.Viruses
{
    [Virus("beeper", "Beeper", "Sends a few annoying beeps through the speaker repeatedly on a set interval. The threatlevel determines the interval.")]
    public class BeeperVirus : IVirus
    {
        private Timer _virusTimer = null;

        public void Disinfect()
        {
            _virusTimer.Stop();
            _virusTimer = null;
        }

        public void Infect(int threatlevel)
        {
            _virusTimer = new Timer();
            _virusTimer.Interval = 5000 / threatlevel;
            _virusTimer.Tick += (o, a) =>
            {
                Engine.AudioManager.PlayStream(Properties.Resources._3beepvirus);
            };
            _virusTimer.Start();
        }
    }
}
