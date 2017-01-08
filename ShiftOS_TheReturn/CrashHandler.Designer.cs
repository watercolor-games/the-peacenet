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

namespace ShiftOS.Engine
{
    partial class CrashHandler
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrashHandler));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rtbcrash = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnjump = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F);
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(726, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "The ShiftOS client has experienced a fatal bug.";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(20, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(800, 136);
            this.label2.TabIndex = 1;
            this.label2.Text = @"We are terribly sorry for interrupting your gameplay, but unfortunately ShiftOS encountered a major problem that it couldn't recover from. This can be caused by any of the following reasons:

 - Broken MUD scripts (check with the script author if you see this error after running a script)
 - A bug in the ShiftOS code
 - Incorrect or malformed server message (also could be a bug in ShiftOS.)

We were safely able to save your game and disconnect you from the multi-user domain. We have also sent the crash info to the developers of ShiftOS for you. Below is a copy of what we sent for your reference:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.ImageLocation = "https://archive.raytron.org/shiftos/styles/black_pearl/imageset/shiftos.png";
            this.pictureBox1.Location = new System.Drawing.Point(12, 560);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(414, 105);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // rtbcrash
            // 
            this.rtbcrash.BackColor = System.Drawing.Color.Black;
            this.rtbcrash.EnableAutoDragDrop = true;
            this.rtbcrash.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.rtbcrash.ForeColor = System.Drawing.Color.Gray;
            this.rtbcrash.Location = new System.Drawing.Point(20, 214);
            this.rtbcrash.Name = "rtbcrash";
            this.rtbcrash.Size = new System.Drawing.Size(800, 340);
            this.rtbcrash.TabIndex = 3;
            this.rtbcrash.Text = "";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(444, 560);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(382, 47);
            this.label3.TabIndex = 4;
            this.label3.Text = "You may click \"Jump back in\" to attempt to recover the session. If you experience" +
    " this screen again, try quitting the game and relaunching. If that doesn\'t help," +
    " there may be something terribly wrong.";
            // 
            // btnjump
            // 
            this.btnjump.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnjump.Location = new System.Drawing.Point(736, 642);
            this.btnjump.Name = "btnjump";
            this.btnjump.Size = new System.Drawing.Size(84, 23);
            this.btnjump.TabIndex = 5;
            this.btnjump.Text = "Jump back in";
            this.btnjump.UseVisualStyleBackColor = true;
            this.btnjump.Click += new System.EventHandler(this.btnjump_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(646, 642);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Close ShiftOS";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CrashHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(838, 677);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnjump);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rtbcrash);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "CrashHandler";
            this.Text = "CrashHandler";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox rtbcrash;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnjump;
        private System.Windows.Forms.Button button1;
    }
}