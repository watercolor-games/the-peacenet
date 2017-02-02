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
            this.pgrereg = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.txtruname = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtrpass = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtrsys = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pglogin = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.txtluser = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtlpass = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.flbuttons.SuspendLayout();
            this.page1.SuspendLayout();
            this.page2.SuspendLayout();
            this.page3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.page4.SuspendLayout();
            this.pgrereg.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pglogin.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
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
            // pgrereg
            // 
            this.pgrereg.Controls.Add(this.tableLayoutPanel2);
            this.pgrereg.Controls.Add(this.label11);
            this.pgrereg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgrereg.Location = new System.Drawing.Point(138, 0);
            this.pgrereg.Name = "pgrereg";
            this.pgrereg.Size = new System.Drawing.Size(352, 300);
            this.pgrereg.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtruname, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtrpass, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtrsys, 1, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(20, 61);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(212, 72);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Username:";
            // 
            // txtruname
            // 
            this.txtruname.Location = new System.Drawing.Point(109, 3);
            this.txtruname.Name = "txtruname";
            this.txtruname.Size = new System.Drawing.Size(100, 20);
            this.txtruname.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Password:";
            // 
            // txtrpass
            // 
            this.txtrpass.Location = new System.Drawing.Point(109, 29);
            this.txtrpass.Name = "txtrpass";
            this.txtrpass.PasswordChar = '*';
            this.txtrpass.Size = new System.Drawing.Size(100, 20);
            this.txtrpass.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "System Name:";
            // 
            // txtrsys
            // 
            this.txtrsys.Location = new System.Drawing.Point(109, 55);
            this.txtrsys.Name = "txtrsys";
            this.txtrsys.Size = new System.Drawing.Size(100, 20);
            this.txtrsys.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.label11.Location = new System.Drawing.Point(16, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(321, 26);
            this.label11.TabIndex = 3;
            this.label11.Text = "User information";
            // 
            // pglogin
            // 
            this.pglogin.Controls.Add(this.tableLayoutPanel3);
            this.pglogin.Controls.Add(this.label15);
            this.pglogin.Controls.Add(this.button1);
            this.pglogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pglogin.Location = new System.Drawing.Point(138, 0);
            this.pglogin.Name = "pglogin";
            this.pglogin.Size = new System.Drawing.Size(352, 300);
            this.pglogin.TabIndex = 9;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.label12, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtluser, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label13, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtlpass, 1, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(20, 61);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(212, 72);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Username:";
            // 
            // txtluser
            // 
            this.txtluser.Location = new System.Drawing.Point(109, 3);
            this.txtluser.Name = "txtluser";
            this.txtluser.Size = new System.Drawing.Size(100, 20);
            this.txtluser.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 26);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Password:";
            // 
            // txtlpass
            // 
            this.txtlpass.Location = new System.Drawing.Point(109, 29);
            this.txtlpass.Name = "txtlpass";
            this.txtlpass.PasswordChar = '*';
            this.txtlpass.Size = new System.Drawing.Size(100, 20);
            this.txtlpass.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.label15.Location = new System.Drawing.Point(16, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(321, 26);
            this.label15.TabIndex = 3;
            this.label15.Text = "User information";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(129, 142);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 21);
            this.button1.TabIndex = 4;
            this.button1.Text = "Register";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FakeSetupScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 329);
            this.Controls.Add(this.pglogin);
            this.Controls.Add(this.pgrereg);
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
            this.Text = "ShiftOS";
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
            this.pgrereg.ResumeLayout(false);
            this.pgrereg.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pglogin.ResumeLayout(false);
            this.pglogin.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
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
        private System.Windows.Forms.Panel pgrereg;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtruname;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtrpass;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtrsys;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel pglogin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtluser;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtlpass;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button1;
    }
}