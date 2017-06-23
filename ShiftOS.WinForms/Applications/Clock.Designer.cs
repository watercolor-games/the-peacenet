namespace ShiftOS.WinForms.Applications
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
            this.lbheader = new System.Windows.Forms.Label();
            this.lbcurrenttime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbheader
            // 
            this.lbheader.AutoSize = true;
            this.lbheader.Location = new System.Drawing.Point(234, 183);
            this.lbheader.Name = "lbheader";
            this.lbheader.Size = new System.Drawing.Size(66, 13);
            this.lbheader.TabIndex = 0;
            this.lbheader.Tag = "header2";
            this.lbheader.Text = "Current time:";
            // 
            // lbcurrenttime
            // 
            this.lbcurrenttime.AutoSize = true;
            this.lbcurrenttime.Location = new System.Drawing.Point(294, 140);
            this.lbcurrenttime.Name = "lbcurrenttime";
            this.lbcurrenttime.Size = new System.Drawing.Size(135, 13);
            this.lbcurrenttime.TabIndex = 1;
            this.lbcurrenttime.Tag = "header1";
            this.lbcurrenttime.Text = "000001 seconds since helll";
            // 
            // Clock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbcurrenttime);
            this.Controls.Add(this.lbheader);
            this.Name = "Clock";
            this.Size = new System.Drawing.Size(527, 260);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbheader;
        private System.Windows.Forms.Label lbcurrenttime;
    }
}
