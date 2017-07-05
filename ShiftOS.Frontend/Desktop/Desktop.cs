using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;


namespace ShiftOS.Frontend.Desktop
{
    public class Desktop : GUI.Control, IDesktop
    {
        public Desktop()
        {
            SaveSystem.GameReady += () =>
            {
                Show();
                SetupDesktop();
            };
        }

        public string DesktopName
        {
            get
            {
                return "ShiftOS MonoGame Desktop";
            }
        }

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

        public void OpenAppLauncher(Point loc)
        {
        }

        public void PopulateAppLauncher(LauncherItem[] items)
        {
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

        protected override void OnLayout()
        {
            SendToBack();
            X = 0;
            Y = 0;
            Width = GetSize().Width;
            Height = GetSize().Height;
            Invalidate();
        }

        private List<PanelButtonData> PanelButtons = new List<PanelButtonData>();

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
                var color = LoadedSkin.DesktopPanelColor.ToMonoColor();
                gfx.DrawRectangle(0, dp_position, dp_width, dp_height, color);
            }

            //Alright, now App Launcher.
            var al_left = LoadedSkin.AppLauncherFromLeft;
            var holderSize = LoadedSkin.AppLauncherHolderSize;
            if (UIManager.SkinTextures.ContainsKey("applauncher"))
            {
                gfx.DrawRectangle(al_left.X, dp_position + al_left.Y, holderSize.Width, holderSize.Height, UIManager.SkinTextures["applauncher"]);
            }

            //Panel clock.

            var panelClockRight = LoadedSkin.DesktopPanelClockFromRight;
            var panelClockTextColor = LoadedSkin.DesktopPanelClockColor.ToMonoColor();

            var dateTimeString = DateTime.Now.TimeOfDay.ToString();

            var measure = gfx.MeasureString(dateTimeString, LoadedSkin.DesktopPanelClockFont);

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
                var pcBGColor = LoadedSkin.DesktopPanelClockBackgroundColor.ToMonoColor();
                gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, pcBGColor);
            }

            int text_left = (panelclockwidth - (int)measure.X) / 2;
            int text_top = (dp_height - (int)measure.Y) / 2;

            //draw string
            gfx.DrawString(dateTimeString, panelclockleft + text_left, dp_position + text_top, panelClockTextColor, LoadedSkin.DesktopPanelClockFont);

            int initialGap = LoadedSkin.PanelButtonHolderFromLeft;
            int offset = initialGap;

            foreach(var pbtn in PanelButtons)
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
                    gfx.DrawRectangle(offset, dp_position + pbtnfromtop, pbtnwidth, pbtnheight, LoadedSkin.PanelButtonColor.ToMonoColor());
                }

                //now we draw the text

                gfx.DrawString(pbtn.Title, offset + 2, dp_position + pbtnfromtop + 2, LoadedSkin.PanelButtonTextColor.ToMonoColor(), LoadedSkin.PanelButtonFont);

                offset += LoadedSkin.PanelButtonSize.Width;
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
}
