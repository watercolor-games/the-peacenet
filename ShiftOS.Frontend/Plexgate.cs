using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Plexgate : Game
    {
        internal GraphicsDeviceManager graphicsDevice;
        SpriteBatch spriteBatch;

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
                    1280,
                    720
                );

            Content.RootDirectory = "Content";
            graphicsDevice.PreferMultiSampling = true;

            //Make window borderless
            Window.IsBorderless = false;

            //Set the title
            Window.Title = "Plex";



            //Fullscreen
            graphicsDevice.IsFullScreen = uconf.Fullscreen;

            // keyboard events
            keyboardListener.KeyPressed += KeyboardListener_KeyPressed;


            
            Story.MissionComplete += (mission) =>
            {
                var mc = new Apps.MissionComplete(mission);
                AppearanceManager.SetupDialog(mc);
            };

#if DEBUG
            DebugText.Visible = true;
            DebugText.AutoSize = true;
            UIManager.AddHUD(DebugText);
            UIManager.AddHUD(Watermark);
            Watermark.AutoSize = true;
            Watermark.Text = $@"Project: Plex
Copyright (c) {DateTime.Now.Year} Watercolor Games
For internal use only.";
            Watermark.Opacity = 0.75;
            Watermark.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace.Name, 16F, System.Drawing.FontStyle.Bold);
