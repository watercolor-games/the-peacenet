using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.Frontend
{
    class PayloadFunc
    {
        public static void DoHackFunction(int function)
        {
            switch (function)
            {
                default:
                    break;
                case 1:
                    Hacking.CurrentHackable.DoConnectionTimeout = false;
                    break;
            }
        }
    }
}
