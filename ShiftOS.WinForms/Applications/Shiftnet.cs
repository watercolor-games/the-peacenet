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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using Newtonsoft.Json;
using static ShiftOS.Engine.SkinEngine;
using ShiftOS.WinForms.Tools;
using System.IO;
using System.Reflection;

namespace ShiftOS.WinForms.Applications
{
    //OH MY GOD. I HAD TO CORRECT HEAPS OF BADLY PLACED CURLY BRACES.
    //
    //I DO NOT CARE WHAT THE BLUE SMILEY FACE SAYS. CURLY BRACES BELONG ON THEIR OWN LINE.
    //
    //READ THE DAMN SHIFTOS CODING GUIDELINES.

    [Launcher("{TITLE_SHIFTNET}", true, "al_shiftnet", "{AL_NETWORKING}")]
    [MultiplayerOnly]
    [DefaultTitle("{TITLE_SHIFTNET}")]
    [WinOpen("{WO_SHIFTNET}")]
    [RequiresUpgrade("victortran_shiftnet")]
    [DefaultIcon("iconShiftnet")]
    public partial class Shiftnet : UserControl, IShiftOSWindow, IShiftnetClient
    {
        public Shiftnet()
        {
            InitializeComponent();
        }

        public string CurrentUrl { get; set; }


        public Stack<string> History = new Stack<string>();
        public Stack<string> Future = new Stack<string>();

        public IShiftnetSite CurrentPage = null;

        public void OnLoad()
        {
            NavigateToUrl("shiftnet/main");
        }

        public void OnSkinLoad()
        {
            CurrentPage?.OnSkinLoad();
            btnback.Location = new Point(2, 2);
            btnforward.Location = new Point(btnback.Left + btnback.Width + 2, 2);
            txturl.Location = new Point(btnforward.Left + btnforward.Width + 2, 2);
            txturl.Width = flcontrols.Width - btnback.Width - 2 - btnforward.Width - 2 - (btngo.Width*2) - 2;
            btngo.Location = new Point(flcontrols.Width - btngo.Width - 2, 2);
            flcontrols.BackColor = SkinEngine.LoadedSkin.TitleBackgroundColor;
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            CurrentPage?.OnUpgrade();
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            try
            {
                string hist = History.Pop();
                if (!string.IsNullOrEmpty(hist))
                {
                    Future.Push(hist);
                    NavigateToUrl(hist);
                }
            }
            catch
            {

            }
        }

        private void btnforward_Click(object sender, EventArgs e)
        {
            try
            {
                string fut = Future.Pop();
                if (!string.IsNullOrEmpty(fut))
                {
                    History.Push(CurrentUrl);
                    NavigateToUrl(fut);
                }
            }
            catch
            {

            }
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txturl.Text))
            {
                Future.Clear();
                History.Push(CurrentUrl);
                NavigateToUrl(txturl.Text);
            }
        }

        private void txturl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btngo_Click(sender, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        public void NavigateToUrl(string url)
        {
            
            txturl.Text = url;
            try
            {
                foreach (var type in Array.FindAll(ReflectMan.Types, t => t.GetInterfaces().Contains(typeof(IShiftnetSite)) && t.BaseType == typeof(UserControl) && Shiftorium.UpgradeAttributesUnlocked(t)))
                {
                    var attribute = type.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftnetSiteAttribute && (x as ShiftnetSiteAttribute).Url == url) as ShiftnetSiteAttribute;
                    if (attribute != null)
                    {
                            var obj = (IShiftnetSite)Activator.CreateInstance(type, null);
                            obj.GoToUrl += (u) =>
                            {
                                History.Push(CurrentUrl);
                                NavigateToUrl(u);
                            };
                            obj.GoBack += () =>
                            {
                                string u = History.Pop();
                                Future.Push(u);
                                NavigateToUrl(u);
                            };
                            CurrentPage = obj;
                            this.pnlcanvas.Controls.Clear();
                            this.pnlcanvas.Controls.Add((UserControl)obj);
                            ((UserControl)obj).Show();
                            ((UserControl)obj).Dock = DockStyle.Fill;
                            obj.OnUpgrade();
                            obj.OnSkinLoad();
                            obj.Setup();
                            AppearanceManager.SetWindowTitle(this, attribute.Name + " - Shiftnet");
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                pnlcanvas.Controls.Clear();
                var tlbl = new Label();
                tlbl.Text = "Server error in \"" + url + "\" application.";
                tlbl.Tag = "header1";
                tlbl.AutoSize = true;
                tlbl.Location = new Point(10, 10);
                tlbl.Dock = DockStyle.Top;
                pnlcanvas.Controls.Add(tlbl);
                tlbl.Show();

                var crash = new Label();
                crash.Dock = DockStyle.Fill;
                crash.AutoSize = false;
                crash.Text = ex.ToString();
                pnlcanvas.Controls.Add(crash);
                crash.Show();
                crash.BringToFront();
                ControlManager.SetupControls(pnlcanvas);
                return;
            }
            pnlcanvas.Controls.Clear();
            var lbl = new Label();
            lbl.Text = "Page not found!";
            lbl.Tag = "header1";
            lbl.AutoSize = true;
            lbl.Location = new Point(10, 10);
            pnlcanvas.Controls.Add(lbl);
            lbl.Show();

        }

        public void RefreshSite()
        {
            NavigateToUrl(CurrentUrl);
        }
    }
}

