using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine;
using Plex.Frontend.GUI;
using Plex.Objects;

namespace Plex.Frontend.GraphicsSubsystem
{
    public static class UIManager
    {
        private static List<GUI.Control> topLevels = new List<GUI.Control>();

        public static void ClearTopLevels()
        {
            while(topLevels.Count > 0)
            {
                StopHandling(topLevels[0]);
            }
        }

        private static List<GUI.Control> hudctrls = new List<GUI.Control>();
        public static System.Drawing.Size Viewport { get; set; }
        public static GUI.Control FocusedControl = null;
        private static Plexgate _game = null;


        public static void Crash()
        {
            _game.Crash();
        }

        public static void SetTutorialOverlay(Rectangle mouserect, string text, Action complete)
        {
            _game.TutorialOverlayText = text;
            _game.MouseEventBounds = mouserect;
            _game.TutorialOverlayCompleted = complete;
            _game.IsInTutorial = true;
        }

        public static void Init(Plexgate sentience)
        {
            _game = sentience;
        }

        public static bool Fullscreen
        {
            get
            {
                return _game.graphicsDevice.IsFullScreen;
            }
            set
            {
                var uconf = Objects.UserConfig.Get();
                uconf.Fullscreen = value;
                System.IO.File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(uconf, Newtonsoft.Json.Formatting.Indented));
                _game.graphicsDevice.IsFullScreen = value;
                _game.graphicsDevice.ApplyChanges();
            }
        }

        public static System.Drawing.Size ScreenSize
        {
            get
            {
                try
                {
                    return new System.Drawing.Size(_game.graphicsDevice.PreferredBackBufferWidth, _game.graphicsDevice.PreferredBackBufferHeight);
                }
                catch
                {
                    var conf = UserConfig.Get();
                    return new System.Drawing.Size(conf.ScreenWidth, conf.ScreenHeight);
                }
            }
        }

        public static void BringToFront(GUI.Control ctrl)
        {
            topLevels.Remove(ctrl);
            topLevels.Add(ctrl);
        }

        public static void LayoutUpdate(GameTime gameTime)
        {
            foreach (var toplevel in topLevels.ToArray())
                toplevel.Layout(gameTime);
            foreach (var toplevel in hudctrls.ToArray())
                toplevel.Layout(gameTime);
        }

        public static Dictionary<int, RenderTarget2D> TextureCaches = new Dictionary<int, RenderTarget2D>();
        public static Dictionary<int, RenderTarget2D> HUDCaches = new Dictionary<int, RenderTarget2D>();


        public static void DrawTArgets(SpriteBatch batch)
        {
            DrawTargetsInternal(batch, ref topLevels, ref TextureCaches);
        }


        public static void DrawHUD(SpriteBatch batch)
        {
            DrawTargetsInternal(batch, ref hudctrls, ref HUDCaches);
        }

