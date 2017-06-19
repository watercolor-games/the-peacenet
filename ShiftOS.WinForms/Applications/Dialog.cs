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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [DefaultTitle("{TITLE_INFOBOX}")]
    [DefaultIcon("iconInfoBox")]
    public partial class Dialog : UserControl, IShiftOSWindow, IInfobox
    {
        public Dialog()
        {
            InitializeComponent();
        }

        public string Title { get; private set; }

        public void OnLoad()
        {
            if(!string.IsNullOrWhiteSpace(Title))
                AppearanceManager.SetWindowTitle(this, this.Title);
            ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.infobox);
        }

        public void OnSkinLoad()
        {
            if(!string.IsNullOrWhiteSpace(Title))
                AppearanceManager.SetWindowTitle(this, this.Title);

            int top = btnok.Top;
            btnok.CenterParent();
            btnyes.Parent.CenterParent();
            btnok.Top = top;
            btnyes.Parent.Top = top;
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        internal void OpenInternal(string title, string msg, Action c)
        {
            Title = title;
            AppearanceManager.SetupDialog(this);
            lbmessage.Text = msg;
            txtinput.Hide();
            flyesno.Hide();
            btnok.Show();
            btnok.Click += (o, a) =>
            {
                AppearanceManager.Close(this);
                c?.Invoke();
            };

        }

        public void Open(string title, string msg, Action c = null)
        {
            new Dialog().OpenInternal(title, msg, c);
        }

        public void PromptTextInternal(string title, string message, Action<string> callback, bool isPassword)
        {
            Title = title;
            AppearanceManager.SetupDialog(this);
            lbmessage.Text = message;
            txtinput.UseSystemPasswordChar = isPassword;
            txtinput.Show();
            flyesno.Hide();
            btnok.Show();
            btnok.Click += (o, a) =>
            {
                callback?.Invoke(txtinput.Text);
                AppearanceManager.Close(this);
            };
            txtinput.KeyDown += (o, a) =>
            {
                if(a.KeyCode == Keys.Enter)
                {
                    a.SuppressKeyPress = true;
                    callback?.Invoke(txtinput.Text);
                    AppearanceManager.Close(this);
                }
            };
        }

        public void PromptText(string title, string message, Action<string> callback, bool isPassword)
        {
            new Dialog().PromptTextInternal(title, message, callback, isPassword);
        }

        public void PromptYesNo(string title, string message, Action<bool> callback)
        {
            new Dialog().PromptYesNoInternal(title, message, callback);
        }



        public void PromptYesNoInternal(string title, string message, Action<bool> callback)
        {
            Title = title;
            AppearanceManager.SetupDialog(this);
            lbmessage.Text = message;
            txtinput.Hide();
            flyesno.Show();
            btnok.Hide();
            btnyes.Click += (o, a) =>
            {
                callback?.Invoke(true);
                AppearanceManager.Close(this);
            };
            btnno.Click += (o, a) =>
            {
                callback?.Invoke(false);
                AppearanceManager.Close(this);
            };
        }
    }
}
