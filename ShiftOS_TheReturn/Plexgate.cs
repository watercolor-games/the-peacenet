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
using Plex.Frontend.GraphicsSubsystem;
using Plex.Objects;

namespace Plex.Frontend
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Plexgate : Game
    {
        internal GraphicsDeviceManager graphicsDevice;
        SpriteBatch spriteBatch;

        internal UdpClient _mpClient = null;
        internal IPAddress IPAddress = null;
        internal int Port = 0;
        internal Thread ServerThread = null;

        private GUI.TextControl _objectiveTitle = new GUI.TextControl();
        private GUI.TextControl _objectiveDesc = new GUI.TextControl();
        private int objectiveState = 0;
        private double _objectiveStateValue = 0.0;

        public event Action Initializing;

        public void FireInitialized()
        {
            Initializing?.Invoke();
        }

#if DEBUG
        private GUI.TextControl DebugText = new GUI.TextControl();
        private GUI.TextControl Watermark = new GUI.TextControl();
#endif
        private GUI.TextControl SystemError = new GUI.TextControl();
        private GUI.TextControl SystemErrorText = new GUI.TextControl();



        private Color ShroudColor = Color.Black;

        public bool IsInTutorial = false;
        public Rectangle MouseEventBounds;
        public string TutorialOverlayText = "";
        public Action TutorialOverlayCompleted = null;

        //Crash variables
        public bool IsCrashed = false;
        public double CrashAnimMS = 0.0;
        


        public void Crash()
        {
            CrashAnimMS = 0;
            IsCrashed = true;
        }


        private bool isFailing = false;
        private double failFadeInMS = 0;
        private const double failFadeMaxMS = 500;
        private string failMessage = "";
        private string failRealMessage = "";
        private double failFadeOutMS = 0;
        private bool failEnded = false;
        private double failCharAddMS = 0;

        public RenderTarget2D GameRenderTarget = null;

        public Color UITint = Color.White;

        private bool DisplayDebugInfo = false;

        private KeyboardListener keyboardListener = new KeyboardListener ();

        public Plexgate()
        {
            Story.FailureRequested += (message) =>
            {
                failMessage = "";
                failRealMessage = message;
                isFailing = true;
                failFadeInMS = 0;
                failFadeOutMS = 0;
                failEnded = false;
            };
            graphicsDevice = new GraphicsDeviceManager(this);
            var uconf = Objects.UserConfig.Get();
            graphicsDevice.PreferredBackBufferHeight = uconf.ScreenHeight;
            graphicsDevice.PreferredBackBufferWidth = uconf.ScreenWidth;
            SkinEngine.SkinLoaded += () =>
            {
                UIManager.ResetSkinTextures(GraphicsDevice);
                UIManager.InvalidateAll();
            };
            UIManager.Viewport = new System.Drawing.Size(
                    uconf.ScreenWidth,
                    uconf.ScreenHeight
                );

            Content.RootDirectory = "Content";
            graphicsDevice.PreferMultiSampling = false;

            //Make window borderless
            Window.IsBorderless = false;

            //Set the title
            Window.Title = "Plex";



            //Fullscreen
            graphicsDevice.IsFullScreen = uconf.Fullscreen;

            // keyboard events
            keyboardListener.KeyPressed += KeyboardListener_KeyPressed;


            
            
#if DEBUG
            DebugText.Visible = true;
            DebugText.AutoSize = true;
            UIManager.AddHUD(DebugText);
#endif
            UIManager.AddHUD(_objectiveTitle);
            UIManager.AddHUD(_objectiveDesc);
            UIManager.AddHUD(SystemError);
            SystemError.Visible = false;
            UIManager.AddHUD(SystemErrorText);
            SystemErrorText.Visible = false;

            //Frametime not limited to 16.66 Hz / 60 FPS
            IsFixedTimeStep = true;
            graphicsDevice.SynchronizeWithVerticalRetrace = true;
            graphicsDevice.GraphicsProfile = GraphicsProfile.HiDef;

        }

        private double lowestfps = 0;

        private void KeyboardListener_KeyPressed(object sender, KeyboardEventArgs e)
        {

            if (e.Key == Keys.F11)
            {
                UIManager.Fullscreen = !UIManager.Fullscreen;
            }
            else if(e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Modifiers.HasFlag(KeyboardModifiers.Shift) && e.Key == Keys.S)
            {
                SkinEngine.LoadEngineDefault();
            }
            else if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Keys.D)
            {
                lowestfps = double.MaxValue;
                highestfps = 0;
                DisplayDebugInfo = !DisplayDebugInfo;
            }
            else if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Keys.E)
            {
                UIManager.ExperimentalEffects = !UIManager.ExperimentalEffects;
            }
            else
            {
                // Notice: I would personally recommend just using KeyboardEventArgs instead of KeyEvent
                // from now on, but what ever. -phath0m
                UIManager.ProcessKeyEvent(new KeyEvent(e));
            }
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Story.ObjectiveStarted += () =>
            {
                objectiveState = 1;
                _objectiveStateValue = 0.0;
                _objectiveTitle.Text = Story.CurrentObjectives.First().Name;
                _objectiveDesc.Text = Story.CurrentObjectives.First().Description;

            };

            Story.ObjectiveComplete += () =>
            {
                objectiveState = 2;
                _objectiveStateValue = 0.0;
            };

            ATextRenderer strategy = null;
            try
            {
                strategy = new Engine.TextRenderers.NativeTextRenderer();
            }
            catch
            {
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    strategy = new Engine.TextRenderers.WindowsFormsTextRenderer();
                }
                else
                {
                    strategy = new Engine.TextRenderers.GdiPlusTextRenderer();
                }
			}
			
			TextRenderer.Init(strategy);
			Console.WriteLine(strategy.GetType().ToString());


            //Before we do ANYTHING, we've got to initiate the Plex engine.
            UIManager.GraphicsDevice = GraphicsDevice;



            
            //While we're having a damn initiation fuckfest, let's get the hacking engine running.
            Hacking.Initiate();




            ClientThread = new Thread(() =>
            {
                while (true)
                {
                    if (this.IPAddress != null)
                    {
                        try
                        {
                            var ep = new IPEndPoint(this.IPAddress, this.Port);
                            var data = _mpClient.Receive(ref ep);
                            var content = Encoding.UTF8.GetString(data);
                            msSinceLastReply = 0.0;
                            if (content == "beat")
                            {
                                System.Diagnostics.Debug.Print("Pong");
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("Message received.");
                                try
                                {
                                    var msg = JsonConvert.DeserializeObject<PlexServerHeader>(content);
                                    ServerManager.HandleMessage(msg);
                                }
                                catch
                                {

                                }
                            }

                        }
                        catch { }
                    }
                }
            });
            ClientThread.Start();



            UIManager.Init(this);

            _mpClient = new UdpClient();

            Initializing?.Invoke();

            base.Initialize();

        }

        private double msSinceLastReply = 0.0;

        private Thread ClientThread = null;

        
        private Texture2D MouseTexture = null;

         /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GameRenderTarget = new RenderTarget2D(graphicsDevice.GraphicsDevice, UIManager.Viewport.Width, UIManager.Viewport.Height, false, graphicsDevice.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);

            UIManager.ResetSkinTextures(GraphicsDevice);


            // TODO: use this.Content to load your game content here
            var bmp = Engine.Properties.Resources.cursor_9x_pointer;
            var _lock = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] rgb = new byte[Math.Abs(_lock.Stride) * _lock.Height];
            Marshal.Copy(_lock.Scan0, rgb, 0, rgb.Length);
            bmp.UnlockBits(_lock);
            MouseTexture = new Texture2D(GraphicsDevice, bmp.Width, bmp.Height);
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

            ServerThread?.Abort();
            // TODO: Unload any non ContentManager content here
        }
        
        private double mouseMS = 0;

        private MouseState LastMouseState;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if(objectiveState == 0)
            {
                _objectiveTitle.Visible = false;
                _objectiveDesc.Visible = false;
            }
            else
            {
                _objectiveTitle.Visible = true;
                _objectiveDesc.Visible = true;
                _objectiveTitle.AutoSize = true;
                _objectiveTitle.Font = SkinEngine.LoadedSkin.Header3Font;
                _objectiveDesc.AutoSize = true;
                _objectiveDesc.Font = SkinEngine.LoadedSkin.MainFont;
                _objectiveTitle.MaxWidth = 300;
                _objectiveDesc.MaxWidth = 300;
                _objectiveStateValue = MathHelper.Clamp((float)_objectiveStateValue + ((float)gameTime.ElapsedGameTime.TotalSeconds * 1.5F), 0, 1);
                switch (objectiveState)
                {
                    case 1:
                        _objectiveTitle.Opacity = 1.0;
                        _objectiveDesc.Opacity = 1.0;
                        _objectiveTitle.X = (int)MathHelper.Lerp(0 - _objectiveTitle.Width, 15, (float)_objectiveStateValue);
                        _objectiveDesc.X = _objectiveTitle.X;
                        break;
                    case 2:
                        _objectiveTitle.Opacity = 1.0 - (float)_objectiveStateValue;
                        _objectiveDesc.Opacity = _objectiveTitle.Opacity;
                        if (_objectiveStateValue == 1.0)
                            objectiveState = 0;
                        break;
                }

                _objectiveDesc.Y = (UIManager.Viewport.Height - _objectiveDesc.Height) - 15;
                _objectiveTitle.Y = _objectiveDesc.Y - _objectiveTitle.Height - 15;
            }
            try
            {
                if (IPAddress != null)
                {
                    msSinceLastReply += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if ((int)msSinceLastReply >= 2000)
                    {
                        var bytes = Encoding.UTF8.GetBytes("heart");
                        _mpClient.Send(bytes, bytes.Length);
                        System.Diagnostics.Debug.Print("Ping.");
                    }
                    if (msSinceLastReply >= 10000)
                    {
                        ServerManager.Disconnect(DisconnectType.Error, "The server took too long to respond.");
                    }
                }
            }
            catch { }
            if (isFailing)
            {
                if (failFadeInMS < failFadeMaxMS)
                    failFadeInMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (failEnded == false)
                {
                    shroudOpacity = (float)GUI.ProgressBar.linear(failFadeInMS, 0, failFadeMaxMS, 0, 1);
                    if (shroudOpacity >= 1)
                    {
                        if (failMessage == failRealMessage + "|")
                        {
                            var keydata = Keyboard.GetState();

                            if (keydata.GetPressedKeys().FirstOrDefault(x => x != Keys.None) != Keys.None)
                            {
                                failEnded = true;
                            }
                        }
                        else
                        {
                            failCharAddMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                            if (failCharAddMS >= 75)
                            {
                                failMessage = failRealMessage.Substring(0, failMessage.Length) + "|";
                                failCharAddMS = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (failFadeOutMS < failFadeMaxMS)
                    {
                        failFadeOutMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                    }

                    shroudOpacity = 1 - (float)GUI.ProgressBar.linear(failFadeOutMS, 0, failFadeMaxMS, 0, 1);

                    if (shroudOpacity <= 0)
                    {
                        isFailing = false;
                    }
                }
            }
            else
            {
                if (UIManager.CrossThreadOperations.Count > 0)
                {
                    var action = UIManager.CrossThreadOperations.Dequeue();
                    action?.Invoke();
                }
                if (IsActive)
                {
                    //Let's get the mouse state
                    var mouseState = Mouse.GetState(this.Window);
                    bool prc = true;
                    int x = mouseState.X;
                    int y = mouseState.Y;
                    bool lastclicked = LastMouseState.LeftButton == ButtonState.Pressed;
                    LastMouseState = new MouseState(mouseState.X, mouseState.Y, mouseState.ScrollWheelValue, mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2);
                    if (IsInTutorial)
                    {
                        if (!(x >= MouseEventBounds.X && x <= MouseEventBounds.Right) || !(y >= MouseEventBounds.Y && y <= MouseEventBounds.Bottom))
                            prc = false;
                    }
                    if (prc == true)
                    {

                        UIManager.ProcessMouseState(LastMouseState, mouseMS);
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            mouseMS = 0;
                            if (IsInTutorial && lastclicked == false)
                            {
                                IsInTutorial = false;
                                TutorialOverlayCompleted?.Invoke();

                            }
                        }
                        else
                        {
                            mouseMS += gameTime.ElapsedGameTime.TotalMilliseconds;

                        }
                    }
                }
                //So we have mouse input, and the UI layout system working...

                //But an OS isn't useful without the keyboard!

                //Let's see how keyboard input works.

                keyboardListener.Update(gameTime);


                //Cause layout update on all elements
                UIManager.LayoutUpdate(gameTime);

                //Some hackables have a connection timeout applied to them.
                //We must update timeout values here, and disconnect if the timeout
                //hits zero.

            }

            if (IsCrashed)
            {
                CrashAnimMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                if((CrashAnimMS > 500 && CrashAnimMS < 600) || CrashAnimMS > 800)
                {
                    ShroudColor = Color.Black;
                    shroudOpacity = 1;
                }
                else
                {
                    shroudOpacity = 0;
                }

                if (CrashAnimMS > 1800)
                {
                    ShroudColor = Color.Blue;
                }

                if (CrashAnimMS > 2400)
                {
                    string e_systemerror = "System error";
                    string e_errtext = @"Plexgate has experienced a fatal error and the Plex kernel has shut your user experience down.

In order for you to regain your graphical user experience, you will need to start a text shell, connect to your system, diagnose and correct the error. Reboot the system when you are done.

To begin this process, strike the [T] key while holding <CTRL>.";
                    int cwidth = 1280 - 400;
                    var titlemeasure = GraphicsContext.MeasureString(e_systemerror, new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace.Name, 20f, System.Drawing.FontStyle.Bold), Engine.GUI.TextAlignment.TopLeft);
                    SystemError.X = 200;
                    SystemError.Y = 200;
                    SystemError.AutoSize = false;
                    SystemError.Width = (int)titlemeasure.X;
                    SystemError.Height = (int)titlemeasure.Y;
                    SystemError.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace.Name, 20f, System.Drawing.FontStyle.Bold);
                    SystemError.Text = e_systemerror;

                    SystemErrorText.Text = e_errtext;
                    SystemErrorText.AutoSize = false;
                    SystemErrorText.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace.Name, 12f);
                    var e_measure = GraphicsContext.MeasureString(e_errtext, SystemErrorText.Font, Engine.GUI.TextAlignment.TopLeft, cwidth);
                    SystemErrorText.Width = (int)e_measure.X;
                    SystemErrorText.Height = (int)e_measure.Y;
                    SystemErrorText.X = 200;
                    SystemErrorText.Y = SystemError.Y + SystemError.Height + 20;

                    SystemError.Visible = true;
                    SystemErrorText.Visible = true;
                }
            }
            else
            {
                SystemError.Visible = false;
                SystemErrorText.Visible = false;

            }

