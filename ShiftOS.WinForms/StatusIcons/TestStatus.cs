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

namespace ShiftOS.WinForms.StatusIcons
{
    [DefaultIcon("iconShiftorium")]
    public partial class TestStatus : UserControl, IStatusIcon
    {
        public TestStatus()
        {
            InitializeComponent();
        }

        public void Setup()
        {
        }
    }
}
