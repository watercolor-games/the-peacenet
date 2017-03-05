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

namespace ShiftOS.WinForms.Applications
{
    partial class ShiftSweeper
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelGameStatus = new System.Windows.Forms.PictureBox();
            this.gamePanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonEasy = new System.Windows.Forms.Button();
            this.buttonMedium = new System.Windows.Forms.Button();
            this.buttonHard = new System.Windows.Forms.Button();
            this.lblmines = new System.Windows.Forms.Label();
            this.lbltime = new System.Windows.Forms.Label();
            this.lblinfo = new System.Windows.Forms.Label();
            this.lblinfo2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.panelGameStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // panelGameStatus
            // 
            this.panelGameStatus.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.SweeperNormalFace;
            this.panelGameStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelGameStatus.Image = global::ShiftOS.WinForms.Properties.Resources.SweeperNormalFace;
            this.panelGameStatus.Location = new System.Drawing.Point(264, 3);
            this.panelGameStatus.Name = "panelGameStatus";
            this.panelGameStatus.Size = new System.Drawing.Size(32, 32);
            this.panelGameStatus.TabIndex = 0;
            this.panelGameStatus.TabStop = false;
            this.panelGameStatus.Click += new System.EventHandler(this.panelGameStatus_Click);
            // 
            // gamePanel
            // 
            this.gamePanel.AutoScroll = true;
            this.gamePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gamePanel.ColumnCount = 1;
            this.gamePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gamePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.gamePanel.Location = new System.Drawing.Point(4, 40);
            this.gamePanel.MaximumSize = new System.Drawing.Size(553, 308);
            this.gamePanel.MinimumSize = new System.Drawing.Size(553, 308);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.RowCount = 1;
            this.gamePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.gamePanel.Size = new System.Drawing.Size(553, 308);
            this.gamePanel.TabIndex = 1;
            this.gamePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gamePanel_Paint);
            // 
            // buttonEasy
            // 
            this.buttonEasy.Location = new System.Drawing.Point(4, 354);
            this.buttonEasy.Name = "buttonEasy";
            this.buttonEasy.Size = new System.Drawing.Size(75, 23);
            this.buttonEasy.TabIndex = 2;
            this.buttonEasy.Text = "Easy";
            this.buttonEasy.UseVisualStyleBackColor = true;
            this.buttonEasy.Click += new System.EventHandler(this.buttonEasy_Click);
            // 
            // buttonMedium
            // 
            this.buttonMedium.Location = new System.Drawing.Point(244, 354);
            this.buttonMedium.Name = "buttonMedium";
            this.buttonMedium.Size = new System.Drawing.Size(75, 23);
            this.buttonMedium.TabIndex = 3;
            this.buttonMedium.Text = "Medium";
            this.buttonMedium.UseVisualStyleBackColor = true;
            this.buttonMedium.Click += new System.EventHandler(this.buttonMedium_Click);
            // 
            // buttonHard
            // 
            this.buttonHard.Location = new System.Drawing.Point(482, 354);
            this.buttonHard.Name = "buttonHard";
            this.buttonHard.Size = new System.Drawing.Size(75, 23);
            this.buttonHard.TabIndex = 4;
            this.buttonHard.Text = "Hard";
            this.buttonHard.UseVisualStyleBackColor = true;
            this.buttonHard.Click += new System.EventHandler(this.buttonHard_Click);
            // 
            // lblmines
            // 
            this.lblmines.AutoSize = true;
            this.lblmines.Location = new System.Drawing.Point(317, 4);
            this.lblmines.Name = "lblmines";
            this.lblmines.Size = new System.Drawing.Size(47, 13);
            this.lblmines.TabIndex = 5;
            this.lblmines.Text = "Mines: 0";
            // 
            // lbltime
            // 
            this.lbltime.AutoSize = true;
            this.lbltime.Location = new System.Drawing.Point(317, 22);
            this.lbltime.Name = "lbltime";
            this.lbltime.Size = new System.Drawing.Size(42, 13);
            this.lbltime.TabIndex = 6;
            this.lbltime.Text = "Time: 0";
            // 
            // lblinfo
            // 
            this.lblinfo.AutoSize = true;
            this.lblinfo.Location = new System.Drawing.Point(4, 4);
            this.lblinfo.Name = "lblinfo";
            this.lblinfo.Size = new System.Drawing.Size(108, 13);
            this.lblinfo.TabIndex = 7;
            this.lblinfo.Text = "Click to uncover tiles.";
            // 
            // lblinfo2
            // 
            this.lblinfo2.AutoSize = true;
            this.lblinfo2.Location = new System.Drawing.Point(4, 22);
            this.lblinfo2.Name = "lblinfo2";
            this.lblinfo2.Size = new System.Drawing.Size(231, 13);
            this.lblinfo2.TabIndex = 8;
            this.lblinfo2.Text = "Right click to flag, middle click to question mark";
            // 
            // ShiftSweeper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblinfo2);
            this.Controls.Add(this.lblinfo);
            this.Controls.Add(this.lbltime);
            this.Controls.Add(this.lblmines);
            this.Controls.Add(this.buttonHard);
            this.Controls.Add(this.buttonMedium);
            this.Controls.Add(this.buttonEasy);
            this.Controls.Add(this.gamePanel);
            this.Controls.Add(this.panelGameStatus);
            this.Name = "ShiftSweeper";
            this.Size = new System.Drawing.Size(596, 426);
            ((System.ComponentModel.ISupportInitialize)(this.panelGameStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox panelGameStatus;
        private System.Windows.Forms.TableLayoutPanel gamePanel;
        private System.Windows.Forms.Button buttonEasy;
        private System.Windows.Forms.Button buttonMedium;
        private System.Windows.Forms.Button buttonHard;
        private System.Windows.Forms.Label lblmines;
        private System.Windows.Forms.Label lbltime;
        private System.Windows.Forms.Label lblinfo;
        private System.Windows.Forms.Label lblinfo2;
    }
}