#if DEBUG
            DebugText.Visible = DisplayDebugInfo;
            if (DebugText.Visible)
            {
                DebugText.X = 5;
                DebugText.Y = 5;
                

            }

            Watermark.X = (1280 - Watermark.Width) / 2;
            Watermark.Y = (720 - Watermark.Height) / 2;
#endif

            base.Update(gameTime);
        }

        float shroudOpacity = 0.0f;

        private GUI.TextControl framerate = new GUI.TextControl();

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            UIManager.DrawControlsToTargets(GraphicsDevice, spriteBatch);
            UIManager.DrawHUDToTargets(GraphicsDevice, spriteBatch);


            graphicsDevice.GraphicsDevice.SetRenderTarget(GameRenderTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            RasterizerState.CullNone);
            //Create a graphics context so we can draw shit
            var gfx = new GraphicsContext(graphicsDevice.GraphicsDevice, spriteBatch, 0, 0, 1280, 720);
            //Draw the desktop BG.
            UIManager.DrawBackgroundLayer(GraphicsDevice, spriteBatch, 640, 480);

            spriteBatch.End();


            //The desktop is drawn, now we can draw the UI.
            UIManager.DrawTArgets(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            RasterizerState.CullNone);

            //draw tutorial overlay
            if (IsInTutorial)
            {
                gfx.DrawRectangle(0, 0, MouseEventBounds.X, 720, Color.Black * 0.5F);
                gfx.DrawRectangle(MouseEventBounds.X, 0, 1280 - MouseEventBounds.X, MouseEventBounds.Y, Color.Black * 0.5F);
                gfx.DrawRectangle(MouseEventBounds.Right, MouseEventBounds.Y, 1280 - MouseEventBounds.Right, MouseEventBounds.Height, Color.Black * 0.5F);
                gfx.DrawRectangle(MouseEventBounds.X, MouseEventBounds.Bottom, 1280 - MouseEventBounds.X, 720 - MouseEventBounds.Bottom, Color.Black * 0.5F);

                var tutmeasure = GraphicsContext.MeasureString(TutorialOverlayText, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft, 1280 / 3);
                int textX = ((MouseEventBounds.X) >= (1280 / 2)) ? MouseEventBounds.X - (int)tutmeasure.X - 15 : MouseEventBounds.Right + 15;
                int textY = ((MouseEventBounds.Y) >= (720 / 2)) ? MouseEventBounds.Y - (int)tutmeasure.Y - 15 : MouseEventBounds.Bottom + 15;
                if (textX < 15)
                    textX = 15;
                if (textX > 1265)
                    textX = 1265 - (int)tutmeasure.X;
                if (textY < 0)
                    textY = 15;
                if (textY > 705)
                    textY = 705 - (int)tutmeasure.Y;
                gfx.DrawString(TutorialOverlayText, textX, textY, Color.White, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft, (int)tutmeasure.X);

            }


            spriteBatch.Draw(UIManager.SkinTextures["PureWhite"], new Rectangle(0, 0, UIManager.Viewport.Width, UIManager.Viewport.Height), ShroudColor * shroudOpacity);

            if (isFailing && failFadeInMS >= failFadeMaxMS)
            {
                string objectiveFailed = "- OBJECTIVE FAILURE -";
                string prompt = "[press any key to dismiss this message and return to your sentience]";
                int textMaxWidth = UIManager.Viewport.Width / 3;
                var topMeasure = GraphicsContext.MeasureString(objectiveFailed, SkinEngine.LoadedSkin.HeaderFont, Engine.GUI.TextAlignment.TopLeft, textMaxWidth);
                var msgMeasure = GraphicsContext.MeasureString(failMessage, SkinEngine.LoadedSkin.Header3Font, Engine.GUI.TextAlignment.Middle, textMaxWidth);
                var pMeasure = GraphicsContext.MeasureString(prompt, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.Middle, textMaxWidth);

                gfx.DrawString(objectiveFailed, (UIManager.Viewport.Width - (int)topMeasure.X) / 2, UIManager.Viewport.Height / 3, Color.White, SkinEngine.LoadedSkin.HeaderFont, Engine.GUI.TextAlignment.TopLeft, textMaxWidth);
                gfx.DrawString(failMessage, (UIManager.Viewport.Width - (int)msgMeasure.X) / 2, (UIManager.Viewport.Height - (int)msgMeasure.Y) / 2, Color.White, SkinEngine.LoadedSkin.Header3Font, Engine.GUI.TextAlignment.Middle, textMaxWidth);
                gfx.DrawString(prompt, (UIManager.Viewport.Width - (int)pMeasure.X) / 2, UIManager.Viewport.Height - (UIManager.Viewport.Height / 3), Color.White, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.Middle, textMaxWidth);
            }

