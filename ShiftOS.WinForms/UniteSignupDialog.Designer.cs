namespace ShiftOS.WinForms
{
    partial class UniteSignupDialog
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
            this.btnlogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtroot = new System.Windows.Forms.TextBox();
            this.txtsys = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtuname = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnlogin
            // 
            this.btnlogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnlogin.Location = new System.Drawing.Point(293, 165);
            this.btnlogin.Name = "btnlogin";
            this.btnlogin.Size = new System.Drawing.Size(75, 23);
            this.btnlogin.TabIndex = 18;
            this.btnlogin.Text = "{GEN_OK}";
            this.btnlogin.UseVisualStyleBackColor = true;
            this.btnlogin.Click += new System.EventHandler(this.btnlogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 13);
            this.label1.TabIndex = 6;
            this.label1.Tag = "header2";
            this.label1.Text = "{TITLE_SYSTEMPREPARATION}";
            // 
            // txtroot
            // 
            this.txtroot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtroot.Location = new System.Drawing.Point(107, 168);
            this.txtroot.Name = "txtroot";
            this.txtroot.Size = new System.Drawing.Size(81, 20);
            this.txtroot.TabIndex = 17;
            this.txtroot.UseSystemPasswordChar = true;
            // 
            // txtsys
            // 
            this.txtsys.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtsys.Location = new System.Drawing.Point(20, 100);
            this.txtsys.Name = "txtsys";
            this.txtsys.Size = new System.Drawing.Size(336, 20);
            this.txtsys.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(17, 123);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(351, 43);
            this.label7.TabIndex = 17;
            this.label7.Text = "You\'ll also need to create a new username and password. You will need this inform" +
    "ation to access your computer, so don\'t forget it!";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(17, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(339, 30);
            this.label5.TabIndex = 14;
            this.label5.Text = "You\'ll need to choose a hostname. This will identify your computer to other membe" +
    "rs of the Digital Society.";
            // 
            // txtuname
            // 
            this.txtuname.Location = new System.Drawing.Point(20, 168);
            this.txtuname.Name = "txtuname";
            this.txtuname.Size = new System.Drawing.Size(81, 20);
            this.txtuname.TabIndex = 16;
            // 
            // UniteSignupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtuname);
            this.Controls.Add(this.txtroot);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtsys);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnlogin);
            this.Controls.Add(this.label1);
            this.Name = "UniteSignupDialog";
            this.Size = new System.Drawing.Size(386, 208);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnlogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtroot;
        private System.Windows.Forms.TextBox txtsys;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtuname;
    }
}
