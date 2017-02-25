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

namespace ShiftOS.WinForms.Applications
{
    public partial class AudioPlayer : UserControl, IShiftOSWindow
    {
        public AudioPlayer()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            
        }

        public void OnSkinLoad()
        {
            
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            
        }
    }
}
