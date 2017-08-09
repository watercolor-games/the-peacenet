using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ShiftOS.Engine;
using ShiftOS.Frontend.Apps;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;


namespace ShiftOS.Frontend.Desktop
{
    public class Desktop : GUI.Control, IDesktop
    {
        bool alOpen = false;
        int alX = 0;
        int alY = 0;

        public Desktop()
        {
            SaveSystem.GameReady += () =>
            {
                Show();
                SetupDesktop();
                if (!Shiftorium.UpgradeInstalled("tutorial1"))
                {
                    Story.Start("tutorial1");
                }
            };

            MouseMove += (loc) =>
            {
                if(alOpen == true)
                {
                    if(loc.X >= alX && loc.Y >= alY)
                    {
                        int height = LauncherItems[0].Height * LauncherItems.Count;
                        int width = LauncherItems[0].Width;
                        if(loc.X <= alX + width && loc.Y <= alY + height)
                        {
                            foreach(var item in LauncherItems)
                            {
                                if(loc.X >= alX && loc.Y >= alY + item.Y && loc.X <= alX + width && loc.Y <= alY + item.Y + item.Height)
                                {
                                    alSelectedItem = LauncherItems.IndexOf(item);
                                    Invalidate();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            alSelectedItem = -1;
                            Invalidate();
                            return;
                        }
                    }
                }
            };
        }

        public string DesktopName
        {
            get
            {
                return "ShiftOS MonoGame Desktop";
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
            alOpen = false;
            Invalidate();
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
            alX = loc.X;
            alY = loc.Y;
            alOpen = true;
            alSelectedItem = -1;
            Invalidate();
        }

        public void PopulateAppLauncher(LauncherItem[] items)
        {
            int y = 0;
            int height = 0;
            int[] widths = new int[items.Length];
                LauncherItems.Clear();
                for(int i = 0; i < items.Length; i++)
                {
                    string name = Localization.Parse(items[i].DisplayData.Name);
                    var measure = GraphicsContext.MeasureString(name, LoadedSkin.MainFont);
                    if (height < (int)measure.Y)
                        height = (int)measure.Y;
                    widths[i] = 120 + (int)measure.X;
                    
                }

                int width = widths.Max();

                foreach(var aitem in items)
                {
                    var item = new AppLauncherItem
                    {
                        Data = aitem,
                        X = 0,
                        Y = y,
                        Height = height,
                        Width = width
                    };
                    LauncherItems.Add(item);
                    y += item.Height;
                }
                
            
            Invalidate();
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
            if (alOpen)
                BringToFront();
            else
                SendToBack();
            X = 0;
            Y = 0;
            Width = GetSize().Width;
            Height = GetSize().Height;
            var now = DateTime.Now.TimeOfDay;
            var newDateTimeString = $"{now.Hours}:{now.Minutes}:{now.Seconds} - ";
            if(Hacking.CurrentHackable == null)
            {
                newDateTimeString += SaveSystem.CurrentSave.SystemName;
            }
            else
            {
                newDateTimeString += Hacking.CurrentHackable.Data.SystemName;
            }
            if(newDateTimeString != dateTimeString)
            {
                dateTimeString = newDateTimeString;
                Invalidate();
            }

        }

        private List<PanelButtonData> PanelButtons = new List<PanelButtonData>();
        private List<AppLauncherItem> LauncherItems = new List<AppLauncherItem>();

        public override void MouseStateChanged()
        {
            //This statement closes the app launcher. If we do this after opening it, we can't open it at all as it instantly closes.
            if (alOpen == true && MouseLeftDown == true)
            {
                if(alSelectedItem != -1)
                {
                    var item = LauncherItems[alSelectedItem];
                    AppearanceManager.SetupWindow((IShiftOSWindow)Activator.CreateInstance(item.Data.LaunchType, null));
                }
                alOpen = false;
                Invalidate();
                return;
            }


            var al_left = LoadedSkin.AppLauncherFromLeft;

            int al_top = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : Height - LoadedSkin.DesktopPanelHeight;


            var al_size = LoadedSkin.AppLauncherHolderSize;
            if(MouseX >= al_left.X && MouseY >= al_left.Y + al_top && MouseX <= al_left.X + al_size.Width && MouseY <= al_left.Y + al_top + al_size.Height)
            {
                if(alOpen == false && MouseLeftDown == true)
                {
                    alX = 0;
                    if(LoadedSkin.DesktopPanelPosition == 0)
                    {
                        alY = LoadedSkin.DesktopPanelHeight;
                    }
                    else
                    {
                        alY = (Height - LoadedSkin.DesktopPanelHeight) - (LauncherItems[0].Height * LauncherItems.Count);
                    }
                    alOpen = true;
                    Invalidate();
                }
                
            }

        }

        protected override void OnPaint(GraphicsContext gfx)
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
                gfx.DrawRectangle(0, dp_position, dp_width, dp_height, UIManager.SkinTextures["desktoppanel"]);
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
                gfx.DrawRectangle(al_left.X, dp_position + al_left.Y, holderSize.Width, holderSize.Height, UIManager.SkinTextures["applauncher"]);
            }
            var altextmeasure = GraphicsContext.MeasureString(LoadedSkin.AppLauncherText, LoadedSkin.AppLauncherFont);
            int altextx = (holderSize.Width - (int)altextmeasure.X) / 2;
            int altexty = (holderSize.Height - (int)altextmeasure.Y) / 2;
            gfx.DrawString(LoadedSkin.AppLauncherText, altextx, altexty, LoadedSkin.AppLauncherTextColor.ToMonoColor(), LoadedSkin.AppLauncherFont);
            //Panel clock.

            var panelClockRight = LoadedSkin.DesktopPanelClockFromRight;
            var panelClockTextColor = LoadedSkin.DesktopPanelClockColor.ToMonoColor();

            
            var measure = GraphicsContext.MeasureString(dateTimeString, LoadedSkin.DesktopPanelClockFont);

            int panelclockleft = Width - (int)measure.X;
            int panelclockwidth = Width - panelclockleft;

            if (UIManager.SkinTextures.ContainsKey("panelclockbg"))
            {
                //draw the background using panelclock texture
                gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, UIManager.SkinTextures["panelclockbg"]);
            }
            else
            {
                //draw using the bg color
                var pcBGColor = UIManager.SkinTextures["DesktopPanelClockBackgroundColor"];
                gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, pcBGColor);
            }