#endif

            UIManager.AddHUD(SystemError);
            SystemError.Visible = false;
            UIManager.AddHUD(SystemErrorText);
            SystemErrorText.Visible = false;

            //Frametime not limited to 16.66 Hz / 60 FPS
            IsFixedTimeStep = false;
            graphicsDevice.SynchronizeWithVerticalRetrace = true;
            graphicsDevice.GraphicsProfile = GraphicsProfile.HiDef;

        }

        private void KeyboardListener_KeyPressed(object sender, KeyboardEventArgs e)
        {

            if (e.Key == Keys.F11)
            {
                UIManager.Fullscreen = !UIManager.Fullscreen;
            }
            else if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Keys.D)
            {
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
            OutOfBoxExperience.Init(new MonoGameOOBE());

            //Before we do ANYTHING, we've got to initiate the Plex engine.
            UIManager.GraphicsDevice = GraphicsDevice;

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

            FileSkimmerBackend.Init(new MGFSLayer());



            //Create a main menu
            var mm = new MainMenu();
            UIManager.AddTopLevel(mm);
            UIManager.Init(this);

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
            GameRenderTarget = new RenderTarget2D(graphicsDevice.GraphicsDevice, 1280, 720);

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);

            UIManager.ResetSkinTextures(GraphicsDevice);


            // TODO: use this.Content to load your game content here
            var bmp = Properties.Resources.cursor_9x_pointer;
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

                //Let's get the mouse state
                var mouseState = Mouse.GetState(this.Window);
                int x = (int)GUI.ProgressBar.linear(mouseState.X, 0, graphicsDevice.PreferredBackBufferWidth, 0, 1280);
                int y = (int)GUI.ProgressBar.linear(mouseState.Y, 0, graphicsDevice.PreferredBackBufferHeight, 0, 720);
                bool prc = true;
                bool lastclicked = LastMouseState.LeftButton == ButtonState.Pressed;
                LastMouseState = new MouseState(x, y, mouseState.ScrollWheelValue, mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2);
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
                //So we have mouse input, and the UI layout system working...

                //But an OS isn't useful without the keyboard!

                //Let's see how keyboard input works.

                keyboardListener.Update(gameTime);


                //Cause layout update on all elements
                UIManager.LayoutUpdate(gameTime);

                timeSinceLastPurge += gameTime.ElapsedGameTime.TotalSeconds;

                if (timeSinceLastPurge > 2)
                {
                    GraphicsContext.StringCaches.Clear();
                    timeSinceLastPurge = 0;
                    GC.Collect();
                }


                //Some hackables have a connection timeout applied to them.
                //We must update timeout values here, and disconnect if the timeout
                //hits zero.

                if (Hacking.CurrentHackable != null)
                {
                    if (Hacking.CurrentHackable.DoConnectionTimeout)
                    {
                        Hacking.CurrentHackable.MillisecondsCountdown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                        shroudOpacity = (float)GUI.ProgressBar.linear(Hacking.CurrentHackable.MillisecondsCountdown, Hacking.CurrentHackable.TotalConnectionTimeMS, 0, 0, 1);
                        if (Hacking.CurrentHackable.MillisecondsCountdown <= 0)
                        {
                            Hacking.FailHack();
                        }
                    }
                }
                else
                {
                    shroudOpacity = 0;
                }
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
                    var titlemeasure = GraphicsContext.MeasureString(e_systemerror, new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace.Name, 20f, System.Drawing.FontStyle.Bold));
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
                    var e_measure = GraphicsContext.MeasureString(e_errtext, SystemErrorText.Font, cwidth);
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


            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.MultiSampleAntiAlias = true;

            graphicsDevice.GraphicsDevice.SetRenderTarget(GameRenderTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            rasterizerState);
            //Create a graphics context so we can draw shit
            var gfx = new GraphicsContext(graphicsDevice.GraphicsDevice, spriteBatch, 0, 0, 1280, 720);
            //Draw the desktop BG.
            UIManager.DrawBackgroundLayer(GraphicsDevice, spriteBatch, 640, 480);

            //The desktop is drawn, now we can draw the UI.
            UIManager.DrawTArgets(spriteBatch);


            //draw tutorial overlay
            if (IsInTutorial)
            {
                gfx.DrawRectangle(0, 0, MouseEventBounds.X, 720, Color.Black * 0.5F);
                gfx.DrawRectangle(MouseEventBounds.X, 0, 1280 - MouseEventBounds.X, MouseEventBounds.Y, Color.Black * 0.5F);
                gfx.DrawRectangle(MouseEventBounds.Right, MouseEventBounds.Y, 1280 - MouseEventBounds.Right, MouseEventBounds.Height, Color.Black * 0.5F);
                gfx.DrawRectangle(MouseEventBounds.X, MouseEventBounds.Bottom, 1280 - MouseEventBounds.X, 720 - MouseEventBounds.Bottom, Color.Black * 0.5F);

                var tutmeasure = GraphicsContext.MeasureString(TutorialOverlayText, SkinEngine.LoadedSkin.MainFont, 1280 / 3);
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
                gfx.DrawString(TutorialOverlayText, textX, textY, Color.White, SkinEngine.LoadedSkin.MainFont, (int)tutmeasure.X);

            }


            spriteBatch.Draw(UIManager.SkinTextures["PureWhite"], new Rectangle(0, 0, UIManager.Viewport.Width, UIManager.Viewport.Height), ShroudColor * shroudOpacity);

            if (isFailing && failFadeInMS >= failFadeMaxMS)
            {
                string objectiveFailed = "- OBJECTIVE FAILURE -";
                string prompt = "[press any key to dismiss this message and return to your sentience]";
                int textMaxWidth = UIManager.Viewport.Width / 3;
                var topMeasure = GraphicsContext.MeasureString(objectiveFailed, SkinEngine.LoadedSkin.HeaderFont, textMaxWidth);
                var msgMeasure = GraphicsContext.MeasureString(failMessage, SkinEngine.LoadedSkin.Header3Font, textMaxWidth);
                var pMeasure = GraphicsContext.MeasureString(prompt, SkinEngine.LoadedSkin.MainFont, textMaxWidth);

                gfx.DrawString(objectiveFailed, (UIManager.Viewport.Width - (int)topMeasure.X) / 2, UIManager.Viewport.Height / 3, Color.White, SkinEngine.LoadedSkin.HeaderFont, textMaxWidth);
                gfx.DrawString(failMessage, (UIManager.Viewport.Width - (int)msgMeasure.X) / 2, (UIManager.Viewport.Height - (int)msgMeasure.Y) / 2, Color.White, SkinEngine.LoadedSkin.Header3Font, textMaxWidth);
                gfx.DrawString(prompt, (UIManager.Viewport.Width - (int)pMeasure.X) / 2, UIManager.Viewport.Height - (UIManager.Viewport.Height / 3), Color.White, SkinEngine.LoadedSkin.MainFont, textMaxWidth);
            }

            if (Hacking.CurrentHackable != null)
            {
                if (Hacking.CurrentHackable.DoConnectionTimeout)
                {
                    string str = $"Timeout in {(Hacking.CurrentHackable.MillisecondsCountdown / 1000).ToString("#.##")} seconds.";
                    var measure = GraphicsContext.MeasureString(str, SkinEngine.LoadedSkin.HeaderFont);
                    gfx.DrawString(str, 5, (gfx.Height - ((int)measure.Y) - 5), Color.Red, SkinEngine.LoadedSkin.HeaderFont);
                }
            }
