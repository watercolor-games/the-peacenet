namespace ShiftOS.WinForms.ShiftnetSites
{
    partial class ShiftOSOnline
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShiftOSOnline));
            this.lbtitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnsubscribe = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbtitle
            // 
            this.lbtitle.AutoSize = true;
            this.lbtitle.Location = new System.Drawing.Point(242, 294);
            this.lbtitle.Name = "lbtitle";
            this.lbtitle.Size = new System.Drawing.Size(76, 13);
            this.lbtitle.TabIndex = 0;
            this.lbtitle.Tag = "header1";
            this.lbtitle.Text = "ShiftOS Online";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(42, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 137);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnsubscribe
            // 
            this.btnsubscribe.AutoSize = true;
            this.btnsubscribe.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnsubscribe.Location = new System.Drawing.Point(45, 401);
            this.btnsubscribe.Name = "btnsubscribe";
            this.btnsubscribe.Size = new System.Drawing.Size(96, 23);
            this.btnsubscribe.TabIndex = 2;
            this.btnsubscribe.Tag = "header3";
            this.btnsubscribe.Text = "Subscribe today!";
            this.btnsubscribe.UseVisualStyleBackColor = true;
            this.btnsubscribe.Click += new System.EventHandler(this.btnsubscribe_Click);
            // 
            // ShiftOSOnline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnsubscribe);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbtitle);
            this.Name = "ShiftOSOnline";
            this.Size = new System.Drawing.Size(562, 469);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbtitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnsubscribe;
    }
}
