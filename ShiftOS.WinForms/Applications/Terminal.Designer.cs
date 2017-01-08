using System.Windows.Forms;

namespace ShiftOS.WinForms.Applications
{
    partial class Terminal
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
            this.rtbterm = new Controls.TerminalBox();
            this.SuspendLayout();
            // 
            // rtbterm
            // 
            this.rtbterm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbterm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbterm.Location = new System.Drawing.Point(0, 0);
            this.rtbterm.Name = "rtbterm";
            this.rtbterm.Size = new System.Drawing.Size(493, 295);
            this.rtbterm.TabIndex = 0;
            this.rtbterm.Text = "";
            this.rtbterm.TextChanged += new System.EventHandler(this.rtbterm_TextChanged);
            // 
            // Terminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbterm);
            this.Name = "Terminal";
            this.Text = "{TERMINAL_NAME}";
            this.Size = new System.Drawing.Size(493, 295);
            this.Tag = "hidden";
            this.Load += new System.EventHandler(this.Terminal_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.TerminalBox rtbterm = new Controls.TerminalBox();
    }
}