using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.WinForms.DesktopWidgets
{
    [DesktopWidget("Terminal Widget", "Run commands inside a miniature Terminal.")]
    public partial class TerminalWidget : Applications.Terminal, IDesktopWidget
    {
        public TerminalWidget() : base()
        {
            this.Size = new Size(320, 200);
        }

        public void Setup()
        {
            OnLoad();
        }
    }
}
