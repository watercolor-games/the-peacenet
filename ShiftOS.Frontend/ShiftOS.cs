using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        
        public ShiftOS()
        {
            GraphicsDevice = new GraphicsDeviceManager(this);
            GraphicsDevice.PreferredBackBufferHeight = 1080;
            GraphicsDevice.PreferredBackBufferWidth = 1920;
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

            //First things first, let's initiate the window manager.
            AppearanceManager.Initiate(new Desktop.WindowManager());
            //Cool. Now the engine's window management system talks to us.

            //Also initiate the desktop
            Engine.Desktop.Init(new Desktop.Desktop());

            //Now we can initiate the Infobox subsystem
            Engine.Infobox.Init(new Infobox());


            //Let's initiate the engine just for a ha.
            
            TerminalBackend.TerminalRequested += () =>
            {
                AppearanceManager.SetupWindow(new Apps.Terminal());
            };

            //We'll use sandbox mode
            SaveSystem.IsSandbox = false;

            SaveSystem.Begin(true);

            base.Initialize();

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

                //Now let's process it.
                UIManager.ProcessMouseState(mouseState);

            //Cause layout update on all elements
            UIManager.LayoutUpdate();

            //set framerate
            framerate.Text = "ShiftOS 1.0 Beta 4\r\nCopyright (c) 2017 ShiftOS\r\nFPS: " + (1000 / gameTime.ElapsedGameTime.TotalMilliseconds);

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
                    var shift = keystate.IsKeyDown(Keys.LeftShift) || keystate.IsKeyDown(Keys.RightShift);
                    var alt = keystate.IsKeyDown(Keys.LeftAlt) || keystate.IsKeyDown(Keys.RightAlt);
                    var control = keystate.IsKeyDown(Keys.LeftControl) || keystate.IsKeyDown(Keys.RightControl);

                    var e = new KeyEvent(control, alt, shift, lastKey);
                    UIManager.ProcessKeyEvent(e);
                }
                kb_elapsedms += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                kb_elapsedms = 0;
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
            this.spriteBatch.Begin();
            //Draw the desktop BG.
            var graphics = GraphicsDevice.GraphicsDevice;
            UIManager.DrawBackgroundLayer(graphics, spriteBatch, 640, 480);

            //The desktop is drawn, now we can draw the UI.
            UIManager.DrawControls(graphics, spriteBatch);

            //Draw a mouse cursor



            var mousepos = Mouse.GetState(this.Window).Position;
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X, mousepos.Y, MouseTexture.Width, MouseTexture.Height), Color.White);



            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
