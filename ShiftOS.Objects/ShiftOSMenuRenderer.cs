using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.Objects
{
    class ShiftOSMenuRenderer : ToolStripProfessionalRenderer
    {
        public ShiftOSMenuRenderer() : base(new ShiftOSColorTable())
        {

        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            
        }
    }

    public class ShiftOSColorTable : ProfessionalColorTable
    {

    }
}
