namespace ShiftOS.WinForms
{
    partial class FakeSetupScreen
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlheader = new System.Windows.Forms.Panel();
            this.flbuttons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnnext = new System.Windows.Forms.Button();
            this.btnback = new System.Windows.Forms.Button();
            this.page1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.page2 = new System.Windows.Forms.Panel();
            this.lbbyteszeroed = new System.Windows.Forms.Label();
            this.pgformatprogress = new System.Windows.Forms.ProgressBar();
            this.lbformattitle = new System.Windows.Forms.Label();
            this.page3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtnewusername = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtnewpassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtnewsysname = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.page4 = new System.Windows.Forms.Panel();
            this.txtlicenseagreement = new System.Windows.Forms.RichTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.flbuttons.SuspendLayout();
            this.page1.SuspendLayout();
            this.page2.SuspendLayout();
            this.page3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.page4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlheader
            // 
            this.pnlheader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlheader.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlheader.Location = new System.Drawing.Point(0, 0);
            this.pnlheader.Name = "pnlheader";
            this.pnlheader.Size = new System.Drawing.Size(138, 300);
            this.pnlheader.TabIndex = 0;
            // 
            // flbuttons
            // 
            this.flbuttons.AutoSize = true;
            this.flbuttons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flbuttons.Controls.Add(this.btnnext);
            this.flbuttons.Controls.Add(this.btnback);
            this.flbuttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flbuttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flbuttons.Location = new System.Drawing.Point(0, 300);
            this.flbuttons.Name = "flbuttons";
            this.flbuttons.Size = new System.Drawing.Size(490, 29);
            this.flbuttons.TabIndex = 1;
            // 
            // btnnext
            // 
            this.btnnext.Location = new System.Drawing.Point(412, 3);
            this.btnnext.Name = "btnnext";
            this.btnnext.Size = new System.Drawing.Size(75, 23);
            this.btnnext.TabIndex = 0;
            this.btnnext.Text = "Next";
            this.btnnext.UseVisualStyleBackColor = true;
            this.btnnext.Click += new System.EventHandler(this.btnnext_Click);
            // 
            // btnback
            // 
            this.btnback.Location = new System.Drawing.Point(331, 3);
            this.btnback.Name = "btnback";
            this.btnback.Size = new System.Drawing.Size(75, 23);
            this.btnback.TabIndex = 1;
            this.btnback.Text = "Back";
            this.btnback.UseVisualStyleBackColor = true;
            this.btnback.Click += new System.EventHandler(this.btnback_Click);
            // 
            // page1
            // 
            this.page1.Controls.Add(this.label2);
            this.page1.Controls.Add(this.label1);
            this.page1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page1.Location = new System.Drawing.Point(138, 0);
            this.page1.Name = "page1";
            this.page1.Size = new System.Drawing.Size(352, 300);
            this.page1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(20, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(320, 199);
            this.label2.TabIndex = 1;
            this.label2.Text = "This wizard will guide you through the installation and configuration of ShiftOS." +
    "\r\n\r\nPress Next to continue.";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.label1.Location = new System.Drawing.Point(19, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(321, 44);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome to the ShiftOS installation wizard.";
            // 
            // page2
            // 
            this.page2.Controls.Add(this.lbbyteszeroed);
            this.page2.Controls.Add(this.pgformatprogress);
            this.page2.Controls.Add(this.lbformattitle);
            this.page2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page2.Location = new System.Drawing.Point(138, 0);
            this.page2.Name = "page2";
            this.page2.Size = new System.Drawing.Size(352, 300);
            this.page2.TabIndex = 2;
            // 
            // lbbyteszeroed
            // 
            this.lbbyteszeroed.AutoSize = true;
            this.lbbyteszeroed.Location = new System.Drawing.Point(20, 91);
            this.lbbyteszeroed.Name = "lbbyteszeroed";
            this.lbbyteszeroed.Size = new System.Drawing.Size(127, 13);
            this.lbbyteszeroed.TabIndex = 5;
            this.lbbyteszeroed.Text = "Bytes zeroed: 0/1000000";
            // 
            // pgformatprogress
            // 
            this.pgformatprogress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgformatprogress.Location = new System.Drawing.Point(23, 61);
            this.pgformatprogress.Name = "pgformatprogress";
            this.pgformatprogress.Size = new System.Drawing.Size(317, 23);
            this.pgformatprogress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgformatprogress.TabIndex = 4;
            // 
            // lbformattitle
            // 
            this.lbformattitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbformattitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.lbformattitle.Location = new System.Drawing.Point(16, 9);
            this.lbformattitle.Name = "lbformattitle";
            this.lbformattitle.Size = new System.Drawing.Size(321, 26);
            this.lbformattitle.TabIndex = 3;
            this.lbformattitle.Text = "Formatting your drive...";
            // 
            // page3
            // 
            this.page3.Controls.Add(this.tableLayoutPanel1);
            this.page3.Controls.Add(this.label4);
            this.page3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page3.Location = new System.Drawing.Point(138, 0);
            this.page3.Name = "page3";
            this.page3.Size = new System.Drawing.Size(352, 300);
            this.page3.TabIndex = 6;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtnewusername, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtnewpassword, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtnewsysname, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 61);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(212, 72);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Username:";
            // 
            // txtnewusername
            // 
            this.txtnewusername.Location = new System.Drawing.Point(109, 3);
            this.txtnewusername.Name = "txtnewusername";
            this.txtnewusername.Size = new System.Drawing.Size(100, 20);
            this.txtnewusername.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Password:";
            // 
            // txtnewpassword
            // 
            this.txtnewpassword.Location = new System.Drawing.Point(109, 29);
            this.txtnewpassword.Name = "txtnewpassword";
            this.txtnewpassword.PasswordChar = '*';
            this.txtnewpassword.Size = new System.Drawing.Size(100, 20);
            this.txtnewpassword.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "System Name:";
            // 
            // txtnewsysname
            // 
            this.txtnewsysname.Location = new System.Drawing.Point(109, 55);
            this.txtnewsysname.Name = "txtnewsysname";
            this.txtnewsysname.Size = new System.Drawing.Size(100, 20);
            this.txtnewsysname.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.label4.Location = new System.Drawing.Point(16, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(321, 26);
            this.label4.TabIndex = 3;
            this.label4.Text = "User information";
            // 
            // page4
            // 
            this.page4.Controls.Add(this.txtlicenseagreement);
            this.page4.Controls.Add(this.label10);
            this.page4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page4.Location = new System.Drawing.Point(138, 0);
            this.page4.Name = "page4";
            this.page4.Size = new System.Drawing.Size(352, 300);
            this.page4.TabIndex = 7;
            // 
            // txtlicenseagreement
            // 
            this.txtlicenseagreement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtlicenseagreement.Location = new System.Drawing.Point(23, 58);
            this.txtlicenseagreement.Name = "txtlicenseagreement";
            this.txtlicenseagreement.ReadOnly = true;
            this.txtlicenseagreement.Size = new System.Drawing.Size(326, 228);
            this.txtlicenseagreement.TabIndex = 4;
            this.txtlicenseagreement.Text = "";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.label10.Location = new System.Drawing.Point(16, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(321, 26);
            this.label10.TabIndex = 3;
            this.label10.Text = "License Agreement";
            // 
            // FakeSetupScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 329);
            this.Controls.Add(this.page3);
            this.Controls.Add(this.page4);
            this.Controls.Add(this.page2);
            this.Controls.Add(this.page1);
            this.Controls.Add(this.pnlheader);
            this.Controls.Add(this.flbuttons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FakeSetupScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FakeSetupScreen";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FakeSetupScreen_FormClosing);
            this.flbuttons.ResumeLayout(false);
            this.page1.ResumeLayout(false);
            this.page2.ResumeLayout(false);
            this.page2.PerformLayout();
            this.page3.ResumeLayout(false);
            this.page3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.page4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlheader;
        private System.Windows.Forms.FlowLayoutPanel flbuttons;
        private System.Windows.Forms.Button btnnext;
        private System.Windows.Forms.Button btnback;
        private System.Windows.Forms.Panel page1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel page2;
        private System.Windows.Forms.Label lbformattitle;
        private System.Windows.Forms.Label lbbyteszeroed;
        private System.Windows.Forms.ProgressBar pgformatprogress;
        private System.Windows.Forms.Panel page3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtnewusername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtnewpassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtnewsysname;
        private System.Windows.Forms.Panel page4;
        private System.Windows.Forms.RichTextBox txtlicenseagreement;
        private System.Windows.Forms.Label label10;
    }
}