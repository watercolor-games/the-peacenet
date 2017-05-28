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
using System.Diagnostics;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Snakey", true, "al_snakey", "Games")]
    [RequiresUpgrade("snakey")]
    [WinOpen("snakey")]
    [DefaultIcon("iconSnakey")]
    public partial class Snakey : UserControl, IShiftOSWindow
    {
        private int[,] snakemap = null; // 0 - Nothing, 1 - Body, 2 - Head, 3 - Tail
        private int[,] snakepartlist = null;
        private int snakedirection = 0; // 0 - Left, 1 - Down, 2 - Right, 3 - Up
        private Timer snakeupdater = new Timer();
        private int snakelength = 0;
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
            PictureBox head = null;
            PictureBox tail = null;

            for (int x = 0; x < 10; x++)
            {
                if (head != null) break;
                for (int y = 0; y < 10; y++)
                {
                    if (head != null) break;
                    if (snakemap[x, y] == 2)
                    {
                        head = (PictureBox)tableLayoutPanel1.GetControlFromPosition(x, y);
                        break;
                    }
                }
            }

            for (int x = 0; x < 10; x++)
            {
                if (tail != null) break;
                for (int y = 0; y < 10; y++)
                {
                    if (tail != null) break;
                    if (snakemap[x, y] == 3)
                    {
                        tail = (PictureBox)tableLayoutPanel1.GetControlFromPosition(x, y);
                        break;
                    }
                }
            }

            int headX = int.Parse(head.Name.Split('b')[1]);
            int headY = int.Parse(head.Name.Split('b')[2]);
            int newHeadX = headX;
            int newHeadY = headY;

            int tailX = int.Parse(tail.Name.Split('b')[1]);
            int tailY = int.Parse(tail.Name.Split('b')[2]);
            int newTailX = tailX;
            int newTailY = tailY;

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

            if (newHeadX > 9 || newHeadX < 0 || newHeadY > 9 || newHeadY < 0) return;

            int newheadlocation = snakemap[newHeadX, newHeadY];
            snakemap[newHeadX, newHeadY] = 2;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(newHeadX, newHeadY)).Image = headImg;
            snakemap[headX, headY] = 1;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(headX, headY)).Image = Properties.Resources.SnakeyBody;
            if (!extending)
            {
                tail.Image = Properties.Resources.SnakeyBG; //mikey its here
                snakemap[tailX, tailY] = 0;
                tableLayoutPanel1.Refresh();
                snakepartlist[newHeadX, newHeadY] = snakelength;
                bool[,] exemptlist = new bool[10, 10];
                bool splassigned = false;
                for (int s = snakelength; s > 0; s--)
                {
                    splassigned = false;
                    for (int x = 0; x < 10; x++)
                    {
                        if (splassigned) break;
                        for (int y = 0; y < 10; y++)
                        {
                            if (splassigned) break;
                            if (exemptlist[x, y]) continue;
                            if (x == newHeadX && y == newHeadY) continue;
                            if (snakepartlist[x, y] == snakelength)
                            {
                                snakepartlist[x, y]--;
                                exemptlist[x, y] = true;
                                splassigned = true;
                                break;
                            }
                        }
                    }
                }
                Image tailImg = null;
                switch (tailDirection())
                {
                    case 0:
                        newTailX = tailX - 1;
                        break;

                    case 1:
                        newTailY = tailY + 1;
                        break;

                    case 2:
                        newTailX = tailX + 1;
                        break;

                    case 3:
                        newTailY = tailY - 1;
                        break;

                    default:
                        break;
                }
                switch (nextTailDirection(newTailX, newTailY))
                {
                    case 0:
                        tailImg = Properties.Resources.SnakeyTailL;
                        break;

                    case 1:
                        tailImg = Properties.Resources.SnakeyTailD;
                        break;

                    case 2:
                        tailImg = Properties.Resources.SnakeyTailR;
                        break;

                    case 3:
                        tailImg = Properties.Resources.SnakeyTailU;
                        break;

                    default:
                        break;
                }
                ((PictureBox)tableLayoutPanel1.GetControlFromPosition(newTailX, newTailY)).Image = tailImg;
                snakemap[newTailX, newTailY] = 3;
            }
            if (extending)
            {
                snakepartlist[newHeadX, newHeadY] = snakelength;
                extending = false;
            }
            if (newheadlocation == 1)
            {
                gameover();
            }
            if (newheadlocation == 4)
            {
                extending = true;
                snakelength++;
                placefruit();
            }
        }

        private int nextTailDirection(int x, int y)
        {
            if (snakepartlist[x - 1, y] == 2) return 0;
            if (snakepartlist[x, y + 1] == 2) return 1;
            if (snakepartlist[x + 1, y] == 2) return 2;
            if (snakepartlist[x, y - 1] == 2) return 3;
            return -1;
        }

        private void gameover()
        {
            throw new NotImplementedException();
        }

        private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    snakedirection = 3;
                    break;

                case Keys.Down:
                    snakedirection = 1;
                    break;

                case Keys.Left:
                    snakedirection = 0;
                    break;

                case Keys.Right:
                    snakedirection = 2;
                    break;

                default:
                    break;
            }
            Debug.Print("Snake Direction Value: " + snakedirection.ToString());
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
            picBox.Margin = new Padding(0);

            return picBox;
        }

        public void OnSkinLoad() { }

        public bool OnUnload()
        {
            snakeupdater.Stop();
            return true;
        }

        public void OnUpgrade() { }

        private void button1_Click(object sender, EventArgs e)
        {
            if (snakemap != null)
            {
                clearGame();
                makeGrid();
            }
            snakemap = new int[10, 10];
            snakepartlist = new int[10, 10];
            snakemap[5, 5] = 2;
            snakepartlist[5, 5] = 4;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(5, 5)).Image = Properties.Resources.SnakeyHeadL;
            snakemap[6, 5] = 1;
            snakepartlist[6, 5] = 3;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(6, 5)).Image = Properties.Resources.SnakeyBody;
            snakemap[7, 5] = 1;
            snakepartlist[7, 5] = 2;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(7, 5)).Image = Properties.Resources.SnakeyBody;
            snakemap[8, 5] = 3;
            snakepartlist[8, 5] = 1;
            ((PictureBox)tableLayoutPanel1.GetControlFromPosition(8, 5)).Image = Properties.Resources.SnakeyTailL;
            snakelength = 4;
            placefruit();
            snakeupdater.Start();
        }

        private void placefruit()
        {
            Random rnd1 = new Random();

            while (true)
            {
                int fruitX = rnd1.Next(10);
                int fruitY = rnd1.Next(10);

                if (snakemap[fruitX, fruitY] == 0)
                {
                    snakemap[fruitX, fruitY] = 4;
                    ((PictureBox)tableLayoutPanel1.GetControlFromPosition(fruitX, fruitY)).Image = Properties.Resources.SnakeyFruit;
                    break;
                }
            }
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

        private int tailDirection()
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
            if (tail.Image == Properties.Resources.SnakeyTailL) return 0;
            if (tail.Image == Properties.Resources.SnakeyTailD) return 1;
            if (tail.Image == Properties.Resources.SnakeyTailR) return 2;
            if (tail.Image == Properties.Resources.SnakeyTailU) return 3;

            return -1;
        }
    
    }
}
