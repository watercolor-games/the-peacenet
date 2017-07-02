using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            Content.RootDirectory = "Content";
            //Make the mouse cursor visible.
            this.IsMouseVisible = true;

            //Don't allow ALT+F4
            this.Window.AllowAltF4 = false;

            //Make window borderless
            Window.IsBorderless = true;

            //Set the title
            Window.Title = "ShiftOS";

            

            //Fullscreen
            GraphicsDevice.IsFullScreen = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //We'll start by initializing the BARE FUNDAMENTALS of the ShiftOS engine.
            //This'll be enough to do skinning, fs, windowmanagement etc
            //so that we can make the main menu and yeah

            //Let's add a control to test something
            var textControl = new GUI.TextControl();
            textControl.Width = 640;
            textControl.Height = 480;
            UIManager.AddTopLevel(textControl);

            UIManager.AddTopLevel(framerate);
            framerate.Width = 640;
            framerate.Height = 480;
            framerate.TextAlign = GUI.TextAlign.BottomRight;
            

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

            //set framerate
            framerate.Text = "ShiftOS 1.0 Beta 4\r\nCopyright (c) 2017 ShiftOS\r\nFPS: " + (1 / gameTime.ElapsedGameTime.TotalSeconds);


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
