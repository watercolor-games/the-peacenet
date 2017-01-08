using ShiftOS.WinForms.Controls;

namespace ShiftOS.WinForms.Applications
{
    partial class Pong
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.counter = new System.Windows.Forms.Timer(this.components);
            this.tmrcountdown = new System.Windows.Forms.Timer(this.components);
            this.tmrstoryline = new System.Windows.Forms.Timer(this.components);
            this.pgcontents = new ShiftOS.WinForms.Controls.Canvas();
            this.pnlgamestats = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.lblnextstats = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.lblpreviousstats = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.btnplayon = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.btncashout = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.lbllevelreached = new System.Windows.Forms.Label();
            this.pnlhighscore = new System.Windows.Forms.Panel();
            this.lbhighscore = new System.Windows.Forms.ListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.pnlfinalstats = new System.Windows.Forms.Panel();
            this.btnplayagain = new System.Windows.Forms.Button();
            this.lblfinalcodepoints = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.lblfinalcomputerreward = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.lblfinallevelreward = new System.Windows.Forms.Label();
            this.lblfinallevelreached = new System.Windows.Forms.Label();
            this.lblfinalcodepointswithtext = new System.Windows.Forms.Label();
            this.pnllose = new System.Windows.Forms.Panel();
            this.lblmissedout = new System.Windows.Forms.Label();
            this.lblbutyougained = new System.Windows.Forms.Label();
            this.btnlosetryagain = new System.Windows.Forms.Button();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.pnlintro = new System.Windows.Forms.Panel();
            this.Label6 = new System.Windows.Forms.Label();
            this.btnstartgame = new System.Windows.Forms.Button();
            this.Label8 = new System.Windows.Forms.Label();
            this.lblbeatai = new System.Windows.Forms.Label();
            this.lblcountdown = new System.Windows.Forms.Label();
            this.ball = new ShiftOS.WinForms.Controls.Canvas();
            this.paddleHuman = new System.Windows.Forms.PictureBox();
            this.paddleComputer = new System.Windows.Forms.Panel();
            this.lbllevelandtime = new System.Windows.Forms.Label();
            this.lblstatscodepoints = new System.Windows.Forms.Label();
            this.lblstatsY = new System.Windows.Forms.Label();
            this.lblstatsX = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button2 = new System.Windows.Forms.Button();
            this.pgcontents.SuspendLayout();
            this.pnlgamestats.SuspendLayout();
            this.pnlhighscore.SuspendLayout();
            this.pnlfinalstats.SuspendLayout();
            this.pnllose.SuspendLayout();
            this.pnlintro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paddleHuman)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameTimer
            // 
            this.gameTimer.Interval = 30;
            this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            // 
            // counter
            // 
            this.counter.Interval = 1000;
            this.counter.Tick += new System.EventHandler(this.counter_Tick);
            // 
            // tmrcountdown
            // 
            this.tmrcountdown.Interval = 1000;
            this.tmrcountdown.Tick += new System.EventHandler(this.countdown_Tick);
            // 
            // tmrstoryline
            // 
            this.tmrstoryline.Interval = 1000;
            this.tmrstoryline.Tick += new System.EventHandler(this.tmrstoryline_Tick);
            // 
            // pgcontents
            // 
            this.pgcontents.BackColor = System.Drawing.Color.White;
            this.pgcontents.Controls.Add(this.pnlhighscore);
            this.pgcontents.Controls.Add(this.pnlgamestats);
            this.pgcontents.Controls.Add(this.pnlfinalstats);
            this.pgcontents.Controls.Add(this.pnllose);
            this.pgcontents.Controls.Add(this.pnlintro);
            this.pgcontents.Controls.Add(this.lblbeatai);
            this.pgcontents.Controls.Add(this.lblcountdown);
            this.pgcontents.Controls.Add(this.ball);
            this.pgcontents.Controls.Add(this.paddleHuman);
            this.pgcontents.Controls.Add(this.paddleComputer);
            this.pgcontents.Controls.Add(this.lbllevelandtime);
            this.pgcontents.Controls.Add(this.lblstatscodepoints);
            this.pgcontents.Controls.Add(this.lblstatsY);
            this.pgcontents.Controls.Add(this.lblstatsX);
            this.pgcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgcontents.Location = new System.Drawing.Point(0, 0);
            this.pgcontents.Name = "pgcontents";
            this.pgcontents.Size = new System.Drawing.Size(700, 400);
            this.pgcontents.TabIndex = 20;
            this.pgcontents.Paint += new System.Windows.Forms.PaintEventHandler(this.pgcontents_Paint);
            this.pgcontents.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pongMain_MouseMove);
            // 
            // pnlgamestats
            // 
            this.pnlgamestats.Controls.Add(this.button1);
            this.pnlgamestats.Controls.Add(this.label12);
            this.pnlgamestats.Controls.Add(this.lblnextstats);
            this.pnlgamestats.Controls.Add(this.Label7);
            this.pnlgamestats.Controls.Add(this.lblpreviousstats);
            this.pnlgamestats.Controls.Add(this.Label4);
            this.pnlgamestats.Controls.Add(this.btnplayon);
            this.pnlgamestats.Controls.Add(this.Label3);
            this.pnlgamestats.Controls.Add(this.btncashout);
            this.pnlgamestats.Controls.Add(this.Label2);
            this.pnlgamestats.Controls.Add(this.lbllevelreached);
            this.pnlgamestats.Location = new System.Drawing.Point(56, 76);
            this.pnlgamestats.Name = "pnlgamestats";
            this.pnlgamestats.Size = new System.Drawing.Size(466, 284);
            this.pnlgamestats.TabIndex = 6;
            this.pnlgamestats.Visible = false;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(32, 223);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(191, 35);
            this.button1.TabIndex = 10;
            this.button1.Text = "{PONG_VIEW_HIGHSCORES}";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnhighscore_Click);
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(8, 187);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(245, 33);
            this.label12.TabIndex = 9;
            this.label12.Text = "{PONG_HIGHSCORE_EXP}";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblnextstats
            // 
            this.lblnextstats.AutoSize = true;
            this.lblnextstats.Location = new System.Drawing.Point(278, 136);
            this.lblnextstats.Name = "lblnextstats";
            this.lblnextstats.Size = new System.Drawing.Size(0, 13);
            this.lblnextstats.TabIndex = 8;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(278, 119);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(124, 16);
            this.Label7.TabIndex = 7;
            this.Label7.Text = "Next Level Stats:";
            // 
            // lblpreviousstats
            // 
            this.lblpreviousstats.AutoSize = true;
            this.lblpreviousstats.Location = new System.Drawing.Point(278, 54);
            this.lblpreviousstats.Name = "lblpreviousstats";
            this.lblpreviousstats.Size = new System.Drawing.Size(0, 13);
            this.lblpreviousstats.TabIndex = 6;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(278, 37);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(154, 16);
            this.Label4.TabIndex = 5;
            this.Label4.Text = "Previous Level Stats:";
            // 
            // btnplayon
            // 
            this.btnplayon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnplayon.Location = new System.Drawing.Point(32, 147);
            this.btnplayon.Name = "btnplayon";
            this.btnplayon.Size = new System.Drawing.Size(191, 35);
            this.btnplayon.TabIndex = 4;
            this.btnplayon.Text = "Play on for 3 codepoints!";
            this.btnplayon.UseVisualStyleBackColor = true;
            this.btnplayon.Click += new System.EventHandler(this.btnplayon_Click);
            // 
            // Label3
            // 
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(8, 111);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(245, 33);
            this.Label3.TabIndex = 3;
            this.Label3.Text = "{PONG_PLAYON_DESC}";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btncashout
            // 
            this.btncashout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncashout.Location = new System.Drawing.Point(32, 73);
            this.btncashout.Name = "btncashout";
            this.btncashout.Size = new System.Drawing.Size(191, 35);
            this.btncashout.TabIndex = 2;
            this.btncashout.Text = "Cash out with 1 codepoint!";
            this.btncashout.UseVisualStyleBackColor = true;
            this.btncashout.Click += new System.EventHandler(this.btncashout_Click);
            // 
            // Label2
            // 
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(8, 37);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(245, 33);
            this.Label2.TabIndex = 1;
            this.Label2.Text = "{PONG_CASHOUT_DESC}";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbllevelreached
            // 
            this.lbllevelreached.AutoSize = true;
            this.lbllevelreached.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbllevelreached.Location = new System.Drawing.Point(149, 6);
            this.lbllevelreached.Name = "lbllevelreached";
            this.lbllevelreached.Size = new System.Drawing.Size(185, 20);
            this.lbllevelreached.TabIndex = 0;
            this.lbllevelreached.Text = "You Reached Level 2!";
            // 
            // pnlhighscore
            // 
            this.pnlhighscore.Controls.Add(this.lbhighscore);
            this.pnlhighscore.Controls.Add(this.flowLayoutPanel1);
            this.pnlhighscore.Controls.Add(this.label10);
            this.pnlhighscore.Location = new System.Drawing.Point(67, 29);
            this.pnlhighscore.Name = "pnlhighscore";
            this.pnlhighscore.Size = new System.Drawing.Size(539, 311);
            this.pnlhighscore.TabIndex = 14;
            this.pnlhighscore.Visible = false;
            // 
            // lbhighscore
            // 
            this.lbhighscore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbhighscore.FormattingEnabled = true;
            this.lbhighscore.Location = new System.Drawing.Point(0, 36);
            this.lbhighscore.MultiColumn = true;
            this.lbhighscore.Name = "lbhighscore";
            this.lbhighscore.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbhighscore.Size = new System.Drawing.Size(539, 246);
            this.lbhighscore.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Top;
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(539, 36);
            this.label10.TabIndex = 0;
            this.label10.Text = "{HIGH_SCORES}";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlfinalstats
            // 
            this.pnlfinalstats.Controls.Add(this.btnplayagain);
            this.pnlfinalstats.Controls.Add(this.lblfinalcodepoints);
            this.pnlfinalstats.Controls.Add(this.Label11);
            this.pnlfinalstats.Controls.Add(this.lblfinalcomputerreward);
            this.pnlfinalstats.Controls.Add(this.Label9);
            this.pnlfinalstats.Controls.Add(this.lblfinallevelreward);
            this.pnlfinalstats.Controls.Add(this.lblfinallevelreached);
            this.pnlfinalstats.Controls.Add(this.lblfinalcodepointswithtext);
            this.pnlfinalstats.Location = new System.Drawing.Point(172, 74);
            this.pnlfinalstats.Name = "pnlfinalstats";
            this.pnlfinalstats.Size = new System.Drawing.Size(362, 226);
            this.pnlfinalstats.TabIndex = 9;
            this.pnlfinalstats.Visible = false;
            // 
            // btnplayagain
            // 
            this.btnplayagain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnplayagain.Location = new System.Drawing.Point(5, 194);
            this.btnplayagain.Name = "btnplayagain";
            this.btnplayagain.Size = new System.Drawing.Size(352, 29);
            this.btnplayagain.TabIndex = 16;
            this.btnplayagain.Text = "{PLAY}";
            this.btnplayagain.UseVisualStyleBackColor = true;
            this.btnplayagain.Click += new System.EventHandler(this.btnplayagain_Click);
            // 
            // lblfinalcodepoints
            // 
            this.lblfinalcodepoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfinalcodepoints.Location = new System.Drawing.Point(3, 124);
            this.lblfinalcodepoints.Name = "lblfinalcodepoints";
            this.lblfinalcodepoints.Size = new System.Drawing.Size(356, 73);
            this.lblfinalcodepoints.TabIndex = 15;
            this.lblfinalcodepoints.Text = "134 CP";
            this.lblfinalcodepoints.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(162, 82);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(33, 33);
            this.Label11.TabIndex = 14;
            this.Label11.Text = "+";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblfinalcomputerreward
            // 
            this.lblfinalcomputerreward.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfinalcomputerreward.Location = new System.Drawing.Point(193, 72);
            this.lblfinalcomputerreward.Name = "lblfinalcomputerreward";
            this.lblfinalcomputerreward.Size = new System.Drawing.Size(151, 52);
            this.lblfinalcomputerreward.TabIndex = 12;
            this.lblfinalcomputerreward.Text = "34";
            this.lblfinalcomputerreward.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label9
            // 
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(179, 31);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(180, 49);
            this.Label9.TabIndex = 11;
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblfinallevelreward
            // 
            this.lblfinallevelreward.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfinallevelreward.Location = new System.Drawing.Point(12, 72);
            this.lblfinallevelreward.Name = "lblfinallevelreward";
            this.lblfinallevelreward.Size = new System.Drawing.Size(151, 52);
            this.lblfinallevelreward.TabIndex = 10;
            this.lblfinallevelreward.Text = "100";
            this.lblfinallevelreward.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblfinallevelreached
            // 
            this.lblfinallevelreached.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfinallevelreached.Location = new System.Drawing.Point(3, 31);
            this.lblfinallevelreached.Name = "lblfinallevelreached";
            this.lblfinallevelreached.Size = new System.Drawing.Size(170, 49);
            this.lblfinallevelreached.TabIndex = 9;
            this.lblfinallevelreached.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblfinalcodepointswithtext
            // 
            this.lblfinalcodepointswithtext.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfinalcodepointswithtext.Location = new System.Drawing.Point(3, 2);
            this.lblfinalcodepointswithtext.Name = "lblfinalcodepointswithtext";
            this.lblfinalcodepointswithtext.Size = new System.Drawing.Size(356, 26);
            this.lblfinalcodepointswithtext.TabIndex = 1;
            this.lblfinalcodepointswithtext.Text = "You cashed out with 134 codepoints!";
            this.lblfinalcodepointswithtext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnllose
            // 
            this.pnllose.Controls.Add(this.lblmissedout);
            this.pnllose.Controls.Add(this.lblbutyougained);
            this.pnllose.Controls.Add(this.btnlosetryagain);
            this.pnllose.Controls.Add(this.Label5);
            this.pnllose.Controls.Add(this.Label1);
            this.pnllose.Location = new System.Drawing.Point(209, 71);
            this.pnllose.Name = "pnllose";
            this.pnllose.Size = new System.Drawing.Size(266, 214);
            this.pnllose.TabIndex = 10;
            this.pnllose.Visible = false;
            // 
            // lblmissedout
            // 
            this.lblmissedout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmissedout.Location = new System.Drawing.Point(3, 175);
            this.lblmissedout.Name = "lblmissedout";
            this.lblmissedout.Size = new System.Drawing.Size(146, 35);
            this.lblmissedout.TabIndex = 3;
            this.lblmissedout.Text = "You Missed Out On: 500 Codepoints";
            this.lblmissedout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblbutyougained
            // 
            if (ShiftoriumFrontend.UpgradeInstalled("pong_upgrade_2"))
            {
                this.lblbutyougained.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.lblbutyougained.Location = new System.Drawing.Point(3, 125);
                this.lblbutyougained.Name = "lblbutyougained";
                this.lblbutyougained.Size = new System.Drawing.Size(146, 35);
                this.lblbutyougained.TabIndex = 3;
                this.lblbutyougained.Text = "But you gained 5 Codepoints";
                this.lblbutyougained.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            } else
            {
                this.lblbutyougained.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.lblbutyougained.Location = new System.Drawing.Point(3, 125);
                this.lblbutyougained.Name = "lblbutyougained";
                this.lblbutyougained.Size = new System.Drawing.Size(0, 0);
                this.lblbutyougained.TabIndex = 3;
                this.lblbutyougained.Text = "But you gained 5 Codepoints";
                this.lblbutyougained.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            }
            // 
            // btnlosetryagain
            // 
            this.btnlosetryagain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnlosetryagain.Location = new System.Drawing.Point(155, 176);
            this.btnlosetryagain.Name = "btnlosetryagain";
            this.btnlosetryagain.Size = new System.Drawing.Size(106, 35);
            this.btnlosetryagain.TabIndex = 2;
            this.btnlosetryagain.Text = "Try Again";
            this.btnlosetryagain.UseVisualStyleBackColor = true;
            this.btnlosetryagain.Click += new System.EventHandler(this.btnlosetryagain_Click);
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(7, 26);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(260, 163);
            this.Label5.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(0, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(266, 16);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "You lose!";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlintro
            // 
            this.pnlintro.Controls.Add(this.Label6);
            this.pnlintro.Controls.Add(this.btnstartgame);
            this.pnlintro.Controls.Add(this.Label8);
            this.pnlintro.Location = new System.Drawing.Point(52, 29);
            this.pnlintro.Name = "pnlintro";
            this.pnlintro.Size = new System.Drawing.Size(595, 303);
            this.pnlintro.TabIndex = 13;
            // 
            // Label6
            // 
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(3, 39);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(589, 227);
            this.Label6.TabIndex = 15;
            this.Label6.Text = "{PONG_DESC}";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label6.Click += new System.EventHandler(this.Label6_Click);
            // 
            // btnstartgame
            // 
            this.btnstartgame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnstartgame.Location = new System.Drawing.Point(186, 273);
            this.btnstartgame.Name = "btnstartgame";
            this.btnstartgame.Size = new System.Drawing.Size(242, 28);
            this.btnstartgame.TabIndex = 15;
            this.btnstartgame.Text = "{PLAY}";
            this.btnstartgame.UseVisualStyleBackColor = true;
            this.btnstartgame.Click += new System.EventHandler(this.btnstartgame_Click);
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.ForeColor = System.Drawing.Color.Black;
            this.Label8.Location = new System.Drawing.Point(250, 5);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(280, 31);
            this.Label8.TabIndex = 14;
            this.Label8.Text = "{PONG_WELCOME}";
            this.Label8.Click += new System.EventHandler(this.Label8_Click);
            // 
            // lblbeatai
            // 
            this.lblbeatai.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblbeatai.Location = new System.Drawing.Point(47, 41);
            this.lblbeatai.Name = "lblbeatai";
            this.lblbeatai.Size = new System.Drawing.Size(600, 30);
            this.lblbeatai.TabIndex = 8;
            this.lblbeatai.Text = "You got 2 codepoints for beating the Computer!";
            this.lblbeatai.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblbeatai.Visible = false;
            // 
            // lblcountdown
            // 
            this.lblcountdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcountdown.Location = new System.Drawing.Point(182, 152);
            this.lblcountdown.Name = "lblcountdown";
            this.lblcountdown.Size = new System.Drawing.Size(315, 49);
            this.lblcountdown.TabIndex = 7;
            this.lblcountdown.Text = "3";
            this.lblcountdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblcountdown.Visible = false;
            // 
            // ball
            // 
            this.ball.BackColor = System.Drawing.Color.Black;
            this.ball.Location = new System.Drawing.Point(300, 152);
            this.ball.Name = "ball";
            this.ball.Size = new System.Drawing.Size(20, 20);
            this.ball.TabIndex = 2;
            this.ball.MouseEnter += new System.EventHandler(this.ball_MouseEnter);
            this.ball.MouseLeave += new System.EventHandler(this.ball_MouseLeave);
            // 
            // paddleHuman
            // 
            this.paddleHuman.BackColor = System.Drawing.Color.Black;
            this.paddleHuman.Location = new System.Drawing.Point(10, 134);
            this.paddleHuman.Name = "paddleHuman";
            this.paddleHuman.Size = new System.Drawing.Size(20, 100);
            this.paddleHuman.TabIndex = 3;
            this.paddleHuman.TabStop = false;
            // 
            // paddleComputer
            // 
            this.paddleComputer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paddleComputer.BackColor = System.Drawing.Color.Black;
            this.paddleComputer.Location = new System.Drawing.Point(666, 134);
            this.paddleComputer.MaximumSize = new System.Drawing.Size(20, 100);
            this.paddleComputer.Name = "paddleComputer";
            this.paddleComputer.Size = new System.Drawing.Size(20, 100);
            this.paddleComputer.TabIndex = 1;
            // 
            // lbllevelandtime
            // 
            this.lbllevelandtime.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbllevelandtime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbllevelandtime.Location = new System.Drawing.Point(0, 0);
            this.lbllevelandtime.Name = "lbllevelandtime";
            this.lbllevelandtime.Size = new System.Drawing.Size(700, 22);
            this.lbllevelandtime.TabIndex = 4;
            this.lbllevelandtime.Text = "Level: 1 - 58 Seconds Left";
            this.lbllevelandtime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblstatscodepoints
            // 
            this.lblstatscodepoints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblstatscodepoints.Font = new System.Drawing.Font("Georgia", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstatscodepoints.Location = new System.Drawing.Point(239, 356);
            this.lblstatscodepoints.Name = "lblstatscodepoints";
            this.lblstatscodepoints.Size = new System.Drawing.Size(219, 35);
            this.lblstatscodepoints.TabIndex = 12;
            this.lblstatscodepoints.Text = "Codepoints: ";
            this.lblstatscodepoints.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblstatsY
            // 
            this.lblstatsY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblstatsY.Font = new System.Drawing.Font("Georgia", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstatsY.Location = new System.Drawing.Point(542, 356);
            this.lblstatsY.Name = "lblstatsY";
            this.lblstatsY.Size = new System.Drawing.Size(144, 35);
            this.lblstatsY.TabIndex = 11;
            this.lblstatsY.Text = "Yspeed:";
            this.lblstatsY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblstatsX
            // 
            this.lblstatsX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblstatsX.Font = new System.Drawing.Font("Georgia", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstatsX.Location = new System.Drawing.Point(3, 356);
            this.lblstatsX.Name = "lblstatsX";
            this.lblstatsX.Size = new System.Drawing.Size(144, 35);
            this.lblstatsX.TabIndex = 5;
            this.lblstatsX.Text = "Xspeed: ";
            this.lblstatsX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.button2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 282);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(539, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2.Location = new System.Drawing.Point(476, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(60, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "{CLOSE}";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Pong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pgcontents);
            this.DoubleBuffered = true;
            this.Name = "Pong";
            this.Text = "{PONG_NAME}";
            this.Size = new System.Drawing.Size(820, 500);
            this.Load += new System.EventHandler(this.Pong_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pongMain_MouseMove);
            this.pgcontents.ResumeLayout(false);
            this.pnlgamestats.ResumeLayout(false);
            this.pnlgamestats.PerformLayout();
            this.pnlhighscore.ResumeLayout(false);
            this.pnlhighscore.PerformLayout();
            this.pnlfinalstats.ResumeLayout(false);
            this.pnlfinalstats.PerformLayout();
            this.pnllose.ResumeLayout(false);
            this.pnlintro.ResumeLayout(false);
            this.pnlintro.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paddleHuman)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Panel paddleComputer;
        internal System.Windows.Forms.Timer gameTimer;
        internal System.Windows.Forms.PictureBox paddleHuman;
        internal System.Windows.Forms.Label lbllevelandtime;
        internal System.Windows.Forms.Label lblstatsX;
        internal System.Windows.Forms.Timer counter;
        internal System.Windows.Forms.Panel pnlgamestats;
        internal System.Windows.Forms.Label lblnextstats;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label lblpreviousstats;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Button btnplayon;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Button btncashout;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label lbllevelreached;
        internal System.Windows.Forms.Label lblcountdown;
        internal System.Windows.Forms.Timer tmrcountdown;
        internal System.Windows.Forms.Label lblbeatai;
        internal System.Windows.Forms.Panel pnlfinalstats;
        internal System.Windows.Forms.Button btnplayagain;
        internal System.Windows.Forms.Label lblfinalcodepoints;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.Label lblfinalcomputerreward;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Label lblfinallevelreward;
        internal System.Windows.Forms.Label lblfinallevelreached;
        internal System.Windows.Forms.Label lblfinalcodepointswithtext;
        internal System.Windows.Forms.Panel pnllose;
        internal System.Windows.Forms.Label lblmissedout;
        internal System.Windows.Forms.Label lblbutyougained;
        internal System.Windows.Forms.Button btnlosetryagain;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label lblstatscodepoints;
        internal System.Windows.Forms.Label lblstatsY;
        internal System.Windows.Forms.Panel pnlintro;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Button btnstartgame;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Timer tmrstoryline;
        private System.Windows.Forms.Panel pnlhighscore;
        private System.Windows.Forms.ListBox lbhighscore;
        private System.Windows.Forms.Label label10;
        internal Canvas pgcontents;
        internal Canvas ball;
        internal System.Windows.Forms.Button button1;
        internal System.Windows.Forms.Label label12;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button2;
    }
}