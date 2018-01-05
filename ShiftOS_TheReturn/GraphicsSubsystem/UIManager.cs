using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.TextRenderers;
using Plex.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plex.Engine.Themes;
using Plex.Engine.Config;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;

namespace Plex.Engine.GraphicsSubsystem
{
    public class UIManager : IEngineComponent, IConfigurable
    {
        private class UIContainer : IEntity, ILoadable, IDisposable
        {
            [Dependency]
            private ThemeManager _thememgr = null;

            [Dependency]
            private AppDataManager _appdata = null;

            [Dependency]
            private UIManager _ui = null;

            [Dependency]
            private ConfigManager _config = null;

            [Dependency]
            private Plexgate _plexgate = null;

            private List<Control> _toplevels = new List<Control>();

            private Control _focused = null;
            private SpriteFont _monospace = null;
            private string _screenshots = null;
            private double _debugUpdTimer;
            private string _debug = "";
            private PerformanceCounter _debugCpu;
            private bool ShowPerfCounters = true;
            private MouseState _lastMouseState;

            public Control[] Controls
            {
                get
                {
                    return _toplevels.ToArray();
                }
            }

            public void AddControl(Control ctrl)
            {
                if (ctrl == null)
                    return;
                if (_toplevels.Contains(ctrl))
                    return;
                _toplevels.Add(ctrl);
                ctrl.SetManager(_ui);
                ctrl.SetTheme(_thememgr.Theme);
                ctrl.Invalidate(true);
            }

            public void RemoveControl(Control ctrl, bool dispose)
            {
                if (ctrl == null)
                    return;
                if (!_toplevels.Contains(ctrl))
                    return;
                _toplevels.Remove(ctrl);
                if (dispose)
                    ctrl.Dispose();
            }

            public bool IsFocused(Control ctrl)
            {
                return ctrl == _focused;
            }

            public void SetFocus(Control ctrl)
            {
                if (_focused == ctrl)
                    return;
                var alreadyFocused = _focused;
                _focused = ctrl;
                if (alreadyFocused != null)
                {
                    alreadyFocused.Invalidate();
                }
                ctrl.Invalidate();

            }

            public void Draw(GameTime time, GraphicsContext ctx)
            {
                foreach (var ctrl in Controls)
                {
                    if (!ctrl.Visible)
                        continue;
                    if (ctrl.Opacity > 0)
                        ctrl.Draw(time, ctx);
                }

                ctx.Device.SetRenderTarget(_plexgate.GameRenderTarget);
                ctx.BeginDraw();
                foreach (var ctrl in Controls)
                {
                    if (!ctrl.Visible)
                        continue;
                    if (ctrl.BackBuffer != null && ctrl.Opacity > 0)
                    {
                        var tint = (ctrl.Enabled) ? Color.White : Color.Gray;
                        if (_ui.IgnoreControlOpacity)
                        {
                            ctx.Batch.Draw(ctrl.BackBuffer, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), tint);
                        }
                        else
                        {
                            ctx.Batch.Draw(ctrl.BackBuffer, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), (tint * ctrl.Opacity));
                        }
                    }
                    else
                    {
                        ctrl.Invalidate();
                    }
                }
                ctx.EndDraw();
                if (ShowPerfCounters == false)
                    return;
                ctx.BeginDraw();
                _debugUpdTimer += time.ElapsedGameTime.TotalSeconds;
                if (_debugUpdTimer >= 1)
                {
                    _debug = $"{Math.Round(1 / time.ElapsedGameTime.TotalSeconds)} FPS | {GC.GetTotalMemory(false) / 1048576} MiB RAM | {Math.Round(_debugCpu.NextValue())}% CPU";
                    _debugUpdTimer %= 1;
                }
                ctx.Batch.DrawString(_monospace, _debug, Vector2.Zero, Color.White);
                ctx.EndDraw();

            }

            public void OnKeyEvent(KeyboardEventArgs e)
            {
                if (e.Key == Keys.F4)
                {
                    ShowPerfCounters = !ShowPerfCounters;
                    return;
                }
                if (e.Key == Keys.F11)
                {
                    bool fullscreen = (bool)_config.GetValue("uiFullscreen", true);
                    fullscreen = !fullscreen;
                    _config.SetValue("uiFullscreen", fullscreen);
                    _ui.ApplyConfig();
                    return;
                }
                if (e.Key == Keys.F3)
                {
                    string fname = DateTime.Now.ToString("yyyy-M-dd_HH-mm-ss") + ".png";
                    using (var fstream = File.OpenWrite(Path.Combine(_screenshots, fname)))
                    {
                        _plexgate.GameRenderTarget.SaveAsPng(fstream, _plexgate.GameRenderTarget.Width, _plexgate.GameRenderTarget.Height);
                    }
                    return;
                }

                if (_focused != null)
                    _focused.ProcessKeyboardEvent(e);
            }

