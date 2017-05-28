namespace ShiftOS.WinForms.ShiftnetSites
{
    partial class DesktopWidgetWebsite
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesktopWidgetWebsite));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbltitle = new System.Windows.Forms.Label();
            this.lbwhatissuperdesk = new System.Windows.Forms.Label();
            this.lbthisissuperdesk = new System.Windows.Forms.Label();
            this.lbgetthepackage = new System.Windows.Forms.Label();
            this.lbpackagedesc = new System.Windows.Forms.Label();
            this.lnkdownload = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ShiftOS.WinForms.Properties.Resources.SuperDesk_screenshot;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(15, 96);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(813, 414);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.WaitOnLoad = true;
            // 
            // lbltitle
            // 
            this.lbltitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbltitle.Location = new System.Drawing.Point(0, 0);
            this.lbltitle.Name = "lbltitle";
            this.lbltitle.Size = new System.Drawing.Size(754, 81);
            this.lbltitle.TabIndex = 1;
            this.lbltitle.Tag = "header1";
            this.lbltitle.Text = "SuperDesk - Empower the ShiftOS Desktop!";
            this.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbwhatissuperdesk
            // 
            this.lbwhatissuperdesk.AutoSize = true;
            this.lbwhatissuperdesk.Location = new System.Drawing.Point(89, 536);
            this.lbwhatissuperdesk.Name = "lbwhatissuperdesk";
            this.lbwhatissuperdesk.Size = new System.Drawing.Size(105, 13);
            this.lbwhatissuperdesk.TabIndex = 2;
            this.lbwhatissuperdesk.Tag = "header2";
            this.lbwhatissuperdesk.Text = "What is SuperDesk?";
            // 
            // lbthisissuperdesk
            // 
            this.lbthisissuperdesk.Location = new System.Drawing.Point(47, 589);
            this.lbthisissuperdesk.Name = "lbthisissuperdesk";
            this.lbthisissuperdesk.Size = new System.Drawing.Size(681, 123);
            this.lbthisissuperdesk.TabIndex = 3;
            this.lbthisissuperdesk.Text = resources.GetString("lbthisissuperdesk.Text");
            this.lbthisissuperdesk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbgetthepackage
            // 
            this.lbgetthepackage.AutoSize = true;
            this.lbgetthepackage.Location = new System.Drawing.Point(319, 747);
            this.lbgetthepackage.Name = "lbgetthepackage";
            this.lbgetthepackage.Size = new System.Drawing.Size(87, 13);
            this.lbgetthepackage.TabIndex = 4;
            this.lbgetthepackage.Tag = "header2";
            this.lbgetthepackage.Text = "Get the package";
            // 
            // lbpackagedesc
            // 
            this.lbpackagedesc.Location = new System.Drawing.Point(37, 773);
            this.lbpackagedesc.Name = "lbpackagedesc";
            this.lbpackagedesc.Size = new System.Drawing.Size(681, 53);
            this.lbpackagedesc.TabIndex = 5;
            this.lbpackagedesc.Text = "SuperDesk is currently in an alpha stage, and we are offering the open alpha for " +
    "free. Simply download the .stp file and open it in your Installer to install the" +
    " SuperDesk framework.";
            this.lbpackagedesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkdownload
            // 
            this.lnkdownload.AutoSize = true;
            this.lnkdownload.Location = new System.Drawing.Point(192, 1055);
            this.lnkdownload.Name = "lnkdownload";
            this.lnkdownload.Size = new System.Drawing.Size(96, 13);
            this.lnkdownload.TabIndex = 6;
            this.lnkdownload.TabStop = true;
            this.lnkdownload.Tag = "header3";
            this.lnkdownload.Text = "Click to Download!";
            this.lnkdownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkdownload_LinkClicked);
            // 
            // DesktopWidgetWebsite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lnkdownload);
            this.Controls.Add(this.lbpackagedesc);
            this.Controls.Add(this.lbgetthepackage);
            this.Controls.Add(this.lbthisissuperdesk);
            this.Controls.Add(this.lbwhatissuperdesk);
            this.Controls.Add(this.lbltitle);
            this.Controls.Add(this.pictureBox1);
            this.Name = "DesktopWidgetWebsite";
            this.Size = new System.Drawing.Size(754, 1151);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbltitle;
        private System.Windows.Forms.Label lbwhatissuperdesk;
        private System.Windows.Forms.Label lbthisissuperdesk;
        private System.Windows.Forms.Label lbgetthepackage;
        private System.Windows.Forms.Label lbpackagedesc;
        private System.Windows.Forms.LinkLabel lnkdownload;
    }
}
