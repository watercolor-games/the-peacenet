namespace ShiftOS.WinForms.Applications
{
    partial class VirusScanner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VirusScanner));
            this.pnlsidebar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnscanfs = new System.Windows.Forms.Button();
            this.btnscanmem = new System.Windows.Forms.Button();
            this.btnscanfile = new System.Windows.Forms.Button();
            this.btnexit = new System.Windows.Forms.Button();
            this.pnlbody = new System.Windows.Forms.Panel();
            this.pnlsummary = new System.Windows.Forms.Panel();
            this.flviruses = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlscanner = new System.Windows.Forms.Panel();
            this.lbscanstatus = new System.Windows.Forms.Label();
            this.pgscannerprogress = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlintro = new System.Windows.Forms.Panel();
            this.lbintrotext = new System.Windows.Forms.Label();
            this.lbintrotitle = new System.Windows.Forms.Label();
            this.lbstatus = new System.Windows.Forms.Label();
            this.pnlsidebar.SuspendLayout();
            this.pnlbody.SuspendLayout();
            this.pnlsummary.SuspendLayout();
            this.pnlscanner.SuspendLayout();
            this.pnlintro.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlsidebar
            // 
            this.pnlsidebar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlsidebar.Controls.Add(this.btnscanfs);
            this.pnlsidebar.Controls.Add(this.btnscanmem);
            this.pnlsidebar.Controls.Add(this.btnscanfile);
            this.pnlsidebar.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlsidebar.Location = new System.Drawing.Point(4, 4);
            this.pnlsidebar.Name = "pnlsidebar";
            this.pnlsidebar.Size = new System.Drawing.Size(138, 399);
            this.pnlsidebar.TabIndex = 0;
            // 
            // btnscanfs
            // 
            this.btnscanfs.Location = new System.Drawing.Point(3, 3);
            this.btnscanfs.Name = "btnscanfs";
            this.btnscanfs.Size = new System.Drawing.Size(135, 23);
            this.btnscanfs.TabIndex = 0;
            this.btnscanfs.Text = "Scan filesystem";
            this.btnscanfs.UseVisualStyleBackColor = true;
            this.btnscanfs.Click += new System.EventHandler(this.btnscanfs_Click);
            // 
            // btnscanmem
            // 
            this.btnscanmem.Location = new System.Drawing.Point(3, 32);
            this.btnscanmem.Name = "btnscanmem";
            this.btnscanmem.Size = new System.Drawing.Size(135, 23);
            this.btnscanmem.TabIndex = 1;
            this.btnscanmem.Text = "Scan memory";
            this.btnscanmem.UseVisualStyleBackColor = true;
            this.btnscanmem.Click += new System.EventHandler(this.btnscanmem_Click);
            // 
            // btnscanfile
            // 
            this.btnscanfile.Location = new System.Drawing.Point(3, 61);
            this.btnscanfile.Name = "btnscanfile";
            this.btnscanfile.Size = new System.Drawing.Size(135, 23);
            this.btnscanfile.TabIndex = 2;
            this.btnscanfile.Text = "Scan file";
            this.btnscanfile.UseVisualStyleBackColor = true;
            this.btnscanfile.Click += new System.EventHandler(this.btnscanfile_Click);
            // 
            // btnexit
            // 
            this.btnexit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnexit.Location = new System.Drawing.Point(7, 409);
            this.btnexit.Name = "btnexit";
            this.btnexit.Size = new System.Drawing.Size(135, 23);
            this.btnexit.TabIndex = 1;
            this.btnexit.Text = "Exit";
            this.btnexit.UseVisualStyleBackColor = true;
            this.btnexit.Click += new System.EventHandler(this.btnexit_Click);
            // 
            // pnlbody
            // 
            this.pnlbody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlbody.Controls.Add(this.pnlsummary);
            this.pnlbody.Controls.Add(this.pnlscanner);
            this.pnlbody.Controls.Add(this.pnlintro);
            this.pnlbody.Location = new System.Drawing.Point(149, 4);
            this.pnlbody.Name = "pnlbody";
            this.pnlbody.Size = new System.Drawing.Size(498, 399);
            this.pnlbody.TabIndex = 2;
            // 
            // pnlsummary
            // 
            this.pnlsummary.Controls.Add(this.flviruses);
            this.pnlsummary.Controls.Add(this.label1);
            this.pnlsummary.Controls.Add(this.label3);
            this.pnlsummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlsummary.Location = new System.Drawing.Point(0, 0);
            this.pnlsummary.Name = "pnlsummary";
            this.pnlsummary.Size = new System.Drawing.Size(498, 399);
            this.pnlsummary.TabIndex = 3;
            // 
            // flviruses
            // 
            this.flviruses.AutoScroll = true;
            this.flviruses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flviruses.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flviruses.Location = new System.Drawing.Point(0, 66);
            this.flviruses.Margin = new System.Windows.Forms.Padding(0);
            this.flviruses.Name = "flviruses";
            this.flviruses.Padding = new System.Windows.Forms.Padding(10);
            this.flviruses.Size = new System.Drawing.Size(498, 333);
            this.flviruses.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 33);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10);
            this.label1.Size = new System.Drawing.Size(184, 33);
            this.label1.TabIndex = 2;
            this.label1.Text = "Below is a list of all viruses found.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(10);
            this.label3.Size = new System.Drawing.Size(101, 33);
            this.label3.TabIndex = 0;
            this.label3.Tag = "header3";
            this.label3.Text = "Scan complete.";
            // 
            // pnlscanner
            // 
            this.pnlscanner.Controls.Add(this.lbscanstatus);
            this.pnlscanner.Controls.Add(this.pgscannerprogress);
            this.pnlscanner.Controls.Add(this.label2);
            this.pnlscanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlscanner.Location = new System.Drawing.Point(0, 0);
            this.pnlscanner.Name = "pnlscanner";
            this.pnlscanner.Size = new System.Drawing.Size(498, 399);
            this.pnlscanner.TabIndex = 2;
            // 
            // lbscanstatus
            // 
            this.lbscanstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbscanstatus.Location = new System.Drawing.Point(0, 33);
            this.lbscanstatus.Name = "lbscanstatus";
            this.lbscanstatus.Padding = new System.Windows.Forms.Padding(10);
            this.lbscanstatus.Size = new System.Drawing.Size(498, 343);
            this.lbscanstatus.TabIndex = 2;
            this.lbscanstatus.Text = "label1";
            // 
            // pgscannerprogress
            // 
            this.pgscannerprogress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pgscannerprogress.Location = new System.Drawing.Point(0, 376);
            this.pgscannerprogress.Maximum = 100;
            this.pgscannerprogress.Name = "pgscannerprogress";
            this.pgscannerprogress.Padding = new System.Windows.Forms.Padding(10);
            this.pgscannerprogress.Size = new System.Drawing.Size(498, 23);
            this.pgscannerprogress.TabIndex = 1;
            this.pgscannerprogress.Text = "shiftedProgressBar1";
            this.pgscannerprogress.Value = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(10);
            this.label2.Size = new System.Drawing.Size(81, 33);
            this.label2.TabIndex = 0;
            this.label2.Tag = "header3";
            this.label2.Text = "Scanning...";
            // 
            // pnlintro
            // 
            this.pnlintro.Controls.Add(this.lbintrotext);
            this.pnlintro.Controls.Add(this.lbintrotitle);
            this.pnlintro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlintro.Location = new System.Drawing.Point(0, 0);
            this.pnlintro.Name = "pnlintro";
            this.pnlintro.Size = new System.Drawing.Size(498, 399);
            this.pnlintro.TabIndex = 0;
            // 
            // lbintrotext
            // 
            this.lbintrotext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbintrotext.Location = new System.Drawing.Point(0, 33);
            this.lbintrotext.Name = "lbintrotext";
            this.lbintrotext.Padding = new System.Windows.Forms.Padding(10);
            this.lbintrotext.Size = new System.Drawing.Size(498, 366);
            this.lbintrotext.TabIndex = 1;
            this.lbintrotext.Tag = "";
            this.lbintrotext.Text = resources.GetString("lbintrotext.Text");
            // 
            // lbintrotitle
            // 
            this.lbintrotitle.AutoSize = true;
            this.lbintrotitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbintrotitle.Location = new System.Drawing.Point(0, 0);
            this.lbintrotitle.Name = "lbintrotitle";
            this.lbintrotitle.Padding = new System.Windows.Forms.Padding(10);
            this.lbintrotitle.Size = new System.Drawing.Size(93, 33);
            this.lbintrotitle.TabIndex = 0;
            this.lbintrotitle.Tag = "header3";
            this.lbintrotitle.Text = "Virus Scanner";
            // 
            // lbstatus
            // 
            this.lbstatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbstatus.AutoSize = true;
            this.lbstatus.Location = new System.Drawing.Point(152, 410);
            this.lbstatus.Name = "lbstatus";
            this.lbstatus.Size = new System.Drawing.Size(48, 13);
            this.lbstatus.TabIndex = 3;
            this.lbstatus.Text = "Grade: 1";
            // 
            // VirusScanner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbstatus);
            this.Controls.Add(this.pnlbody);
            this.Controls.Add(this.btnexit);
            this.Controls.Add(this.pnlsidebar);
            this.Name = "VirusScanner";
            this.Size = new System.Drawing.Size(650, 435);
            this.pnlsidebar.ResumeLayout(false);
            this.pnlbody.ResumeLayout(false);
            this.pnlsummary.ResumeLayout(false);
            this.pnlsummary.PerformLayout();
            this.pnlscanner.ResumeLayout(false);
            this.pnlscanner.PerformLayout();
            this.pnlintro.ResumeLayout(false);
            this.pnlintro.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel pnlsidebar;
        private System.Windows.Forms.Button btnscanfs;
        private System.Windows.Forms.Button btnscanmem;
        private System.Windows.Forms.Button btnscanfile;
        private System.Windows.Forms.Button btnexit;
        private System.Windows.Forms.Panel pnlbody;
        private System.Windows.Forms.Panel pnlintro;
        private System.Windows.Forms.Label lbintrotext;
        private System.Windows.Forms.Label lbintrotitle;
        private System.Windows.Forms.Label lbstatus;
        private System.Windows.Forms.Panel pnlscanner;
        private System.Windows.Forms.Label lbscanstatus;
        private Controls.ShiftedProgressBar pgscannerprogress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlsummary;
        private System.Windows.Forms.FlowLayoutPanel flviruses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}
