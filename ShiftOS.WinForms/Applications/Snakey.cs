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

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Snakey", true, "al_snakey", "Games")]
    [RequiresUpgrade("snakey")]
    [WinOpen("snakey")]
    [DefaultIcon("iconSnakey")]
    public partial class Snakey : UserControl, IShiftOSWindow
    {
        private int[,] snakemap;
        private int snakedirection = 0; // 0 - Left, 1 - Down, 2 - Right, 3 - Up

        public Snakey()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            makeGrid();
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Up:
                    snakedirection = 3;
                    break;

                case (char)Keys.Down:
                    snakedirection = 1;
                    break;

                case (char)Keys.Left:
                    snakedirection = 0;
                    break;

                case (char)Keys.Right:
                    snakedirection = 2;
                    break;

                default:
                    break;
            }
        }

        private void makeGrid()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    tableLayoutPanel1.Controls.Add(newPicBox(x, y), x, y);
                }
            }
        }

        private PictureBox newPicBox(int x, int y)
        {
            PictureBox picBox = new PictureBox();

            picBox.Size = new System.Drawing.Size(20, 20);
            picBox.Image = Properties.Resources.SnakeyBG;
            picBox.Name = "pb" + x.ToString() + "b" + y.ToString();

            return picBox;
        }

        public void OnSkinLoad() { }

        public bool OnUnload() { return true; }

        public void OnUpgrade() { }
    }
}
