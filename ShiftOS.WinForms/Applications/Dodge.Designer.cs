namespace ShiftOS.WinForms.Applications
{
    partial class Dodge
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dodge));
            this.pgcontents = new System.Windows.Forms.Panel();
            this.player = new System.Windows.Forms.PictureBox();
            this.QuitButton = new System.Windows.Forms.PictureBox();
            this.BeginButton = new System.Windows.Forms.PictureBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.object_small2 = new System.Windows.Forms.PictureBox();
            this.object_mid2 = new System.Windows.Forms.PictureBox();
            this.object_large = new System.Windows.Forms.PictureBox();
            this.object_small = new System.Windows.Forms.PictureBox();
            this.object_mid = new System.Windows.Forms.PictureBox();
            this.scorelabel = new System.Windows.Forms.Label();
            this.PicBonus = new System.Windows.Forms.PictureBox();
            this.clock = new System.Windows.Forms.Timer(this.components);
            this.pgcontents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.player)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QuitButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BeginButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_small2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_mid2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_large)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_small)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_mid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicBonus)).BeginInit();
            this.SuspendLayout();
            // 
            // pgcontents
            // 
            this.pgcontents.BackColor = System.Drawing.Color.White;
            this.pgcontents.Controls.Add(this.player);
            this.pgcontents.Controls.Add(this.QuitButton);
            this.pgcontents.Controls.Add(this.BeginButton);
            this.pgcontents.Controls.Add(this.DescriptionLabel);
            this.pgcontents.Controls.Add(this.object_small2);
            this.pgcontents.Controls.Add(this.object_mid2);
            this.pgcontents.Controls.Add(this.object_large);
            this.pgcontents.Controls.Add(this.object_small);
            this.pgcontents.Controls.Add(this.object_mid);
            this.pgcontents.Controls.Add(this.scorelabel);
            this.pgcontents.Controls.Add(this.PicBonus);
            this.pgcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgcontents.Location = new System.Drawing.Point(0, 0);
            this.pgcontents.Name = "pgcontents";
            this.pgcontents.Size = new System.Drawing.Size(597, 567);
            this.pgcontents.TabIndex = 21;
            this.pgcontents.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pgcontents_MouseMove);
            // 
            // player
            // 
            this.player.BackColor = System.Drawing.Color.Transparent;
            this.player.Image = ((System.Drawing.Image)(resources.GetObject("player.Image")));
            this.player.Location = new System.Drawing.Point(192, 445);
            this.player.Name = "player";
            this.player.Size = new System.Drawing.Size(32, 32);
            this.player.TabIndex = 18;
            this.player.TabStop = false;
            // 
            // QuitButton
            // 
            this.QuitButton.Image = ((System.Drawing.Image)(resources.GetObject("QuitButton.Image")));
            this.QuitButton.Location = new System.Drawing.Point(216, 424);
            this.QuitButton.Name = "QuitButton";
            this.QuitButton.Size = new System.Drawing.Size(200, 50);
            this.QuitButton.TabIndex = 12;
            this.QuitButton.TabStop = false;
            this.QuitButton.Click += new System.EventHandler(this.QuitButton_Click);
            // 
            // BeginButton
            // 
            this.BeginButton.Image = ((System.Drawing.Image)(resources.GetObject("BeginButton.Image")));
            this.BeginButton.Location = new System.Drawing.Point(3, 424);
            this.BeginButton.Name = "BeginButton";
            this.BeginButton.Size = new System.Drawing.Size(200, 50);
            this.BeginButton.TabIndex = 11;
            this.BeginButton.TabStop = false;
            this.BeginButton.Click += new System.EventHandler(this.BeginButton_Click);
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionLabel.Location = new System.Drawing.Point(3, 3);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(413, 409);
            this.DescriptionLabel.TabIndex = 10;
            this.DescriptionLabel.Text = "Placeholder";
            // 
            // object_small2
            // 
            this.object_small2.Image = ((System.Drawing.Image)(resources.GetObject("object_small2.Image")));
            this.object_small2.Location = new System.Drawing.Point(75, 43);
            this.object_small2.Name = "object_small2";
            this.object_small2.Size = new System.Drawing.Size(75, 20);
            this.object_small2.TabIndex = 17;
            this.object_small2.TabStop = false;
            // 
            // object_mid2
            // 
            this.object_mid2.Image = ((System.Drawing.Image)(resources.GetObject("object_mid2.Image")));
            this.object_mid2.Location = new System.Drawing.Point(279, 134);
            this.object_mid2.Name = "object_mid2";
            this.object_mid2.Size = new System.Drawing.Size(125, 20);
            this.object_mid2.TabIndex = 16;
            this.object_mid2.TabStop = false;
            // 
            // object_large
            // 
            this.object_large.Image = ((System.Drawing.Image)(resources.GetObject("object_large.Image")));
            this.object_large.Location = new System.Drawing.Point(49, 208);
            this.object_large.Name = "object_large";
            this.object_large.Size = new System.Drawing.Size(175, 20);
            this.object_large.TabIndex = 15;
            this.object_large.TabStop = false;
            // 
            // object_small
            // 
            this.object_small.Image = ((System.Drawing.Image)(resources.GetObject("object_small.Image")));
            this.object_small.Location = new System.Drawing.Point(290, 294);
            this.object_small.Name = "object_small";
            this.object_small.Size = new System.Drawing.Size(75, 20);
            this.object_small.TabIndex = 13;
            this.object_small.TabStop = false;
            // 
            // object_mid
            // 
            this.object_mid.Image = ((System.Drawing.Image)(resources.GetObject("object_mid.Image")));
            this.object_mid.Location = new System.Drawing.Point(58, 371);
            this.object_mid.Name = "object_mid";
            this.object_mid.Size = new System.Drawing.Size(125, 20);
            this.object_mid.TabIndex = 14;
            this.object_mid.TabStop = false;
            // 
            // scorelabel
            // 
            this.scorelabel.AutoSize = true;
            this.scorelabel.BackColor = System.Drawing.Color.Transparent;
            this.scorelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scorelabel.Location = new System.Drawing.Point(3, 4);
            this.scorelabel.Name = "scorelabel";
            this.scorelabel.Size = new System.Drawing.Size(51, 55);
            this.scorelabel.TabIndex = 19;
            this.scorelabel.Text = "0";
            // 
            // PicBonus
            // 
            this.PicBonus.Image = ((System.Drawing.Image)(resources.GetObject("PicBonus.Image")));
            this.PicBonus.Location = new System.Drawing.Point(187, 84);
            this.PicBonus.Name = "PicBonus";
            this.PicBonus.Size = new System.Drawing.Size(16, 11);
            this.PicBonus.TabIndex = 20;
            this.PicBonus.TabStop = false;
            this.PicBonus.Visible = false;
            // 
            // clock
            // 
            this.clock.Interval = 20;
            this.clock.Tick += new System.EventHandler(this.clock_Tick);
            // 
            // Dodge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pgcontents);
            this.Name = "Dodge";
            this.Size = new System.Drawing.Size(597, 567);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Dodge_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Dodge_KeyUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Dodge_MouseMove);
            this.pgcontents.ResumeLayout(false);
            this.pgcontents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.player)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QuitButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BeginButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_small2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_mid2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_large)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_small)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.object_mid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicBonus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Panel pgcontents;
        internal System.Windows.Forms.PictureBox player;
        internal System.Windows.Forms.PictureBox QuitButton;
        internal System.Windows.Forms.PictureBox BeginButton;
        internal System.Windows.Forms.Label DescriptionLabel;
        internal System.Windows.Forms.PictureBox object_small2;
        internal System.Windows.Forms.PictureBox object_mid2;
        internal System.Windows.Forms.PictureBox object_large;
        internal System.Windows.Forms.PictureBox object_small;
        internal System.Windows.Forms.PictureBox object_mid;
        internal System.Windows.Forms.Label scorelabel;
        internal System.Windows.Forms.PictureBox PicBonus;
        internal System.Windows.Forms.Timer clock;
    }
}
