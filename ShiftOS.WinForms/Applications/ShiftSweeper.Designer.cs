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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.easyPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonE = new System.Windows.Forms.Button();
            this.buttonM = new System.Windows.Forms.Button();
            this.buttonH = new System.Windows.Forms.Button();
            this.lblmines = new System.Windows.Forms.Label();
            this.lbltime = new System.Windows.Forms.Label();
            this.lblinfo = new System.Windows.Forms.Label();
            this.lblinfo2 = new System.Windows.Forms.Label();
            this.flagButton = new System.Windows.Forms.PictureBox();
            this.mediumPanel = new System.Windows.Forms.TableLayoutPanel();
            this.hardPanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flagButton)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.SweeperNormalFace;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::ShiftOS.WinForms.Properties.Resources.SweeperNormalFace;
            this.pictureBox1.Location = new System.Drawing.Point(264, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // easyPanel
            // 
            this.easyPanel.ColumnCount = 9;
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.easyPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.easyPanel.Location = new System.Drawing.Point(4, 40);
            this.easyPanel.Name = "easyPanel";
            this.easyPanel.RowCount = 9;
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11136F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11136F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11136F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11136F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11136F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11136F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11136F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11025F));
            this.easyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11025F));
            this.easyPanel.Size = new System.Drawing.Size(553, 308);
            this.easyPanel.TabIndex = 1;
            // 
            // buttonE
            // 
            this.buttonE.Location = new System.Drawing.Point(4, 354);
            this.buttonE.Name = "buttonE";
            this.buttonE.Size = new System.Drawing.Size(75, 23);
            this.buttonE.TabIndex = 2;
            this.buttonE.Text = "Easy";
            this.buttonE.UseVisualStyleBackColor = true;
            this.buttonE.Click += new System.EventHandler(this.buttonE_Click);
            // 
            // buttonM
            // 
            this.buttonM.Location = new System.Drawing.Point(244, 354);
            this.buttonM.Name = "buttonM";
            this.buttonM.Size = new System.Drawing.Size(75, 23);
            this.buttonM.TabIndex = 3;
            this.buttonM.Text = "Medium";
            this.buttonM.UseVisualStyleBackColor = true;
            this.buttonM.Click += new System.EventHandler(this.buttonM_Click);
            // 
            // buttonH
            // 
            this.buttonH.Location = new System.Drawing.Point(482, 354);
            this.buttonH.Name = "buttonH";
            this.buttonH.Size = new System.Drawing.Size(75, 23);
            this.buttonH.TabIndex = 4;
            this.buttonH.Text = "Hard";
            this.buttonH.UseVisualStyleBackColor = true;
            this.buttonH.Click += new System.EventHandler(this.buttonH_Click);
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
            this.lblinfo2.Size = new System.Drawing.Size(233, 13);
            this.lblinfo2.TabIndex = 8;
            this.lblinfo2.Text = "Click the button on the right to toggle flag mode.";
            // 
            // flagButton
            // 
            this.flagButton.Image = global::ShiftOS.WinForms.Properties.Resources.SweeperTileBlock;
            this.flagButton.Location = new System.Drawing.Point(537, 14);
            this.flagButton.Name = "flagButton";
            this.flagButton.Size = new System.Drawing.Size(20, 20);
            this.flagButton.TabIndex = 9;
            this.flagButton.TabStop = false;
            this.flagButton.Click += new System.EventHandler(this.flagButton_Click);
            // 
            // mediumPanel
            // 
            this.mediumPanel.ColumnCount = 16;
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249042F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.249044F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.mediumPanel.Location = new System.Drawing.Point(4, 41);
            this.mediumPanel.Name = "mediumPanel";
            this.mediumPanel.RowCount = 16;
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249183F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.248558F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.248558F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.mediumPanel.Size = new System.Drawing.Size(553, 307);
            this.mediumPanel.TabIndex = 2;
            // 
            // hardPanel
            // 
            this.hardPanel.ColumnCount = 30;
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.448275F));
            this.hardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.hardPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.hardPanel.Location = new System.Drawing.Point(4, 40);
            this.hardPanel.Name = "hardPanel";
            this.hardPanel.RowCount = 16;
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249183F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249182F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.248558F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.248558F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.hardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.251231F));
            this.hardPanel.Size = new System.Drawing.Size(553, 308);
            this.hardPanel.TabIndex = 3;
            // 
            // ShiftSweeper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hardPanel);
            this.Controls.Add(this.mediumPanel);
            this.Controls.Add(this.flagButton);
            this.Controls.Add(this.lblinfo2);
            this.Controls.Add(this.lblinfo);
            this.Controls.Add(this.lbltime);
            this.Controls.Add(this.lblmines);
            this.Controls.Add(this.buttonH);
            this.Controls.Add(this.buttonM);
            this.Controls.Add(this.buttonE);
            this.Controls.Add(this.easyPanel);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ShiftSweeper";
            this.Size = new System.Drawing.Size(596, 426);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flagButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel easyPanel;
        private System.Windows.Forms.Button buttonE;
        private System.Windows.Forms.Button buttonM;
        private System.Windows.Forms.Button buttonH;
        private System.Windows.Forms.Label lblmines;
        private System.Windows.Forms.Label lbltime;
        private System.Windows.Forms.Label lblinfo;
        private System.Windows.Forms.Label lblinfo2;
        private System.Windows.Forms.PictureBox flagButton;
        private System.Windows.Forms.TableLayoutPanel mediumPanel;
        private System.Windows.Forms.TableLayoutPanel hardPanel;
    }
}
