using ShiftOS.WinForms.Controls;

namespace ShiftOS.WinForms.Applications
{
    partial class ShiftoriumFrontend
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbupgradedesc = new System.Windows.Forms.Label();
            this.pnlupgradeactions = new System.Windows.Forms.Panel();
            this.btnbuy = new System.Windows.Forms.Button();
            this.lbupgradetitle = new System.Windows.Forms.Label();
            this.pnllist = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pgupgradeprogress = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.lbupgrades = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlupgradeactions.SuspendLayout();
            this.pnllist.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pnllist);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(782, 427);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbupgradedesc);
            this.panel2.Controls.Add(this.pnlupgradeactions);
            this.panel2.Controls.Add(this.lbupgradetitle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(406, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(376, 427);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            if (ShiftoriumFrontend.UpgradeInstalled("shiftorium_gui_codepoints_display"))
            {
                this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                this.label2.AutoSize = true;
                this.label2.Location = new System.Drawing.Point(128, 357);
                this.label2.Name = "label2";
                this.label2.Size = new System.Drawing.Size(135, 13);
                this.label2.TabIndex = 3;
                this.label2.Text = "You have: %cp Codepoints";
                this.label2.Click += new System.EventHandler(this.label2_Click);
            } else
            {
                this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                this.label2.AutoSize = true;
                this.label2.Location = new System.Drawing.Point(128, 357);
                this.label2.Name = "label2";
                this.label2.Size = new System.Drawing.Size(1, 1);
                this.label2.TabIndex = 3;
                this.label2.Text = "";
                this.label2.Click += new System.EventHandler(this.label2_Click);
            }
            // 
            // lbupgradedesc
            // 
            this.lbupgradedesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbupgradedesc.Location = new System.Drawing.Point(0, 42);
            this.lbupgradedesc.Name = "lbupgradedesc";
            this.lbupgradedesc.Size = new System.Drawing.Size(376, 348);
            this.lbupgradedesc.TabIndex = 2;
            this.lbupgradedesc.Text = "{SHIFTORIUM_EXP}";
            this.lbupgradedesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbupgradedesc.UseCompatibleTextRendering = true;
            // 
            // pnlupgradeactions
            // 
            this.pnlupgradeactions.Controls.Add(this.btnbuy);
            this.pnlupgradeactions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlupgradeactions.Location = new System.Drawing.Point(0, 390);
            this.pnlupgradeactions.Name = "pnlupgradeactions";
            this.pnlupgradeactions.Size = new System.Drawing.Size(376, 37);
            this.pnlupgradeactions.TabIndex = 1;
            // 
            // btnbuy
            // 
            this.btnbuy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnbuy.AutoSize = true;
            this.btnbuy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnbuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnbuy.Location = new System.Drawing.Point(327, 9);
            this.btnbuy.Name = "btnbuy";
            this.btnbuy.Size = new System.Drawing.Size(37, 25);
            this.btnbuy.TabIndex = 0;
            this.btnbuy.Text = "Buy";
            this.btnbuy.UseVisualStyleBackColor = true;
            this.btnbuy.Visible = false;
            this.btnbuy.Click += new System.EventHandler(this.btnbuy_Click);
            // 
            // lbupgradetitle
            // 
            this.lbupgradetitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbupgradetitle.Location = new System.Drawing.Point(0, 0);
            this.lbupgradetitle.Name = "lbupgradetitle";
            this.lbupgradetitle.Size = new System.Drawing.Size(376, 42);
            this.lbupgradetitle.TabIndex = 0;
            this.lbupgradetitle.Text = "{WELCOME_TO_SHIFTORIUM}";
            this.lbupgradetitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbupgradetitle.UseCompatibleTextRendering = true;
            // 
            // pnllist
            // 
            this.pnllist.Controls.Add(this.label2);
            this.pnllist.Controls.Add(this.label1);
            this.pnllist.Controls.Add(this.pgupgradeprogress);
            this.pnllist.Controls.Add(this.lbupgrades);
            this.pnllist.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnllist.Location = new System.Drawing.Point(0, 0);
            this.pnllist.Name = "pnllist";
            this.pnllist.Size = new System.Drawing.Size(406, 427);
            this.pnllist.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 399);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "{UPGRADE_PROGRESS}:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 399);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 2;
            int upgradepercent = (pgupgradeprogress.Value / 100) * 100;
            this.label3.Text = upgradepercent.ToString();
            // 
            // pgupgradeprogress
            // 
            this.pgupgradeprogress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgupgradeprogress.BlockSize = 5;
            this.pgupgradeprogress.Location = new System.Drawing.Point(146, 390);
            this.pgupgradeprogress.Maximum = 100;
            this.pgupgradeprogress.Name = "pgupgradeprogress";
            this.pgupgradeprogress.Size = new System.Drawing.Size(254, 23);
            this.pgupgradeprogress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgupgradeprogress.TabIndex = 1;
            this.pgupgradeprogress.Value = 25;
            // 
            // lbupgrades
            // 
            this.lbupgrades.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbupgrades.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbupgrades.FormattingEnabled = true;
            this.lbupgrades.Location = new System.Drawing.Point(3, 66);
            this.lbupgrades.Name = "lbupgrades";
            this.lbupgrades.Size = new System.Drawing.Size(397, 277);
            this.lbupgrades.TabIndex = 0;
            this.lbupgrades.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbupgrades_DrawItem);
            // 
            // ShiftoriumFrontend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.LightGreen;
            this.Name = "ShiftoriumFrontend";
            this.Text = "{SHIFTORIUM_NAME}";
            this.Size = new System.Drawing.Size(782, 427);
            this.Load += new System.EventHandler(this.Shiftorium_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlupgradeactions.ResumeLayout(false);
            this.pnlupgradeactions.PerformLayout();
            this.pnllist.ResumeLayout(false);
            this.pnllist.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnllist;
        private System.Windows.Forms.ListBox lbupgrades;
        private System.Windows.Forms.Label lbupgradedesc;
        private System.Windows.Forms.Panel pnlupgradeactions;
        private System.Windows.Forms.Label lbupgradetitle;
        private System.Windows.Forms.Button btnbuy;
        private ShiftedProgressBar pgupgradeprogress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}