            int text_left = (panelclockwidth - (int)measure.X) / 2;
            int text_top = (dp_height - (int)measure.Y) / 2;

            //draw string
            gfx.DrawString(dateTimeString, panelclockleft + text_left, dp_position + text_top, panelClockTextColor, LoadedSkin.DesktopPanelClockFont);

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
                    gfx.DrawRectangle(offset, dp_position + pbtnfromtop, pbtnwidth, pbtnheight, UIManager.SkinTextures["panelbutton"]);
                }
                else
                {
                    gfx.DrawRectangle(offset, dp_position + pbtnfromtop, pbtnwidth, pbtnheight, UIManager.SkinTextures["PanelButtonColor"]);
                }

                //now we draw the text

                gfx.DrawString(pbtn.Title, offset + 2, dp_position + pbtnfromtop + 2, LoadedSkin.PanelButtonTextColor.ToMonoColor(), LoadedSkin.PanelButtonFont);

                offset += LoadedSkin.PanelButtonSize.Width;
            }

            if (alOpen)
            {
                int height = (LauncherItems[0].Height * LauncherItems.Count) + 2;
                int width = LauncherItems[0].Width + 2;
                gfx.DrawRectangle(alX, alY, width, height, UIManager.SkinTextures["Menu_MenuBorder"]);
                gfx.DrawRectangle(alX+1, alY+1, width-2, height-2, UIManager.SkinTextures["Menu_ToolStripDropDownBackground"]);
                gfx.DrawRectangle(alX+1, alY+1, 18, height-2, UIManager.SkinTextures["Menu_ImageMarginGradientBegin"]);

                foreach(var item in LauncherItems)
                {
                    if(LauncherItems.IndexOf(item) == alSelectedItem)
                    {
                        gfx.DrawRectangle(alX+1, alY + item.Y+1, item.Width-2, item.Height, UIManager.SkinTextures["Menu_MenuItemSelected"]);
                    }
                    gfx.DrawString(Localization.Parse(item.Data.DisplayData.Name), alX + 21, alY + item.Y+1, LoadedSkin.Menu_TextColor.ToMonoColor(), LoadedSkin.MainFont);
                }
            }
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
