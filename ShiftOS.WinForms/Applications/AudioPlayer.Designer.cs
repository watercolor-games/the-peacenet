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
    partial class AudioPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioPlayer));
            this.wpaudio = new AxWMPLib.AxWindowsMediaPlayer();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.lbtracks = new System.Windows.Forms.ListBox();
            this.flcontrols = new System.Windows.Forms.FlowLayoutPanel();
            this.btnplay = new System.Windows.Forms.Button();
            this.pgplaytime = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addSongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shuffleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.wpaudio)).BeginInit();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.flcontrols.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wpaudio
            // 
            this.wpaudio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpaudio.Enabled = true;
            this.wpaudio.Location = new System.Drawing.Point(0, 0);
            this.wpaudio.Name = "wpaudio";
            this.wpaudio.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("wpaudio.OcxState")));
            this.wpaudio.Size = new System.Drawing.Size(798, 471);
            this.wpaudio.TabIndex = 0;
            this.wpaudio.Visible = false;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lbtracks);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.flcontrols);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(798, 447);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(798, 471);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // lbtracks
            // 
            this.lbtracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbtracks.FormattingEnabled = true;
            this.lbtracks.Location = new System.Drawing.Point(0, 0);
            this.lbtracks.Name = "lbtracks";
            this.lbtracks.Size = new System.Drawing.Size(798, 418);
            this.lbtracks.TabIndex = 1;
            this.lbtracks.SelectedIndexChanged += new System.EventHandler(this.lbtracks_SelectedIndexChanged);
            // 
            // flcontrols
            // 
            this.flcontrols.AutoSize = true;
            this.flcontrols.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flcontrols.Controls.Add(this.btnplay);
            this.flcontrols.Controls.Add(this.pgplaytime);
            this.flcontrols.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flcontrols.Location = new System.Drawing.Point(0, 418);
            this.flcontrols.Name = "flcontrols";
            this.flcontrols.Size = new System.Drawing.Size(798, 29);
            this.flcontrols.TabIndex = 0;
            // 
            // btnplay
            // 
            this.btnplay.AutoSize = true;
            this.btnplay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnplay.Location = new System.Drawing.Point(3, 3);
            this.btnplay.Name = "btnplay";
            this.btnplay.Size = new System.Drawing.Size(37, 23);
            this.btnplay.TabIndex = 0;
            this.btnplay.Text = "Play";
            this.btnplay.UseVisualStyleBackColor = true;
            this.btnplay.Click += new System.EventHandler(this.btnplay_Click);
            // 
            // pgplaytime
            // 
            this.pgplaytime.Location = new System.Drawing.Point(46, 3);
            this.pgplaytime.Maximum = 100;
            this.pgplaytime.Name = "pgplaytime";
            this.pgplaytime.Size = new System.Drawing.Size(749, 23);
            this.pgplaytime.TabIndex = 1;
            this.pgplaytime.Tag = "keepbg";
            this.pgplaytime.Text = "shiftedProgressBar1";
            this.pgplaytime.Value = 0;
            this.pgplaytime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.startScrub);
            this.pgplaytime.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pgplaytime_MouseMove);
            this.pgplaytime.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pgplaytime_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSongToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.shuffleToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.loopToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(798, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // addSongToolStripMenuItem
            // 
            this.addSongToolStripMenuItem.Name = "addSongToolStripMenuItem";
            this.addSongToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.addSongToolStripMenuItem.Text = "Add Song";
            this.addSongToolStripMenuItem.Click += new System.EventHandler(this.addSongToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // shuffleToolStripMenuItem
            // 
            this.shuffleToolStripMenuItem.Name = "shuffleToolStripMenuItem";
            this.shuffleToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.shuffleToolStripMenuItem.Text = "Shuffle";
            this.shuffleToolStripMenuItem.Click += new System.EventHandler(this.shuffleToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // loopToolStripMenuItem
            // 
            this.loopToolStripMenuItem.CheckOnClick = true;
            this.loopToolStripMenuItem.Name = "loopToolStripMenuItem";
            this.loopToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.loopToolStripMenuItem.Text = "Loop";
            // 
            // AudioPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.wpaudio);
            this.Name = "AudioPlayer";
            this.Size = new System.Drawing.Size(798, 471);
            ((System.ComponentModel.ISupportInitialize)(this.wpaudio)).EndInit();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.flcontrols.ResumeLayout(false);
            this.flcontrols.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer wpaudio;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addSongToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shuffleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ListBox lbtracks;
        private System.Windows.Forms.FlowLayoutPanel flcontrols;
        private System.Windows.Forms.Button btnplay;
        private Controls.ShiftedProgressBar pgplaytime;
        private System.Windows.Forms.ToolStripMenuItem loopToolStripMenuItem;
    }
}
