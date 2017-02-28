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
        private int[,] snakemap = null; // 0 - Nothing, 1 - Body, 2 - Head, 3 - Tail
        private int snakedirection = 0; // 0 - Left, 1 - Down, 2 - Right, 3 - Up
        private Timer snakeupdater = new Timer();
        private bool extending = false;

        public Snakey()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            snakeupdater.Interval = 250;
            snakeupdater.Tick += updateSnake;
            makeGrid();
        }

        private void updateSnake(object sender, EventArgs e)
        {
            Control head = null;

            for (int x = 0; x < 10; x++)
            {
                if (head != null) break;
                for (int y = 0; y < 10; y++)
                {
                    if (snakemap[x, y] == 2)
                    {
                        head = tableLayoutPanel1.GetControlFromPosition(x, y);
                        break;
                    }
                }
            }

            if (head == null) return;

            int headX = int.Parse(head.Name.Split('b')[1]); // NRE was here
            int headY = int.Parse(head.Name.Split('b')[2]);

            int newHeadX = headX;
            int newHeadY = headY;

            Image headImg = null;
            switch (snakedirection)
            {
                case 0:
                    newHeadX = headX - 1;
                    headImg = Properties.Resources.SnakeyHeadL;
                    break;

                case 1:
                    newHeadY = headY + 1;
                    headImg = Properties.Resources.SnakeyHeadD;
                    break;

                case 2:
                    newHeadX = headX + 1;
                    headImg = Properties.Resources.SnakeyHeadR;
                    break;

                case 3:
                    newHeadY = headY - 1;
                    headImg = Properties.Resources.SnakeyHeadU;
                    break;

                default:
                    break;
            }

            if (newHeadX > 9 | newHeadX < 0 | newHeadY > 9 | newHeadY < 0) return;
            snakemap[newHeadX, newHeadY] = 2;
            tableLayoutPanel1.GetControlFromPosition(newHeadX, newHeadY).BackgroundImage = headImg;
            snakemap[headX, headY] = 1;
            tableLayoutPanel1.GetControlFromPosition(headX, headY).BackgroundImage = Properties.Resources.SnakeyBody;
            if (!extending)
            {

            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (snakemap != null)
            {
                clearGame();
                makeGrid();
            }
            snakemap = new int[10, 10];
            snakemap[5, 5] = 2;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(5, 5)).Image = Properties.Resources.SnakeyHeadL;
            for (int x = 6; x < 8; x++)
            {
                snakemap[x, 5] = 1;
                ((PictureBox)tableLayoutPanel1.GetControlFromPosition(x, 5)).Image = Properties.Resources.SnakeyBody;
            }
            snakemap[8, 5] = 3;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(8, 5)).Image = Properties.Resources.SnakeyTailL;
            snakeupdater.Start();
        }

        private void clearGame()
        {
            snakemap = null;
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    tableLayoutPanel1.Controls.Remove(tableLayoutPanel1.GetControlFromPosition(x, y));
                }
            }
        }

        private int getTailDirection()
        {
            PictureBox tail = null;

            for (int x = 0; x < 10; x++)
            {
                if (tail != null) break;
                for (int y = 0; y < 10; y++)
                {
                    if (snakemap[x, y] == 3)
                    {
                        tail = (PictureBox)tableLayoutPanel1.GetControlFromPosition(x, y);
                        break;
                    }
                }
            }
            if (tail.BackgroundImage == Properties.Resources.SnakeyTailL) return 0;
            if (tail.BackgroundImage == Properties.Resources.SnakeyTailD) return 1;
            if (tail.BackgroundImage == Properties.Resources.SnakeyTailR) return 2;
            if (tail.BackgroundImage == Properties.Resources.SnakeyTailU) return 3;

            return -1;
        }
    }
}
