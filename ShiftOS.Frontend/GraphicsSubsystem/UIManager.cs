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
        public static System.Drawing.Size Viewport { get; set; }
        public static GUI.Control FocusedControl = null;

        public static void BringToFront(GUI.Control ctrl)
        {
            topLevels.Remove(ctrl);
            topLevels.Add(ctrl);
        }

        public static void LayoutUpdate(GameTime gameTime)
        {
            foreach (var toplevel in topLevels.ToArray())
                toplevel.Layout(gameTime);
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

        public static Dictionary<int, RenderTarget2D> TextureCaches = new Dictionary<int, RenderTarget2D>();

        public static void DrawTArgets(SpriteBatch batch)
        {
            foreach(var ctrl in topLevels.ToArray())
            {
                if (ctrl.Visible == true)
                {
                    int hc = ctrl.GetHashCode();
                    if (!TextureCaches.ContainsKey(hc))
                    {
                        ctrl.Invalidate();
                        continue;
                    }
                    var _target = TextureCaches[hc];
                    if (ExperimentalEffects)
                    {
                        for (int i = 5; i > 0; i--)
                        {
                            batch.Draw(_target, new Rectangle(ctrl.X - i, ctrl.Y - i, ctrl.Width+(i*2), ctrl.Height+(i*2)), new Color(Color.Black, 255 / (i * 2)));
                        }
                    }

                    batch.Draw(_target, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), Color.White);
                }
            }
        }

        public static void SendToBack(Control ctrl)
       { 
            topLevels.Remove(ctrl);
            topLevels.Insert(0, ctrl);
        }

        public static void DrawControlsToTargets(GraphicsDevice graphics, SpriteBatch batch, int width, int height)
        {
            foreach (var ctrl in topLevels.ToArray().Where(x=>x.Visible==true))
            {
                RenderTarget2D _target;
                int hc = ctrl.GetHashCode();
                if (!TextureCaches.ContainsKey(hc))
                {
                    _target = new RenderTarget2D(
                                    graphics,
                                    ctrl.Width,
                                    ctrl.Height,
                                    false,
                                    graphics.PresentationParameters.BackBufferFormat,
                                    DepthFormat.Depth24);
                    TextureCaches.Add(hc, _target);
                }
                else
                {
                    _target = TextureCaches[hc];
                    if(_target.Width != ctrl.Width || _target.Height != ctrl.Height)
                    {
                        _target = new RenderTarget2D(
                graphics,
                ctrl.Width,
                ctrl.Height,
                false,
                graphics.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
                        TextureCaches[hc] = _target;

                    }
                }
                if (ctrl.RequiresPaint)
                {
                    graphics.SetRenderTarget(_target);
                    graphics.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                    batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, DepthStencilState.Default,
                                    RasterizerState.CullNone);
                    graphics.Clear(Color.Transparent);
                    var gfxContext = new GraphicsContext(graphics, batch, 0, 0, _target.Width, _target.Height);
                    ctrl.Paint(gfxContext);
                    
                    graphics.SetRenderTarget(null);
                    TextureCaches[hc] = _target;
                    batch.End();
                }
            }
        }

        public static void AddTopLevel(GUI.Control ctrl)
        {
            if (!topLevels.Contains(ctrl))
                topLevels.Add(ctrl);

        }

        public static void InvalidateAll()
        {
            foreach(var ctrl in topLevels)
            {
                ctrl.Invalidate();
            }
        }

        public static void ProcessMouseState(MouseState state, double lastLeftClickMS)
        {
            foreach(var ctrl in topLevels.ToArray())
            {
                ctrl.ProcessMouseState(state, lastLeftClickMS);
                
            }
        }

        public static void ProcessKeyEvent(KeyEvent e)
        {
            if (e.ControlDown && e.Key == Keys.T)
            {
                AppearanceManager.SetupWindow(new Apps.Terminal());
                return;
            }
            FocusedControl?.ProcessKeyEvent(e);
        }

        private static Texture2D DesktopBackground = null;

        public static Dictionary<string, Texture2D> SkinTextures = new Dictionary<string, Texture2D>();

        public static void ResetSkinTextures(GraphicsDevice graphics)
        {
            SkinTextures.Clear();
            foreach(var byteArray in SkinEngine.LoadedSkin.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x=>x.FieldType == typeof(byte[])))
            {
                var imgAttrib = byteArray.GetCustomAttributes(false).FirstOrDefault(x => x is ImageAttribute) as ImageAttribute;
                if(imgAttrib != null)
                {
                    var img = SkinEngine.GetImage(imgAttrib.Name);
                    if(img != null)
                    {
                        var bmp = (System.Drawing.Bitmap)img;
                        var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        var data = new byte[Math.Abs(lck.Stride) * lck.Height];
                        Marshal.Copy(lck.Scan0, data, 0, data.Length);
                        bmp.UnlockBits(lck);
                        var tex2 = new Texture2D(graphics, bmp.Width, bmp.Height);
                        for(int i = 0; i < data.Length; i += 4)
                        {
                            byte r = data[i];
                            byte b = data[i + 2];
                            if (r == 1 && b == 1 && data[i + 1] == 1)
                            {
                                data[i + 3] = 0;
                            }
                            data[i] = b;
                            data[i + 2] = r;
                        }
                        tex2.SetData<byte>(data);
                        SkinTextures.Add(imgAttrib.Name, tex2);
                    }
                }
            }

            foreach(var colorfield in SkinEngine.LoadedSkin.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x=>x.FieldType == typeof(System.Drawing.Color)))
            {
                var color = (System.Drawing.Color)colorfield.GetValue(SkinEngine.LoadedSkin);
                var tex2 = new Texture2D(graphics, 1, 1);
                tex2.SetData<byte>(new[] { color.R, color.G, color.B, color.A });
                SkinTextures.Add(colorfield.Name, tex2);
            }

            

        }


        public static bool ExperimentalEffects = true;

        public static Queue<Action> CrossThreadOperations = new Queue<Action>();
        internal static GraphicsDevice GraphicsDevice;

        public static void DrawBackgroundLayer(GraphicsDevice graphics, SpriteBatch batch, int width, int height)
        {
            if (SkinEngine.LoadedSkin == null)
                SkinEngine.Init();
            graphics.Clear(SkinEngine.LoadedSkin.DesktopColor.ToMonoColor());
            if (SkinTextures.ContainsKey("desktopbackground"))
            {
                batch.Draw(SkinTextures["desktopbackground"], new Rectangle(0, 0, Viewport.Width, Viewport.Height), Color.White);
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

            int hc = ctrl.GetHashCode();
            if (TextureCaches.ContainsKey(hc))
            {
                TextureCaches[hc].Dispose();
                TextureCaches.Remove(hc);
            }

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
