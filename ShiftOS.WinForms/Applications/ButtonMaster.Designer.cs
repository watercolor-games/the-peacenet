namespace ShiftOS.WinForms.Applications
{
    partial class ButtonMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonMaster));
            this.startpnl = new System.Windows.Forms.Panel();
            this.quitbtn = new System.Windows.Forms.Button();
            this.playbtn = new System.Windows.Forms.Button();
            this.difficultysel = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.desclabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.scorecounter = new System.Windows.Forms.Label();
            this.startpnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // startpnl
            // 
            this.startpnl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startpnl.Controls.Add(this.quitbtn);
            this.startpnl.Controls.Add(this.playbtn);
            this.startpnl.Controls.Add(this.difficultysel);
            this.startpnl.Controls.Add(this.label3);
            this.startpnl.Controls.Add(this.desclabel);
            this.startpnl.Controls.Add(this.label2);
            this.startpnl.Controls.Add(this.label1);
            this.startpnl.Location = new System.Drawing.Point(27, 31);
            this.startpnl.Name = "startpnl";
            this.startpnl.Size = new System.Drawing.Size(580, 409);
            this.startpnl.TabIndex = 0;
            // 
            // quitbtn
            // 
            this.quitbtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.quitbtn.Location = new System.Drawing.Point(291, 303);
            this.quitbtn.Name = "quitbtn";
            this.quitbtn.Size = new System.Drawing.Size(58, 23);
            this.quitbtn.TabIndex = 7;
            this.quitbtn.Text = "Quit";
            this.quitbtn.UseVisualStyleBackColor = true;
            this.quitbtn.Click += new System.EventHandler(this.quitbtn_Click);
            // 
            // playbtn
            // 
            this.playbtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.playbtn.Location = new System.Drawing.Point(229, 303);
            this.playbtn.Name = "playbtn";
            this.playbtn.Size = new System.Drawing.Size(56, 23);
            this.playbtn.TabIndex = 6;
            this.playbtn.Text = "Play";
            this.playbtn.UseVisualStyleBackColor = true;
            this.playbtn.Click += new System.EventHandler(this.playbtn_Click);
            // 
            // difficultysel
            // 
            this.difficultysel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.difficultysel.FormattingEnabled = true;
            this.difficultysel.Location = new System.Drawing.Point(152, 202);
            this.difficultysel.Name = "difficultysel";
            this.difficultysel.Size = new System.Drawing.Size(276, 95);
            this.difficultysel.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(243, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 20);
            this.label3.TabIndex = 4;
            this.label3.Tag = "header2";
            this.label3.Text = "Difficulty";
            // 
            // desclabel
            // 
            this.desclabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.desclabel.Location = new System.Drawing.Point(3, 67);
            this.desclabel.Name = "desclabel";
            this.desclabel.Size = new System.Drawing.Size(577, 112);
            this.desclabel.TabIndex = 2;
            this.desclabel.Tag = "";
            this.desclabel.Text = resources.GetString("desclabel.Text");
            this.desclabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(235, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "by Appscape Games";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(211, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 25);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header1";
            this.label1.Text = "ButtonMaster";
            // 
            // scorecounter
            // 
            this.scorecounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.scorecounter.BackColor = System.Drawing.Color.Transparent;
            this.scorecounter.Font = new System.Drawing.Font("Courier New", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scorecounter.ForeColor = System.Drawing.Color.Red;
            this.scorecounter.Location = new System.Drawing.Point(548, 454);
            this.scorecounter.Name = "scorecounter";
            this.scorecounter.Size = new System.Drawing.Size(92, 26);
            this.scorecounter.TabIndex = 1;
            this.scorecounter.Tag = "keepfont keepfg keepbg";
            this.scorecounter.Text = "score";
            this.scorecounter.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.scorecounter.Visible = false;
            // 
            // ButtonMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.BlueBlueVerticalGradient;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.scorecounter);
            this.Controls.Add(this.startpnl);
            this.Name = "ButtonMaster";
            this.Size = new System.Drawing.Size(640, 480);
            this.startpnl.ResumeLayout(false);
            this.startpnl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel startpnl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label desclabel;
        private System.Windows.Forms.ListBox difficultysel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button playbtn;
        private System.Windows.Forms.Button quitbtn;
        private System.Windows.Forms.Label scorecounter;
    }
}
