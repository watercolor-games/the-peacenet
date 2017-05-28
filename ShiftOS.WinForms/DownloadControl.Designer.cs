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

namespace ShiftOS.WinForms
{
    partial class DownloadControl
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
            this.pcicon = new System.Windows.Forms.PictureBox();
            this.lbshiftneturl = new System.Windows.Forms.Label();
            this.pgprogress = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pcicon)).BeginInit();
            this.SuspendLayout();
            // 
            // pcicon
            // 
            this.pcicon.BackColor = System.Drawing.Color.Black;
            this.pcicon.Location = new System.Drawing.Point(4, 4);
            this.pcicon.Name = "pcicon";
            this.pcicon.Size = new System.Drawing.Size(42, 42);
            this.pcicon.TabIndex = 1;
            this.pcicon.TabStop = false;
            // 
            // lbshiftneturl
            // 
            this.lbshiftneturl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbshiftneturl.Location = new System.Drawing.Point(52, 4);
            this.lbshiftneturl.Name = "lbshiftneturl";
            this.lbshiftneturl.Size = new System.Drawing.Size(323, 42);
            this.lbshiftneturl.TabIndex = 2;
            this.lbshiftneturl.Text = "label1";
            this.lbshiftneturl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pgprogress
            // 
            this.pgprogress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgprogress.Location = new System.Drawing.Point(4, 52);
            this.pgprogress.Maximum = 100;
            this.pgprogress.Name = "pgprogress";
            this.pgprogress.Size = new System.Drawing.Size(371, 23);
            this.pgprogress.TabIndex = 0;
            this.pgprogress.Text = "shiftedProgressBar1";
            this.pgprogress.Value = 0;
            // 
            // DownloadControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbshiftneturl);
            this.Controls.Add(this.pcicon);
            this.Controls.Add(this.pgprogress);
            this.Name = "DownloadControl";
            this.Size = new System.Drawing.Size(382, 82);
            ((System.ComponentModel.ISupportInitialize)(this.pcicon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ShiftedProgressBar pgprogress;
        private System.Windows.Forms.PictureBox pcicon;
        private System.Windows.Forms.Label lbshiftneturl;
    }
}
