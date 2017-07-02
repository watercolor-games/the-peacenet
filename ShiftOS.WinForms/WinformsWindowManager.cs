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
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    internal class WinformsWindowManager : WindowManager
    {
        public int DesktopHeight
        {
            get
            {
                return Desktop.Size.Height - ((Shiftorium.UpgradeInstalled("desktop") == true) ? SkinEngine.LoadedSkin.DesktopPanelHeight : 0);
            }
        }

        public int TopLocation
        {
            get
            {
                if (!Shiftorium.UpgradeInstalled("desktop"))
                    return 0;
                return ((SkinEngine.LoadedSkin.DesktopPanelPosition == 0) ? SkinEngine.LoadedSkin.DesktopPanelHeight : 0);
            }
        }

        public override void Close(IShiftOSWindow win)
        {
            (win as UserControl).Close();
        }

        public override void InvokeAction(Action act)
        {
            Desktop.InvokeOnWorkerThread(act);
        }

        public WinformsWindowManager()
        {
            Shiftorium.Installed += () =>
            {
                SetupWindows();
            };
        }

        public override void Maximize(IWindowBorder form)
        {
            try
            {
                var deskSize = new Size(0, 0);
                deskSize.Width = Screen.PrimaryScreen.Bounds.Width;
                deskSize.Height = Screen.PrimaryScreen.Bounds.Height;

                deskSize.Height -= SkinEngine.LoadedSkin.DesktopPanelHeight;

                var deskStart = new Point((SkinEngine.LoadedSkin.DesktopPanelPosition == 0) ? SkinEngine.LoadedSkin.DesktopPanelHeight : 0, 0);

                (form as WindowBorder).Location = deskStart;
                (form as WindowBorder).Size = deskSize;


            }
            catch
            {
            }
        }

        public override void Minimize(IWindowBorder border)
        {
            try
            {

            }
            catch { }
        }

        public override void SetTitle(IShiftOSWindow win, string title)
        {
            var wb = (win as UserControl).ParentForm as WindowBorder;
            wb.SetTitle(title);
        }

        public override void SetupDialog(IShiftOSWindow form)
        {
            if (!Shiftorium.UpgradeAttributesUnlocked(form.GetType()))
            {
                Console.WriteLine("{APP_NOT_FOUND}");
                return;
            }

            var wb = new WindowBorder(form as UserControl);
            wb.IsDialog = true;

            Desktop.ShowWindow(wb);
            form.OnLoad();
            form.OnSkinLoad();
            form.OnUpgrade();

        }

        public override void SetupWindow(IShiftOSWindow form)
        {
            foreach(var attr in form.GetType().GetCustomAttributes(true))
            {
                if(attr is MultiplayerOnlyAttribute)
                {
                    if(KernelWatchdog.MudConnected == false)
                    {
                        Infobox.PromptYesNo("Disconnected from MUD", "This application requires a connection to the MUD. Would you like to reconnect?", new Action<bool>((answer) =>
                        {
                            if(answer == true)
                            {
                                KernelWatchdog.MudConnected = true;
                                SetupWindow(form);
                            }
                        }));
                        return;
                    }
                }
            }

            if (!Shiftorium.UpgradeAttributesUnlocked(form.GetType()))
            {
                Console.WriteLine("{APP_NOT_FOUND}");
                return;
            }

            if (SaveSystem.CurrentSave != null)
            {
                if (!form.GetType().Name.Contains(typeof(Applications.Dialog).Name))
                {
                    int maxWindows = 0;

                    //Window manager will step in here.
                    if (Shiftorium.UpgradeInstalled("wm_unlimited_windows"))
                    {
                        maxWindows = 0;
                    }
                    else if (Shiftorium.UpgradeInstalled("wm_4_windows"))
                    {
                        maxWindows = 4;
                    }
                    else if (Shiftorium.UpgradeInstalled("window_manager"))
                    {
                        maxWindows = 2;
                    }
                    else
                    {
                        maxWindows = 1;
                    }


                    if (maxWindows > 0)
                    {
                        var windows = new List<WindowBorder>();
                        foreach(var WB in AppearanceManager.OpenForms)
                        {
                            if (WB is WindowBorder)
                                windows.Add(WB as WindowBorder);
                        }

                        List<WindowBorder> formstoclose = new List<WindowBorder>(windows.Where(x => x.IsDialog == false).ToArray());

                        while (formstoclose.Count > maxWindows - 1)
                        {
                            this.Close(formstoclose[0].ParentWindow);
                            AppearanceManager.OpenForms.Remove(formstoclose[0]);
                            formstoclose.RemoveAt(0);
                            
                        }
                    }
                }
            }

            var wb = new WindowBorder(form as UserControl);

            FormClosedEventHandler onClose = (o,a)=> { };
            onClose = (o, a) =>
            {
                SetupWindows();
                wb.FormClosed -= onClose;
            };
            wb.FormClosed += onClose;
            Desktop.ShowWindow(wb);
            SetupWindows();
            form.OnLoad();
            form.OnSkinLoad();
            form.OnUpgrade();

        }

        public void SetupWindows()
        {
            var windows = new List<WindowBorder>();
            foreach(var win in AppearanceManager.OpenForms)
            {
                if (win is WindowBorder)
                    if ((win as WindowBorder).IsDialog == false)
                        windows.Add(win as WindowBorder);
            }

            if (Shiftorium.UpgradeInstalled("wm_free_placement"))
                return;

            else if (windows.Count == 4)
            {
                var w1 = windows[0];
                var w2 = windows[1];
                var w3 = windows[2];
                var w4 = windows[3];
                w1.Location = new Point(0, TopLocation);
                w1.Width = Desktop.Size.Width / 2;
                w1.Height = DesktopHeight / 2;
                w2.Left = w1.Width;
                w2.Width = w1.Width;
                w2.Height = w1.Height;
                w2.Top = w1.Top;
                w3.Top = w2.Height;
                w3.Height = w1.Height;
                w3.Left = 0;
                w3.Width = w1.Width;
                w4.Width = w3.Width;
                w4.Top = w3.Top;
                w4.Left = w3.Width;
                w4.Height = w3.Height;
            }
            else if(windows.Count == 3)
            {
                var w1 = windows[0];
                var w2 = windows[1];
                var w3 = windows[2];
                w1.Location = new Point(0, TopLocation);
                w1.Width = Desktop.Size.Width / 2;
                w1.Height = DesktopHeight / 2;
                w2.Left = w1.Width;
                w2.Width = w1.Width;
                w2.Height = w1.Height;
                w2.Top = w1.Top;
                w3.Top = w2.Height;
                w3.Height = w1.Height;
                w3.Left = 0;
                w3.Width = w1.Width + w2.Width;
            }
            else if (windows.Count == 2)
            {
                var w1 = windows[0];
                var w2 = windows[1];

                w1.Location = new Point(0, TopLocation);
                w1.Width = Desktop.Size.Width / 2;
                w1.Height = DesktopHeight;
                w2.Left = w1.Width;
                w2.Width = w1.Width;
                w2.Height = w1.Height;
                w2.Top = w1.Top;

            }
            else if(windows.Count == 1)
            {
                var win = windows.FirstOrDefault();
                if(win != null)
                {
                    win.Size = new Size(Desktop.Size.Width, DesktopHeight);
                    win.Location = new Point(0, this.TopLocation);
                }
            }

        }
    }
}
