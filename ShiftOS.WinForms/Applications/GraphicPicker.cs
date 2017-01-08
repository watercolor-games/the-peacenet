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
    public partial class GraphicPicker : UserControl, IShiftOSWindow
    {
        public GraphicPicker(Image old, string name, ImageLayout layout, Action<byte[], Image, ImageLayout> cb)
        {
            InitializeComponent();
            SelectedLayout = layout;
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
            AppearanceManager.SetupDialog(new FileDialog(new[] { ".png", ".jpg", ".bmp", ".pic" }, FileOpenerStyle.Open, new Action<string>((file) =>
            {
                ImageAsBinary = Utils.ReadAllBytes(file);
                System.IO.File.WriteAllBytes("temp_bin.bmp", ImageAsBinary);
                Image = SkinEngine.ImageFromBinary(ImageAsBinary);
                Image.Save("temp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
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
