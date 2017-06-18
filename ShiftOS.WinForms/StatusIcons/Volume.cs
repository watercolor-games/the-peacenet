using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.WinForms.Controls;

namespace ShiftOS.WinForms.StatusIcons
{
    [DefaultIcon("iconSpeaker")]
    public partial class Volume : UserControl, IStatusIcon
    {
        public Volume()
        {
            InitializeComponent();
        }

        public void Setup()
        {
            lbvolume.Top = (this.Height - lbvolume.Height) / 2;
            bool moving = false;
            var prg = new ShiftedProgressBar();
            prg.Tag = "ignoreal"; //Don't close AL or current widget when this control is clicked.
            this.Controls.Add(prg);
            prg.Height = this.Height / 3;
            prg.Maximum = 100;
            prg.Value = SaveSystem.CurrentSave.MusicVolume;
            prg.Width = this.Width - lbvolume.Width - 50;
            prg.Left = lbvolume.Left + lbvolume.Width + 15;
            prg.Top = (this.Height - prg.Height) / 2;
            prg.MouseDown += (o, a) =>
            {
                moving = true;
            };

            prg.MouseMove += (o, a) =>
            {
                if (moving)
                {
                    int val = (int)linear(a.X, 0, prg.Width, 0, 100);
                    SaveSystem.CurrentSave.MusicVolume = val;
                    prg.Value = val;
                }
            };
            
            prg.MouseUp += (o, a) =>
            {
                moving = false;
            };
            prg.Show();
        }

        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}
