using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Peacenet.Applications;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using floaty = System.Single;
using Plex.Engine.GUI;
using doublefloaty = System.Double;
using Microsoft.Xna.Framework.Content;

namespace Peacenet
{
    /// <summary>
    /// A <see cref="IEntity"/> which acts as the Peacegate OS bootscreen. 
    /// </summary>
    public class OSEntity : IEntity, ILoadable, IDisposable
    {
        #region Boot animation

        private int _osIntroState = 0;
        private System.Drawing.Font _bootFont = new System.Drawing.Font("Monda", 60F);
        private floaty _peacegateIconOpacity = 0;
        private doublefloaty _peacegateRide = 0;
        private bool _wgDeskOpen = false;
        private DesktopWindow _desktop = null;

        #endregion

        #region Window entities

        private Terminal _init = null;

        #endregion

        #region Engine components

        [Dependency]
        private UIManager _ui = null;
        
        [Dependency]
        private WindowSystem _winmgr = null;

        #endregion

        #region Textures

        private Texture2D _peacegate = null;

        #endregion



        /// <inheritdoc/>
        public void Dispose()
        {
            if(_desktop != null)
            {
                if(_desktop.Visible)
                {
                    _desktop.Close();
                }
                _desktop.Dispose();
                _desktop = null;
            }
            _peacegate.Dispose();
        }

        /// <summary>
        /// Retrieves this OS entity's desktop.
        /// </summary>
        public DesktopWindow Desktop
        {
            get
            {
                return _desktop;
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime time, GraphicsContext ctx)
        {
            int peacegateX = (_ui.ScreenWidth - _peacegate.Width) / 2;
            int peacegateYMax = (_ui.ScreenHeight - _peacegate.Height) / 2;
            int peacegateYMin = peacegateYMax + (int)(_ui.ScreenHeight * 0.15);
            int peacegateY = (int)MathHelper.Lerp(peacegateYMin, peacegateYMax, _peacegateIconOpacity);
            ctx.BeginDraw();
            ctx.DrawRectangle(peacegateX, peacegateY, _peacegate.Width, _peacegate.Height, _peacegate, Color.White * _peacegateIconOpacity);

            int _textY = peacegateY + _peacegate.Height + 25;
            string text = "Welcome to Peacegate.";
            var measure = TextRenderer.MeasureText(text, _bootFont, int.MaxValue, TextAlignment.TopLeft, Plex.Engine.TextRenderers.WrapMode.None);
            int _textX = ((_ui.ScreenWidth - (int)measure.X) / 2);
            ctx.DrawString(text, _textX, _textY, Color.White * _peacegateIconOpacity, _bootFont, TextAlignment.TopLeft, int.MaxValue, Plex.Engine.TextRenderers.WrapMode.None);

            ctx.EndDraw();
        }

        /// <inheritdoc/>
        public void OnKeyEvent(KeyboardEventArgs e)
        {
            if (_wgDeskOpen)
            {
                if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Microsoft.Xna.Framework.Input.Keys.T)
                {
                    var term = new Applications.Terminal(_winmgr);
                    term.Show();
                }
            }
        }

        /// <inheritdoc/>
        public void OnMouseUpdate(MouseState mouse)
        {
        }

        /// <inheritdoc/>
        public void Update(GameTime time)
        {
            switch (_osIntroState)
            {
                case 0:
                    _init = new Applications.SystemInitTerminal(_winmgr);
                    _init.Show();
                    _osIntroState++;
                    break;

                case 1:
                    if (_init.Visible == false || _init.Disposed == true)
                        _osIntroState++;
                    break;
                case 2:
                    _peacegateIconOpacity += (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_peacegateIconOpacity >= 1F)
                    {
                        _peacegateRide = 0;
                        _osIntroState++;
                    }
                    break;
                case 3:
                    _peacegateRide += time.ElapsedGameTime.TotalSeconds;
                    if(_peacegateRide>=5)
                    {
                        _osIntroState++;
                    }
                    break;
                case 4:
                    _peacegateIconOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_peacegateIconOpacity <= 0)
                    {
                        _osIntroState++;
                    }
                    break;
                case 5:
                    _wgDeskOpen = true;
                    var desk = new DesktopWindow(_winmgr);
                    desk.Show();
                    _desktop = desk;
                    _osIntroState = -1;
                    break;
            }

        }

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _osIntroState = 0;
            _peacegate = content.Load<Texture2D>("Desktop/UIIcons/Peacegate");
        }
    }
}
