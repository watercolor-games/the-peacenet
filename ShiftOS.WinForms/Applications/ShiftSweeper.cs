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

namespace ShiftOS.WinForms.Applications {
    [Launcher("{TITLE_SHIFTSWEEPER}", true, "al_shiftsweeper", "{AL_GAMES}")]
    [AppscapeEntry("shiftsweeper", "{TITLE_SHIFTSWEEPER}", "{DESC_SHIFTSWEEPER}", 1600, 800, "shiftletters", "{AL_GAMES}")]
    [MultiplayerOnly]
    [WinOpen("{WO_SHIFTSWEEPER}")]
    [DefaultIcon("iconShiftSweeper")]
    public partial class ShiftSweeper : UserControl, IShiftOSWindow {
        int[,] game;
        int[,] currentGame;
        bool gameCreated = false;
        int gameBombCount;
        int currentGameWidth;
        int currentGameHeight;

        private Timer ticking = new Timer();
        private int minetimer;

        private static readonly Random random = new Random();

        Size tileSize;

        Dictionary<int, string> buttonImages = new Dictionary<int, string>(){
            {-3, "?" },
            {-2, ">" },
            {-1, "#" },
            {0, " " },
            {1, "1" },
            {2, "2" },
            {3, "3" },
            {4, "4" },
            {5, "5" },
            {6, "6" },
            {7, "7" },
            {8, "8" },
            {9, "9" }
        };

        Dictionary<int, Bitmap> buttonImagess = new Dictionary<int, Bitmap>(){
            {-2, Properties.Resources.SweeperTileFlag }
        };

        // Properties.Resources.SweeperTileBomb

        const int QUESTIONED = -3;
        const int FLAGGED = -2;
        const int UNDISCOVERED = -1;
        const int REMOVE = -10;

        public ShiftSweeper() {
            InitializeComponent();
        }

        private void Ticking_Tick(object sender, EventArgs e) {
            minetimer++;
            lbltime.Text = "Time: " + minetimer.ToString();
            /*lbltime.Text = Localization.Parse("{SHIFTSWEEPER_TIME}", new Dictionary<string, string>{
                {"%time", minetimer.ToString()}
            });*/
        }

        public void OnLoad() {
            OnUpgrade();

            ticking.Interval = 1000;
            ticking.Tick += Ticking_Tick;
        }

        public void OnSkinLoad() {

        }

        public bool OnUnload() {
            return true;
        }

        public void OnUpgrade() {
            buttonEasy.Visible = true;
            buttonMedium.Visible = ShiftoriumFrontend.UpgradeInstalled("shiftsweeper_medium");
            buttonHard.Visible = ShiftoriumFrontend.UpgradeInstalled("shiftsweeper_hard");
        }