#if DEBUG
            if (DisplayDebugInfo)
            {
                //So we need to draw the shroud for the debug text because I don't have foreground/background colors implemented into the UI framework yet.
                gfx.DrawRectangle(0, 0, (int)DebugText.Width + 10, (int)DebugText.Height + 10, Color.Black * 0.75F);
            }

#endif
            //Since we've drawn all the shrouds and stuff...
            //we can draw the HUD.
            UIManager.DrawHUD(spriteBatch);
            //Draw a mouse cursor
            var mousepos = LastMouseState;
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X + 1, mousepos.Y + 1, MouseTexture.Width, MouseTexture.Height), Color.Black * 0.5f);
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X, mousepos.Y, MouseTexture.Width, MouseTexture.Height), Color.White);

            spriteBatch.End();
            graphicsDevice.GraphicsDevice.SetRenderTarget(null);
            var bloom = UIManager.Bloom(GameRenderTarget);
            graphicsDevice.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            rasterizerState);
            spriteBatch.Draw(GameRenderTarget, new Rectangle(0, 0, graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight), Color.White);
            if(bloom != GameRenderTarget)
                spriteBatch.Draw(bloom, new Rectangle(0, 0, graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        
            graphicsDevice.GraphicsDevice.SetRenderTarget(null);
            base.Draw(gameTime);
#if DEBUG
            var color = Color.White;
            double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
            if (fps <= 20)
                color = Color.Red;
            DebugText.Text = $@"Plex
=======================

Copyright (c) 2017 Plex Developers

Debug information - {fps} FPS

CTRL+D: toggle debug menu
CTRL+E: toggle experimental effects (experimental effects enabled: {UIManager.ExperimentalEffects})
Use the ""debug"" Terminal Command for engine debug commands.

Current time: {DateTime.Now}
Text cache: {GraphicsContext.StringCaches.Count}";
#endif
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

    public class MGFSLayer : IFileSkimmer
    {
        public string GetFileExtension(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.CommandFormat:
                    return ".cf";
                case FileType.Executable:
                    return ".saa";
                case FileType.Filesystem:
                    return ".mfs";
                case FileType.Image:
                    return ".png";
                case FileType.JSON:
                    return ".json";
                case FileType.Lua:
                    return ".lua";
                case FileType.Python:
                    return ".py";
                case FileType.Skin:
                    return ".skn";
                case FileType.TextFile:
                    return ".txt";
                default:
                    return ".scrtm";
            }
        }

        public void GetPath(string[] filetypes, FileOpenerStyle style, Action<string> callback)
        {
            var fs = new Apps.FileSkimmer();
            fs.IsDialog = true;
            fs.DialogMode = style;
            fs.FileFilters = filetypes;
            fs.DialogCallback = callback;
            AppearanceManager.SetupDialog(fs);
        }

        public void OpenDirectory(string path)
        {
            if (!Objects.ShiftFS.Utils.DirectoryExists(path))
                return;
            var fs = new Apps.FileSkimmer();
            fs.Navigate(path);
            AppearanceManager.SetupWindow(fs);
        }
    }

}
