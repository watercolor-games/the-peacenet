/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [DefaultTitle("{TITLE_CHOOSEGRAPHIC}")] [DefaultIcon("icongraphicpicker")]
    public partial class GraphicPicker : UserControl, IShiftOSWindow
    {
        public GraphicPicker(Image old, string name, ImageLayout layout, Action<byte[], Image, ImageLayout> cb)
        {
            InitializeComponent();
            SelectedLayout = layout;
            Image = old;
            if (Image != null)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ImageAsBinary = ms.ToArray();
                }
            }
            Callback = cb;
            lblobjecttoskin.Text = name;
            
        }

        public Action<byte[], Image, ImageLayout> Callback;

        public ImageLayout SelectedLayout { get; private set; }

        public void btncancel_Click(object s, EventArgs a)
        {
            this.Close(); //don't invoke callback
        }

        public void btnreset_Click(object s, EventArgs a)
        {
            this.ImageAsBinary = null;
            this.Image = null;
            Setup();
        }

        public void btnapply_Click(object s, EventArgs a)
        {
            Callback?.Invoke(this.ImageAsBinary, this.Image, this.SelectedLayout);
            this.Close();
        }

        public byte[] ImageAsBinary { get; set; }
        public Image Image { get; set; }

        public void Setup()
        {
            picidle.BackgroundImage = Image;
            picidle.BackgroundImageLayout = SelectedLayout;
        }

        public void btnidlebrowse_Click(object s, EventArgs a)
        {
            AppearanceManager.SetupDialog(new FileDialog(new[] { ".png", ".gif", ".jpg", ".bmp", ".pic" }, FileOpenerStyle.Open, new Action<string>((file) =>
            {
                ImageAsBinary = Utils.ReadAllBytes(file);
                System.IO.File.WriteAllBytes("temp_bin.bmp", ImageAsBinary);
                Image = SkinEngine.ImageFromBinary(ImageAsBinary);
                Setup();
            })));
        }

        public void btnzoom_Click(object s, EventArgs a)
        {
            this.SelectedLayout = ImageLayout.Zoom;
            Setup();
        }

        public void btncentre_Click(object s, EventArgs a)
        {
            this.SelectedLayout = ImageLayout.Center;
            Setup();
        }

        public void btnstretch_Click(object s, EventArgs a)
        {
            this.SelectedLayout = ImageLayout.Stretch;
            Setup();
        }

        public void btntile_Click(object s, EventArgs a)
        {
            this.SelectedLayout = ImageLayout.Tile;
            Setup();
        }

        public void Graphic_Picker_Load(object s, EventArgs a)
        {
            Setup();
        }

        public void OnLoad()
        {
            Setup();
        }

        public void OnSkinLoad()
        {
            Setup();
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            Setup();
        }
    }
}
