using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.Apps;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;


namespace Plex.Frontend.Desktop
{
    public class Desktop : GUI.TextControl, IDesktop
    {
        public PlexSkin LoadedSkin
        {
            get
            {
                return (PlexSkin)SkinEngine.LoadedSkin;
            }
        }

        public Menu _appLauncher = new Menu();

        public Desktop()
        {
            SaveSystem.GameReady += () =>
            {
                AudioPlayerSubsystem.Startup();
                Show();
                SetupDesktop();
                if (!Upgrades.UpgradeInstalled("tutorial1"))
                {
                    Story.Start("tutorial1");
                }
            };
            Click += () =>
            {
                if (_appLauncher.Visible == false)
                {
                    if (MouseX >= LoadedSkin.AppLauncherFromLeft.X && MouseX <= LoadedSkin.AppLauncherFromLeft.X + LoadedSkin.AppLauncherHolderSize.Width)
                    {
                        int dp_pos = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : Height - LoadedSkin.DesktopPanelHeight;
                        int dp_height = LoadedSkin.DesktopPanelHeight;
                        if (MouseY >= dp_pos && MouseY <= dp_pos + dp_height)
                        {
                            int al_x = LoadedSkin.AppLauncherFromLeft.X;
                            _appLauncher.Layout(new GameTime());
                            int al_y = (dp_pos == 0) ? dp_height : (dp_pos - _appLauncher.Height);
                            OpenAppLauncher(new System.Drawing.Point(al_x, al_y));
                        }
                    }
                }
                else
                {
                    HideAppLauncher();
                }
            };
        }

        public string DesktopName
        {
            get
            {
                return "Plex MonoGame Desktop";
            }
        }

        private int alSelectedItem = -1;

        public void Close()
        {
            UIManager.StopHandling(this);
        }

        public Size GetSize()
        {
            return UIManager.Viewport;
        }

        public void HideAppLauncher()
        {
            _appLauncher.Hide();
        }

        public void InvokeOnWorkerThread(Action act)
        {
            UIManager.CrossThreadOperations.Enqueue(act);
        }

        public void KillWindow(IWindowBorder border)
        {
        }

        public void MaximizeWindow(IWindowBorder brdr)
        {
        }

        public void MinimizeWindow(IWindowBorder brdr)
        {
        }

        public void OpenAppLauncher(System.Drawing.Point loc)
        {
            _appLauncher.X = loc.X;
            _appLauncher.Y = loc.Y;
            _appLauncher.Show();
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            int dp_height = LoadedSkin.DesktopPanelHeight;
            int dp_position = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : Height - dp_height;
            int dp_width = Width;
            //Panel clock.

            var panelClockRight = LoadedSkin.DesktopPanelClockFromRight;
            var panelClockTextColor = LoadedSkin.DesktopPanelClockColor.ToMonoColor();


            var pcMeasure = GraphicsContext.MeasureString(dateTimeString, LoadedSkin.DesktopPanelClockFont, Engine.GUI.TextAlignment.TopRight);
            int panelclockleft = Width - (int)pcMeasure.X;
            int panelclockwidth = Width - panelclockleft;
            
            gfx.DrawString(dateTimeString, panelclockleft, dp_position + LoadedSkin.DesktopPanelClockFromRight.Y, LoadedSkin.DesktopPanelClockColor.ToMonoColor(), LoadedSkin.DesktopPanelClockFont, Engine.GUI.TextAlignment.TopRight);

            int al_holder_width = LoadedSkin.AppLauncherHolderSize.Width;
            
            var almeasure = GraphicsContext.MeasureString(LoadedSkin.AppLauncherText, LoadedSkin.AppLauncherFont, Engine.GUI.TextAlignment.TopLeft);
            gfx.DrawString(LoadedSkin.AppLauncherText, (al_holder_width - (int)almeasure.X) / 2, (LoadedSkin.AppLauncherHolderSize.Height - (int)almeasure.Y) / 2, LoadedSkin.AppLauncherTextColor.ToMonoColor(), LoadedSkin.AppLauncherFont, Engine.GUI.TextAlignment.TopLeft);

            int initialGap = LoadedSkin.PanelButtonHolderFromLeft;
            int offset = initialGap;

            foreach (var pbtn in PanelButtons.ToArray())
            {
                offset += LoadedSkin.PanelButtonFromLeft.X;

                int pbtnfromtop = LoadedSkin.PanelButtonFromTop;
                int pbtnwidth = LoadedSkin.PanelButtonSize.Width;
                int pbtnheight = LoadedSkin.PanelButtonSize.Height;

                //now we draw the text

                gfx.DrawString(pbtn.Title, offset + 2, dp_position + pbtnfromtop + 2, LoadedSkin.PanelButtonTextColor.ToMonoColor(), LoadedSkin.PanelButtonFont, Engine.GUI.TextAlignment.TopLeft);

                offset += LoadedSkin.PanelButtonSize.Width;
            }
        }


        public void PopulateAppLauncher(LauncherItem[] items)
        {
            _appLauncher.ClearItems();
            List<string> cats = new List<string>();
            foreach(var item in items.OrderBy(x => Localization.Parse(x.DisplayData.Category)))
            {
                if (!cats.Contains(Localization.Parse(item.DisplayData.Category)))
                    cats.Add(Localization.Parse(item.DisplayData.Category));
            }
            foreach(var cat in cats)
            {
                var catitem = new MenuItem();
                catitem.Text = cat;

                foreach(var item in items.Where(x => Localization.Parse(x.DisplayData.Category) == cat).OrderBy(x=>Localization.Parse(x.DisplayData.Name)))
                {
                    var alitem = new MenuItem();
                    alitem.Text = Localization.Parse(item.DisplayData.Name);
                    alitem.ItemActivated += () =>
                    {
                        AppearanceManager.SetupWindow((IPlexWindow)Activator.CreateInstance(item.LaunchType, null));
                        HideAppLauncher();
                    };
                    catitem.AddItem(alitem);
                }

                _appLauncher.AddItem(catitem);
            }

            var shutdown = new MenuItem
            {
                Text = "Shut down",
            };
            shutdown.ItemActivated += () => { PlexCommands.Shutdown(); };
            _appLauncher.AddItem(shutdown);
        }

