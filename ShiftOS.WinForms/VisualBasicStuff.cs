using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;

namespace ShiftOS.WinForms
{
    public static class My
    {
        static My()
        {
            _computer = new Computer();
        }

        private static Computer _computer;

        public static Computer Computer
        {
            get
            {
                return _computer;
            }
        }
    }
}
