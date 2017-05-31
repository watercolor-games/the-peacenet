namespace ShiftOS.WinForms.StatusIcons
{
    partial class Volume
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
            this.lbvolume = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbvolume
            // 
            this.lbvolume.AutoSize = true;
            this.lbvolume.Location = new System.Drawing.Point(4, 4);
            this.lbvolume.Name = "lbvolume";
            this.lbvolume.Size = new System.Drawing.Size(81, 13);
            this.lbvolume.TabIndex = 0;
            this.lbvolume.Text = "System volume:";
            // 
            // Volume
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbvolume);
            this.Name = "Volume";
            this.Size = new System.Drawing.Size(444, 44);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbvolume;
    }
}
