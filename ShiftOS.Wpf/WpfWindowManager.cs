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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ShiftOS.Engine;

namespace ShiftOS.Wpf
{
    public class WpfWindowManager : WindowManager
    {
        public override void Close(IShiftOSWindow win)
        {
            IWindowBorder brdrToClose = null;

            foreach(WpfWindowBorder brdr in AppearanceManager.OpenForms)
            {
                if(brdr.ParentWindow == win)
                {
                    brdrToClose = brdr;
                }
            }

            if (brdrToClose != null)
            {
                brdrToClose.Close();
                AppearanceManager.OpenForms.Remove(brdrToClose);
            }

        }

        public override void InvokeAction(Action act)
        {
            App.Current.Dispatcher.Invoke(act);
        }

        public override void Maximize(IWindowBorder border)
        {
            var wb = (WpfWindowBorder)border;
        }

        public override void Minimize(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public override void SetupDialog(IShiftOSWindow win)
        {
            var brdr = new WpfWindowBorder(win);
            brdr.IsDialog = true;
            AppearanceManager.OpenForms.Add(brdr);
        }

        public override void SetupWindow(IShiftOSWindow win)
        {
            var brdr = new WpfWindowBorder(win);

            AppearanceManager.OpenForms.Add(brdr);
        }
    }
}
