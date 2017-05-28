namespace ShiftOS.WinForms.Applications
{
    partial class TutorialBox
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
            this.lbltuttext = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbltuttext
            // 
            this.lbltuttext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbltuttext.Location = new System.Drawing.Point(0, 0);
            this.lbltuttext.Name = "lbltuttext";
            this.lbltuttext.Size = new System.Drawing.Size(401, 134);
            this.lbltuttext.TabIndex = 0;
            // 
            // TutorialBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbltuttext);
            this.Name = "TutorialBox";
            this.Size = new System.Drawing.Size(401, 134);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbltuttext;
    }
}
