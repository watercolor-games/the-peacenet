namespace ShiftOS.WinForms.Applications
{
    partial class UpdateManager
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
            this.lbupdatetitle = new System.Windows.Forms.Label();
            this.pnlupdatebar = new System.Windows.Forms.Panel();
            this.pgdownload = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.btnaction = new System.Windows.Forms.Button();
            this.btnclose = new System.Windows.Forms.Button();
            this.wbstatus = new System.Windows.Forms.WebBrowser();
            this.pnlupdatebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbupdatetitle
            // 
            this.lbupdatetitle.AutoSize = true;
            this.lbupdatetitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbupdatetitle.Location = new System.Drawing.Point(0, 0);
            this.lbupdatetitle.Margin = new System.Windows.Forms.Padding(10);
            this.lbupdatetitle.Name = "lbupdatetitle";
            this.lbupdatetitle.Size = new System.Drawing.Size(117, 13);
            this.lbupdatetitle.TabIndex = 0;
            this.lbupdatetitle.Tag = "header1";
            this.lbupdatetitle.Text = "Checking for updates...";
            // 
            // pnlupdatebar
            // 
            this.pnlupdatebar.Controls.Add(this.pgdownload);
            this.pnlupdatebar.Controls.Add(this.btnaction);
            this.pnlupdatebar.Controls.Add(this.btnclose);
            this.pnlupdatebar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlupdatebar.Location = new System.Drawing.Point(0, 426);
            this.pnlupdatebar.Name = "pnlupdatebar";
            this.pnlupdatebar.Size = new System.Drawing.Size(597, 33);
            this.pnlupdatebar.TabIndex = 1;
            // 
            // pgdownload
            // 
            this.pgdownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgdownload.Location = new System.Drawing.Point(86, 4);
            this.pgdownload.Maximum = 100;
            this.pgdownload.Name = "pgdownload";
            this.pgdownload.Size = new System.Drawing.Size(427, 23);
            this.pgdownload.TabIndex = 2;
            this.pgdownload.Text = "Updating...";
            this.pgdownload.Value = 0;
            // 
            // btnaction
            // 
            this.btnaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnaction.Location = new System.Drawing.Point(519, 4);
            this.btnaction.Name = "btnaction";
            this.btnaction.Size = new System.Drawing.Size(75, 23);
            this.btnaction.TabIndex = 1;
            this.btnaction.Text = "Update";
            this.btnaction.UseVisualStyleBackColor = true;
            // 
            // btnclose
            // 
            this.btnclose.Location = new System.Drawing.Point(4, 4);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(75, 23);
            this.btnclose.TabIndex = 0;
            this.btnclose.Text = "{CLOSE}";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // wbstatus
            // 
            this.wbstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbstatus.Location = new System.Drawing.Point(0, 13);
            this.wbstatus.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbstatus.Name = "wbstatus";
            this.wbstatus.Size = new System.Drawing.Size(597, 413);
            this.wbstatus.TabIndex = 2;
            // 
            // UpdateManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wbstatus);
            this.Controls.Add(this.pnlupdatebar);
            this.Controls.Add(this.lbupdatetitle);
            this.Name = "UpdateManager";
            this.Size = new System.Drawing.Size(597, 459);
            this.pnlupdatebar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbupdatetitle;
        private System.Windows.Forms.Panel pnlupdatebar;
        private Controls.ShiftedProgressBar pgdownload;
        private System.Windows.Forms.Button btnaction;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.WebBrowser wbstatus;
    }
}
