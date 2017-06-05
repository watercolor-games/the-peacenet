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

using Newtonsoft.Json;
using ShiftOS.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.Engine
{
    public class OutOfBoxExperience
    {
        private static IOobe _oobe = null;

        public static void Init(IOobe oobe)
        {
            _oobe = oobe; // takes the oobe and makes it an IOobe
        }

        public static void Start(Save save)
        {
            //if its null then FUCK YOU DID THE WRONG THING
            if (_oobe == null)
                throw new InvalidOperationException("OOBE frontend not activated! This function can't be used! Please use OutOfBoxExperience.Init() passing an IOobe-implementing object to start the OOBE frontend.");


            _oobe.StartShowing(save); //tells the save data to start showing the oobe


        }

        public static void PromptForLogin()
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                _oobe.PromptForLogin(); //prompts for login, what did you expect
            }));
        }

        public static void ShowSaveTransfer(Save save)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                _oobe.ShowSaveTransfer(save); //triggers save transfer if not done already
                
            }));
        }
    }

    //triggers all the above events
    public interface IOobe
    {
        void StartShowing(Save save);
        void ShowSaveTransfer(Save save);
        void PromptForLogin();
    }
}
