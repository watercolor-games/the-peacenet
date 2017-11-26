using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Objects;
using Plex.Engine.Interfaces;
using System.Reflection;

namespace Plex.Engine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Plexgate : Game
    {
        private static Plexgate _instance = null;

        internal static Plexgate GetInstance()
        {
            if (_instance == null)
                throw new InvalidOperationException("Plexgate has not been initiated. Therefore, you can't access its context.");
            return _instance;
        }

        private List<ComponentInfo> _components = new List<ComponentInfo>();
        private GraphicsContext _ctx = null;

        internal GraphicsDeviceManager graphicsDevice;
        SpriteBatch spriteBatch;
        public RenderTarget2D GameRenderTarget = null;
        private KeyboardListener keyboardListener = new KeyboardListener();
        public Plexgate()
        {
            if (_instance != null)
                throw new InvalidOperationException("Plexgate is already running! You cannot create multiple instances of Plexgate at the same time in one process. Instead, please let the already-running instance shut down fully.");
            graphicsDevice = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphicsDevice.PreferMultiSampling = false;
            //Make window borderless
            Window.IsBorderless = false;
            //Set the title
            Window.Title = "Plex";
            int w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2;
            int h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/2;
            graphicsDevice.PreferredBackBufferWidth = w;
            graphicsDevice.PreferredBackBufferHeight = h;


            //Fullscreen
            graphicsDevice.IsFullScreen = false;

            // keyboard events
            keyboardListener.KeyPressed += KeyboardListener_KeyPressed;

            IsFixedTimeStep = true;
            graphicsDevice.SynchronizeWithVerticalRetrace = true;
            graphicsDevice.GraphicsProfile = GraphicsProfile.HiDef;
        }

        private void KeyboardListener_KeyPressed(object sender, KeyboardEventArgs e)
        {
            foreach (var component in _components)
                component.Component.OnKeyboardEvent(e);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _instance = this;
            Logger.Log("Beginning engine initialization.");
            foreach(var type in ReflectMan.Types.Where(x=>x.GetInterfaces().Contains(typeof(IEngineComponent))))
            {
                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    Logger.Log($"Found {type.Name}, but it doesn't have a parameterless constructor, so it's ignored.  Probably a mistake.", LogType.Warning, "moduleloader");
                    continue;
                }
                Logger.Log($"Found {type.Name}", LogType.Info, "moduleloader");
                var componentInfo = new ComponentInfo
                {
                    IsInitiated = false,
                    Component = (IEngineComponent)Activator.CreateInstance(type, null)
                };
                _components.Add(componentInfo);
            }
            foreach(var component in _components)
            {
                Logger.Log($"{component.Component.GetType().Name}: Injecting dependencies...", LogType.Info, "moduleloader");
                Inject(component.Component);
            }
            //I know. This is redundant. I'm only doing this as a safety precaution, to prevent crashes with modules that try to access uninitiated modules as they're initiating.
            foreach (var component in _components)
            {
                RecursiveInit(component.Component);
            }
            Logger.Log("Done initiating engine.");
            base.Initialize();
        }

        private void RecursiveInit(IEngineComponent component)
        {
            foreach (var field in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttributes(false).Any(t => t is DependencyAttribute)))
            {
                if (field.FieldType == this.GetType())
                    continue;
                else
                {
                    var c = GetEngineComponent(field.FieldType);
                    RecursiveInit(c);
                }
            }
            if (_components.FirstOrDefault(x => x.Component == component).IsInitiated == false)
            {
                component.Initiate();
                _components.FirstOrDefault(x => x.Component == component).IsInitiated = true;
            }
        }

        public IEngineComponent GetEngineComponent(Type t)
        {
            if (!typeof(IEngineComponent).IsAssignableFrom(t) || t.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"{t.Name} is not an IEngineComponent, or does not provide a parameterless constructor.");
            return _components.First(x => t.IsAssignableFrom(x.Component.GetType())).Component;
        }

        internal object Inject(object client)
        {
            foreach (var field in client.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttributes(false).Any(t => t is DependencyAttribute)))
            {
                if (field.FieldType == this.GetType())
                    field.SetValue(client, this);
                else
                    field.SetValue(client, GetEngineComponent(field.FieldType));
            }
            return client;
        }


        private Texture2D MouseTexture = null;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {


            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);

            // TODO: use this.Content to load your game content here
            var bmp = Engine.Properties.Resources.cursor_9x_pointer;
            var _lock = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] rgb = new byte[Math.Abs(_lock.Stride) * _lock.Height];
            Marshal.Copy(_lock.Scan0, rgb, 0, rgb.Length);
            bmp.UnlockBits(_lock);
            MouseTexture = new Texture2D(GraphicsDevice, bmp.Width, bmp.Height);
            MouseTexture.SetData<byte>(rgb);
            rgb = null;
            base.LoadContent();
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            MouseTexture.Dispose();
            MouseTexture = null;
            Logger.Log("Unloading all engine modules!");
            while(_components.Count > 0)
            {
                var component = _components[0];
                Logger.Log("Unloading: " + component.Component.GetType().Name, LogType.Info, "moduleloader");
                component.Component.Unload();
                _components.RemoveAt(0);
            }
            Logger.Log("Done.");
            // TODO: Unload any non ContentManager content here
            _instance = null;
        }

        private MouseState LastMouseState;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if(GameRenderTarget == null)
                //Setup the game's rendertarget so it matches the desired resolution.
                GameRenderTarget = new RenderTarget2D(GraphicsDevice, graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight, false, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
            if(_ctx==null)
                _ctx = new GraphicsContext(GraphicsDevice, spriteBatch, 0, 0, GameRenderTarget.Width, GameRenderTarget.Height);

            keyboardListener.Update(gameTime);
            if (IsActive)
            {
                //Let's get the mouse state
                var mouseState = Mouse.GetState(this.Window);
                LastMouseState = mouseState;

                foreach(var component in _components)
                {
                    component.Component.OnGameUpdate(gameTime);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(GameRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            foreach(var component in _components)
            {
                component.Component.OnFrameDraw(gameTime, _ctx);
            }
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            RasterizerState.CullNone);
            spriteBatch.Draw(MouseTexture, new Rectangle(LastMouseState.X, LastMouseState.Y, MouseTexture.Width, MouseTexture.Height), Color.White);

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            RasterizerState.CullNone);
            spriteBatch.Draw(GameRenderTarget, new Rectangle(0, 0, GameRenderTarget.Width, GameRenderTarget.Height), Color.White);
            spriteBatch.End();
        }
    }

    public class ComponentInfo
    {
        public bool IsInitiated { get; set; }
        public IEngineComponent Component { get; set; }
    }
}
