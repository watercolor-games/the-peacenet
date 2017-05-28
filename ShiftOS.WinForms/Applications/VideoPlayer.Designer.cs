namespace ShiftOS.WinForms.Applications
{
    partial class VideoPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoPlayer));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.flcontrols = new System.Windows.Forms.FlowLayoutPanel();
            this.btnplay = new System.Windows.Forms.Button();
            this.pgplaytime = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addSongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wpaudio = new AxWMPLib.AxWindowsMediaPlayer();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.flcontrols.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wpaudio)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.wpaudio);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.flcontrols);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(805, 402);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(805, 426);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // flcontrols
            // 
            this.flcontrols.AutoSize = true;
            this.flcontrols.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flcontrols.Controls.Add(this.btnplay);
            this.flcontrols.Controls.Add(this.pgplaytime);
            this.flcontrols.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flcontrols.Location = new System.Drawing.Point(0, 373);
            this.flcontrols.Name = "flcontrols";
            this.flcontrols.Size = new System.Drawing.Size(805, 29);
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
            this.addSongToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(805, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // addSongToolStripMenuItem
            // 
            this.addSongToolStripMenuItem.Name = "addSongToolStripMenuItem";
            this.addSongToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.addSongToolStripMenuItem.Text = "Open Video";
            this.addSongToolStripMenuItem.Click += new System.EventHandler(this.addSongToolStripMenuItem_Click);
            // 
            // wpaudio
            // 
            this.wpaudio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpaudio.Enabled = true;
            this.wpaudio.Location = new System.Drawing.Point(0, 0);
            this.wpaudio.Name = "wpaudio";
            this.wpaudio.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("wpaudio.OcxState")));
            this.wpaudio.Size = new System.Drawing.Size(805, 373);
            this.wpaudio.TabIndex = 2;
            this.wpaudio.Visible = false;
            // 
            // VideoPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "VideoPlayer";
            this.Size = new System.Drawing.Size(805, 426);
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
            ((System.ComponentModel.ISupportInitialize)(this.wpaudio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.FlowLayoutPanel flcontrols;
        private System.Windows.Forms.Button btnplay;
        private Controls.ShiftedProgressBar pgplaytime;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addSongToolStripMenuItem;
        private AxWMPLib.AxWindowsMediaPlayer wpaudio;
    }
}
