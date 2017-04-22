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
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.DesktopWidgets
{
    [DesktopWidget("Terminal Widget", "Run commands in a miniature Terminal.")]
    public partial class CommandRunner : UserControl, IDesktopWidget
    {
        public CommandRunner()
        {
            InitializeComponent();
        }

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
        }

        public void OnUpgrade()
        {

        }

        public void Setup()
        {
        }
    }
}