        private static void DrawTargetsInternal(SpriteBatch batch, ref List<Control> controls, ref Dictionary<int, RenderTarget2D> targets)
        {
            foreach (var ctrl in controls.ToArray())
            {
                if (ctrl.Visible == true)
                {
                    int hc = ctrl.GetHashCode();
                    if (!targets.ContainsKey(hc))
                    {
                        ctrl.Invalidate();
                        continue;
                    }
                    var _target = targets[hc];
                    if (_target.Width != ctrl.Width || _target.Height != ctrl.Height)
                    {
                        ctrl.Invalidate();
                        continue;
                    }
                    if (ExperimentalEffects)
                    {
                        for (int i = 5; i > 0; i--)
                        {
                            batch.Draw(_target, new Rectangle(ctrl.X - i, ctrl.Y - i, ctrl.Width + (i * 2), ctrl.Height + (i * 2)), new Color(Color.Black, 255 / (i * 2)));
                        }
                    }

                    batch.Draw(_target, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), _game.UITint);
                }
            }
        }


        public static void SendToBack(Control ctrl)
       { 
            topLevels.Remove(ctrl);
            topLevels.Insert(0, ctrl);
        }

        public static void DrawControlsToTargetsInternal(GraphicsDevice graphics, SpriteBatch batch, int width, int height, ref List<Control> controls, ref Dictionary<int, RenderTarget2D> targets)
        {
            foreach (var ctrl in controls.ToArray().Where(x=>x.Visible==true))
            {
                RenderTarget2D _target;
                int hc = ctrl.GetHashCode();
                if (!targets.ContainsKey(hc))
                {
                    _target = new RenderTarget2D(
                                    graphics,
                                    Math.Max(1,ctrl.Width),
                                    Math.Max(1,ctrl.Height),
                                    false,
                                    graphics.PresentationParameters.BackBufferFormat,
                                    DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                    targets.Add(hc, _target);
                }
                else
                {
                    _target = targets[hc];
                    if(_target.Width != ctrl.Width || _target.Height != ctrl.Height)
                    {
                        _target = new RenderTarget2D(
                graphics,
                Math.Max(1,ctrl.Width),
                Math.Max(1,ctrl.Height),
                false,
                graphics.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                        targets[hc] = _target;

                    }
                }
                if (ctrl.RequiresPaint)
                {
                    graphics.SetRenderTarget(_target);
                    graphics.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                    batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, GraphicsDevice.DepthStencilState,
                                    RasterizerState.CullNone);
                    graphics.Clear(Color.Transparent);
                    var gfxContext = new GraphicsContext(graphics, batch, 0, 0, _target.Width, _target.Height);
                    ctrl.Paint(gfxContext, _target);

                    batch.End();
                    graphics.SetRenderTarget(_game.GameRenderTarget);
                    TextureCaches[hc] = _target;
                }
            }
        }

        public static event Action SinglePlayerStarted;

        public static void StartSPServer()
        {
            SinglePlayerStarted?.Invoke();
        }

        public static void DrawControlsToTargets(GraphicsDevice device, SpriteBatch batch)
        {
            DrawControlsToTargetsInternal(device, batch, Viewport.Width, Viewport.Height, ref topLevels, ref TextureCaches);
        }

        public static void DrawHUDToTargets(GraphicsDevice device, SpriteBatch batch)
        {
            DrawControlsToTargetsInternal(device, batch, Viewport.Width, Viewport.Height, ref hudctrls, ref HUDCaches);
        }


        public static void AddTopLevel(GUI.Control ctrl)
        {
            if (!topLevels.Contains(ctrl))
                topLevels.Add(ctrl);
            FocusedControl = ctrl;
            ctrl.Invalidate();
        }

        public static void AddHUD(GUI.Control ctrl)
        {
            if (!hudctrls.Contains(ctrl))
                hudctrls.Add(ctrl);
            ctrl.Invalidate();
        }


        public static void InvalidateAll()
        {
            foreach(var ctrl in topLevels)
            {
                ctrl.Invalidate();
            }
            foreach (var ctrl in hudctrls)
            {
                ctrl.Invalidate();
            }
        }

        public static Control[] TopLevels
        {
            get
            {
                return topLevels.ToArray();
            }
        }

        public static void ProcessMouseState(MouseState state, double lastLeftClickMS)
        {
            bool rclick = true;
            bool hidemenus = true;
            foreach (var ctrl in topLevels.ToArray().Where(x=>x != null).Where(x=>x.Visible == true).OrderByDescending(x=>topLevels.IndexOf(x)))
            {
                if (ctrl.ProcessMouseState(state, lastLeftClickMS))
                {
                    if(ctrl is Menu||ctrl is MenuItem)
                    {
                        hidemenus = false;
                    }
                    rclick = false;
                    break;
                }
            }
            if (hidemenus == true)
            {
                if (!(state.LeftButton == ButtonState.Released && state.MiddleButton == ButtonState.Released && state.RightButton == ButtonState.Released))
                {
                    var menus = topLevels.Where(x => x is Menu || x is MenuItem);
                    foreach (var menu in menus.ToArray())
                        (menu as Menu).Hide();
                }
            }
            if (rclick == true)
            {
                if (rmouselast == false && state.RightButton == ButtonState.Pressed)
                {
                    rmouselast = true;
                    ScreenRightclicked?.Invoke(state.X, state.Y);
                    return;
                }
                if (rmouselast == true && state.RightButton == ButtonState.Released)
                {
                    rmouselast = false;
                    return;
                }
            }
        }

        public static event Action<int, int> ScreenRightclicked;

        private static bool rmouselast = false;

        public static void ProcessKeyEvent(KeyEvent e)
        {
            if (e.ControlDown && e.Key == Keys.T)
            {
                TerminalBackend.OpenTerminal();
                return;
            }
            FocusedControl?.ProcessKeyEvent(e);
        }

        private static Texture2D DesktopBackground = null;

        public static Dictionary<string, Texture2D> SkinTextures = new Dictionary<string, Texture2D>();

        public static void ResetSkinTextures(GraphicsDevice graphics)
        {
            bool rgb101 = true;
            if (SaveSystem.CurrentSave != null)
                rgb101 = SaveSystem.CurrentSave.UseRGB101Compatibility;
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
                            byte g = data[i + 1];
                            byte b = data[i + 2];
                            if (rgb101)
                            {
                                if (r == 1 && b == 1 && g == 0)
                                {
                                    data[i + 3] = 0;
                                }
                            }
                            else
                            {
                                byte ar = SaveSystem.CurrentSave.AccentR;
                                byte ag = SaveSystem.CurrentSave.AccentG;
                                byte ab = SaveSystem.CurrentSave.AccentB;
                                byte br = SkinEngine.LoadedSkin.AccentKey.R;
                                byte bg = SkinEngine.LoadedSkin.AccentKey.G;
                                byte bb = SkinEngine.LoadedSkin.AccentKey.B;

                                if (br == r && bg == g && bb == b)
                                {
                                    r = ar;
                                    data[i + 1] = ag;
                                    b = ab;
                                }
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
                if(rgb101)
                {
                    if (color.R == 1 && color.G == 0 && color.B == 1)
                    {
                        color = System.Drawing.Color.FromArgb(0, 0, 0, 0);
                    }
                }
                else
                {
                    byte ar = SaveSystem.CurrentSave.AccentR;
                    byte ag = SaveSystem.CurrentSave.AccentG;
                    byte ab = SaveSystem.CurrentSave.AccentB;
                    byte br = SkinEngine.LoadedSkin.AccentKey.R;
                    byte bg = SkinEngine.LoadedSkin.AccentKey.G;
                    byte bb = SkinEngine.LoadedSkin.AccentKey.B;
                    if (color.R == br && color.G == bg && color.B == bb)
                        color = System.Drawing.Color.FromArgb(ar, ag, ab);
                }

                tex2.SetData<byte>(new[] { color.R, color.G, color.B, color.A });
                SkinTextures.Add(colorfield.Name, tex2);
            }

            var pureWhite = new Texture2D(graphics, 1, 1);
            pureWhite.SetData<byte>(new byte[] { 255, 255, 255, 255 });
            SkinTextures.Add("PureWhite", pureWhite);

        }

        public static void SetUITint(Color color)
        {
            _game.UITint = color;
        }


        public static bool ExperimentalEffects = true;

        public static Queue<Action> CrossThreadOperations = new Queue<Action>();
        public static GraphicsDevice GraphicsDevice;

        public static void DrawBackgroundLayer(GraphicsDevice graphics, SpriteBatch batch, int width, int height)
        {
            try
            {
                batch.Draw(SkinTextures["DesktopColor"], new Rectangle(0, 0, Viewport.Width, Viewport.Height), _game.UITint);

                graphics.Clear(SkinEngine.LoadedSkin.DesktopColor.ToMonoColor());
                if (SkinTextures.ContainsKey("desktopbackground"))
                {
                    batch.Draw(SkinTextures["desktopbackground"], new Rectangle(0, 0, Viewport.Width, Viewport.Height), _game.UITint);
                }
            }
            catch { }
        }

        public static Color ToMonoColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static void StopHandling(GUI.Control ctrl)
        {
            if (topLevels.Contains(ctrl))
                topLevels.Remove(ctrl);

            int hc = ctrl.GetHashCode();
            if (TextureCaches.ContainsKey(hc))
            {
                TextureCaches[hc].Dispose();
                TextureCaches.Remove(hc);
            }
            ctrl.Dispose();
            ctrl = null;
        }

        public static void ConnectToServer(string host, int port)
        {
            _game._mpClient = new UdpClient();
            var he = Dns.GetHostEntry(host);
            var ip = he.AddressList.Last();
            bool isMP = false;
            NetworkClient.Connect(ip, port);
            PingServer(ip, port);
            NetworkClient.Send(Encoding.UTF8.GetBytes("ismp"), Encoding.UTF8.GetBytes("ismp").Length);
            var ep = new IPEndPoint(ip, port);
            var dgram = NetworkClient.Receive(ref ep);
            isMP = (dgram[0] == 1) ? true : false;
            _game.IPAddress = ip;
            _game.Port = port;
            //Start session management for this server...
            SaveSystem.IsSandbox = isMP; //If we're on a multiplayer server then disable the story system.
            if (isMP)
            {
                ServerManager.StartSessionManager(host, port);
            }
            else
            {
                ServerManager.StartSinglePlayer(host, port);
            }
        }

        private static void PingServer(IPAddress ip, int port)
        {
            Exception error = null;
            var heart = Encoding.UTF8.GetBytes("heart");
            NetworkClient.Send(heart, heart.Length);
            var beat = Encoding.UTF8.GetBytes("beat");
            bool done = false;
            var t = new Thread(() =>
            {
                try
                {
                    var ep = new System.Net.IPEndPoint(ip, port);
                    byte[] receive = new byte[4];
                    while (Encoding.UTF8.GetString(receive) != "beat")
                    {
                        receive = NetworkClient.Receive(ref ep);
                    }
                    done = true;
                }
                catch(Exception ex)
                {
                    error = ex;
                    done = true;
                }
            });
            t.Start();
            int ms = 0;
            while(ms < 4000 && done == false)
            {
                ms++;
                Thread.Sleep(1);
            }
            if(done == false)
            {
                t.Abort();
                throw new NetworkTimeoutException(ip,port);
            }
            if (done == true && error != null)
                throw error;
        }

        public static System.Net.Sockets.UdpClient NetworkClient
        {
            get
            {
                return _game._mpClient;
            }
        }

        public static Plexgate Game
        {
            get
            {
                return _game;
            }
        }

        internal static void StopHandlingHUD(GUI.Control ctrl)
        {
            if (hudctrls.Contains(ctrl))
                hudctrls.Remove(ctrl);

            int hc = ctrl.GetHashCode();
            if (HUDCaches.ContainsKey(hc))
            {
                HUDCaches[hc].Dispose();
                HUDCaches.Remove(hc);
            }
            ctrl.Dispose();

            ctrl = null;
        }

    }

    public class KeyEvent
    {

        public KeyEvent(KeyboardEventArgs e)
        {
            ControlDown = false;
            ShiftDown = e.Modifiers.HasFlag(KeyboardModifiers.Shift);
            ControlDown = e.Modifiers.HasFlag(KeyboardModifiers.Control);
            AltDown = e.Modifiers.HasFlag(KeyboardModifiers.Alt);
            Key = e.Key;
            KeyChar = e.Character ?? '\0' ;
        }


        public bool ControlDown { get; private set; }
        public bool AltDown { get; private set; }
        public bool ShiftDown { get; set; }
        public Keys Key { get; private set; }

        public char KeyChar { get; private set; }
    }
}
