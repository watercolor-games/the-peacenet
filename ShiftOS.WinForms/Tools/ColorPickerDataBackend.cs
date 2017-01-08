/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
