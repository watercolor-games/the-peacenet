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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public static class ReflectMan
    {
        private static Assembly[] asms = null;
        public static Assembly[] Asms
        {
            get
            {
                if (asms == null)
                    LoadAssemblies();
                return asms;
            }
        }

        private static Type[] types = null;
        public static Type[] Types
        {
            get
            {
                if (types == null)
                    FindTypes();
                return types;
            }
        }

        private static void LoadAssemblies()
        {
            var ret = new List<Assembly>();
            foreach (var exe in Array.FindAll(System.IO.Directory.GetFiles(Environment.CurrentDirectory), n => n.EndsWith(".exe") || n.EndsWith(".dll")))
                try
                {
                    var asm = Assembly.LoadFile(exe);
                    if (asm.GetReferencedAssemblies().Contains("ShiftOS.Engine") || asm.FullName.Contains("ShiftOS.Engine"))
                        ret.Add(asm);
                }
                catch { } // probably not a managed assembly
            asms = ret.ToArray();
        }

        private static void FindTypes()
        {
            var ret = new List<Type>();
            foreach (var asm in Asms)
                ret.AddRange(asm.GetTypes());
            types = ret.ToArray();
        }
    }
}
