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
    [Launcher("ShiftSweeper", true, "al_shiftsweeper", "Games")]
    [RequiresUpgrade("shiftsweeper")]
    [WinOpen("shiftsweeper")]
    [DefaultIcon("iconShiftSweeper")]
    public partial class ShiftSweeper : UserControl, IShiftOSWindow
    {
        private bool gameplayed = false;
        private bool flagtime = false;
        private int mineCount = 0;
        private int origminecount;
        private int[,] minemap; //Represents status of tiles. 0-8 = how many mines surrounding. -1 = mine. -2 = flagged mine. -3 to -11 = flagged safe.
        private Timer ticking = new Timer();
        private int minetimer;
        private TableLayoutPanel minefieldPanel;

        public ShiftSweeper() { InitializeComponent(); }

        public void OnLoad()
        {
            buttonE.Visible = true;
            buttonM.Visible = ShiftoriumFrontend.UpgradeInstalled("shiftsweeper_medium");
            buttonH.Visible = ShiftoriumFrontend.UpgradeInstalled("shiftsweeper_hard");
            ticking.Interval = 1000;
            ticking.Tick += Ticking_Tick;
            easyPanel.Visible = false;
            mediumPanel.Visible = false;
            hardPanel.Visible = false;
        }

        private void Ticking_Tick(object sender, EventArgs e)
        {
            minetimer++;
            lbltime.Text = "Time: " + minetimer.ToString();
        }

        public void OnSkinLoad() { }

        public bool OnUnload() { return true; }

        public void OnUpgrade() { }

        private void buttonE_Click(object sender, EventArgs e) { startGame(0); }

        private void clearPreviousGame()
        {
            if (minemap != null) for (int x = 0; x < minefieldPanel.ColumnCount; x++)
            {
                for (int y = 0; y < minefieldPanel.RowCount; y++)
                {
                    minemap[x, y] = 0;

                    if (minefieldPanel.GetControlFromPosition(x,y) != null)
                    {
                            minefieldPanel.Controls.Remove(minefieldPanel.GetControlFromPosition(x, y));
                    }
                }
            }

        }

        private void startGame(int d)
        {
            pictureBox1.Image = Properties.Resources.SweeperNormalFace;
            clearPreviousGame();
            lbltime.Text = "Time: 0";
            minetimer = 0;
            ticking.Start();
            if (minefieldPanel != null) minefieldPanel.Visible = false;
            switch (d)
            {
                case 0:
                    minefieldPanel = easyPanel;
                    mineCount = 10;
                    minefieldPanel.ColumnCount = 9;
                    minefieldPanel.RowCount = 9;
                    break;

                case 1:
                    minefieldPanel = mediumPanel;
                    mineCount = 40;
                    minefieldPanel.ColumnCount = 16;
                    minefieldPanel.RowCount = 16;
                    break;

                case 2:
                    minefieldPanel = hardPanel;
                    mineCount = 99;
                    minefieldPanel.ColumnCount = 30;
                    minefieldPanel.RowCount = 16;
                    break;

                default:
                    throw new NullReferenceException();
            }
            minefieldPanel.Visible = true;
            origminecount = mineCount;
            lblmines.Text = "Mines: " + mineCount.ToString();
            buttonE.Enabled = false;
            buttonM.Enabled = false;
            buttonH.Enabled = false;
            gameplayed = true;
            makegrid();
        }

        private void makegrid()
        {
            Random rnd1 = new Random();
            minemap = new int[minefieldPanel.ColumnCount, minefieldPanel.RowCount];

            // Makes the minefield full of buttons
            for (int x = 0; x < minefieldPanel.ColumnCount; x++)
            {
                for (int y = 0; y < minefieldPanel.RowCount; y++)
                {
                    minemap[x, y] = 0;
                    minefieldPanel.Controls.Add(makeButton(x, y), x, y);
                }
            }

            // Placing the mines
            int currminecount = mineCount;
            while (currminecount > 0)
            {
                int mineX = rnd1.Next(minefieldPanel.ColumnCount);
                int mineY = rnd1.Next(minefieldPanel.RowCount);

                if (minemap[mineX, mineY] == 0)
                {
                    minemap[mineX, mineY] = -1;
                    currminecount--;
                }
            }

            // Setting the numbers
            for (int x = 0; x < minefieldPanel.ColumnCount; x++)
            {
                for (int y = 0; y < minefieldPanel.RowCount; y++)
                {
                    if (minemap[x, y] != -1)
                    {
                        int numMines = 0;
                        for (int xx = -1; xx < 2; xx++)
                        {
                            for (int yy = -1; yy < 2; yy++)
                            {
                                if (x + xx >= 0 && y + yy >= 0 && x + xx < minefieldPanel.ColumnCount && y + yy < minefieldPanel.RowCount)
                                {
                                    if (minemap[x + xx, y + yy] == -1) numMines++;
                                }
                            }
                        }
                        minemap[x, y] = numMines;
                    }
                }
            }
        }

        private Button makeButton(int col, int row)
        {
            Button bttn = new Button();

            bttn.Text = "";
            bttn.Name = col.ToString() + " " + row.ToString();
            Controls.AddRange(new System.Windows.Forms.Control[] { bttn, });
            bttn.Size = new System.Drawing.Size(minefieldPanel.Width / minefieldPanel.ColumnCount, (minefieldPanel.Height / minefieldPanel.RowCount) + 10);
            bttn.Click += new System.EventHandler(bttnOnclick);
            bttn.BackgroundImage = Properties.Resources.SweeperTileBlock;
            bttn.BackgroundImageLayout = ImageLayout.Stretch;

            return bttn;
        }

        private void bttnOnclick(object sender, EventArgs e)
        {
            if (!ticking.Enabled) return;

            Button bttnClick = sender as Button;

            if (bttnClick == null) return; //not a button.

            string[] split = bttnClick.Name.Split(new Char[] { ' ' });

            int x = System.Convert.ToInt32(split[0]);
            int y = System.Convert.ToInt32(split[1]);


            if (!flagtime)
            {
                if (minemap[x, y] == -1)
                {
                    ticking.Enabled = false;

                    buttonE.Enabled = true;
                    buttonM.Enabled = true;
                    buttonH.Enabled = true;

                    pictureBox1.BackgroundImage = Properties.Resources.SweeperLoseFace;

                    for (int xx = 0; xx < minefieldPanel.ColumnCount; xx++)
                    {
                        for (int yy = 0; yy < minefieldPanel.RowCount; yy++)
                        {
                            pictureBox1.BackgroundImage = Properties.Resources.SweeperLoseFace;
                            minefieldPanel.GetControlFromPosition(xx, yy).Enabled = false;
                            if (minemap[xx, yy] == -1)
                            {
                                minefieldPanel.GetControlFromPosition(xx, yy).BackgroundImage = Properties.Resources.SweeperTileBomb;
                            }

                        }
                    }
                    pictureBox1.Image = Properties.Resources.SweeperLoseFace;
                }
                else if (minemap[x, y] < -1) return;
                else removeBlank(x, y);
            }
            else
            {
                if (!bttnClick.Enabled) return;

                if (minemap[x, y] < -1)
                {
                    minemap[x, y] = (minemap[x, y] * -1) - 3;
                    bttnClick.BackgroundImage = Properties.Resources.SweeperTileBlock;
                    mineCount++;
                }
                else
                {
                    minemap[x, y] = (minemap[x, y] * -1) - 3;
                    bttnClick.BackgroundImage = Properties.Resources.SweeperTileFlag;
                    mineCount--;
                }
                lblmines.Text = "Mines: " + mineCount.ToString();
                bool wrongflags = false;
                if (mineCount == 0)
                {
                    for (int xx = 0; xx < minefieldPanel.ColumnCount; xx++)
                    {
                        if (wrongflags) break;
                        for (int yy = 0; yy < minefieldPanel.RowCount; yy++)
                        {
                            if (wrongflags) break;
                            if (minemap[xx, yy] < -2) wrongflags = true;
                        }
                    }
                    if (!wrongflags)
                    {
                        ticking.Enabled = false;

                        buttonE.Enabled = true;
                        buttonM.Enabled = true;
                        buttonH.Enabled = true;

                        for (int xx = 0; xx < minefieldPanel.ColumnCount; xx++)
                        {
                            for (int yy = 0; yy < minefieldPanel.RowCount; yy++)
                            {
                                minefieldPanel.GetControlFromPosition(xx, yy).Enabled = false;
                            }
                        }

                        Int32 cp = 0;
                        origminecount = origminecount * 10;
                        if (minetimer < 31) cp = (origminecount * 3);
                        if (minetimer < 61) cp = (Int32)(origminecount * 2.5);
                        if (minetimer < 91) cp = (origminecount * 2);
                        if (minetimer < 121) cp = (Int32)(origminecount * 1.5);
                        if (minetimer > 120) cp = (origminecount * 1);
                        SaveSystem.TransferCodepointsFrom("shiftsweeper", cp);
                        pictureBox1.Image = Properties.Resources.SweeperWinFace;
                    }
                }
            }
        }

        private void removeBlank(int x, int y)
        {
            minefieldPanel.GetControlFromPosition(x, y).Enabled = false;
            trueform(x, y);
            if (minemap[x, y] != 0) return;
            for (int xx = -1; xx < 2; xx++)
            {
                for (int yy = -1; yy < 2; yy++)
                {
                    if (x + xx >= 0 && y + yy >= 0 && x + xx < minefieldPanel.ColumnCount && y + yy < minefieldPanel.RowCount)
                    {
                        if (minefieldPanel.GetControlFromPosition(x + xx, y + yy).Enabled && minemap[x+xx,y+yy] != -1 && minemap[x + xx, y + yy] != -2)
                        {
                            minefieldPanel.GetControlFromPosition(x + xx, y + yy).Enabled = false;
                            trueform(x + xx, y + yy);
                            if (minemap[x + xx, y + yy] == 0)
                            {
                                removeBlank(x + xx, y + yy);
                            }
                        }
                    }
                }
            }
        }

        private void trueform(int x, int y)
        {
            Button bttn = (Button)minefieldPanel.GetControlFromPosition(x, y);
            if (minemap[x,y] == 0) bttn.BackgroundImage = Properties.Resources.SweeperTile0;
            else if (minemap[x, y] == 1) bttn.BackgroundImage = Properties.Resources.SweeperTile1;
            else if (minemap[x, y] == 2) bttn.BackgroundImage = Properties.Resources.SweeperTile2;
            else if (minemap[x, y] == 3) bttn.BackgroundImage = Properties.Resources.SweeperTile3;
            else if (minemap[x, y] == 4) bttn.BackgroundImage = Properties.Resources.SweeperTile4;
            else if (minemap[x, y] == 5) bttn.BackgroundImage = Properties.Resources.SweeperTile5;
            else if (minemap[x, y] == 6) bttn.BackgroundImage = Properties.Resources.SweeperTile6;
            else if (minemap[x, y] == 7) bttn.BackgroundImage = Properties.Resources.SweeperTile7;
            else if (minemap[x, y] == 8) bttn.BackgroundImage = Properties.Resources.SweeperTile8;
        }

        private void buttonM_Click(object sender, EventArgs e) { startGame(1); }

        private void buttonH_Click(object sender, EventArgs e) { startGame(2); }

        private void flagButton_Click(object sender, EventArgs e)
        {
            if (flagtime)
            {
                flagButton.Image = Properties.Resources.SweeperTileBlock;
                flagtime = false;
            }
            else
            {
                flagButton.Image = Properties.Resources.SweeperTileFlag;
                flagtime = true;
            }
        }
    }
}
