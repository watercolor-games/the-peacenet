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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            this.panel1 = new System.Windows.Forms.Panel();
            this.rtbchat = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tschatid = new System.Windows.Forms.ToolStripLabel();
            this.tsuserdata = new System.Windows.Forms.ToolStripLabel();
            this.tsbottombar = new System.Windows.Forms.ToolStrip();
            this.txtuserinput = new System.Windows.Forms.ToolStripTextBox();
            this.btnsend = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tsbottombar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rtbchat);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.tsbottombar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(633, 318);
            this.panel1.TabIndex = 0;
            // 
            // rtbchat
            // 
            this.rtbchat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbchat.HideSelection = false;
            this.rtbchat.Location = new System.Drawing.Point(0, 25);
            this.rtbchat.Name = "rtbchat";
            this.rtbchat.Size = new System.Drawing.Size(633, 268);
            this.rtbchat.TabIndex = 1;
            this.rtbchat.Text = "";
            this.rtbchat.TextChanged += new System.EventHandler(this.rtbchat_TextChanged);
            this.rtbchat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox1_KeyDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tschatid,
            this.tsuserdata});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(633, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tschatid
            // 
            this.tschatid.Name = "tschatid";
            this.tschatid.Size = new System.Drawing.Size(86, 22);
            this.tschatid.Text = "toolStripLabel1";
            // 
            // tsuserdata
            // 
            this.tsuserdata.Name = "tsuserdata";
            this.tsuserdata.Size = new System.Drawing.Size(86, 22);
            this.tsuserdata.Text = "toolStripLabel1";
            // 
            // tsbottombar
            // 
            this.tsbottombar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsbottombar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtuserinput,
            this.btnsend});
            this.tsbottombar.Location = new System.Drawing.Point(0, 293);
            this.tsbottombar.Name = "tsbottombar";
            this.tsbottombar.Size = new System.Drawing.Size(633, 25);
            this.tsbottombar.TabIndex = 3;
            this.tsbottombar.Text = "toolStrip2";
            // 
            // txtuserinput
            // 
            this.txtuserinput.AutoSize = false;
            this.txtuserinput.Name = "txtuserinput";
            this.txtuserinput.Size = new System.Drawing.Size(100, 25);
            this.txtuserinput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtuserinput_KeyDown);
            // 
            // btnsend
            // 
            this.btnsend.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnsend.Image = ((System.Drawing.Image)(resources.GetObject("btnsend.Image")));
            this.btnsend.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnsend.Name = "btnsend";
            this.btnsend.Size = new System.Drawing.Size(37, 22);
            this.btnsend.Text = "Send";
            this.btnsend.Click += new System.EventHandler(this.btnsend_Click);
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Chat";
            this.Size = new System.Drawing.Size(633, 318);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tsbottombar.ResumeLayout(false);
            this.tsbottombar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox rtbchat;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel tschatid;
        private System.Windows.Forms.ToolStripLabel tsuserdata;
        private System.Windows.Forms.ToolStrip tsbottombar;
        private System.Windows.Forms.ToolStripTextBox txtuserinput;
        private System.Windows.Forms.ToolStripButton btnsend;
    }
}
