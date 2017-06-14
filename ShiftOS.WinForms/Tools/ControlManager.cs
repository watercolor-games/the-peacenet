
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using static ShiftOS.Engine.AppearanceManager;


namespace ShiftOS.WinForms.Tools
{
    public static class ControlManager
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

        public static void Close(this UserControl ctrl)
        {
            for (int i = 0; i < AppearanceManager.OpenForms.Count; i++)
            {
                if (OpenForms[i].ParentWindow == ctrl)
                {
                    (OpenForms[i] as Form).Close();
                    return;
                }
            }
        }


        internal static Color ConvertColor(ConsoleColor cCol)
        {
            switch (cCol)
            {
                case ConsoleColor.Black:
                    return Color.Black;
                case ConsoleColor.Gray:
                    return Color.Gray;
                case ConsoleColor.DarkGray:
                    return Color.DarkGray;
                case ConsoleColor.Blue:
                    return Color.Blue;
                case ConsoleColor.Cyan:
                    return Color.Cyan;
                case ConsoleColor.DarkBlue:
                    return Color.DarkBlue;
                case ConsoleColor.DarkCyan:
                    return Color.DarkCyan;
                case ConsoleColor.DarkGreen:
                    return Color.DarkGreen;
                case ConsoleColor.DarkMagenta:
                    return Color.DarkMagenta;
                case ConsoleColor.DarkRed:
                    return Color.DarkRed;
                case ConsoleColor.DarkYellow:
                    return Color.YellowGreen;
                case ConsoleColor.Yellow:
                    return Color.Yellow;
                case ConsoleColor.Green:
                    return Color.Green;
                case ConsoleColor.Magenta:
                    return Color.Magenta;
                case ConsoleColor.Red:
                    return Color.Red;
                case ConsoleColor.White:
                    return Color.White;
                default:
                    return Color.Black;
            }

        }

        public static void SetCursor(Control ctrl)
        {
#if STUPID
            if (!(ctrl is WebBrowser))
            {
                var mouse = SkinEngine.GetImage("mouse");
                if (mouse == null)
                    mouse = Properties.Resources.DefaultMouse;

                var mBmp = new Bitmap(mouse);
                mBmp.MakeTransparent(Color.FromArgb(1, 0, 1));
                var gfx = Graphics.FromImage(mBmp);
                var handle = mBmp.GetHicon();

                var cursor = new Cursor(handle);
                ctrl.Cursor = cursor;
            }
#endif
        }

        /// <summary>
        /// Centers the control along its parent.
        /// </summary>
        /// <param name="ctrl">The control to center (this is an extension method - you can call it on a control as though it was a method in that control)</param>
        public static void CenterParent(this Control ctrl)
        {
            try
            {
                ctrl.Location = new Point(
                        (ctrl.Parent.Width - ctrl.Width) / 2,
                        (ctrl.Parent.Height - ctrl.Height) / 2
                    );
            }
            catch { }
        }

