using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public static class ConsoleEx
    {
        static ConsoleEx()
        {
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;

            Bold = false;
            Italic = false;
            Underline = false;
        }

        public static ConsoleColor ForegroundColor { get; set; }
        public static ConsoleColor BackgroundColor { get; set; }

        public static bool Bold { get; set; }
        public static bool Italic { get; set; }
        public static bool Underline { get; set; }
    }
}
