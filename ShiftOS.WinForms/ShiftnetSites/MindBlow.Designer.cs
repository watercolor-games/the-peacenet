namespace ShiftOS.WinForms.ShiftnetSites
{
    partial class MindBlow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MindBlow));
            this.nav = new System.Windows.Forms.Panel();
            this.title = new System.Windows.Forms.Label();
            this.aboutpnl = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.buybtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.aboutbtn = new System.Windows.Forms.LinkLabel();
            this.tutorialpnl = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.tutorialbtn = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nav.SuspendLayout();
            this.aboutpnl.SuspendLayout();
            this.tutorialpnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // nav
            // 
            this.nav.Controls.Add(this.tutorialbtn);
            this.nav.Controls.Add(this.aboutbtn);
            this.nav.Controls.Add(this.title);
            this.nav.Dock = System.Windows.Forms.DockStyle.Left;
            this.nav.Location = new System.Drawing.Point(0, 0);
            this.nav.Name = "nav";
            this.nav.Size = new System.Drawing.Size(200, 470);
            this.nav.TabIndex = 0;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(3, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(101, 24);
            this.title.TabIndex = 0;
            this.title.Text = "MindBlow";
            // 
            // aboutpnl
            // 
            this.aboutpnl.Controls.Add(this.pictureBox1);
            this.aboutpnl.Controls.Add(this.label2);
            this.aboutpnl.Controls.Add(this.buybtn);
            this.aboutpnl.Controls.Add(this.label1);
            this.aboutpnl.Location = new System.Drawing.Point(206, 0);
            this.aboutpnl.Name = "aboutpnl";
            this.aboutpnl.Size = new System.Drawing.Size(512, 470);
            this.aboutpnl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(313, 65);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buybtn
            // 
            this.buybtn.Location = new System.Drawing.Point(6, 76);
            this.buybtn.Name = "buybtn";
            this.buybtn.Size = new System.Drawing.Size(75, 23);
            this.buybtn.TabIndex = 1;
            this.buybtn.Text = "Buy Now";
            this.buybtn.UseVisualStyleBackColor = true;
            this.buybtn.Click += new System.EventHandler(this.buybtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "(Price: 50,000 Codepoints)";
            // 
            // aboutbtn
            // 
            this.aboutbtn.AutoSize = true;
            this.aboutbtn.Location = new System.Drawing.Point(4, 24);
            this.aboutbtn.Name = "aboutbtn";
            this.aboutbtn.Size = new System.Drawing.Size(85, 13);
            this.aboutbtn.TabIndex = 1;
            this.aboutbtn.TabStop = true;
            this.aboutbtn.Text = "About/Purchase";
            this.aboutbtn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aboutbtn_LinkClicked);
            // 
            // tutorialpnl
            // 
            this.tutorialpnl.Controls.Add(this.label3);
            this.tutorialpnl.Location = new System.Drawing.Point(209, 0);
            this.tutorialpnl.Name = "tutorialpnl";
            this.tutorialpnl.Size = new System.Drawing.Size(506, 467);
            this.tutorialpnl.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(375, 208);
            this.label3.TabIndex = 0;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // tutorialbtn
            // 
            this.tutorialbtn.AutoSize = true;
            this.tutorialbtn.Location = new System.Drawing.Point(4, 37);
            this.tutorialbtn.Name = "tutorialbtn";
            this.tutorialbtn.Size = new System.Drawing.Size(35, 13);
            this.tutorialbtn.TabIndex = 2;
            this.tutorialbtn.TabStop = true;
            this.tutorialbtn.Text = "Guide";
            this.tutorialbtn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.tutorialbtn_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ShiftOS.WinForms.Properties.Resources.mindblow;
            this.pictureBox1.Location = new System.Drawing.Point(6, 105);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(390, 295);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // MindBlow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aboutpnl);
            this.Controls.Add(this.tutorialpnl);
            this.Controls.Add(this.nav);
            this.Name = "MindBlow";
            this.Size = new System.Drawing.Size(718, 470);
            this.Resize += new System.EventHandler(this.MindBlow_Resize);
            this.nav.ResumeLayout(false);
            this.nav.PerformLayout();
            this.aboutpnl.ResumeLayout(false);
            this.aboutpnl.PerformLayout();
            this.tutorialpnl.ResumeLayout(false);
            this.tutorialpnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel nav;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Panel aboutpnl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buybtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel aboutbtn;
        private System.Windows.Forms.Panel tutorialpnl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel tutorialbtn;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
