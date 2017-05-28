namespace ShiftOS.WinForms.DesktopWidgets
{
    partial class UpgradePercentage
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
            this.pgupgrades = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.lbstatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pgupgrades
            // 
            this.pgupgrades.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pgupgrades.Location = new System.Drawing.Point(0, 99);
            this.pgupgrades.Maximum = 100;
            this.pgupgrades.Name = "pgupgrades";
            this.pgupgrades.Size = new System.Drawing.Size(227, 23);
            this.pgupgrades.TabIndex = 0;
            this.pgupgrades.Text = "shiftedProgressBar1";
            this.pgupgrades.Value = 0;
            // 
            // lbstatus
            // 
            this.lbstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbstatus.Location = new System.Drawing.Point(0, 0);
            this.lbstatus.Name = "lbstatus";
            this.lbstatus.Size = new System.Drawing.Size(227, 99);
            this.lbstatus.TabIndex = 1;
            this.lbstatus.Text = "label1";
            this.lbstatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpgradePercentage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbstatus);
            this.Controls.Add(this.pgupgrades);
            this.Name = "UpgradePercentage";
            this.Size = new System.Drawing.Size(227, 122);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ShiftedProgressBar pgupgrades;
        private System.Windows.Forms.Label lbstatus;
    }
}
