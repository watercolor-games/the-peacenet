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
            this.pnlstart = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtchatid = new System.Windows.Forms.TextBox();
            this.btnjoin = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbchat = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tschatid = new System.Windows.Forms.ToolStripLabel();
            this.tsuserdata = new System.Windows.Forms.ToolStripLabel();
            this.lbtyping = new System.Windows.Forms.ToolStripLabel();
            this.tsbottombar = new System.Windows.Forms.ToolStrip();
            this.txtuserinput = new System.Windows.Forms.ToolStripTextBox();
            this.btnsend = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnsendfile = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.pnlstart.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tsbottombar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlstart);
            this.panel1.Controls.Add(this.rtbchat);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.tsbottombar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(633, 318);
            this.panel1.TabIndex = 0;
            // 
            // pnlstart
            // 
            this.pnlstart.Controls.Add(this.flowLayoutPanel1);
            this.pnlstart.Controls.Add(this.label3);
            this.pnlstart.Controls.Add(this.label2);
            this.pnlstart.Controls.Add(this.label1);
            this.pnlstart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlstart.Location = new System.Drawing.Point(0, 25);
            this.pnlstart.Name = "pnlstart";
            this.pnlstart.Size = new System.Drawing.Size(633, 268);
            this.pnlstart.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.txtchatid);
            this.flowLayoutPanel1.Controls.Add(this.btnjoin);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 118);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(633, 29);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // txtchatid
            // 
            this.txtchatid.Location = new System.Drawing.Point(3, 3);
            this.txtchatid.Name = "txtchatid";
            this.txtchatid.Size = new System.Drawing.Size(192, 20);
            this.txtchatid.TabIndex = 0;
            // 
            // btnjoin
            // 
            this.btnjoin.Location = new System.Drawing.Point(201, 3);
            this.btnjoin.Name = "btnjoin";
            this.btnjoin.Size = new System.Drawing.Size(75, 23);
            this.btnjoin.TabIndex = 1;
            this.btnjoin.Text = "Join";
            this.btnjoin.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 85);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(10);
            this.label3.Size = new System.Drawing.Size(79, 33);
            this.label3.TabIndex = 2;
            this.label3.Tag = "header3";
            this.label3.Text = "Join a chat";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 33);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(10);
            this.label2.Size = new System.Drawing.Size(633, 52);
            this.label2.TabIndex = 1;
            this.label2.Text = "SimpleSRC is a simple chat program that utilises the ShiftOS Relay Chat protocol." +
    " All you have to do is enter a chat code or system name, and SimpleSRC will try " +
    "to initiate a chat for you.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10);
            this.label1.Size = new System.Drawing.Size(143, 33);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header1";
            this.label1.Text = "Welcome to SimpleSRC!";
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
            this.tsuserdata,
            this.lbtyping,
            this.toolStripSeparator1,
            this.btnsendfile});
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
            // lbtyping
            // 
            this.lbtyping.Name = "lbtyping";
            this.lbtyping.Size = new System.Drawing.Size(86, 22);
            this.lbtyping.Text = "toolStripLabel1";
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnsendfile
            // 
            this.btnsendfile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnsendfile.Image = ((System.Drawing.Image)(resources.GetObject("btnsendfile.Image")));
            this.btnsendfile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnsendfile.Name = "btnsendfile";
            this.btnsendfile.Size = new System.Drawing.Size(65, 22);
            this.btnsendfile.Text = "Send a file";
            this.btnsendfile.Click += new System.EventHandler(this.btnsendfile_Click);
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
            this.pnlstart.ResumeLayout(false);
            this.pnlstart.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
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
        private System.Windows.Forms.Panel pnlstart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox txtchatid;
        private System.Windows.Forms.Button btnjoin;
        private System.Windows.Forms.ToolStripLabel lbtyping;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnsendfile;
    }
}
