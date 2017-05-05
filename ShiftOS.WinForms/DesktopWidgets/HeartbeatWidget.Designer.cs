namespace ShiftOS.WinForms.DesktopWidgets
{
    partial class HeartbeatWidget
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
            this.lbheartbeat = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbheartbeat
            // 
            this.lbheartbeat.AutoSize = true;
            this.lbheartbeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbheartbeat.Location = new System.Drawing.Point(0, 0);
            this.lbheartbeat.Name = "lbheartbeat";
            this.lbheartbeat.Size = new System.Drawing.Size(35, 13);
            this.lbheartbeat.TabIndex = 0;
            this.lbheartbeat.Text = "label1";
            // 
            // HeartbeatWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.lbheartbeat);
            this.Name = "HeartbeatWidget";
            this.Size = new System.Drawing.Size(35, 13);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbheartbeat;
    }
}
