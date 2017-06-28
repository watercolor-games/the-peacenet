using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Viruses
{
    [Virus("windows_everywhere", "Windows Everywhere", "Makes the windows dance around the screen if the user has the WM Free Placement upgrade. Speed depends on threatlevel.")]
    [RequiresUpgrade("wm_free_placement")]
    public class WindowsEverywhere : IVirus
    {
        private System.Windows.Forms.Timer timer = null;
        private int ThreatLevel = 1;
        private Dictionary<string, System.Drawing.Point> Velocities = null;


        public void Infect(int threatlevel)
        {
            Velocities = new Dictionary<string, System.Drawing.Point>();
            ThreatLevel = threatlevel;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            timer.Tick += (o, a) =>
            {
                foreach (var win in AppearanceManager.OpenForms)
                {
                    var border = (win as WindowBorder);
                    var loc = border.Location;
                    var velocity = new System.Drawing.Point(1, 1);
                    string vKey = border.GetType().Name + "_" + border.GetHashCode();
                    if (!Velocities.ContainsKey(vKey))
                    {
                        Velocities.Add(vKey, velocity);
                    }
                    else
                    {
                        velocity = Velocities[vKey];
                    }

                    //Calculate proper velocity.
                    if (border.Top <= 0)
                    {
                        velocity.Y = 1;
                    }
                    if (border.Top + border.Height >= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
                    {
                        velocity.Y = -1;
                    }
                    if (border.Left <= 0)
                    {
                        velocity.X = 1;
                    }
                    if (border.Left + border.Width >= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width)
                    {
                        velocity.X = -1;
                    }
                    //save velocity in memory.
                    Velocities[vKey] = velocity;
                    //convert using threatlevel

                    var velocityX = velocity.X * (threatlevel * 4);
                    var velocityY = velocity.Y * (threatlevel * 4);

                    loc.X += velocityX;
                    loc.Y += velocityY;

                    border.Location = loc;
                }
            };
            timer.Start();
        }

        public void Disinfect()
        {
            timer.Stop();
            Velocities.Clear();
            ThreatLevel = 0;
            timer = null;
        }
    }
}
