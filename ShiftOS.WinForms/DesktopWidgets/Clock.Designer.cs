namespace ShiftOS.WinForms.DesktopWidgets
{
    partial class Clock
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
            this.lbtime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbtime
            // 
            this.lbtime.BackColor = System.Drawing.Color.Transparent;
            this.lbtime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbtime.Location = new System.Drawing.Point(0, 0);
            this.lbtime.Name = "lbtime";
            this.lbtime.Size = new System.Drawing.Size(210, 49);
            this.lbtime.TabIndex = 0;
            this.lbtime.Tag = "header2 keepbg";
            this.lbtime.Text = "label1";
            this.lbtime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Clock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lbtime);
            this.Name = "Clock";
            this.Size = new System.Drawing.Size(210, 49);
            this.Tag = "keepbg";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbtime;
    }
}
