namespace ShiftOS.WinForms.ShiftnetSites
{
    partial class ShiftSoft_Ping
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fllist = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10);
            this.label1.Size = new System.Drawing.Size(48, 33);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header2";
            this.label1.Text = "Ping";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 33);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(10);
            this.label2.Size = new System.Drawing.Size(734, 56);
            this.label2.TabIndex = 1;
            this.label2.Text = "Ping is a simple service that lets you see all the sites on the shiftnet/ cluster" +
    ". These sites are safe for you to use and do not contain malware or other threat" +
    "s.";
            // 
            // fllist
            // 
            this.fllist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fllist.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.fllist.Location = new System.Drawing.Point(0, 89);
            this.fllist.Name = "fllist";
            this.fllist.Size = new System.Drawing.Size(734, 389);
            this.fllist.TabIndex = 2;
            // 
            // ShiftSoft_Ping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fllist);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ShiftSoft_Ping";
            this.Size = new System.Drawing.Size(734, 478);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel fllist;
    }
}
