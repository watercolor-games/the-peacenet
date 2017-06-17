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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Receives events from the interpreter.
    /// </summary>
    public interface IBFListener
    {
        void PointerMoved(ushort newval);
        void IPtrMoved(int newval);
        void MemChanged(ushort pos, byte newval);
        void MemReset();
    }
    /// <summary>
    /// Interprets Brainfuck code.
    /// </summary>
    public class BFInterpreter
    {
        private byte[] mem;
        private ushort ptr;
        private string prg;
        private object lck;
        private Stream str;

        public IBFListener lst = null;
        
        public BFInterpreter(Stream io, IBFListener listener = null, string program = "")
        {
            lck = new object();
            prg = program;
            str = io;
            Reset();
            lst = listener;
        }
        public void Execute()
        {
            lock (lck)
                for (int iptr = 0; iptr < prg.Length; iptr++)
                {
                    if (lst != null)
                        lst.IPtrMoved(iptr);
                    switch (prg[iptr])
                    {
                        case '>':
                            ptr++;
                            if (lst != null)
                                lst.PointerMoved(ptr);
                            break;
                        case '<':
                            ptr--;
                            if (lst != null)
                                lst.PointerMoved(ptr);
                            break;
                        case '+':
                            mem[ptr]++;
                            if (lst != null)
                                lst.MemChanged(ptr, mem[ptr]);
                            break;
                        case '-':
                            mem[ptr]--;
                            if (lst != null)
                                lst.MemChanged(ptr, mem[ptr]);
                            break;
                        case '.':
                            str.WriteByte(mem[ptr]);
                            break;
                        case ',':
                            mem[ptr] = (byte)str.ReadByte();
                            if (lst != null)
                                lst.MemChanged(ptr, mem[ptr]);
                            break;
                        case '[':
                            if (mem[ptr] == 0)
                                while (prg[iptr] != ']')
                                {
                                    iptr++;
                                    if (lst != null)
                                        lst.IPtrMoved(iptr);
                                }
                            break;
                        case ']':
                            if (mem[ptr] != 0)
                                while (prg[iptr] != '[')
                                {
                                    iptr--;
                                    if (lst != null)
                                        lst.IPtrMoved(iptr);
                                }
                            break;
                    }
                }
        }
        public void Execute(string program)
        {
            lock (lck)
                prg = program;
            Execute();
        }
        public void Reset()
        {
            lock (lck)
            {
                mem = new byte[30000];
                ptr = 0;
                if (lst != null)
                {
                    lst.MemReset();
                    lst.PointerMoved(0);
                }
            }
        }
    }
}
