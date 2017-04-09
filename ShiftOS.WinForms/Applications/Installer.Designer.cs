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
            this.pginstall = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.lbprogress = new System.Windows.Forms.Label();
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
            this.pnlselectfile.Location = new System.Drawing.Point(17, 48);
            this.pnlselectfile.Name = "pnlselectfile";
            this.pnlselectfile.Size = new System.Drawing.Size(414, 85);
            this.pnlselectfile.TabIndex = 1;
            this.pnlselectfile.VisibleChanged += new System.EventHandler(this.pnlselectfile_VisibleChanged);
            // 
            // pginstall
            // 
            this.pginstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pginstall.BlockSize = 5;
            this.pginstall.Location = new System.Drawing.Point(17, 161);
            this.pginstall.Maximum = 100;
            this.pginstall.Name = "pginstall";
            this.pginstall.Size = new System.Drawing.Size(414, 23);
            this.pginstall.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pginstall.TabIndex = 2;
            this.pginstall.Text = "shiftedProgressBar1";
            this.pginstall.Value = 0;
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbtitle;
        private System.Windows.Forms.Panel pnlselectfile;
        private Controls.ShiftedProgressBar pginstall;
        private System.Windows.Forms.Label lbprogress;
    }
}
