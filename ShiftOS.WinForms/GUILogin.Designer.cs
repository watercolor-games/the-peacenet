namespace ShiftOS.WinForms
{
    partial class GUILogin
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
            this.pnlloginform = new System.Windows.Forms.Panel();
            this.btnlogin = new System.Windows.Forms.Button();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.lblogintitle = new System.Windows.Forms.Label();
            this.btnshutdown = new System.Windows.Forms.Button();
            this.pnlloginform.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlloginform
            // 
            this.pnlloginform.Controls.Add(this.btnlogin);
            this.pnlloginform.Controls.Add(this.txtpassword);
            this.pnlloginform.Controls.Add(this.txtusername);
            this.pnlloginform.Location = new System.Drawing.Point(13, 13);
            this.pnlloginform.Name = "pnlloginform";
            this.pnlloginform.Size = new System.Drawing.Size(358, 236);
            this.pnlloginform.TabIndex = 0;
            this.pnlloginform.Tag = "";
            // 
            // btnlogin
            // 
            this.btnlogin.AutoSize = true;
            this.btnlogin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnlogin.Location = new System.Drawing.Point(159, 163);
            this.btnlogin.Name = "btnlogin";
            this.btnlogin.Size = new System.Drawing.Size(43, 23);
            this.btnlogin.TabIndex = 2;
            this.btnlogin.Text = "Login";
            this.btnlogin.UseVisualStyleBackColor = true;
            this.btnlogin.Click += new System.EventHandler(this.btnlogin_Click);
            // 
            // txtpassword
            // 
            this.txtpassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtpassword.Location = new System.Drawing.Point(69, 115);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.Size = new System.Drawing.Size(300, 20);
            this.txtpassword.TabIndex = 1;
            this.txtpassword.UseSystemPasswordChar = true;
            // 
            // txtusername
            // 
            this.txtusername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtusername.Location = new System.Drawing.Point(3, 14);
            this.txtusername.Name = "txtusername";
            this.txtusername.Size = new System.Drawing.Size(300, 20);
            this.txtusername.TabIndex = 0;
            // 
            // lblogintitle
            // 
            this.lblogintitle.AutoSize = true;
            this.lblogintitle.Location = new System.Drawing.Point(99, 553);
            this.lblogintitle.Name = "lblogintitle";
            this.lblogintitle.Size = new System.Drawing.Size(103, 13);
            this.lblogintitle.TabIndex = 1;
            this.lblogintitle.Tag = "header1 keepbg";
            this.lblogintitle.Text = "Welcome to ShiftOS";
            // 
            // btnshutdown
            // 
            this.btnshutdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnshutdown.AutoSize = true;
            this.btnshutdown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnshutdown.Location = new System.Drawing.Point(924, 652);
            this.btnshutdown.Name = "btnshutdown";
            this.btnshutdown.Size = new System.Drawing.Size(65, 23);
            this.btnshutdown.TabIndex = 2;
            this.btnshutdown.Text = "Shutdown";
            this.btnshutdown.UseVisualStyleBackColor = true;
            this.btnshutdown.Click += new System.EventHandler(this.btnshutdown_Click);
            // 
            // GUILogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1001, 687);
            this.Controls.Add(this.btnshutdown);
            this.Controls.Add(this.lblogintitle);
            this.Controls.Add(this.pnlloginform);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "GUILogin";
            this.Text = "GUILogin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GUILogin_FormClosing);
            this.Load += new System.EventHandler(this.GUILogin_Load);
            this.pnlloginform.ResumeLayout(false);
            this.pnlloginform.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlloginform;
        private System.Windows.Forms.Button btnlogin;
        private System.Windows.Forms.TextBox txtpassword;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.Label lblogintitle;
        private System.Windows.Forms.Button btnshutdown;
    }
}