namespace ShiftOS.MFSProfiler
{
    partial class Infobox
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
            this.txtinput = new System.Windows.Forms.TextBox();
            this.lbmessage = new System.Windows.Forms.Label();
            this.flyesno = new System.Windows.Forms.FlowLayoutPanel();
            this.btnyes = new System.Windows.Forms.Button();
            this.btnno = new System.Windows.Forms.Button();
            this.btnok = new System.Windows.Forms.Button();
            this.pbicon = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.flyesno.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbicon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtinput);
            this.panel1.Controls.Add(this.lbmessage);
            this.panel1.Controls.Add(this.flyesno);
            this.panel1.Controls.Add(this.btnok);
            this.panel1.Controls.Add(this.pbicon);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(325, 138);
            this.panel1.TabIndex = 0;
            // 
            // txtinput
            // 
            this.txtinput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtinput.Location = new System.Drawing.Point(88, 77);
            this.txtinput.Name = "txtinput";
            this.txtinput.Size = new System.Drawing.Size(234, 20);
            this.txtinput.TabIndex = 4;
            // 
            // lbmessage
            // 
            this.lbmessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbmessage.Location = new System.Drawing.Point(85, 19);
            this.lbmessage.Name = "lbmessage";
            this.lbmessage.Size = new System.Drawing.Size(237, 55);
            this.lbmessage.TabIndex = 3;
            this.lbmessage.Text = "label1";
            this.lbmessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flyesno
            // 
            this.flyesno.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flyesno.AutoSize = true;
            this.flyesno.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flyesno.Controls.Add(this.btnyes);
            this.flyesno.Controls.Add(this.btnno);
            this.flyesno.Location = new System.Drawing.Point(129, 95);
            this.flyesno.Name = "flyesno";
            this.flyesno.Size = new System.Drawing.Size(78, 29);
            this.flyesno.TabIndex = 2;
            this.flyesno.WrapContents = false;
            // 
            // btnyes
            // 
            this.btnyes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnyes.AutoSize = true;
            this.btnyes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnyes.Location = new System.Drawing.Point(3, 3);
            this.btnyes.Name = "btnyes";
            this.btnyes.Size = new System.Drawing.Size(35, 23);
            this.btnyes.TabIndex = 4;
            this.btnyes.Text = "Yes";
            this.btnyes.UseVisualStyleBackColor = true;
            this.btnyes.Click += new System.EventHandler(this.btnyes_Click);
            // 
            // btnno
            // 
            this.btnno.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnno.AutoSize = true;
            this.btnno.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnno.Location = new System.Drawing.Point(44, 3);
            this.btnno.Name = "btnno";
            this.btnno.Size = new System.Drawing.Size(31, 23);
            this.btnno.TabIndex = 3;
            this.btnno.Text = "No";
            this.btnno.UseVisualStyleBackColor = true;
            this.btnno.Click += new System.EventHandler(this.btnno_Click);
            // 
            // btnok
            // 
            this.btnok.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnok.AutoSize = true;
            this.btnok.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnok.Location = new System.Drawing.Point(149, 101);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(32, 23);
            this.btnok.TabIndex = 1;
            this.btnok.Text = "OK";
            this.btnok.UseVisualStyleBackColor = true;
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // pbicon
            // 
            this.pbicon.Location = new System.Drawing.Point(14, 19);
            this.pbicon.Name = "pbicon";
            this.pbicon.Size = new System.Drawing.Size(64, 64);
            this.pbicon.TabIndex = 0;
            this.pbicon.TabStop = false;
            // 
            // Infobox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 138);
            this.Controls.Add(this.panel1);
            this.Name = "Infobox";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flyesno.ResumeLayout(false);
            this.flyesno.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbicon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbmessage;
        private System.Windows.Forms.FlowLayoutPanel flyesno;
        private System.Windows.Forms.Button btnyes;
        private System.Windows.Forms.Button btnno;
        private System.Windows.Forms.Button btnok;
        private System.Windows.Forms.PictureBox pbicon;
        private System.Windows.Forms.TextBox txtinput;
    }
}