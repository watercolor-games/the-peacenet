using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.Wpf
{
    class WpfInfoboxFrontend : IInfobox
    {
        public void Open(string title, string msg)
        {
            var inf = new Applications.Infobox(title, msg);
            AppearanceManager.SetupDialog(inf);
        }
    }
}