#if DEBUG
            if (DisplayDebugInfo)
            {
                //So we need to draw the shroud for the debug text because I don't have foreground/background colors implemented into the UI framework yet.
                gfx.DrawRectangle(0, 0, (int)DebugText.Width + 10, (int)DebugText.Height + 10, SkinEngine.LoadedSkin.ControlColor.ToMonoColor() * 0.75F);
            }

#endif
            spriteBatch.End();

            //Since we've drawn all the shrouds and stuff...
            //we can draw the HUD.
            UIManager.DrawHUD(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            RasterizerState.CullNone);

            //Draw a mouse cursor
            var mousepos = LastMouseState;
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X + 1, mousepos.Y + 1, MouseTexture.Width, MouseTexture.Height), Color.Black * 0.5f);
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X, mousepos.Y, MouseTexture.Width, MouseTexture.Height), Color.White);

            spriteBatch.End();
            graphicsDevice.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            RasterizerState.CullNone);
            spriteBatch.Draw(GameRenderTarget, new Rectangle(0, 0, graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        
            framesdrawn++;
            base.Draw(gameTime);
#if DEBUG
            var color = Color.White;
            double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
            if (fps <= 20)
                color = Color.Red;
            highestfps = Math.Max(highestfps, fps);
            lowestfps = Math.Min(lowestfps, fps);
            TotalFPS += fps;
            DebugText.Text = $@"Plex
=======================

Copyright (c) 2017 Plex Developers

Debug information

CTRL+D: toggle debug menu
CTRL+E: toggle experimental effects (experimental effects enabled: {UIManager.ExperimentalEffects})
Use the ""debug"" Terminal Command for engine debug commands.

FPS: {fps} - Highest: {highestfps}, Lowest: {lowestfps} (note: opening or closing this debug text resets these counters)
Average FPS: {TotalFPS / framesdrawn}
Current time: {DateTime.Now}
Memory usage: {(GC.GetTotalMemory(false) / 1024) / 1024} MB
";
#endif
        }
        public double TotalFPS = 0;
        public int framesdrawn = 0;
        public double highestfps = 0;
    }

    

    public static class ImageExtensioons
    {
        public static Texture2D ToTexture2D(this System.Drawing.Image image, GraphicsDevice device)
        {
            var bmp = (System.Drawing.Bitmap)image;
            var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = new byte[Math.Abs(lck.Stride) * lck.Height];
            Marshal.Copy(lck.Scan0, data, 0, data.Length);
            bmp.UnlockBits(lck);
            for (int i = 0; i < data.Length; i += 4)
            {
                byte r = data[i];
                byte b = data[i + 2];
                data[i] = b;
                data[i + 2] = r;
            }
            var tex2 = new Texture2D(device, bmp.Width, bmp.Height);
            tex2.SetData<byte>(data);
            return tex2;
        }
    }


    public static class ConsoleColorExtensions
    {
        public static System.Drawing.Color ToColor(this ConsoleColor cc)
        {
            switch (cc)
            {
                case ConsoleColor.Black:
                    return System.Drawing.Color.Black;
                case ConsoleColor.Blue:
                    return System.Drawing.Color.Blue;
                case ConsoleColor.Cyan:
                    return System.Drawing.Color.Cyan;
                case ConsoleColor.DarkBlue:
                    return System.Drawing.Color.DarkBlue;
                case ConsoleColor.DarkCyan:
                    return System.Drawing.Color.DarkCyan;
                case ConsoleColor.DarkGray:
                    return System.Drawing.Color.DarkGray;
                case ConsoleColor.DarkGreen:
                    return System.Drawing.Color.DarkGreen;
                case ConsoleColor.DarkMagenta:
                    return System.Drawing.Color.DarkMagenta;
                case ConsoleColor.DarkRed:
                    return System.Drawing.Color.DarkRed;
                case ConsoleColor.DarkYellow:
                    return System.Drawing.Color.Orange;
                case ConsoleColor.Gray:
                    return System.Drawing.Color.Gray;
                case ConsoleColor.Green:
                    return System.Drawing.Color.Green;
                case ConsoleColor.Magenta:
                    return System.Drawing.Color.Magenta;
                case ConsoleColor.Red:
                    return System.Drawing.Color.Red;
                case ConsoleColor.White:
                    return System.Drawing.Color.White;
                case ConsoleColor.Yellow:
                    return System.Drawing.Color.Yellow;
            }
            return System.Drawing.Color.Empty;
        }
    }

}
