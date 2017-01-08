using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using System.Threading;

namespace ShiftOS.WinForms.Applications
{
    public partial class CoherenceOverlay : UserControl, IShiftOSWindow
    {
        public CoherenceOverlay(IntPtr handle, CoherenceCommands.RECT rect)
        {
            InitializeComponent();
            this.Load += (o, a) =>
            {
                try
                {
                    int left = this.ParentForm.Left;
                    int top = this.ParentForm.Top;
                    int oldwidth = this.ParentForm.Width;
                    int oldheight = this.ParentForm.Height;

                    var t = new Thread(new ThreadStart(() =>
                    {
                        while (CoherenceCommands.GetWindowRect(handle, ref rect))
                        {

                            if (left != rect.Left - SkinEngine.LoadedSkin.LeftBorderWidth)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    this.ParentForm.Left = rect.Left - SkinEngine.LoadedSkin.LeftBorderWidth;
                                    left = rect.Left - SkinEngine.LoadedSkin.LeftBorderWidth;
                                }));
                            }
                            if (top != rect.Top - SkinEngine.LoadedSkin.TitlebarHeight)
                            {
                                this.Invoke(new Action(() =>
                                {

                                    this.ParentForm.Top = rect.Top - SkinEngine.LoadedSkin.TitlebarHeight;
                                    top = rect.Top - SkinEngine.LoadedSkin.TitlebarHeight;
                                }));
                            }
                            int width = (rect.Right - rect.Left) + 1;
                            int height = (rect.Bottom - rect.Top) + 1;

                            if (oldheight != SkinEngine.LoadedSkin.TitlebarHeight + height + SkinEngine.LoadedSkin.BottomBorderWidth)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    this.ParentForm.Height = SkinEngine.LoadedSkin.TitlebarHeight + height + SkinEngine.LoadedSkin.BottomBorderWidth;
                                    oldheight = SkinEngine.LoadedSkin.TitlebarHeight + height + SkinEngine.LoadedSkin.BottomBorderWidth;
                                }));
                            }
                            if (oldwidth != SkinEngine.LoadedSkin.LeftBorderWidth + width + SkinEngine.LoadedSkin.RightBorderWidth)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    this.ParentForm.Width = SkinEngine.LoadedSkin.LeftBorderWidth + width + SkinEngine.LoadedSkin.RightBorderWidth;
                                    oldwidth = SkinEngine.LoadedSkin.LeftBorderWidth + width + SkinEngine.LoadedSkin.RightBorderWidth;
                                }));
                            }
                        }
                    }));
                    t.IsBackground = true;
                    t.Start();
                }
                catch
                {

                }
            };
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
