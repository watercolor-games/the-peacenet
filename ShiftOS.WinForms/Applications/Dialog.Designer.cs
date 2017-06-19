/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace ShiftOS.WinForms.Applications
{
    partial class Dialog
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
            this.panel1.Size = new System.Drawing.Size(341, 177);
            this.panel1.TabIndex = 0;
            // 
            // txtinput
            // 
            this.txtinput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtinput.Location = new System.Drawing.Point(88, 116);
            this.txtinput.Name = "txtinput";
            this.txtinput.Size = new System.Drawing.Size(250, 20);
            this.txtinput.TabIndex = 4;
            // 
            // lbmessage
            // 
            this.lbmessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbmessage.Location = new System.Drawing.Point(85, 19);
            this.lbmessage.Name = "lbmessage";
            this.lbmessage.Size = new System.Drawing.Size(253, 94);
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
            this.flyesno.Location = new System.Drawing.Point(129, 134);
            this.flyesno.Name = "flyesno";
            this.flyesno.Size = new System.Drawing.Size(157, 29);
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
            this.btnyes.Size = new System.Drawing.Size(75, 23);
            this.btnyes.TabIndex = 4;
            this.btnyes.Text = "{GEN_YES}";
            this.btnyes.UseVisualStyleBackColor = true;
            // 
            // btnno
            // 
            this.btnno.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnno.AutoSize = true;
            this.btnno.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnno.Location = new System.Drawing.Point(84, 3);
            this.btnno.Name = "btnno";
            this.btnno.Size = new System.Drawing.Size(70, 23);
            this.btnno.TabIndex = 3;
            this.btnno.Text = "{GEN_NO}";
            this.btnno.UseVisualStyleBackColor = true;
            // 
            // btnok
            // 
            this.btnok.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnok.AutoSize = true;
            this.btnok.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnok.Location = new System.Drawing.Point(149, 140);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(69, 23);
            this.btnok.TabIndex = 1;
            this.btnok.Text = "{GEN_OK}";
            this.btnok.UseVisualStyleBackColor = true;
            // 
            // pbicon
            // 
            this.pbicon.Location = new System.Drawing.Point(14, 19);
            this.pbicon.Name = "pbicon";
            this.pbicon.Size = new System.Drawing.Size(64, 64);
            this.pbicon.TabIndex = 0;
            this.pbicon.TabStop = false;
            // 
            // Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Dialog";
            this.Size = new System.Drawing.Size(341, 177);
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