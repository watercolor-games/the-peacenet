using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;

namespace ShiftOS.Frontend
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShiftOS : Game
    {
        GraphicsDeviceManager GraphicsDevice;
        SpriteBatch spriteBatch;

        private bool DisplayDebugInfo = false;

        public ShiftOS()
        {
            GraphicsDevice = new GraphicsDeviceManager(this);
            GraphicsDevice.PreferredBackBufferHeight = 1080;
            GraphicsDevice.PreferredBackBufferWidth = 1920;
            SkinEngine.SkinLoaded += () =>
            {
                UIManager.ResetSkinTextures(GraphicsDevice.GraphicsDevice);
                UIManager.InvalidateAll();
            };
            UIManager.Viewport = new System.Drawing.Size(
                    GraphicsDevice.PreferredBackBufferWidth,
                    GraphicsDevice.PreferredBackBufferHeight
                );

            Content.RootDirectory = "Content";


            //Make window borderless
            Window.IsBorderless = false;

            //Set the title
            Window.Title = "ShiftOS";



            //Fullscreen
            GraphicsDevice.IsFullScreen = false;

        }

        private GUI.TextControl _titleLabel = null;
        private Keys lastKey = Keys.None;


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Before we do ANYTHING, we've got to initiate the ShiftOS engine.
            UIManager.GraphicsDevice = GraphicsDevice.GraphicsDevice;

            //Let's get localization going.
            Localization.RegisterProvider(new MonoGameLanguageProvider());

            //First things first, let's initiate the window manager.
            AppearanceManager.Initiate(new Desktop.WindowManager());
            //Cool. Now the engine's window management system talks to us.

            //Also initiate the desktop
            Engine.Desktop.Init(new Desktop.Desktop());

            //While we're having a damn initiation fuckfest, let's get the hacking engine running.
            Hacking.Initiate();

            //Now we can initiate the Infobox subsystem
            Engine.Infobox.Init(new Infobox());

            

            //Let's initiate the engine just for a ha.

            TerminalBackend.TerminalRequested += () =>
            {
                AppearanceManager.SetupWindow(new Apps.Terminal());
            };

            //We'll use sandbox mode
            SaveSystem.IsSandbox = false;
            Engine.Infobox.Show("Test window", "This is a test window.");
            SaveSystem.Begin(true);

            base.Initialize();

        }

        private double timeSinceLastPurge = 0;

        private Texture2D MouseTexture = null;

         /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);

            UIManager.ResetSkinTextures(GraphicsDevice.GraphicsDevice);


            // TODO: use this.Content to load your game content here
            var bmp = Properties.Resources.cursor_9x_pointer;
            var _lock = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] rgb = new byte[Math.Abs(_lock.Stride) * _lock.Height];
            Marshal.Copy(_lock.Scan0, rgb, 0, rgb.Length);
            bmp.UnlockBits(_lock);
            MouseTexture = new Texture2D(GraphicsDevice.GraphicsDevice, bmp.Width, bmp.Height);
            MouseTexture.SetData<byte>(rgb);
            rgb = null;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            MouseTexture = null;
            // TODO: Unload any non ContentManager content here
        }

        private double kb_elapsedms = 0;

        private MouseState LastMouseState;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (UIManager.CrossThreadOperations.Count > 0)
            {
                var action = UIManager.CrossThreadOperations.Dequeue();
                action?.Invoke();
            }

            //Let's get the mouse state
            var mouseState = Mouse.GetState(this.Window);
                LastMouseState = mouseState;
                UIManager.ProcessMouseState(LastMouseState);
            
            //So we have mouse input, and the UI layout system working...

            //But an OS isn't useful without the keyboard!

            //Let's see how keyboard input works.

            //Hmmm... just like the mouse...
            var keystate = Keyboard.GetState();

            //Simple... just iterate through this list and generate some key events?
            var keys = keystate.GetPressedKeys();
            if (keys.Length > 0)
            {
                var key = keys.FirstOrDefault(x => x != Keys.LeftControl && x != Keys.RightControl && x != Keys.LeftShift && x != Keys.RightShift && x != Keys.LeftAlt && x != Keys.RightAlt);
                if(lastKey != key)
                {
                    kb_elapsedms = 0;
                    lastKey = key;
                }
            }
            if (keystate.IsKeyDown(lastKey))
            {
                if (kb_elapsedms == 0 || kb_elapsedms >= 500)
                {
                    if (lastKey == Keys.F11)
                    {
                        GraphicsDevice.IsFullScreen = !GraphicsDevice.IsFullScreen;
                        GraphicsDevice.ApplyChanges();
                    }
                    else
                    {
                        var shift = keystate.IsKeyDown(Keys.LeftShift) || keystate.IsKeyDown(Keys.RightShift);
                        var alt = keystate.IsKeyDown(Keys.LeftAlt) || keystate.IsKeyDown(Keys.RightAlt);
                        var control = keystate.IsKeyDown(Keys.LeftControl) || keystate.IsKeyDown(Keys.RightControl);

                        if (control && lastKey == Keys.D)
                        {
                            DisplayDebugInfo = !DisplayDebugInfo;
                        }
                        else if(control && lastKey == Keys.E)
                        {
                            UIManager.ExperimentalEffects = !UIManager.ExperimentalEffects;
                        }                        
                        else
                        {
                            var e = new KeyEvent(control, alt, shift, lastKey);
                            UIManager.ProcessKeyEvent(e);
                        }
                    }
                }                
                kb_elapsedms += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                kb_elapsedms = 0;
            }

            //Cause layout update on all elements
            UIManager.LayoutUpdate();

            timeSinceLastPurge += gameTime.ElapsedGameTime.TotalSeconds;

            if(timeSinceLastPurge > 30)
            {
                GraphicsContext.StringCaches.Clear();
                timeSinceLastPurge = 0;
            }

            base.Update(gameTime);
        }

        private GUI.TextControl framerate = new GUI.TextControl();

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            UIManager.DrawControlsToTargets(GraphicsDevice.GraphicsDevice, spriteBatch, 0, 0);

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.MultiSampleAntiAlias = true;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearClamp, DepthStencilState.Default,
                            rasterizerState);
            //Draw the desktop BG.
            UIManager.DrawBackgroundLayer(GraphicsDevice.GraphicsDevice, spriteBatch, 640, 480);

            //The desktop is drawn, now we can draw the UI.
            UIManager.DrawTArgets(spriteBatch);

            //Draw a mouse cursor



            var mousepos = Mouse.GetState(this.Window).Position;
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X+1, mousepos.Y+1, MouseTexture.Width, MouseTexture.Height), Color.Black * 0.5f);
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X, mousepos.Y, MouseTexture.Width, MouseTexture.Height), Color.White);

            if (DisplayDebugInfo)
            {
                var gfxContext = new GraphicsContext(GraphicsDevice.GraphicsDevice, spriteBatch, 0, 0, GraphicsDevice.PreferredBackBufferWidth, GraphicsDevice.PreferredBackBufferHeight);
                var color = Color.White;
                double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
                if (fps <= 20)
                    color = Color.Red;
                gfxContext.DrawString($@"ShiftOS 1.0 Beta 4
Copyright (c) 2017 Michael VanOverbeek, Rylan Arbour, RogueAI
This is an unstable build.
FPS: {(fps)}
An FPS below 20 can cause glitches in keyboard and mouse handling. It is advised that if you are getting these
framerates, press CTRL+E to disable fancy effects, close any apps you are not using, and try running in windowed mode
or in a lower resolution.

If all else fails, you can set a breakpoint somewhere in the ShiftOS.Update() or ShiftOS.Draw() methods in the game's source
code and use Visual Studio's debugger to step through the code to find bottlenecks.

If a method takes more than 30 milliseconds to complete, that is a sign that it is bottlenecking the game and may need to be
optimized.

Try using the SkinTextures cache when rendering skin elements, and try using the GraphicsContext.DrawString() method when drawing
text. In this build, we are aware that this method causes bottlenecking, we are working on a caching system for fonts so we do not need to
use the System.Drawing.Graphics class to draw text.

UI render target count: {UIManager.TextureCaches.Count}
Skin texture caches: {UIManager.SkinTextures.Count}
Open windows (excluding dialog boxes): {AppearanceManager.OpenForms.Count}

Experimental effects enabled: {UIManager.ExperimentalEffects}
Fullscreen: {GraphicsDevice.IsFullScreen}
Game resolution: {GraphicsDevice.PreferredBackBufferWidth}x{GraphicsDevice.PreferredBackBufferHeight}", 0, 0, color, new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Bold));
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    [ShiftoriumProvider]
    public class MonoGameShiftoriumProvider : IShiftoriumProvider
    {
        public List<ShiftoriumUpgrade> GetDefaults()
        {
            return JsonConvert.DeserializeObject<List<ShiftoriumUpgrade>>(Properties.Resources.Shiftorium);
        }
    }
}
