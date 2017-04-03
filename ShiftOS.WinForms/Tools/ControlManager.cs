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
            ctrl.Location = new Point(
                    (ctrl.Parent.Width - ctrl.Width) / 2,
                    (ctrl.Parent.Height - ctrl.Height) / 2
                );
        }

        public static void SetupControl(Control ctrl)
        {
            SuspendDrawing(ctrl);
            ctrl.SuspendLayout();
            SetCursor(ctrl);
            if (!(ctrl is MenuStrip) && !(ctrl is ToolStrip) && !(ctrl is StatusStrip) && !(ctrl is ContextMenuStrip))
            {
                string tag = "";

                try
                {
                    tag = ctrl.Tag.ToString();
                }
                catch { }

                if (!tag.Contains("keepbg"))
                {
                    if (ctrl.BackColor != Control.DefaultBackColor)
                    {
                        ctrl.BackColor = SkinEngine.LoadedSkin.ControlColor;
                    }
                }

                ctrl.ForeColor = SkinEngine.LoadedSkin.ControlTextColor;

                ctrl.Font = SkinEngine.LoadedSkin.MainFont;

                if (tag.Contains("header1"))
                {
                    ctrl.Font = SkinEngine.LoadedSkin.HeaderFont;
                }

                if (tag.Contains("header2"))
                {
                    ctrl.Font = SkinEngine.LoadedSkin.Header2Font;
                }

                if (tag.Contains("header3"))
                {
                    ctrl.Font = SkinEngine.LoadedSkin.Header3Font;
                }

                try
                {
                    ctrl.Text = Localization.Parse(ctrl.Text);
                }
                catch
                {

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
            }

            MakeDoubleBuffered(ctrl);
            ctrl.ResumeLayout();
            ResumeDrawing(ctrl);
        }

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

        public static void SetupControls(Control frm)
        {
            SetupControl(frm);

            for (int i = 0; i < frm.Controls.Count; i++)
            {
                SetupControls(frm.Controls[i]);
            }
        }

    }
}
