namespace ShiftOS.WinForms.Applications
{
    partial class ExperienceShifter
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.desktopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appLauncherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnldesktop = new System.Windows.Forms.Panel();
            this.gpdesktopsettings = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlapplauncher = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbdesktops = new System.Windows.Forms.ListBox();
            this.gpalsettings = new System.Windows.Forms.GroupBox();
            this.lblaunchers = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.pnldesktop.SuspendLayout();
            this.pnlapplauncher.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.desktopToolStripMenuItem,
            this.appLauncherToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(582, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // desktopToolStripMenuItem
            // 
            this.desktopToolStripMenuItem.Name = "desktopToolStripMenuItem";
            this.desktopToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.desktopToolStripMenuItem.Text = "Desktop";
            this.desktopToolStripMenuItem.Click += new System.EventHandler(this.desktopToolStripMenuItem_Click);
            // 
            // appLauncherToolStripMenuItem
            // 
            this.appLauncherToolStripMenuItem.Name = "appLauncherToolStripMenuItem";
            this.appLauncherToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.appLauncherToolStripMenuItem.Text = "App Launcher";
            this.appLauncherToolStripMenuItem.Click += new System.EventHandler(this.appLauncherToolStripMenuItem_Click);
            // 
            // pnldesktop
            // 
            this.pnldesktop.Controls.Add(this.lbdesktops);
            this.pnldesktop.Controls.Add(this.gpdesktopsettings);
            this.pnldesktop.Controls.Add(this.label2);
            this.pnldesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnldesktop.Location = new System.Drawing.Point(0, 24);
            this.pnldesktop.Name = "pnldesktop";
            this.pnldesktop.Size = new System.Drawing.Size(582, 407);
            this.pnldesktop.TabIndex = 1;
            // 
            // gpdesktopsettings
            // 
            this.gpdesktopsettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gpdesktopsettings.Location = new System.Drawing.Point(0, 262);
            this.gpdesktopsettings.Name = "gpdesktopsettings";
            this.gpdesktopsettings.Size = new System.Drawing.Size(582, 145);
            this.gpdesktopsettings.TabIndex = 2;
            this.gpdesktopsettings.TabStop = false;
            this.gpdesktopsettings.Text = "Settings for this environment";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(5);
            this.label2.Size = new System.Drawing.Size(57, 23);
            this.label2.TabIndex = 1;
            this.label2.Tag = "header2";
            this.label2.Text = "Desktop";
            // 
            // pnlapplauncher
            // 
            this.pnlapplauncher.Controls.Add(this.lblaunchers);
            this.pnlapplauncher.Controls.Add(this.gpalsettings);
            this.pnlapplauncher.Controls.Add(this.label1);
            this.pnlapplauncher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlapplauncher.Location = new System.Drawing.Point(0, 24);
            this.pnlapplauncher.Name = "pnlapplauncher";
            this.pnlapplauncher.Size = new System.Drawing.Size(582, 407);
            this.pnlapplauncher.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(84, 23);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header2";
            this.label1.Text = "App Launcher";
            // 
            // lbdesktops
            // 
            this.lbdesktops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbdesktops.FormattingEnabled = true;
            this.lbdesktops.Location = new System.Drawing.Point(0, 23);
            this.lbdesktops.Name = "lbdesktops";
            this.lbdesktops.Size = new System.Drawing.Size(582, 239);
            this.lbdesktops.TabIndex = 0;
            // 
            // gpalsettings
            // 
            this.gpalsettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gpalsettings.Location = new System.Drawing.Point(0, 262);
            this.gpalsettings.Name = "gpalsettings";
            this.gpalsettings.Size = new System.Drawing.Size(582, 145);
            this.gpalsettings.TabIndex = 3;
            this.gpalsettings.TabStop = false;
            this.gpalsettings.Text = "Settings for this environment";
            // 
            // lblaunchers
            // 
            this.lblaunchers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblaunchers.FormattingEnabled = true;
            this.lblaunchers.Location = new System.Drawing.Point(0, 23);
            this.lblaunchers.Name = "lblaunchers";
            this.lblaunchers.Size = new System.Drawing.Size(582, 239);
            this.lblaunchers.TabIndex = 4;
            // 
            // ExperienceShifter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlapplauncher);
            this.Controls.Add(this.pnldesktop);
            this.Controls.Add(this.menuStrip1);
            this.Name = "ExperienceShifter";
            this.Size = new System.Drawing.Size(582, 431);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnldesktop.ResumeLayout(false);
            this.pnldesktop.PerformLayout();
            this.pnlapplauncher.ResumeLayout(false);
            this.pnlapplauncher.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem desktopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appLauncherToolStripMenuItem;
        private System.Windows.Forms.Panel pnldesktop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlapplauncher;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gpdesktopsettings;
        private System.Windows.Forms.ListBox lbdesktops;
        private System.Windows.Forms.ListBox lblaunchers;
        private System.Windows.Forms.GroupBox gpalsettings;
    }
}