            public void OnMouseUpdate(MouseState mouse)
            {
                _plexgate.BringToFront(_ui._uiLayer);
                if (mouse == _lastMouseState)
                    return;
                _lastMouseState = mouse;
                //Propagate mouse events.
                var controls = Controls.OrderByDescending(x => Array.IndexOf(Controls, x)).ToArray();
                bool skipEvents = false;
                foreach (var ctrl in controls)
                {
                    if (ctrl.Visible == false)
                        continue;
                    bool res = ctrl.PropagateMouseState(mouse, skipEvents);
                    if (res == true)
                    {
                        skipEvents = true;
                    }
                }
            }

            public void Update(GameTime time)
            {
                foreach (var ctrl in Controls)
                {
                    if (!ctrl.Visible)
                        continue;
                    ctrl.Update(time);
                }


            }

            public void Load(ContentManager content)
            {
                _monospace = content.Load<SpriteFont>("Fonts/Monospace");
                _screenshots = Path.Combine(_appdata.GamePath, "screenshots");
                if (!Directory.Exists(_screenshots))
                    Directory.CreateDirectory(_screenshots);
                _debugUpdTimer = 0;
                _debug = "";
                _debugCpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            }

            public void Dispose()
            {
                Logger.Log("Clearing out ui controls...", LogType.Info, "ui");
                while(Controls.Length>0)
                {
                    var ctrl = Controls[0];
                    RemoveControl(ctrl, true);
                }
                _debug = "";
                _debugCpu = null;
                Logger.Log("UI system is shutdown.");

            }
        }

        private Layer _uiLayer = new Layer();
        private UIContainer _container = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private string _screenshots = "";

        private bool _isShowingUI = true;
        private int _uiFadeState = 1;
        private float _uiFadeAmount = 1;

        public void ShowUI()
        {
            _plexgate.AddLayer(_uiLayer);
        }

        public void HideUI()
        {
            _plexgate.RemoveLayer(_uiLayer);
        }

        public int ScreenWidth
        {
            get
            {
                if (_plexgate.GameRenderTarget == null)
                    return 1;
                return _plexgate.GameRenderTarget.Width;
            }
        }

        public int ScreenHeight
        {
            get
            {
                if (_plexgate.GameRenderTarget == null)
                    return 1;
                return _plexgate.GameRenderTarget.Height;
            }
        }

        public void SetFocus(Control ctrl)
        {
            _container.SetFocus(ctrl);
        }

        public bool IsFocused(Control ctrl)
        {
            if (ctrl == null)
                return false;
            return _container.IsFocused(ctrl);
        }

        public void Add(Control ctrl)
        {
            if (_container.Controls.Contains(ctrl))
                return;
            _container.AddControl(ctrl);
        }

        public void Remove(Control ctrl, bool dispose = true)
        {
            if (!_container.Controls.Contains(ctrl))
                return;
            _container.RemoveControl(ctrl, dispose);
        }

        public string GetClipboardText()
        {
            if (System.Windows.Forms.Clipboard.ContainsText() == false)
                return null;
            return System.Windows.Forms.Clipboard.GetText();
        }

        public void Initiate()
        {
            Logger.Log("Loading text renderer...", LogType.Info, "ui");
            try
            {
                TextRenderer.Init(new NativeTextRenderer());
                Logger.Log("Using native text renderer.", LogType.Info, "ui");
                //TextRenderer.Init(new WindowsFormsTextRenderer());
            }
            catch
            {
                TextRenderer.Init(new WindowsFormsTextRenderer());
                Logger.Log("Couldn't load native text renderer. Falling back to GDI+.", LogType.Error, "ui");

            }
            _uiLayer = new Layer();
            _container = _plexgate.New<UIContainer>();
            _uiLayer.AddEntity(_container);
            _plexgate.AddLayer(_uiLayer);
        }

        private bool _ignoreControlOpacity = false;

        public bool IgnoreControlOpacity
        {
            get
            {
                return _ignoreControlOpacity;
            }
        }

        [Dependency]
        private ConfigManager _config = null;

        public void ApplyConfig()
        {
            bool fullscreen = (bool)_config.GetValue("uiFullscreen", true);
            _plexgate.graphicsDevice.IsFullScreen = fullscreen;
            _plexgate.graphicsDevice.ApplyChanges();
            _ignoreControlOpacity = (bool)_config.GetValue("uiIgnoreControlOpacity", false);
        }
    }
}