        public void startGame(int w, int h, int b) {
            panelGameStatus.Image = Properties.Resources.SweeperNormalFace;

            if (gamePanel.RowCount > w || gamePanel.ColumnCount > h) {
                for (int y = 0; y < gamePanel.ColumnCount; y++) {
                    for (int x = 0; x < gamePanel.RowCount; x++) {
                        updateTile(x, y, REMOVE);
                    }
                }
            }

            gamePanel.RowCount = w;
            gamePanel.ColumnCount = h;

            game = new int[w, h];
            currentGame = new int[w, h];

            tileSize = new Size(23, 23);

            gameCreated = false;

            currentGameWidth = w;
            currentGameHeight = h;
            gameBombCount = b;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    updateTile(x, y, UNDISCOVERED);
                }
            }
        }

        public void updateTile(int x, int y, int type) {
            Button tile = new Button();

            if (type == REMOVE) {
            } else {
                tile.Text = buttonImages.ContainsKey(type) ? buttonImages[type] : "";
                tile.BackgroundImage = buttonImagess.ContainsKey(type) ? buttonImagess[type] : tile.BackgroundImage;
                tile.MouseDown += new MouseEventHandler(tile_ClickDown);
                tile.MouseUp += new MouseEventHandler(tile_Click);
                tile.FlatStyle = FlatStyle.Flat;
                tile.FlatAppearance.BorderSize = (type == 0) ? 0 : 1;

                tile.Size = tileSize;

                currentGame[x, y] = type;
            }

            var controlToRemove = gamePanel.GetControlFromPosition(y, x);

            if (controlToRemove != null)
                gamePanel.Controls.Remove(controlToRemove);

            if (type != REMOVE)
                gamePanel.Controls.Add(tile, y, x);
        }

        private void tile_Click(object sender, EventArgs ee) {
            panelGameStatus.Image = Properties.Resources.SweeperNormalFace;

            MouseEventArgs e = (MouseEventArgs)ee;
            Control sen = (Control)sender;
            TableLayoutPanelCellPosition tilePos = gamePanel.GetPositionFromControl(sen);

            if (e.Button == MouseButtons.Left) {
                revealTile(tilePos.Row, tilePos.Column);
            } else if (e.Button == MouseButtons.Right) {
                toggleTileFlag(tilePos.Row, tilePos.Column);
            } else if (e.Button == MouseButtons.Middle) {
                toggleTileQuestionMark(tilePos.Row, tilePos.Column);
            }
        }

        private void tile_ClickDown(object sender, EventArgs e) {
            panelGameStatus.Image = Properties.Resources.SweeperClickFace;
        }

        private void createGameIfNotExists(int x, int y) {
            if (!gameCreated) {
                createGameIfNotExists();
                int i = 0;

                while (game[x, y] != 0 && i < 1000) {
                    i++;
                    createGameIfNotExists();
                }

                if(i > 999)
                    Infobox.Show("Error", "Failed to generate game");
                return;
            }
        }

        private void createGameIfNotExists() {
            if (!gameCreated) {
                for (int b = 0; b < gameBombCount; b++) { // place bombs
                    game[random.Next(0, currentGameWidth), random.Next(0, currentGameHeight)] = 9;
                }
                gameCreated = true;
                ticking.Start();
            }
        }

        private int getBombCountInArea(int x, int y) {
            createGameIfNotExists(x, y);

            if (game[x, y] == 9) return 9;

            int count = 0;
            int w = currentGameWidth - 1;
            int h = currentGameHeight - 1;

            if (x > 0) count += game[x - 1, y] == 9 ? 1 : 0;
            if (y > 0) count += game[x, y - 1] == 9 ? 1 : 0;
            if (x < w) count += game[x + 1, y] == 9 ? 1 : 0;
            if (y < h) count += game[x, y + 1] == 9 ? 1 : 0;

            if (x > 0 && y > 0) count += game[x - 1, y - 1] == 9 ? 1 : 0;
            if (x < w && y < h) count += game[x + 1, y + 1] == 9 ? 1 : 0;
            if (x < w && y > 0) count += game[x + 1, y - 1] == 9 ? 1 : 0;
            if (x > 0 && y < h) count += game[x - 1, y + 1] == 9 ? 1 : 0;

            return count;
        }

        private void revealTile(int x, int y) {
            createGameIfNotExists(x, y);

            int bombs = getBombCountInArea(x, y);
            int previousBombs = currentGame[x, y];

            updateTile(x, y, bombs);

            if (bombs == 0 && previousBombs == UNDISCOVERED) {
                int w = currentGameWidth - 1;
                int h = currentGameHeight - 1;

                if (x > 0) revealTile(x - 1, y);
                if (y > 0) revealTile(x, y - 1);
                if (x < w) revealTile(x + 1, y);
                if (y < h) revealTile(x, y + 1);

                if (x > 0 && y > 0) revealTile(x - 1, y - 1);
                if (x < w && y < h) revealTile(x + 1, y + 1);
                if (x < w && y > 0) revealTile(x + 1, y - 1);
                if (x > 0 && y < h) revealTile(x - 1, y + 1);
            }

            if (bombs != 0) checkWinGame();
            if (bombs == 9) loseGame();
        }

        private void toggleTileFlag(int x, int y) {
            if(currentGame[x,y] == UNDISCOVERED) {
                updateTile(x, y, FLAGGED);
            } else if(currentGame[x, y] == FLAGGED) {
                updateTile(x, y, UNDISCOVERED);
            }

            checkWinGame();
        }

        private void toggleTileQuestionMark(int x, int y) {
            if (currentGame[x, y] == UNDISCOVERED) {
                updateTile(x, y, QUESTIONED);
            } else if (currentGame[x, y] == QUESTIONED) {
                updateTile(x, y, UNDISCOVERED);
            }

            checkWinGame();
        }

        private void buttonEasy_Click(object sender, EventArgs e) {
            startGame(9, 9, 10);
        }

        private void buttonMedium_Click(object sender, EventArgs e) {
            startGame(16, 16, 40);
        }

        private void buttonHard_Click(object sender, EventArgs e) {
            startGame(30, 30, 99);
        }

        public void winGame() {
            ulong cp = 0;
            ulong origminecount = (ulong)(gameBombCount * 10);
            if (minetimer < 31) cp = (origminecount * 3);
            else if (minetimer < 61) cp = (ulong)(origminecount * 2.5);
            else if (minetimer < 91) cp = (origminecount * 2);
            else if (minetimer < 121) cp = (ulong)(origminecount * 1.5);
            else if (minetimer > 120) cp = (origminecount * 1);
            SaveSystem.TransferCodepointsFrom("shiftsweeper", cp);
            panelGameStatus.Image = Properties.Resources.SweeperWinFace;
            disableAllTiles(false);
        }

        public void loseGame() {
            disableAllTiles(true);
            panelGameStatus.Image = Properties.Resources.SweeperLoseFace;
        }

        public void disableAllTiles(bool showBombs) {
            ticking.Stop();
            minetimer = 0;
            for (int y = 0; y < currentGameHeight; y++) {
                for (int x = 0; x < currentGameWidth; x++) {
                    gamePanel.GetControlFromPosition(y, x).Visible = true;
                    gamePanel.GetControlFromPosition(y,x).Enabled = false;
                    if (game[x,y] == 9 && showBombs) {
                        gamePanel.GetControlFromPosition(y,x).BackgroundImage = Properties.Resources.SweeperTileBomb;
                    }

                }
            }
        }

        public void checkWinGame() {
            for(int y = 0; y < currentGameHeight; y++) {
                for(int x = 0; x < currentGameWidth; x++) {
                    int tile = currentGame[x, y];
                    if(tile == UNDISCOVERED || tile == QUESTIONED) {
                        return;
                    }
                    if(tile == FLAGGED) {
                        if(game[x,y] != 9) {
                            // looks like you flagged the wrong thing...
                            return;
                        }
                    }
                    if(tile == 9) {
                        loseGame();
                        return;
                    }

                }
            }
            winGame();
        }

        private void panelGameStatus_Click(object sender, EventArgs e) {

        }

        private void gamePanel_Paint(object sender, PaintEventArgs e) {

        }
    }
}
