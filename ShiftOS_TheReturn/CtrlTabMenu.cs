using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine {
    static class CtrlTabMenu {
        public static AltTabWindow altTabWindow;

        internal static void Show() {
            if(altTabWindow != null) {
                altTabWindow = new AltTabWindow();
                altTabWindow.Show();
            }
        }

        internal static void CycleBack() {
            altTabWindow.CycleBack();
        }

        internal static void CycleForwards() {
            altTabWindow.CycleForwards();
        }
    }
}
