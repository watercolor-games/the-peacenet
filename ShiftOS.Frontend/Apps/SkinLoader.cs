using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;
using ShiftOS.Frontend.GUI;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.Frontend.Apps
{
    [WinOpen("skinloader")]
    [DefaultTitle("Skin Loader")]
    [Launcher("Skin Loader", false, null, "Customization")]
    [FileHandler("Skin Loader", ".skn", "")]
    public class SkinLoader : GUI.Control, IShiftOSWindow, IFileHandler
    {
        private Skin _skin = null;

        private Button _close = null;
        private Button _default = null;
        private Button _load = null;
        private Button _save = null;
        private Button _apply = null;
        private Dictionary<string, Texture2D> SkinTextures = new Dictionary<string, Texture2D>();

        public SkinLoader()
        {
            _close = new GUI.Button();
            _default = new GUI.Button();
            _load = new GUI.Button();
            _save = new GUI.Button();
            _apply = new GUI.Button();
            _close.AutoSize = true;
            _default.AutoSize = true;
            _load.AutoSize = true;
            _save.AutoSize = true;
            _apply.AutoSize = true;
            AddControl(_close);
            AddControl(_default);
            AddControl(_load);
            AddControl(_save);
            AddControl(_apply);

            _close.Click += ()=> AppearanceManager.Close(this);
            _default.Click += () =>
            {
                _skin = new Skin();
                ResetPreviewData();
            };
            _load.Click += () =>
            {
                FileSkimmerBackend.GetFile(new[] { ".skn" }, FileOpenerStyle.Open, (path) =>
                 {
                     _skin = JsonConvert.DeserializeObject<Skin>(Utils.ReadAllText(path));
                     ResetPreviewData();
                 });
            };
            _save.Click += () =>
            {
                FileSkimmerBackend.GetFile(new[] { ".skn" }, FileOpenerStyle.Save, (path) =>
                {
                    string json = JsonConvert.SerializeObject(_skin, Formatting.Indented);
                    Utils.WriteAllText(path, json);
                });

            };
            _apply.Click += () =>
            {
                string json = JsonConvert.SerializeObject(_skin, Formatting.Indented);
                Utils.WriteAllText(Paths.GetPath("skin.json"), json);
                SkinEngine.LoadSkin();
                Engine.Infobox.Show("Skin applied!", "The operating system's UI has been shifted successfully. Enjoy the new look!");
            };
        }

        public void ResetSkinTextures()
        {
            var graphics = UIManager.GraphicsDevice;
            SkinTextures.Clear();
            foreach (var byteArray in SkinEngine.LoadedSkin.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x => x.FieldType == typeof(byte[])))
            {
                var imgAttrib = byteArray.GetCustomAttributes(false).FirstOrDefault(x => x is ImageAttribute) as ImageAttribute;
                if (imgAttrib != null)
                {
                    var img = SkinEngine.ImageFromBinary((byte[])byteArray.GetValue(_skin));
                    if (img != null)
                    {
                        var bmp = (System.Drawing.Bitmap)img;
                        var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        var data = new byte[Math.Abs(lck.Stride) * lck.Height];
                        Marshal.Copy(lck.Scan0, data, 0, data.Length);
                        bmp.UnlockBits(lck);
                        var tex2 = new Texture2D(graphics, bmp.Width, bmp.Height);
                        for (int i = 0; i < data.Length; i += 4)
                        {
                            byte r = data[i];
                            byte b = data[i + 2];
                            if (r == 1 && b == 1 && data[i + 1] == 1)
                            {
                                data[i + 3] = 0;
                            }
                            data[i] = b;
                            data[i + 2] = r;
                        }
                        tex2.SetData<byte>(data);
                        SkinTextures.Add(imgAttrib.Name, tex2);
                    }
                }
            }

            foreach (var colorfield in SkinEngine.LoadedSkin.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x => x.FieldType == typeof(System.Drawing.Color)))
            {
                var color = (System.Drawing.Color)colorfield.GetValue(_skin);
                var tex2 = new Texture2D(graphics, 1, 1);
                tex2.SetData<byte>(new[] { color.R, color.G, color.B, color.A });
                SkinTextures.Add(colorfield.Name, tex2);
            }

            var pureWhite = new Texture2D(graphics, 1, 1);
            pureWhite.SetData<byte>(new byte[] { 255, 255, 255, 255 });
            SkinTextures.Add("PureWhite", pureWhite);

        }


        public void PaintDesktop(GraphicsContext gfx)
        {
            //Let's get data for the desktop panel.

            //We need the width and the height and the position.

            int dp_height = _skin.DesktopPanelHeight;
            int dp_position = (_skin.DesktopPanelPosition == 0) ? _desktopystart : _desktopystart + (_previewheight - dp_height);
            int dp_width = _previewxwidth;

            //Alright, now we need to know if we should draw using a texture or a color
            if (SkinTextures.ContainsKey("desktoppanel"))
            {
                //Draw with the texture
                gfx.DrawRectangle(_previewxstart, dp_position, dp_width, dp_height, SkinTextures["desktoppanel"]);
            }
            else
            {
                //draw with a color
                var color = SkinTextures["DesktopPanelColor"];
                gfx.DrawRectangle(_previewxstart, dp_position, dp_width, dp_height, color);
            }

            //Alright, now App Launcher.
            var al_left = _skin.AppLauncherFromLeft;
            var holderSize = _skin.AppLauncherHolderSize;
            if (SkinTextures.ContainsKey("applauncher"))
            {
                gfx.DrawRectangle(al_left.X, dp_position + al_left.Y, holderSize.Width, holderSize.Height, SkinTextures["applauncher"]);
            }
            var altextmeasure = gfx.MeasureString(_skin.AppLauncherText, _skin.AppLauncherFont);
            int altextx = _previewxstart + (holderSize.Width - (int)altextmeasure.X) / 2;
            int altexty = _desktopystart + (holderSize.Height - (int)altextmeasure.Y) / 2;
            gfx.DrawString(_skin.AppLauncherText, altextx, altexty, _skin.AppLauncherTextColor.ToMonoColor(), _skin.AppLauncherFont);
            //Panel clock.

            var panelClockRight = _skin.DesktopPanelClockFromRight;
            var panelClockTextColor = _skin.DesktopPanelClockColor.ToMonoColor();

            string dateTimeString = "00:00:00 - localhost";
            var measure = gfx.MeasureString(dateTimeString, _skin.DesktopPanelClockFont);

            int panelclockleft = _previewxstart + (dp_width - (int)measure.X);
            int panelclockwidth = (dp_width - panelclockleft);

            if (SkinTextures.ContainsKey("panelclockbg"))
            {
                //draw the background using panelclock texture
                gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, SkinTextures["panelclockbg"]);
            }
            else
            {
                //draw using the bg color
                var pcBGColor = SkinTextures["DesktopPanelClockBackgroundColor"];
                gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, pcBGColor);
            }

            int text_left = (panelclockwidth - (int)measure.X) / 2;
            int text_top = (dp_height - (int)measure.Y) / 2;

            //draw string
            gfx.DrawString(dateTimeString, panelclockleft + text_left, dp_position + text_top, panelClockTextColor, _skin.DesktopPanelClockFont);

            int initialGap = _previewxstart + _skin.PanelButtonHolderFromLeft;
            int offset = initialGap;

            foreach (var pbtn in PanelButtons.ToArray())
            {
                offset += _skin.PanelButtonFromLeft.X;

                int pbtnfromtop = _skin.PanelButtonFromTop;
                int pbtnwidth = _skin.PanelButtonSize.Width;
                int pbtnheight = _skin.PanelButtonSize.Height;

                //Draw panel button background...
                if (SkinTextures.ContainsKey("panelbutton"))
                {
                    gfx.DrawRectangle(offset, dp_position + pbtnfromtop, pbtnwidth, pbtnheight, SkinTextures["panelbutton"]);
                }
                else
                {
                    gfx.DrawRectangle(offset, dp_position + pbtnfromtop, pbtnwidth, pbtnheight, SkinTextures["PanelButtonColor"]);
                }

                //now we draw the text

                gfx.DrawString(pbtn.Title, offset + 2, dp_position + pbtnfromtop + 2, _skin.PanelButtonTextColor.ToMonoColor(), _skin.PanelButtonFont);

                offset += _skin.PanelButtonSize.Width;
            }

            int alX = _previewxstart;
            int alY = _desktopystart + dp_height;
            int height = (LauncherItems[0].Height * LauncherItems.Count) + 2;
            if (_skin.DesktopPanelPosition == 1)
                alY = _desktopystart + ((_previewheight - dp_height) - height);
            int width = LauncherItems[0].Width + 2;
            gfx.DrawRectangle(alX, alY, width, height, SkinTextures["Menu_MenuBorder"]);
            gfx.DrawRectangle(alX + 1, alY + 1, width - 2, height - 2, SkinTextures["Menu_ToolStripDropDownBackground"]);
            gfx.DrawRectangle(alX + 1, alY + 1, 18, height - 2, SkinTextures["Menu_ImageMarginGradientBegin"]);

            foreach (var item in LauncherItems)
            {
                if (LauncherItems.IndexOf(item) == alSelectedItem)
                {
                    gfx.DrawRectangle(alX + 1, alY + item.Y + 1, item.Width - 2, item.Height, SkinTextures["Menu_MenuItemSelected"]);
                }
                gfx.DrawString(Localization.Parse(item.Data.DisplayData.Name), alX + 21, alY + item.Y + 1, _skin.Menu_TextColor.ToMonoColor(), _skin.MainFont);
            }


        }

        public void ResetPreviewData()
        {
            LauncherItems = new List<Desktop.AppLauncherItem>();
            int font_height = _skin.MainFont.Height;
            LauncherItems.Add(new Desktop.AppLauncherItem
            {
                Data = new LauncherItem
                {
                    DisplayData = new LauncherAttribute("Regular", false, null)
                },
                Height = font_height,
                Width = 200,
                X = 0,
                Y = 0
            });
            LauncherItems.Add(new Desktop.AppLauncherItem
            {
                Data = new LauncherItem
                {
                    DisplayData = new LauncherAttribute("Highlighted", false, null)
                },
                Height = font_height,
                Width = 200,
                X = 0,
                Y = font_height
            });
            PanelButtons = new List<Desktop.PanelButtonData>();
            PanelButtons.Add(new Desktop.PanelButtonData
            {
                Title = "Panel button text"
            });
            ResetSkinTextures();
        }

        private readonly int alSelectedItem = 1;
        private List<Desktop.AppLauncherItem> LauncherItems = null;
        private List<Desktop.PanelButtonData> PanelButtons = null;

        public void OnLoad()
        {
            if (_skin == null)
            {
                _skin = SkinEngine.LoadedSkin;
            }
            ResetPreviewData();
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            while(SkinTextures.Count > 0)
            {
                var key = SkinTextures.First().Key;
                SkinTextures[key].Dispose();
                SkinTextures.Remove(key);
            }
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void OpenFile(string file)
        {
            _skin = JsonConvert.DeserializeObject<Skin>(Utils.ReadAllText(file));
            AppearanceManager.SetupWindow(this);
        }

        private int _bottomheight = 0;
        private int _bottomstart = 0;

        private int _previewxstart = 0;
        private int _previewxwidth = 0;
        private int _desktopystart = 0;
        private int _windowystart = 0;
        private int _previewheight = 0;

        protected override void OnLayout(GameTime gameTime)
        {
            base.OnLayout(gameTime);

            _close.Text = "Close";
            _load.Text = "Load";
            _save.Text = "Save";
            _apply.Text = "Apply";
            _default.Text = "Load default";

            _bottomheight = (_close.Height + 20);
            _bottomstart = Height - _bottomheight;
            _close.X = 10;
            _close.Y = _bottomstart + ((_bottomheight - _close.Height) / 2);
            _default.X = _close.X + _close.Width + 10;
            _default.Y = _bottomstart + ((_bottomheight - _close.Height) / 2);
            _load.X = _default.X + _default.Width + 10;
            _load.Y = _bottomstart + ((_bottomheight - _close.Height) / 2);
            _save.X = _load.X + _load.Width + 10;
            _save.Y = _bottomstart + ((_bottomheight - _close.Height) / 2);
            _apply.X = Width - _apply.Width - 10;
            _apply.Y = _bottomstart + ((_bottomheight - _close.Height) / 2);

            _previewxstart = 10;
            _previewxwidth = Width - 20;
            _previewheight = ((Height - _bottomheight) / 2) - 20;
            _desktopystart = 10;
            _windowystart = _desktopystart + _previewheight + 10;
            Invalidate();
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            /*Paint the desktop rect*/

            //First we gotta paint the desktop bg.
            //I'll steal the code from UIManager.DrawBackground()

            gfx.DrawRectangle(_previewxstart, _desktopystart, _previewxwidth, _previewheight, SkinTextures["DesktopColor"]);
            if (SkinTextures.ContainsKey("desktopbackground"))
            {
                gfx.DrawRectangle(_previewxstart, _desktopystart, _previewxwidth, _previewheight, SkinTextures["desktopbackground"]);
            }
            //Now we actually paint the ui
            PaintDesktop(gfx);

            //Paint the window rect
            gfx.DrawRectangle(_previewxstart, _windowystart, _previewxwidth, _previewheight, Color.Red);

        }
    }
}
