using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.Themes;
using Plex.Engine.GUI;
using Peacenet.Applications;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Cutscene;
using Plex.Engine.Saves;
using Peacenet.MainMenu;
using Microsoft.Xna.Framework.Content;
using Peacenet.PeacegateThemes;
using Plex.Objects;
using Microsoft.Xna.Framework.Input;
using Plex.Engine.Config;

namespace Peacenet
{
    /// <summary>
    /// Provides a simple engine component for displaying the Peacenet splash screen and main menu.
    /// </summary>
    public class SplashScreenComponent : IEngineComponent, ILoadable
    {
        [Dependency]
        private GameLoop _GameLoop = null;

        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private ConfigManager _config = null;

        [Dependency]
        private WindowSystem _winsys = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            Logger.Log("Peacenet is loading its theme now!");
            _theme.Theme = _GameLoop.New<PeacenetTheme>();
            Logger.Log("And now for the save backend.");
            
#if DEBUG
            _GameLoop.GetLayer(LayerType.NoDraw).AddEntity(_GameLoop.New<DebugEntity>());
#endif


        }

        private SplashEntity splash = null;

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            Logger.Log("Applying GUI scale...");

            float renderScale = _config.GetValue<float>("renderScale", 0);
            if (renderScale == 0)
            {
                renderScale = (_GameLoop.GetRenderScreenSize().Y / _GameLoop.BackBufferHeight);
                _config.SetValue("renderScale", renderScale);
                _GameLoop.RenderScale = renderScale;

                var screenScaleSetter = new Applications.ScreenScaleSetter(_winsys, (scale) =>
                {
                    _config.SetValue("renderScale", scale);
                    _config.SaveToDisk();
                    _GameLoop.RenderScale = scale;
                    splash = (_GameLoop.New<SplashEntity>());
                    MakeVisible();
                });
                screenScaleSetter.Show();

            }
            else
            {
                _GameLoop.RenderScale = renderScale;
                splash = (_GameLoop.New<SplashEntity>());
                MakeVisible();
            }
        }
        
        /// <summary>
        /// Re-spawns the <see cref="SplashEntity"/>, causing the main menu to show once again. 
        /// </summary>
        public void Reset()
        {
            splash = _GameLoop.New<SplashEntity>();
            MakeVisible();
        }

        /// <summary>
        /// Makes the splash screen entity layer visible to the renderer.
        /// </summary>
        public void MakeVisible()
        {
            _GameLoop.GetLayer(LayerType.UserInterface).AddEntity(splash);
        }

        /// <summary>
        /// Makes the splash screen entity layer hidden from the renderer.
        /// </summary>
        public void MakeHidden()
        {
            _GameLoop.GetLayer(LayerType.UserInterface).RemoveEntity(splash);
        }

    }

#if DEBUG
    public class DebugEntity : IEntity
    {
        [Dependency]
        private WindowSystem _winsys = null;

        public void Draw(GameTime time, GraphicsContext gfx)
        {
        }

        public void OnGameExit()
        {
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        public void Update(GameTime time)
        {
        }
    }
#endif
}
