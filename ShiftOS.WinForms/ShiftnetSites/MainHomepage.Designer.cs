namespace ShiftOS.WinForms.ShiftnetSites
{
    partial class MainHomepage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainHomepage));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbspecheader = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.flfundamentals = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(710, 65);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header1";
            this.label1.Text = "Welcome to the Shiftnet!";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(710, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "The Shiftnet is a vast network of services, websites, software and so much more f" +
    "or ShiftOS. Have a look around!";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lbspecheader);
            this.panel1.Location = new System.Drawing.Point(27, 140);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(389, 281);
            this.panel1.TabIndex = 2;
            // 
            // lbspecheader
            // 
            this.lbspecheader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbspecheader.Location = new System.Drawing.Point(0, 0);
            this.lbspecheader.Name = "lbspecheader";
            this.lbspecheader.Size = new System.Drawing.Size(389, 42);
            this.lbspecheader.TabIndex = 0;
            this.lbspecheader.Tag = "header3";
            this.lbspecheader.Text = "How to use the Shiftnet";
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(0, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(389, 239);
            this.label3.TabIndex = 1;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.flfundamentals);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(439, 140);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(247, 281);
            this.panel2.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 42);
            this.label4.TabIndex = 1;
            this.label4.Tag = "header3";
            this.label4.Text = "The Fundamentals";
            // 
            // flfundamentals
            // 
            this.flfundamentals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flfundamentals.Location = new System.Drawing.Point(0, 42);
            this.flfundamentals.Name = "flfundamentals";
            this.flfundamentals.Size = new System.Drawing.Size(247, 239);
            this.flfundamentals.TabIndex = 2;
            // 
            // MainHomepage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainHomepage";
            this.Size = new System.Drawing.Size(710, 437);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbspecheader;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.FlowLayoutPanel flfundamentals;
        private System.Windows.Forms.Label label4;
    }
}
