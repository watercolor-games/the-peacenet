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
        public override void Close(IShiftOSWindow win)
        {
            (win as UserControl).Close();
        }

        public override void InvokeAction(Action act)
        {
            Desktop.InvokeOnWorkerThread(act);
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

        public override void SetupDialog(IShiftOSWindow form)
        {
            if (!Shiftorium.UpgradeAttributesUnlocked(form.GetType()))
            {
                Console.WriteLine("{APP_NOT_FOUND}");
                return;
            }

            var wb = new WindowBorder(form as UserControl);
            wb.IsDialog = true;


            wb.Show();
        }

        public override void SetupWindow(IShiftOSWindow form)
        {
            if (!AppearanceManager.CanOpenWindow(form))
            {
                Infobox.Show("{MULTIPLAYER_ONLY}", "{MULTIPLAYER_ONLY_EXP}");
                return;
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
                        List<WindowBorder> formstoclose = new List<WindowBorder>();

                        foreach (WindowBorder frm in AppearanceManager.OpenForms)
                        {
                            formstoclose.Add(frm);

                        }

                        while (formstoclose.Count > maxWindows - 1)
                        {
                            formstoclose[0].Close();
                            formstoclose.RemoveAt(0);
                        }
                    }
                }
            }

            var wb = new WindowBorder(form as UserControl);

            ControlManager.SetupWindows();
        }
    }
}
