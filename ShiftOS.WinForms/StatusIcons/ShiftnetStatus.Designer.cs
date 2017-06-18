namespace ShiftOS.WinForms.StatusIcons
{
    partial class ShiftnetStatus
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
            this.lbserviceprovider = new System.Windows.Forms.Label();
            this.lbstatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(53, 23);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header2";
            this.label1.Text = "Shiftnet";
            // 
            // lbserviceprovider
            // 
            this.lbserviceprovider.AutoSize = true;
            this.lbserviceprovider.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbserviceprovider.Location = new System.Drawing.Point(0, 23);
            this.lbserviceprovider.Name = "lbserviceprovider";
            this.lbserviceprovider.Padding = new System.Windows.Forms.Padding(5);
            this.lbserviceprovider.Size = new System.Drawing.Size(98, 23);
            this.lbserviceprovider.TabIndex = 1;
            this.lbserviceprovider.Tag = "header3";
            this.lbserviceprovider.Text = "Freebie Solutions";
            // 
            // lbstatus
            // 
            this.lbstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbstatus.Location = new System.Drawing.Point(0, 46);
            this.lbstatus.Name = "lbstatus";
            this.lbstatus.Padding = new System.Windows.Forms.Padding(5);
            this.lbstatus.Size = new System.Drawing.Size(331, 144);
            this.lbstatus.TabIndex = 2;
            this.lbstatus.Text = "This will show the Shiftnet status.";
            // 
            // ShiftnetStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbstatus);
            this.Controls.Add(this.lbserviceprovider);
            this.Controls.Add(this.label1);
            this.Name = "ShiftnetStatus";
            this.Size = new System.Drawing.Size(331, 190);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbserviceprovider;
        private System.Windows.Forms.Label lbstatus;
    }
}
