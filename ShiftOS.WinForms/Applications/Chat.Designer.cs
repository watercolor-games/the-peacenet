namespace ShiftOS.WinForms.Applications
{
    partial class Chat
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtuserinput = new System.Windows.Forms.TextBox();
            this.rtbchat = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rtbchat);
            this.panel1.Controls.Add(this.txtuserinput);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(633, 318);
            this.panel1.TabIndex = 0;
            // 
            // txtuserinput
            // 
            this.txtuserinput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtuserinput.Location = new System.Drawing.Point(0, 298);
            this.txtuserinput.Name = "txtuserinput";
            this.txtuserinput.Size = new System.Drawing.Size(633, 20);
            this.txtuserinput.TabIndex = 0;
            this.txtuserinput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtuserinput_KeyDown);
            // 
            // rtbchat
            // 
            this.rtbchat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbchat.Location = new System.Drawing.Point(0, 0);
            this.rtbchat.Name = "rtbchat";
            this.rtbchat.Size = new System.Drawing.Size(633, 298);
            this.rtbchat.TabIndex = 1;
            this.rtbchat.Text = "";
            this.rtbchat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox1_KeyDown);
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Chat";
            this.Text = "Chat";
            this.Size = new System.Drawing.Size(633, 318);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox rtbchat;
        private System.Windows.Forms.TextBox txtuserinput;
    }
}
