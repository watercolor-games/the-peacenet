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
                return _plexgate.GameRenderTarget.Width;
            }
        }

        public int ScreenHeight
        {
            get
            {
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
                TextRenderer.Init(new GdiPlusTextRenderer());
                Logger.Log("Couldn't load native text renderer. Falling back to GDI+.", LogType.Error, "ui");

            }
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
            foreach (var ctrl in _topLevels)
            {
                if (!ctrl.Visible)
                    continue;
                ctrl.Draw(time, ctx);
                ctx.Device.SetRenderTarget(_plexgate.GameRenderTarget);
                ctx.BeginDraw();
                if (IgnoreControlOpacity)
                {
                    ctx.DrawRectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height, ctrl.BackBuffer, Color.White * _uiFadeAmount);
                }
                else
                {
                    ctx.DrawRectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height, ctrl.BackBuffer, Color.White * (_uiFadeAmount * ctrl.Opacity));
                }
                ctx.EndDraw();
            }
            if (ShowPerfCounters == false)
                return;
            ctx.BeginDraw();
            var fps = Math.Round(1 / time.ElapsedGameTime.TotalSeconds);
            ctx.DrawString($"FPS: {fps} - RAM: {(GC.GetTotalMemory(false)/1024)/1024}MB", 0, 0, Color.White, new System.Drawing.Font("Lucida Console", 12F), TextAlignment.TopLeft);
            ctx.EndDraw();
        }

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
            var mouse = Mouse.GetState();
            foreach(var ctrl in _topLevels.ToArray())
            {
                ctrl.Update(time);
            }

            //Propagate mouse events.
            foreach(var ctrl in _topLevels.OrderByDescending(x=>_topLevels.IndexOf(x)))
            {
                if (ctrl.PropagateMouseState(mouse.LeftButton, mouse.MiddleButton, mouse.RightButton))
                    break;
            }
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
