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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlcanvas = new System.Windows.Forms.Panel();
            this.pnllevelwon = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btncashout = new System.Windows.Forms.Button();
            this.lbltitle = new System.Windows.Forms.Label();
            this.pnlgamestart = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnplay = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pnllevelwon.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlgamestart.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlcanvas
            // 
            this.pnlcanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlcanvas.Location = new System.Drawing.Point(0, 0);
            this.pnlcanvas.Name = "pnlcanvas";
            this.pnlcanvas.Size = new System.Drawing.Size(879, 450);
            this.pnlcanvas.TabIndex = 0;
            this.pnlcanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlcanvas_Paint);
            this.pnlcanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlcanvas_MouseMove);
            // 
            // pnllevelwon
            // 
            this.pnllevelwon.Controls.Add(this.label1);
            this.pnllevelwon.Controls.Add(this.panel1);
            this.pnllevelwon.Controls.Add(this.lbltitle);
            this.pnllevelwon.Location = new System.Drawing.Point(57, 75);
            this.pnllevelwon.Name = "pnllevelwon";
            this.pnllevelwon.Size = new System.Drawing.Size(301, 184);
            this.pnllevelwon.TabIndex = 0;
            this.pnllevelwon.Visible = false;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(301, 139);
            this.label1.TabIndex = 1;
            this.label1.Text = "{PONG_BEATLEVELDESC}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btncashout);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 152);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(301, 32);
            this.panel1.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(159, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "{PONG_PLAYON}";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btncashout
            // 
            this.btncashout.Location = new System.Drawing.Point(48, 6);
            this.btncashout.Name = "btncashout";
            this.btncashout.Size = new System.Drawing.Size(93, 23);
            this.btncashout.TabIndex = 0;
            this.btncashout.Text = "{PONG_CASHOUT}";
            this.btncashout.UseVisualStyleBackColor = true;
            this.btncashout.Click += new System.EventHandler(this.btncashout_Click);
            // 
            // lbltitle
            // 
            this.lbltitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbltitle.Location = new System.Drawing.Point(0, 0);
            this.lbltitle.Name = "lbltitle";
            this.lbltitle.Size = new System.Drawing.Size(301, 13);
            this.lbltitle.TabIndex = 0;
            this.lbltitle.Tag = "header2";
            this.lbltitle.Text = "You\'ve reached level 2!";
            this.lbltitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlgamestart
            // 
            this.pnlgamestart.Controls.Add(this.label2);
            this.pnlgamestart.Controls.Add(this.panel3);
            this.pnlgamestart.Controls.Add(this.label3);
            this.pnlgamestart.Location = new System.Drawing.Point(289, 133);
            this.pnlgamestart.Name = "pnlgamestart";
            this.pnlgamestart.Size = new System.Drawing.Size(301, 280);
            this.pnlgamestart.TabIndex = 1;
            this.pnlgamestart.Visible = false;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(301, 206);
            this.label2.TabIndex = 1;
            this.label2.Text = "{PONG_DESC}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnplay);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 248);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(301, 32);
            this.panel3.TabIndex = 2;
            // 
            // btnplay
            // 
            this.btnplay.Location = new System.Drawing.Point(100, 6);
            this.btnplay.Name = "btnplay";
            this.btnplay.Size = new System.Drawing.Size(116, 23);
            this.btnplay.TabIndex = 1;
            this.btnplay.Text = "{PONG_PLAY}";
            this.btnplay.UseVisualStyleBackColor = true;
            this.btnplay.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(301, 42);
            this.label3.TabIndex = 0;
            this.label3.Tag = "header2";
            this.label3.Text = "{PONG_WELCOME}";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Pong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlgamestart);
            this.Controls.Add(this.pnllevelwon);
            this.Controls.Add(this.pnlcanvas);
            this.Name = "Pong";
            this.Size = new System.Drawing.Size(879, 450);
            this.pnllevelwon.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pnlgamestart.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlcanvas;
        private System.Windows.Forms.Panel pnllevelwon;
        private System.Windows.Forms.Label lbltitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btncashout;
        private System.Windows.Forms.Panel pnlgamestart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnplay;
        private System.Windows.Forms.Label label3;
    }
}
