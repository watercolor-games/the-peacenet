namespace ShiftOS.WinForms.Applications
{
    partial class About
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbshiftit = new System.Windows.Forms.Label();
            this.lbaboutdesc = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ShiftOS.WinForms.Properties.Resources.justthes;
            this.pictureBox1.Location = new System.Drawing.Point(14, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(105, 105);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(137, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Tag = "header1";
            this.label1.Text = "ShiftOS";
            // 
            // lbshiftit
            // 
            this.lbshiftit.AutoSize = true;
            this.lbshiftit.Location = new System.Drawing.Point(140, 73);
            this.lbshiftit.Name = "lbshiftit";
            this.lbshiftit.Size = new System.Drawing.Size(84, 13);
            this.lbshiftit.TabIndex = 2;
            this.lbshiftit.Tag = "header2";
            this.lbshiftit.Text = "Shift it your way.";
            // 
            // lbaboutdesc
            // 
            this.lbaboutdesc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbaboutdesc.Location = new System.Drawing.Point(14, 126);
            this.lbaboutdesc.Name = "lbaboutdesc";
            this.lbaboutdesc.Size = new System.Drawing.Size(498, 328);
            this.lbaboutdesc.TabIndex = 3;
            this.lbaboutdesc.Text = "label2";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbaboutdesc);
            this.Controls.Add(this.lbshiftit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "About";
            this.Size = new System.Drawing.Size(532, 474);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbshiftit;
        private System.Windows.Forms.Label lbaboutdesc;
    }
}
