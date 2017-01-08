using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.WinForms.Tools
{
    public static class ColorPickerDataBackend
    {
        private static Color[] _redmemory = new Color[20];
        private static Color[] _greenmemory = new Color[20];
        private static Color[] _bluememory = new Color[20];
        private static Color[] _orangememory = new Color[20];
        private static Color[] _pinkmemory = new Color[20];
        private static Color[] _yellowmemory = new Color[20];
        private static Color[] _purplememory = new Color[20];
        private static Color[] _graymemory = new Color[20];
        private static Color[] _anymemory = new Color[20];
        private static Color[] _brownmemory = new Color[20];


        public static Color[] anymemory
        {
            get { return _anymemory; }
        }

        public static Color[] redmemory
        {
            get { return _redmemory; }
        }

        public static Color[] greenmemory
        {
            get { return _greenmemory; }
        }

        public static Color[] bluememory
        {
            get { return _bluememory; }
        }

        public static Color[] brownmemory
        {
            get { return _brownmemory; }
        }

        public static Color[] graymemory
        {
            get { return _graymemory; }
        }

        public static Color lastcolourpick { get; set; }

        public static Color[] orangememory
        {
            get { return _orangememory; }
        }

        public static Color[] pinkmemory
        {
            get { return _pinkmemory; }
        }

        public static Color[] purplememory
        {
            get { return _purplememory; }
        }

        public static Color[] yellowmemory
        {
            get { return _yellowmemory; }
        }

        
    }



}
