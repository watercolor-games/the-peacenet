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

            Content.RootDirectory = "Content";
            

            //Make window borderless
            Window.IsBorderless = false;

            //Set the title
            Window.Title = "ShiftOS";

            

            //Fullscreen
            GraphicsDevice.IsFullScreen = false;

        }

        private GUI.TextControl _titleLabel = null;

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


            //Let's give it a try.
            Engine.Infobox.Show("Welcome to ShiftOS!", "This is a test infobox. Clicking OK will dismiss it.");

            //Let's set up the Main Menu UI.

            var justthes = new GUI.PictureBox();
            justthes.AutoSize = true;
            justthes.ImageLayout = GUI.ImageLayout.Stretch;
            justthes.Image = Properties.Resources.justthes;
            justthes.X = 15;
            justthes.Y = 15;
            UIManager.AddTopLevel(justthes);

            _titleLabel = new GUI.TextControl();
            _titleLabel.Text = " - main menu - ";
            _titleLabel.AutoSize = true;
            _titleLabel.X = justthes.X;
            _titleLabel.Y = justthes.Y + justthes.Height + 15;
            _titleLabel.Font = SkinEngine.LoadedSkin.HeaderFont;
            UIManager.AddTopLevel(_titleLabel);

            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Let's get the mouse state
            var mouseState = Mouse.GetState(this.Window);

            //Now let's process it.
            UIManager.ProcessMouseState(mouseState);

            //Cause layout update on all elements
            UIManager.LayoutUpdate();

            //set framerate
            framerate.Text = "ShiftOS 1.0 Beta 4\r\nCopyright (c) 2017 ShiftOS\r\nFPS: " + (1 / gameTime.ElapsedGameTime.TotalSeconds);

            //So we have mouse input, and the UI layout system working...

            //But an OS isn't useful without the keyboard!

            //Let's see how keyboard input works.

            //Hmmm... just like the mouse...
            var keystate = Keyboard.GetState();

            //Simple... just iterate through this list and generate some key events?
            var keys = keystate.GetPressedKeys();
            if (keys.Length > 0)
            {
                //Of course, we need modifier keys...
                //First for Control.
                bool controlDown = keys.Contains(Keys.LeftControl) || keys.Contains(Keys.RightControl);
                //Now SHIFT.
                bool shiftDown = keys.Contains(Keys.LeftShift) || keys.Contains(Keys.RightShift);
                //And ALT.
                bool altDown = keys.Contains(Keys.LeftAlt) || keys.Contains(Keys.RightAlt);

                foreach(var key in keys)
                {
                    //This'll make it so we skip the modifier keys.
                    if(key != Keys.LeftAlt && key != Keys.RightAlt && key != Keys.LeftControl && key != Keys.RightControl && key != Keys.LeftShift && key != Keys.RightShift)
                    {
                        var keyevent = new KeyEvent(controlDown, altDown, shiftDown, key);
                        UIManager.ProcessKeyEvent(keyevent);
                    }
                }
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


            var bmp = Properties.Resources.cursor_9x_pointer;
            var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] rgb = new byte[Math.Abs(data.Stride) * data.Height];
            Marshal.Copy(data.Scan0, rgb, 0, rgb.Length);
            bmp.UnlockBits(data);
            var mousepos = Mouse.GetState(this.Window).Position;
            var tex2 = new Texture2D(graphics, bmp.Width, bmp.Height);
            tex2.SetData<byte>(rgb);
            spriteBatch.Draw(tex2, new Rectangle(mousepos.X, mousepos.Y, bmp.Width, bmp.Height), Color.White);



            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
