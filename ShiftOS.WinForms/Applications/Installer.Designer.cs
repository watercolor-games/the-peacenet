namespace ShiftOS.WinForms.Applications
{
    partial class Installer
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
            this.lbtitle = new System.Windows.Forms.Label();
            this.pnlselectfile = new System.Windows.Forms.Panel();
            this.btnstart = new System.Windows.Forms.Button();
            this.btnbrowse = new System.Windows.Forms.Button();
            this.txtfilepath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbprogress = new System.Windows.Forms.Label();
            this.pginstall = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.pnlselectfile.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbtitle
            // 
            this.lbtitle.AutoSize = true;
            this.lbtitle.Location = new System.Drawing.Point(14, 13);
            this.lbtitle.Name = "lbtitle";
            this.lbtitle.Size = new System.Drawing.Size(155, 13);
            this.lbtitle.TabIndex = 0;
            this.lbtitle.Tag = "header2";
            this.lbtitle.Text = "Installing MUD Control Centre...";
            // 
            // pnlselectfile
            // 
            this.pnlselectfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlselectfile.Controls.Add(this.btnstart);
            this.pnlselectfile.Controls.Add(this.btnbrowse);
            this.pnlselectfile.Controls.Add(this.txtfilepath);
            this.pnlselectfile.Controls.Add(this.label1);
            this.pnlselectfile.Location = new System.Drawing.Point(17, 48);
            this.pnlselectfile.Name = "pnlselectfile";
            this.pnlselectfile.Size = new System.Drawing.Size(414, 85);
            this.pnlselectfile.TabIndex = 1;
            this.pnlselectfile.VisibleChanged += new System.EventHandler(this.pnlselectfile_VisibleChanged);
            // 
            // btnstart
            // 
            this.btnstart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnstart.Location = new System.Drawing.Point(255, 59);
            this.btnstart.Name = "btnstart";
            this.btnstart.Size = new System.Drawing.Size(75, 23);
            this.btnstart.TabIndex = 3;
            this.btnstart.Text = "Start";
            this.btnstart.UseVisualStyleBackColor = true;
            this.btnstart.Click += new System.EventHandler(this.btnstart_Click);
            // 
            // btnbrowse
            // 
            this.btnbrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnbrowse.Location = new System.Drawing.Point(336, 60);
            this.btnbrowse.Name = "btnbrowse";
            this.btnbrowse.Size = new System.Drawing.Size(75, 23);
            this.btnbrowse.TabIndex = 2;
            this.btnbrowse.Text = "Browse...";
            this.btnbrowse.UseVisualStyleBackColor = true;
            this.btnbrowse.Click += new System.EventHandler(this.btnbrowse_Click);
            // 
            // txtfilepath
            // 
            this.txtfilepath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtfilepath.Location = new System.Drawing.Point(16, 34);
            this.txtfilepath.Name = "txtfilepath";
            this.txtfilepath.Size = new System.Drawing.Size(395, 20);
            this.txtfilepath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a .stp file to install.";
            // 
            // lbprogress
            // 
            this.lbprogress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbprogress.AutoSize = true;
            this.lbprogress.Location = new System.Drawing.Point(17, 140);
            this.lbprogress.Name = "lbprogress";
            this.lbprogress.Size = new System.Drawing.Size(35, 13);
            this.lbprogress.TabIndex = 3;
            this.lbprogress.Text = "label1";
            // 
            // pginstall
            // 
            this.pginstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pginstall.Location = new System.Drawing.Point(17, 161);
            this.pginstall.Maximum = 100;
            this.pginstall.Name = "pginstall";
            this.pginstall.Size = new System.Drawing.Size(414, 23);
            this.pginstall.TabIndex = 2;
            this.pginstall.Text = "shiftedProgressBar1";
            this.pginstall.Value = 0;
            // 
            // Installer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbprogress);
            this.Controls.Add(this.pginstall);
            this.Controls.Add(this.pnlselectfile);
            this.Controls.Add(this.lbtitle);
            this.Name = "Installer";
            this.Size = new System.Drawing.Size(447, 203);
            this.pnlselectfile.ResumeLayout(false);
            this.pnlselectfile.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbtitle;
        private System.Windows.Forms.Panel pnlselectfile;
        private Controls.ShiftedProgressBar pginstall;
        private System.Windows.Forms.Label lbprogress;
        private System.Windows.Forms.Button btnstart;
        private System.Windows.Forms.Button btnbrowse;
        private System.Windows.Forms.TextBox txtfilepath;
        private System.Windows.Forms.Label label1;
    }
}