        public void PopulatePanelButtons()
        {
            PanelButtons.Clear();
            foreach(var win in AppearanceManager.OpenForms)
            {
                var border = win as WindowBorder;
                var pbtn = new PanelButtonData();
                pbtn.Title = border.Text;
                pbtn.Window = border;
                PanelButtons.Add(pbtn);
            }
            RequireTextRerender();
            Invalidate();            
        }

        public void PushNotification(string app, string title, string message)
        {
        }

        public void RestoreWindow(IWindowBorder brdr)
        {
        }

        public void SetupDesktop()
        {
            Invalidate();
        }

        public void Show()
        {
            UIManager.AddTopLevel(this);
            Visible = true;
            Invalidate();
        }

        public void ShowWindow(IWindowBorder border)
        {
        }

        private string dateTimeString = "";

        protected override void OnLayout(GameTime gameTime)
        {
            SendToBack();
            X = 0;
            Width = GetSize().Width;
            Height = LoadedSkin.DesktopPanelHeight;
            Y = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : GetSize().Height - Height;
            var now = DateTime.Now.TimeOfDay;
            var newDateTimeString = $"{now.Hours}:{now.Minutes}:{now.Seconds} - ";
                if (SaveSystem.CurrentSave != null)
                    newDateTimeString += SaveSystem.CurrentSave.SystemName;
                else
                    newDateTimeString += "localhost";
            if (newDateTimeString != dateTimeString)
            {
                dateTimeString = newDateTimeString;
                RequireTextRerender();
                Invalidate();
            }



        }

        private List<PanelButtonData> PanelButtons = new List<PanelButtonData>();
        
        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            //Let's get data for the desktop panel.

            //We need the width and the height and the position.

            int dp_height = LoadedSkin.DesktopPanelHeight;
            int dp_position = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : Height - dp_height;
            int dp_width = Width;

            //Alright, now we need to know if we should draw using a texture or a color
            if (UIManager.SkinTextures.ContainsKey("desktoppanel"))
            {
                //Draw with the texture
                gfx.DrawRectangle(0, dp_position, dp_width, dp_height, UIManager.SkinTextures["desktoppanel"], SkinEngine.GetImageLayout("desktoppanel"));
            }
            else
            {
                //draw with a color
                var color = UIManager.SkinTextures["DesktopPanelColor"];
                gfx.DrawRectangle(0, dp_position, dp_width, dp_height, color);
            }

            //Alright, now App Launcher.
            var al_left = LoadedSkin.AppLauncherFromLeft;
            var holderSize = LoadedSkin.AppLauncherHolderSize;
            if (UIManager.SkinTextures.ContainsKey("applauncher"))
            {
                gfx.DrawRectangle(al_left.X, dp_position + al_left.Y, holderSize.Width, holderSize.Height, UIManager.SkinTextures["applauncher"], SkinEngine.GetImageLayout("applauncher"));
            }

            var pcMeasure = GraphicsContext.MeasureString(dateTimeString, LoadedSkin.DesktopPanelClockFont, Engine.GUI.TextAlignment.TopRight);
            int panelclockleft = Width - (int)pcMeasure.X;
            int panelclockwidth = Width - panelclockleft;

            if (UIManager.SkinTextures.ContainsKey("panelclockbg"))
            {
                //draw the background using panelclock texture
                gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, UIManager.SkinTextures["panelclockbg"], SkinEngine.GetImageLayout("panelclockbg"));
            }
            else
            {
                if (!UIManager.SkinTextures.ContainsKey("desktoppanel"))
                {
                    //draw using the bg color
                    var pcBGColor = UIManager.SkinTextures["DesktopPanelClockBackgroundColor"];
                    gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, pcBGColor);
                }
            }

            int initialGap = LoadedSkin.PanelButtonHolderFromLeft;
            int offset = initialGap;

            foreach(var pbtn in PanelButtons.ToArray())
            {
                offset += LoadedSkin.PanelButtonFromLeft.X;

                int pbtnfromtop = LoadedSkin.PanelButtonFromTop;
                int pbtnwidth = LoadedSkin.PanelButtonSize.Width;
                int pbtnheight = LoadedSkin.PanelButtonSize.Height;

                //Draw panel button background...
                if (UIManager.SkinTextures.ContainsKey("panelbutton"))
                {
                    gfx.DrawRectangle(offset, dp_position + pbtnfromtop, pbtnwidth, pbtnheight, UIManager.SkinTextures["panelbutton"], SkinEngine.GetImageLayout("panelbutton"));
                }
                else
                {
                    gfx.DrawRectangle(offset, dp_position + pbtnfromtop, pbtnwidth, pbtnheight, UIManager.SkinTextures["PanelButtonColor"]);
                }

                offset += LoadedSkin.PanelButtonSize.Width;
            }

            base.OnPaint(gfx, target);
        }
    }

    public class PanelButtonData
    {
        public string Title { get; set; }
        public WindowBorder Window { get; set; }

        public bool IsActive
        {
            get
            {
                return Window.IsFocusedControl || Window.ContainsFocusedControl;
            }
        }
    }
    

    public class AppLauncherItem
    {
        public Engine.LauncherItem Data { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
