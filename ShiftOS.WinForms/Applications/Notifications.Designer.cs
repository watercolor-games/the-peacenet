namespace ShiftOS.WinForms.Applications
{
    partial class Notifications
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
            this.lblnotifications = new System.Windows.Forms.Label();
            this.fllist = new System.Windows.Forms.FlowLayoutPanel();
            this.btnmarkallread = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblnotifications
            // 
            this.lblnotifications.AutoSize = true;
            this.lblnotifications.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblnotifications.Location = new System.Drawing.Point(0, 0);
            this.lblnotifications.Name = "lblnotifications";
            this.lblnotifications.Padding = new System.Windows.Forms.Padding(10);
            this.lblnotifications.Size = new System.Drawing.Size(85, 33);
            this.lblnotifications.TabIndex = 0;
            this.lblnotifications.Tag = "header1";
            this.lblnotifications.Text = "Notifications";
            // 
            // fllist
            // 
            this.fllist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fllist.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.fllist.Location = new System.Drawing.Point(0, 33);
            this.fllist.Name = "fllist";
            this.fllist.Size = new System.Drawing.Size(437, 487);
            this.fllist.TabIndex = 1;
            // 
            // btnmarkallread
            // 
            this.btnmarkallread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnmarkallread.AutoSize = true;
            this.btnmarkallread.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnmarkallread.Location = new System.Drawing.Point(356, 4);
            this.btnmarkallread.Name = "btnmarkallread";
            this.btnmarkallread.Size = new System.Drawing.Size(78, 23);
            this.btnmarkallread.TabIndex = 2;
            this.btnmarkallread.Text = "Mark all read";
            this.btnmarkallread.UseVisualStyleBackColor = true;
            this.btnmarkallread.Click += new System.EventHandler(this.btnmarkallread_Click);
            // 
            // Notifications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnmarkallread);
            this.Controls.Add(this.fllist);
            this.Controls.Add(this.lblnotifications);
            this.Name = "Notifications";
            this.Size = new System.Drawing.Size(437, 520);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblnotifications;
        private System.Windows.Forms.FlowLayoutPanel fllist;
        private System.Windows.Forms.Button btnmarkallread;
    }
}
