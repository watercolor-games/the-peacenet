using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShiftOS.Engine;
using ShiftOS.Frontend.Desktop;
using ShiftOS.Frontend.GUI;

namespace ShiftOS.Frontend.GraphicsSubsystem
{
    public static class UIManager
    {
        private static List<GUI.Control> topLevels = new List<GUI.Control>();

        public static GUI.Control FocusedControl = null;

        public static void LayoutUpdate()
        {
            foreach (var toplevel in topLevels.ToArray())
                toplevel.Layout();
        }

        public static void Animate(object owner, System.Reflection.PropertyInfo prop, double from, double to, int timeMs)
        {
            var t = new System.Threading.Thread(() =>
            {
                for(int i = 0; i < timeMs; i++)
                {
                    double value = ProgressBar.linear(i, 0, timeMs, from, to);
                    prop.SetValue(owner, value);
                    System.Threading.Thread.Sleep(1);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        public static Dictionary<int, Texture2D> TextureCaches = new Dictionary<int, Texture2D>();

        public static void DrawControls(GraphicsDevice graphics, SpriteBatch batch)
        {
            foreach (var ctrl in topLevels.ToArray())
            {
                int hc = ctrl.GetHashCode();
                if (ctrl.RequiresPaint)
                {
                    var bmp = new System.Drawing.Bitmap(ctrl.Width, ctrl.Height);
                    ctrl.Paint(System.Drawing.Graphics.FromImage(bmp));
                    if (TextureCaches.ContainsKey(hc))
                    {
                        TextureCaches[hc].Dispose();
                        TextureCaches.Remove(hc);
                    }
                    TextureCaches.Add(hc, new Texture2D(graphics, ctrl.Width, ctrl.Height));
                    TextureCaches[hc].SetData<byte>(ctrl.PaintCache);
                }
                batch.Draw(TextureCaches[hc], new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), Color.White);
            }
        }

        public static void AddTopLevel(GUI.Control ctrl)
        {
            if (!topLevels.Contains(ctrl))
                topLevels.Add(ctrl);
            ctrl.Layout();
        }

        public static void ProcessMouseState(MouseState state)
        {
            foreach(var ctrl in topLevels.ToArray())
            {
                ctrl.ProcessMouseState(state);
            }
        }

        public static void ProcessKeyEvent(KeyEvent e)
        {
            FocusedControl?.ProcessKeyEvent(e);
        }

        public static void DrawBackgroundLayer(GraphicsDevice graphics, SpriteBatch batch, int width, int height)
        {
            if (SkinEngine.LoadedSkin == null)
                SkinEngine.Init();
            graphics.Clear(SkinEngine.LoadedSkin.DesktopColor.ToMonoColor());
            var desktopbg = SkinEngine.GetImage("desktopbackground");
            if(desktopbg != null)
            {
                var tex2 = new Texture2D(graphics, desktopbg.Width, desktopbg.Height);
                tex2.SetData<byte>(SkinEngine.LoadedSkin.DesktopBackgroundImage);
                batch.Draw(tex2, new Rectangle(0, 0, width, height), Color.White);
            }
        }

        public static Color ToMonoColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        internal static void StopHandling(GUI.Control ctrl)
        {
            if (topLevels.Contains(ctrl))
                topLevels.Remove(ctrl);
            ctrl = null;
        }
    }

    public class KeyEvent
    {
        public KeyEvent(bool control, bool alt, bool shift, Keys key)
        {
            ControlDown = control;
            AltDown = alt;
            ShiftDown = shift;
            Key = key;
            KeyChar = key.ToCharacter(shift);
        }

        public bool ControlDown { get; private set; }
        public bool AltDown { get; private set; }
        public bool ShiftDown { get; set; }
        public Keys Key { get; private set; }

        public char KeyChar { get; private set; }
    }

    public static class KeysExtensions
    {
        public static char ToCharacter(this Keys key, bool shift)
        {
            char c = '\0';
            switch (key)
            {
                case Keys.Space:
                    c = ' ';
                    break;
                case Keys.A:
                    c = 'a';
                    break;
                case Keys.B:
                    c = 'b';
                    break;
                case Keys.C:
                    c = 'c';
                    break;
                case Keys.D:
                    c = 'd';
                    break;
                case Keys.E:
                    c = 'e';
                    break;
                case Keys.F:
                    c = 'f';
                    break;
                case Keys.G:
                    c = 'g';
                    break;
                case Keys.H:
                    c = 'h';
                    break;
                case Keys.I:
                    c = 'i';
                    break;
                case Keys.J:
                    c = 'j';
                    break;
                case Keys.K:
                    c = 'k';
                    break;
                case Keys.L:
                    c = 'l';
                    break;
                case Keys.M:
                    c = 'm';
                    break;
                case Keys.N:
                    c = 'n';
                    break;
                case Keys.O:
                    c = 'o';
                    break;
                case Keys.P:
                    c = 'p';
                    break;
                case Keys.Q:
                    c = 'q';
                    break;
                case Keys.R:
                    c = 'r';
                    break;
                case Keys.S:
                    c = 's';
                    break;
                case Keys.T:
                    c = 't';
                    break;
                case Keys.U:
                    c = 'u';
                    break;
                case Keys.V:
                    c = 'v';
                    break;
                case Keys.W:
                    c = 'w';
                    break;
                case Keys.X:
                    c = 'x';
                    break;
                case Keys.Y:
                    c = 'y';
                    break;
                case Keys.Z:
                    c = 'z';
                    break;
                case Keys.D0:
                    if (shift)
                        c = ')';
                    else
                        c = '0';
                    break;
                case Keys.D1:
                    if (shift)
                        c = '!';
                    else
                        c = '1';
                    break;
                case Keys.D2:
                    if (shift)
                        c = '@';
                    else
                        c = '2';
                    break;
                case Keys.D3:
                    if (shift)
                        c = '#';
                    else
                        c = '3';
                    break;
                case Keys.D4:
                    if (shift)
                        c = '$';
                    else
                        c = '4';
                    break;
                case Keys.D5:
                    if (shift)
                        c = '%';
                    else
                        c = '5';
                    break;
                case Keys.D6:
                    if (shift)
                        c = '^';
                    else
                        c = '6';
                    break;
                case Keys.D7:
                    if (shift)
                        c = '&';
                    else
                        c = '7';
                    break;
                case Keys.D8:
                    if (shift)
                        c = '*';
                    else
                        c = '8';
                    break;
                case Keys.D9:
                    if (shift)
                        c = '(';
                    else
                        c = '9';
                    break;
                case Keys.OemBackslash:
                    if (shift)
                        c = '|';
                    else
                        c = '\\';
                    break;
                case Keys.OemCloseBrackets:
                    if (shift)
                        c = '}';
                    else
                        c = ']';
                    break;
                case Keys.OemComma:
                    if (shift)
                        c = '<';
                    else
                        c = ',';
                    break;
                case Keys.OemPeriod:
                    if (shift)
                        c = '>';
                    else
                        c = '.';
                    break;
                case Keys.OemQuestion:
                    if (shift)
                        c = '?';
                    else
                        c = '/';
                    break;
                case Keys.OemSemicolon:
                    if (shift)
                        c = ':';
                    else
                        c = ';';
                    break;
                case Keys.OemQuotes:
                    if (shift)
                        c = '"';
                    else
                        c = '\'';
                    break;
                case Keys.OemTilde:
                    if (shift)
                        c = '~';
                    else
                        c = '`';
                    break;
                case Keys.OemMinus:
                    if (shift)
                        c = '_';
                    else
                        c = '-';
                    break;
                case Keys.OemPlus:
                    if (shift)
                        c = '+';
                    else
                        c = '=';
                    break;
            }
            if (char.IsLetter(c))
                if (shift)
                    c = char.ToUpper(c);
            return c;
        }
    }
}
