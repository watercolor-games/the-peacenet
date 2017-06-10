namespace ShiftOS.WinForms.ShiftnetSites
{
    partial class ShiftSoft
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShiftSoft));
            this.label1 = new System.Windows.Forms.Label();
            this.pnldivider = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.flbuttons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnhome = new System.Windows.Forms.Button();
            this.btnservices = new System.Windows.Forms.Button();
            this.btnping = new System.Windows.Forms.Button();
            this.pnlhome = new System.Windows.Forms.Panel();
            this.lbwhere = new System.Windows.Forms.Label();
            this.lbdesc = new System.Windows.Forms.Label();
            this.pnlservices = new System.Windows.Forms.Panel();
            this.lbfreebiedesc = new System.Windows.Forms.Label();
            this.lbfreebie = new System.Windows.Forms.Label();
            this.btnjoinfreebie = new System.Windows.Forms.Button();
            this.flbuttons.SuspendLayout();
            this.pnlhome.SuspendLayout();
            this.pnlservices.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Franklin Gothic Heavy", 24.75F, System.Drawing.FontStyle.Italic);
            this.label1.Location = new System.Drawing.Point(13, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 38);
            this.label1.TabIndex = 0;
            this.label1.Tag = "keepfont";
            this.label1.Text = "Shiftsoft";
            // 
            // pnldivider
            // 
            this.pnldivider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnldivider.Location = new System.Drawing.Point(20, 71);
            this.pnldivider.Name = "pnldivider";
            this.pnldivider.Size = new System.Drawing.Size(654, 2);
            this.pnldivider.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "What do you want to shift today?";
            // 
            // flbuttons
            // 
            this.flbuttons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flbuttons.AutoSize = true;
            this.flbuttons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flbuttons.Controls.Add(this.btnhome);
            this.flbuttons.Controls.Add(this.btnservices);
            this.flbuttons.Controls.Add(this.btnping);
            this.flbuttons.Location = new System.Drawing.Point(515, 17);
            this.flbuttons.Name = "flbuttons";
            this.flbuttons.Size = new System.Drawing.Size(159, 29);
            this.flbuttons.TabIndex = 3;
            // 
            // btnhome
            // 
            this.btnhome.AutoSize = true;
            this.btnhome.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnhome.Location = new System.Drawing.Point(3, 3);
            this.btnhome.Name = "btnhome";
            this.btnhome.Size = new System.Drawing.Size(45, 23);
            this.btnhome.TabIndex = 0;
            this.btnhome.Tag = "header3";
            this.btnhome.Text = "Home";
            this.btnhome.UseVisualStyleBackColor = true;
            this.btnhome.Click += new System.EventHandler(this.btnhome_Click);
            // 
            // btnservices
            // 
            this.btnservices.AutoSize = true;
            this.btnservices.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnservices.Location = new System.Drawing.Point(54, 3);
            this.btnservices.Name = "btnservices";
            this.btnservices.Size = new System.Drawing.Size(58, 23);
            this.btnservices.TabIndex = 1;
            this.btnservices.Tag = "header3";
            this.btnservices.Text = "Services";
            this.btnservices.UseVisualStyleBackColor = true;
            this.btnservices.Click += new System.EventHandler(this.btnservices_Click);
            // 
            // btnping
            // 
            this.btnping.AutoSize = true;
            this.btnping.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnping.Location = new System.Drawing.Point(118, 3);
            this.btnping.Name = "btnping";
            this.btnping.Size = new System.Drawing.Size(38, 23);
            this.btnping.TabIndex = 2;
            this.btnping.Tag = "header3";
            this.btnping.Text = "Ping";
            this.btnping.UseVisualStyleBackColor = true;
            this.btnping.Click += new System.EventHandler(this.btnping_Click);
            // 
            // pnlhome
            // 
            this.pnlhome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlhome.Controls.Add(this.lbdesc);
            this.pnlhome.Controls.Add(this.lbwhere);
            this.pnlhome.Location = new System.Drawing.Point(20, 92);
            this.pnlhome.Name = "pnlhome";
            this.pnlhome.Size = new System.Drawing.Size(654, 271);
            this.pnlhome.TabIndex = 4;
            // 
            // lbwhere
            // 
            this.lbwhere.AutoSize = true;
            this.lbwhere.Location = new System.Drawing.Point(4, 4);
            this.lbwhere.Name = "lbwhere";
            this.lbwhere.Size = new System.Drawing.Size(169, 13);
            this.lbwhere.TabIndex = 0;
            this.lbwhere.Tag = "header2";
            this.lbwhere.Text = "Where do you want to shift today?";
            // 
            // lbdesc
            // 
            this.lbdesc.Location = new System.Drawing.Point(4, 17);
            this.lbdesc.Name = "lbdesc";
            this.lbdesc.Size = new System.Drawing.Size(361, 160);
            this.lbdesc.TabIndex = 1;
            this.lbdesc.Text = resources.GetString("lbdesc.Text");
            // 
            // pnlservices
            // 
            this.pnlservices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlservices.Controls.Add(this.btnjoinfreebie);
            this.pnlservices.Controls.Add(this.lbfreebiedesc);
            this.pnlservices.Controls.Add(this.lbfreebie);
            this.pnlservices.Location = new System.Drawing.Point(20, 92);
            this.pnlservices.Name = "pnlservices";
            this.pnlservices.Size = new System.Drawing.Size(654, 271);
            this.pnlservices.TabIndex = 5;
            // 
            // lbfreebiedesc
            // 
            this.lbfreebiedesc.Location = new System.Drawing.Point(4, 17);
            this.lbfreebiedesc.Name = "lbfreebiedesc";
            this.lbfreebiedesc.Size = new System.Drawing.Size(361, 105);
            this.lbfreebiedesc.TabIndex = 1;
            this.lbfreebiedesc.Text = resources.GetString("lbfreebiedesc.Text");
            // 
            // lbfreebie
            // 
            this.lbfreebie.AutoSize = true;
            this.lbfreebie.Location = new System.Drawing.Point(4, 4);
            this.lbfreebie.Name = "lbfreebie";
            this.lbfreebie.Size = new System.Drawing.Size(88, 13);
            this.lbfreebie.TabIndex = 0;
            this.lbfreebie.Tag = "header2";
            this.lbfreebie.Text = "Freebie Solutions";
            // 
            // btnjoinfreebie
            // 
            this.btnjoinfreebie.AutoSize = true;
            this.btnjoinfreebie.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnjoinfreebie.Location = new System.Drawing.Point(245, 125);
            this.btnjoinfreebie.Name = "btnjoinfreebie";
            this.btnjoinfreebie.Size = new System.Drawing.Size(120, 23);
            this.btnjoinfreebie.TabIndex = 2;
            this.btnjoinfreebie.Text = "Join Freebie Solutions";
            this.btnjoinfreebie.UseVisualStyleBackColor = true;
            this.btnjoinfreebie.Click += new System.EventHandler(this.btnjoinfreebie_Click);
            // 
            // ShiftSoft
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlservices);
            this.Controls.Add(this.pnlhome);
            this.Controls.Add(this.flbuttons);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pnldivider);
            this.Controls.Add(this.label1);
            this.Name = "ShiftSoft";
            this.Size = new System.Drawing.Size(694, 384);
            this.flbuttons.ResumeLayout(false);
            this.flbuttons.PerformLayout();
            this.pnlhome.ResumeLayout(false);
            this.pnlhome.PerformLayout();
            this.pnlservices.ResumeLayout(false);
            this.pnlservices.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnldivider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flbuttons;
        private System.Windows.Forms.Button btnhome;
        private System.Windows.Forms.Button btnservices;
        private System.Windows.Forms.Button btnping;
        private System.Windows.Forms.Panel pnlhome;
        private System.Windows.Forms.Label lbdesc;
        private System.Windows.Forms.Label lbwhere;
        private System.Windows.Forms.Panel pnlservices;
        private System.Windows.Forms.Label lbfreebiedesc;
        private System.Windows.Forms.Label lbfreebie;
        private System.Windows.Forms.Button btnjoinfreebie;
    }
}
