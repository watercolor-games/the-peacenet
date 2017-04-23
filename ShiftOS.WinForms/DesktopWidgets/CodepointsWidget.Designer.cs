namespace ShiftOS.WinForms.DesktopWidgets
{
    partial class CodepointsWidget
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
            this.lbcodepoints = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbcodepoints
            // 
            this.lbcodepoints.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbcodepoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbcodepoints.Location = new System.Drawing.Point(0, 0);
            this.lbcodepoints.Name = "lbcodepoints";
            this.lbcodepoints.Size = new System.Drawing.Size(274, 57);
            this.lbcodepoints.TabIndex = 0;
            this.lbcodepoints.Tag = "header3";
            this.lbcodepoints.Text = "label1";
            this.lbcodepoints.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CodepointsWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbcodepoints);
            this.Name = "CodepointsWidget";
            this.Size = new System.Drawing.Size(274, 57);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbcodepoints;
    }
}
