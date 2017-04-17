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

using ShiftOS.WinForms.Controls;

namespace ShiftOS.WinForms
{
    partial class Oobe
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
            this.components = new System.ComponentModel.Container();
            this.lblHijack = new System.Windows.Forms.Label();
            this.conversationtimer = new System.Windows.Forms.Timer(this.components);
            this.textgen = new System.Windows.Forms.Timer(this.components);
            this.lblhackwords = new System.Windows.Forms.Label();
            this.hackeffecttimer = new System.Windows.Forms.Timer(this.components);
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // lblHijack
            // 
            this.lblHijack.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHijack.AutoSize = true;
            this.lblHijack.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblHijack.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHijack.ForeColor = System.Drawing.Color.DimGray;
            this.lblHijack.Location = new System.Drawing.Point(143, 193);
            this.lblHijack.Name = "lblHijack";
            this.lblHijack.Size = new System.Drawing.Size(18, 25);
            this.lblHijack.TabIndex = 0;
            this.lblHijack.Text = "\\";
            // 
            // textgen
            // 
            this.textgen.Interval = 20;
            // 
            // lblhackwords
            // 
            this.lblhackwords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblhackwords.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblhackwords.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblhackwords.Location = new System.Drawing.Point(0, 0);
            this.lblhackwords.Name = "lblhackwords";
            this.lblhackwords.Size = new System.Drawing.Size(653, 457);
            this.lblhackwords.TabIndex = 1;
            this.lblhackwords.Tag = "header2";
            this.lblhackwords.Text = "Hijack in progress";
            // 
            // hackeffecttimer
            // 
            this.hackeffecttimer.Interval = 50;
            // 
            // Oobe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(653, 457);
            this.Controls.Add(this.lblhackwords);
            this.Controls.Add(this.lblHijack);
            this.Name = "Oobe";
            this.Text = "ShiftOS";
            this.TransparencyKey = System.Drawing.Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Label lblHijack;
        internal System.Windows.Forms.Timer conversationtimer;
        internal System.Windows.Forms.Timer textgen;
        internal System.Windows.Forms.Label lblhackwords;
        internal System.Windows.Forms.Timer hackeffecttimer;
        internal System.ComponentModel.BackgroundWorker BackgroundWorker1;
        #endregion
    }
}