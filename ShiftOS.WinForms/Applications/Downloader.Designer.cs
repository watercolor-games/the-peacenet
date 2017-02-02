namespace ShiftOS.WinForms.Applications
{
    partial class Downloader
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
            this.fllist = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // fllist
            // 
            this.fllist.AutoScroll = true;
            this.fllist.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fllist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fllist.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.fllist.Location = new System.Drawing.Point(0, 0);
            this.fllist.Name = "fllist";
            this.fllist.Size = new System.Drawing.Size(557, 149);
            this.fllist.TabIndex = 0;
            this.fllist.WrapContents = false;
            // 
            // Downloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fllist);
            this.Name = "Downloader";
            this.Size = new System.Drawing.Size(557, 149);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel fllist;
    }
}