        public static void SetupControl(Control ctrl)
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                if (!(ctrl is MenuStrip) && !(ctrl is ToolStrip) && !(ctrl is StatusStrip) && !(ctrl is ContextMenuStrip))
                {
                    string tag = "";

                    try
                    {
                        if (ctrl.Tag != null)
                            tag = ctrl.Tag.ToString();
                    }
                    catch { }

                    if (!tag.Contains("ignoreal"))
                    {
                        ctrl.Click += (o, a) =>
                        {
                            Desktop.HideAppLauncher();
                        };

                    }

                    if (!tag.Contains("keepbg"))
                    {
                        if (ctrl.BackColor != Control.DefaultBackColor)
                        {
                            ctrl.BackColor = SkinEngine.LoadedSkin.ControlColor;
                        }
                    }

                    if (!tag.Contains("keepfont"))
                    {
                        ctrl.ForeColor = SkinEngine.LoadedSkin.ControlTextColor;
                        ctrl.Font = SkinEngine.LoadedSkin.MainFont;
                        if (tag.Contains("header1"))
                        {
                            Desktop.InvokeOnWorkerThread(() =>
                            {
                                ctrl.Font = SkinEngine.LoadedSkin.HeaderFont;
                            });
                        }

                        if (tag.Contains("header2"))
                        {
                            ctrl.Font = SkinEngine.LoadedSkin.Header2Font;
                        }

                        if (tag.Contains("header3"))
                        {

                            ctrl.Font = SkinEngine.LoadedSkin.Header3Font;
                        }
                    }
                    try
                    {
#if !SLOW_LOCALIZATION
                        if (!string.IsNullOrWhiteSpace(ctrl.Text))
                            {
                                string ctrlText = Localization.Parse(ctrl.Text);
                                ctrl.Text = ctrlText;
                            }
#endif
                    }
                    catch
                    {

                    }

                    if (ctrl is Button)
                    {
                        if (!tag.ToLower().Contains("nobuttonskin"))
                        {
                            Button b = ctrl as Button;
                            if (!tag.Contains("keepbg"))
                            {
                                b.BackColor = SkinEngine.LoadedSkin.ButtonBackgroundColor;
                                b.BackgroundImage = SkinEngine.GetImage("buttonidle");
                                b.BackgroundImageLayout = SkinEngine.GetImageLayout("buttonidle");
                            }
                            b.FlatAppearance.BorderSize = SkinEngine.LoadedSkin.ButtonBorderWidth;
                            if (!tag.Contains("keepfg"))
                            {
                                b.FlatAppearance.BorderColor = SkinEngine.LoadedSkin.ButtonForegroundColor;
                                b.ForeColor = SkinEngine.LoadedSkin.ButtonForegroundColor;
                            }
                            if (!tag.Contains("keepfont"))
                                b.Font = SkinEngine.LoadedSkin.ButtonTextFont;

                            Color orig_bg = b.BackColor;

                            b.MouseEnter += (o, a) =>
                            {
                                b.BackColor = SkinEngine.LoadedSkin.ButtonHoverColor;
                                b.BackgroundImage = SkinEngine.GetImage("buttonhover");
                                b.BackgroundImageLayout = SkinEngine.GetImageLayout("buttonhover");
                            };
                            b.MouseLeave += (o, a) =>
                            {
                                b.BackColor = orig_bg;
                                b.BackgroundImage = SkinEngine.GetImage("buttonidle");
                                b.BackgroundImageLayout = SkinEngine.GetImageLayout("buttonidle");
                            };
                            b.MouseUp += (o, a) =>
                            {
                                b.BackColor = orig_bg;
                                b.BackgroundImage = SkinEngine.GetImage("buttonidle");
                                b.BackgroundImageLayout = SkinEngine.GetImageLayout("buttonidle");
                            };

                            b.MouseDown += (o, a) =>
                            {
                                b.BackColor = SkinEngine.LoadedSkin.ButtonPressedColor;
                                b.BackgroundImage = SkinEngine.GetImage("buttonpressed");
                                b.BackgroundImageLayout = SkinEngine.GetImageLayout("buttonpressed");

                            };
                        }
                    }
                }

                if (ctrl is TextBox)
                {
                    (ctrl as TextBox).BorderStyle = BorderStyle.FixedSingle;
                }

                ctrl.KeyDown += (o, a) =>
                {
                    if (a.Control && a.KeyCode == Keys.T)
                    {
                        a.SuppressKeyPress = true;


                        Engine.AppearanceManager.SetupWindow(new Applications.Terminal());
                    }

                    ShiftOS.Engine.Scripting.LuaInterpreter.RaiseEvent("on_key_down", a);
                    //a.Handled = true;
                };
                if (ctrl is Button)
                {
                    (ctrl as Button).FlatStyle = FlatStyle.Flat;
                }
                else if (ctrl is WindowBorder)
                {
                    (ctrl as WindowBorder).Setup();
                }

                
                MakeDoubleBuffered(ctrl);
                ControlSetup?.Invoke(ctrl);
            });
        }



        public static event Action<Control> ControlSetup;

        public static void MakeDoubleBuffered(Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);

        }

        public static void SetupControls(Control frm, bool runInThread = true)
        {
            var ctrls = frm.Controls.ToList();
            for (int i = 0; i < ctrls.Count(); i++)
            {
                SetupControls(ctrls[i]);
            }
            SetupControl(frm);
        }

    }
}
