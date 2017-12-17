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

namespace Plex.Engine.GraphicsSubsystem
{
    public class UIManager : IEngineComponent, IConfigurable
    {
        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private ThemeManager _thememgr = null;

        [Dependency]
        private ConfigManager _config = null;

        [Dependency]
        private AppDataManager _appdata = null;

        private string _screenshots = "";

        private bool _isShowingUI = true;
        private int _uiFadeState = 1;
        private float _uiFadeAmount = 1;

        public void ShowUI()
        {
            if(_isShowingUI == false)
            {
                _isShowingUI = true;
                _uiFadeAmount = 0;
                _uiFadeState = 0;
            }
        }

        public void HideUI()
        {
            if(_isShowingUI == true)
            {
                _isShowingUI = false;
                _uiFadeAmount = 1;
                _uiFadeState = 0;
            }
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

        private List<Control> _topLevels = new List<Control>();

        private Control _focused = null;
        public void SetFocus(Control ctrl)
        {
            if (_focused == ctrl)
                return;
            var alreadyFocused = _focused;
            _focused = ctrl;
            if(alreadyFocused!=null)
            {
                alreadyFocused.Invalidate();
            }
            ctrl.Invalidate();
        }

        public bool IsFocused(Control ctrl)
        {
            if (ctrl == null)
                return false;
            return ctrl == _focused;
        }

        public void Add(Control ctrl)
        {
            if (ctrl == null)
                return;
            if (_topLevels.Contains(ctrl))
                return;
            _topLevels.Add(ctrl);
            ctrl.Invalidate();
            ctrl.SetManager(this);
            ctrl.SetTheme(_thememgr.Theme);
        }

        public bool ShowPerfCounters = true;

        public void Remove(Control ctrl, bool dispose = true)
        {
            if (ctrl == null)
                return;
            if (!_topLevels.Contains(ctrl))
                return;
            _topLevels.Remove(ctrl);
            if (dispose)
                ctrl.Dispose();
        }

        public void Clear()
        {
            while (_topLevels.Count > 0)
            {
                Remove(_topLevels[0]);
            }

        }

        public string GetClipboardText()
        {
            if (System.Windows.Forms.Clipboard.ContainsText() == false)
                return null;
            return System.Windows.Forms.Clipboard.GetText();
        }

        System.Drawing.Font _lucidaConsole = null;

        public void Initiate()
        {
            _monospace = _plexgate.Content.Load<SpriteFont>("Fonts/Monospace");
            _screenshots = Path.Combine(_appdata.GamePath, "screenshots");
            if (!Directory.Exists(_screenshots))
                Directory.CreateDirectory(_screenshots);
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
            _lucidaConsole = new System.Drawing.Font("Lucida Console", 12F);
            _debugUpdTimer = 0;
            _debug = "";
            _debugCpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        public MouseState Mouse
        {
            get
            {
                return _lastState;
            }
        }

        double _debugUpdTimer;
        string _debug = "";
        PerformanceCounter _debugCpu;

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
            try
            {
                foreach (var ctrl in _topLevels)
                {
                    if (!ctrl.Visible)
                        continue;
                    ctrl.Draw(time, ctx);
                    ctx.Device.SetRenderTarget(_plexgate.GameRenderTarget);
                    if (ctrl.BackBuffer != null && ctrl.Opacity>0)
                    {
                        ctx.BeginDraw();
                        if (IgnoreControlOpacity)
                        {
                            ctx.Batch.Draw(ctrl.BackBuffer, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), Color.White * _uiFadeAmount);
                        }
                        else
                        {
                            ctx.Batch.Draw(ctrl.BackBuffer, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), (Color.White*ctrl.Opacity)*_uiFadeAmount);
                        }
                        ctx.EndDraw();
                    }
                    else
                    {
                        ctrl.Invalidate();
                    }
                }
            }
            catch { }
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

        private SpriteFont _monospace = null;

        private MouseState _lastState;

        public void OnGameUpdate(GameTime time)
        {
            if(_uiFadeState == 0)
            {
                if (_isShowingUI == true)
                {
                    _uiFadeAmount += (float)time.ElapsedGameTime.TotalSeconds;
                    if (_uiFadeAmount >= 1f)
                    {
                        _uiFadeState = 1;
                    }
                }
                else
                {
                    _uiFadeAmount -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (_uiFadeAmount <= 0f)
                    {
                        _uiFadeState = 1;
                    }

                }
            }


            if (_isShowingUI == false)
                return;
            try
            {
                foreach (var ctrl in _topLevels)
                {
                    if (!ctrl.Visible)
                        continue;
                    ctrl.Update(time);
                }

                var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
                if (mouse == _lastState)
                    return;
                _lastState = mouse;
                //Propagate mouse events.
                for (int i = _topLevels.Count - 1; i > 0; i--)
                {
                    var ctrl = _topLevels[i];
                    if (ctrl.Visible == false)
                        continue;
                    if (ctrl.PropagateMouseState(mouse.LeftButton, mouse.MiddleButton, mouse.RightButton))
                        break;
                }
            }
            catch { }
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
            if(e.Key == Keys.F11)
            {
                bool fullscreen = (bool)_config.GetValue("uiFullscreen", true);
                fullscreen = !fullscreen;
                _config.SetValue("uiFullscreen", fullscreen);
                ApplyConfig();
                return;
            }
            if(e.Key == Keys.F3)
            {
                string fname = DateTime.Now.ToString("yyyy-M-dd_HH-mm-ss") + ".png";
                using(var fstream = File.OpenWrite(Path.Combine(_screenshots, fname)))
                {
                    _plexgate.GameRenderTarget.SaveAsPng(fstream, _plexgate.GameRenderTarget.Width, _plexgate.GameRenderTarget.Height);
                }
                return;
            }

            if(_isShowingUI)
                if (_focused != null)
                    _focused.ProcessKeyboardEvent(e);
        }

        private bool _ignoreControlOpacityValues = false;

        public bool IgnoreControlOpacity
        {
            get
            {
                return _ignoreControlOpacityValues;
            }
        }

        public void Unload()
        {
            Logger.Log("Clearing out ui controls...", LogType.Info, "ui");
            Clear();
            _lucidaConsole = null;
            _debug = "";
            _debugCpu = null;
            Logger.Log("UI system is shutdown.");
        }

        public void ApplyConfig()
        {
            bool fullscreen = (bool)_config.GetValue("uiFullscreen", true);
            _plexgate.graphicsDevice.IsFullScreen = fullscreen;
            _plexgate.graphicsDevice.ApplyChanges();
            _ignoreControlOpacityValues = (bool)_config.GetValue("uiIgnoreControlOpacity", false);
        }
    }

    public class TopLevel
    {
        public Control Control { get; set; }
        public RenderTarget2D RenderTarget { get; set; }
    }
}
