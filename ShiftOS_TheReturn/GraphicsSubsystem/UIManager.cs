using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine;
using Plex.Engine.TextRenderers;
using Plex.Engine.GUI;
using Plex.Objects;
using Plex.Engine.Config;

namespace Plex.Engine.GraphicsSubsystem
{
    public static class UIManager
    {
        private static List<RenderInfo> topLevels = new List<RenderInfo>();

        public static void ClearTopLevels()
        {
            while(topLevels.Count > 0)
            {
                var tl = topLevels[0];
                tl.Control?.Dispose();
                tl.Target?.Dispose();
                topLevels.RemoveAt(0);
            }
        }

        private static List<RenderInfo> hudctrls = new List<RenderInfo>();
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
                return ConfigurationManager.GetFullscreen();
            }
            set
            {
                ConfigurationManager.SetFullscreen(value);
                ConfigurationManager.ApplyConfig();
            }
        }

        public static void BringToFront(GUI.Control ctrl)
        {
            var tl = topLevels.FirstOrDefault(x => x.Control == ctrl);
            if (tl == null)
                return;
            topLevels.Remove(tl);
            topLevels.Add(tl);
        }

        public static void LayoutUpdate(GameTime gameTime)
        {
            foreach (var toplevel in topLevels.ToArray())
                toplevel.Control.Layout(gameTime);
            foreach (var toplevel in hudctrls.ToArray())
                toplevel.Control.Layout(gameTime);
        }

        public static void DrawTArgets(SpriteBatch batch)
        {
            DrawTargetsInternal(batch, ref topLevels);
        }


        public static void DrawHUD(SpriteBatch batch)
        {
            DrawTargetsInternal(batch, ref hudctrls);
        }

        private static void DrawTargetsInternal(SpriteBatch batch, ref List<RenderInfo> controls)
        {
            foreach (var ctrl in controls.ToArray())
            {
                if (ctrl.Control.Visible == true)
                {
                    var _target = ctrl.Target;
                    if (_target == null)
                    {
                        ctrl.Control.Invalidate();
                        continue;
                    }
                    if (_target.Width != ctrl.Control.Width || _target.Height != ctrl.Control.Height)
                    {
                        ctrl.Control.Invalidate();
                        continue;
                    }
                    batch.Draw(_target, new Rectangle(ctrl.Control.X, ctrl.Control.Y, ctrl.Control.Width, ctrl.Control.Height), _game.UITint);
                }
            }
        }


        public static void SendToBack(Control ctrl)
       {
            var tl = topLevels.FirstOrDefault(x => x.Control == ctrl);
            if (tl == null)
                return;
            topLevels.Remove(tl);
            topLevels.Insert(0, tl);
        }

        public static void DrawControlsToTargetsInternal(GraphicsDevice graphics, SpriteBatch batch, int width, int height, ref List<RenderInfo> controls)
        {
            foreach (var ctrl in controls.ToArray().Where(x => x.Control.Visible == true))
            {
                RenderTarget2D _target = ctrl.Target;
                if (_target == null)
                {
                    _target = new RenderTarget2D(
                                    graphics,
                                    Math.Max(1, ctrl.Control.Width),
                                    Math.Max(1, ctrl.Control.Height),
                                    false,
                                    graphics.PresentationParameters.BackBufferFormat,
                                    DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                    ctrl.Target = _target;
                    ctrl.Control.Invalidate();
                }
                else
                {
                    if (_target.Width != ctrl.Control.Width || _target.Height != ctrl.Control.Height)
                    {
                        _target.Dispose();
                        _target = new RenderTarget2D(
                graphics,
                Math.Max(1, ctrl.Control.Width),
                Math.Max(1, ctrl.Control.Height),
                false,
                graphics.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                        ctrl.Target = _target;
                        ctrl.Control.Invalidate();
                    }
                }
                if (ctrl.Control.RequiresPaint)
                {
                    try
                    {
                        graphics.SetRenderTarget(_target);
                        batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                        SamplerState.LinearWrap, GraphicsDevice.DepthStencilState,
                                        ctrl.Control.RasterizerState);
                        graphics.Clear(Color.Transparent);
                        var gfxContext = new GraphicsContext(graphics, batch, 0, 0, ctrl.Control.Width, ctrl.Control.Height);
                        ctrl.Control.Paint(gfxContext, _target);


                        batch.End();
                        graphics.SetRenderTarget(_game.GameRenderTarget);
                        ctrl.Target = _target;
                    }
                    catch (AccessViolationException)
                    {

                    }
                }
            }
        }

        public static class FourthWall
        {
            public static bool GetFilePath(string title, string wfFilter, FileOpenerStyle style, out string resPath)
            {
                string initDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                bool isFullscreen = _game.graphicsDevice.IsFullScreen;
                string p = "";
                if (isFullscreen)
                {
                    //knock the player out of fullscreen
                    _game.graphicsDevice.IsFullScreen = false;
                    _game.graphicsDevice.ApplyChanges();
                }
                switch (style)
                {
                    case FileOpenerStyle.Open:
                        var opener = new System.Windows.Forms.OpenFileDialog();
                        opener.Filter = wfFilter;
                        opener.Title = title;
                        opener.InitialDirectory = initDir;
                        if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            p = opener.FileName;
                        }
                        break;
                    case FileOpenerStyle.Save:
                        var saver = new System.Windows.Forms.SaveFileDialog();
                        saver.Filter = wfFilter;
                        saver.Title = title;
                        saver.InitialDirectory = initDir;
                        if (saver.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            p = saver.FileName;
                        }
                        break;
                }
                resPath = p;
                if (isFullscreen)
                {
                    //boot the player back into fullscreen
                    _game.graphicsDevice.IsFullScreen = true;
                    _game.graphicsDevice.ApplyChanges();
                }

                return !string.IsNullOrWhiteSpace(resPath);
            }
        }


        public static long Average(this byte[] bytes)
        {
            long total = 0;
            foreach (var b in bytes)
                total += b;
            return total / bytes.Length;
        }

        public static event Action SinglePlayerStarted;

        public static void StartSPServer()
        {
            SinglePlayerStarted?.Invoke();
        }

        public static void DrawControlsToTargets(GraphicsDevice device, SpriteBatch batch)
        {
            DrawControlsToTargetsInternal(device, batch, Viewport.Width, Viewport.Height, ref topLevels);
        }

        public static void DrawHUDToTargets(GraphicsDevice device, SpriteBatch batch)
        {
                DrawControlsToTargetsInternal(device, batch, Viewport.Width, Viewport.Height, ref hudctrls);
        }

        public static void ShowCloudUpload()
        {
            _game.uploading = true;
        }

        public static void HideCloudUpload()
        {
            _game.uploading = false;
        }

        public static void ShowCloudDownload()
        {
            _game.downloading = true;
        }

        public static void HideCloudDownload()
        {
            _game.downloading = false;
        }



        public static void AddTopLevel(GUI.Control ctrl)
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                var tl = topLevels.FirstOrDefault(x => x.Control == ctrl);
                if (tl != null)
                    return;
                topLevels.Add(new RenderInfo { Control = ctrl });
                FocusedControl = ctrl;
                ctrl.Invalidate();
            });
        }

        public static void AddHUD(GUI.Control ctrl)
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                var tl = hudctrls.FirstOrDefault(x => x.Control == ctrl);
                if (tl != null)
                    return;
                hudctrls.Add(new RenderInfo { Control = ctrl });
                ctrl.Invalidate();
            });
        }


        public static void InvalidateAll()
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                foreach (var ctrl in topLevels)
                {
                    ctrl.Control.Invalidate();
                }
                foreach (var ctrl in hudctrls)
                {
                    ctrl.Control.Invalidate();
                }
            });
        }

        public static Control[] TopLevels
        {
            get
            {
                List<Control> ctrls = new List<Control>();
                foreach (var ctrl in topLevels)
                {
                    ctrls.Add(ctrl.Control);
                }
                return ctrls.ToArray();
            }
        }

        public static void ProcessMouseState(MouseState state, double lastLeftClickMS)
        {
            bool rclick = true;
            bool hidemenus = true;
            foreach (var ctrl in topLevels.ToArray().Where(x=>x != null).Where(x=>x.Control.Visible == true).OrderByDescending(x=>topLevels.IndexOf(x)))
            {
                if (ctrl.Control.ProcessMouseState(state, lastLeftClickMS))
                {
                    if(ctrl.Control is Menu||ctrl.Control is MenuItem)
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
                    var menus = topLevels.Where(x => x.Control is Menu || x.Control is MenuItem);
                    foreach (var menu in menus.ToArray())
                        (menu.Control as Menu).Hide();
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

        public static ContentManager ContentLoader
        {
            get
            {
                return _game.Content;
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

        public static void SetUITint(Color color)
        {
            _game.UITint = color;
        }


        public static bool ExperimentalEffects = true;

        public static Queue<Action> CrossThreadOperations = new Queue<Action>();
        public static GraphicsDevice GraphicsDevice
        {
            get
            {
                return _game.graphicsDevice.GraphicsDevice;
            }
        }


        public static void DrawBackgroundLayer(GraphicsDevice graphics, SpriteBatch batch, int width, int height)
        {
            graphics.Clear(Color.Black);
        }

        public static Color ToMonoColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static void StopHandling(GUI.Control ctrl, bool dispose = true)
        {
            var tl = topLevels.FirstOrDefault(x => x.Control == ctrl);
            if (tl == null)
                return;
            tl.Target.Dispose(); //dispose this regardless of the dispose value. rendertargets are expensive.
            if (dispose)
            {
                tl.Control.Dispose();
            }
            topLevels.Remove(tl);
        }

        public static void ConnectToServer(string host, int port)
        {
            if (!ServerManager.ConnectToServer(host, port))
                return;
            bool isMP = true;
            
            using(var sstr = new ServerStream(ServerMessageType.U_CONF))
            {
                var result = sstr.Send();
                using(var reader= new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    isMP = reader.ReadBoolean();
                }
            }

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

        public static Plexgate Game
        {
            get
            {
                return _game;
            }
        }

        internal static void StopHandlingHUD(GUI.Control ctrl, bool dispose = true)
        {
            var tl = hudctrls.FirstOrDefault(x => x.Control == ctrl);
            if (tl == null)
                return;
            tl.Target.Dispose(); //dispose this regardless of the dispose value. rendertargets are expensive.
            if (dispose)
            {
                tl.Control.Dispose();
            }
            hudctrls.Remove(tl);
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

    public class RenderInfo
    {
        public Control Control { get; set; }
        public RenderTarget2D Target { get; set; }
    }
}
