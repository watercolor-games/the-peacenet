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
    partial class FileSkimmer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileSkimmer));
            this.lvitems = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pinnedItems = new System.Windows.Forms.TreeView();
            this.lbcurrentfolder = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToRemoteServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlconnect = new System.Windows.Forms.Panel();
            this.flcbuttons = new System.Windows.Forms.FlowLayoutPanel();
            this.btncancel = new System.Windows.Forms.Button();
            this.btnok = new System.Windows.Forms.Button();
            this.pnlcreds = new System.Windows.Forms.Panel();
            this.txtcpass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtcuser = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtcsys = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbcdesc = new System.Windows.Forms.Label();
            this.lbctitle = new System.Windows.Forms.Label();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnlconnect.SuspendLayout();
            this.flcbuttons.SuspendLayout();
            this.pnlcreds.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvitems
            // 
            this.lvitems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvitems.Location = new System.Drawing.Point(149, 0);
            this.lvitems.Name = "lvitems";
            this.lvitems.Size = new System.Drawing.Size(485, 332);
            this.lvitems.TabIndex = 0;
            this.lvitems.UseCompatibleStateImageBehavior = false;
            this.lvitems.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvitems_ItemSelectionChanged);
            this.lvitems.SelectedIndexChanged += new System.EventHandler(this.lvitems_SelectedIndexChanged);
            this.lvitems.DoubleClick += new System.EventHandler(this.lvitems_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvitems);
            this.panel1.Controls.Add(this.pinnedItems);
            this.panel1.Controls.Add(this.lbcurrentfolder);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(634, 345);
            this.panel1.TabIndex = 1;
            // 
            // pinnedItems
            // 
            this.pinnedItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.pinnedItems.Location = new System.Drawing.Point(0, 0);
            this.pinnedItems.Name = "pinnedItems";
            this.pinnedItems.Size = new System.Drawing.Size(149, 332);
            this.pinnedItems.TabIndex = 3;
            this.pinnedItems.Click += new System.EventHandler(this.pinnedItems_Click);
            // 
            // lbcurrentfolder
            // 
            this.lbcurrentfolder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbcurrentfolder.Location = new System.Drawing.Point(0, 332);
            this.lbcurrentfolder.Name = "lbcurrentfolder";
            this.lbcurrentfolder.Size = new System.Drawing.Size(634, 13);
            this.lbcurrentfolder.TabIndex = 1;
            this.lbcurrentfolder.Text = "label1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.connectToRemoteServerToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.moveToolStripMenuItem,
            this.pinToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(634, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // connectToRemoteServerToolStripMenuItem
            // 
            this.connectToRemoteServerToolStripMenuItem.Name = "connectToRemoteServerToolStripMenuItem";
            this.connectToRemoteServerToolStripMenuItem.Size = new System.Drawing.Size(129, 20);
            this.connectToRemoteServerToolStripMenuItem.Text = "Start Remote Session";
            this.connectToRemoteServerToolStripMenuItem.Click += new System.EventHandler(this.connectToRemoteServerToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.moveToolStripMenuItem.Text = "Move";
            this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
            // 
            // pinToolStripMenuItem
            // 
            this.pinToolStripMenuItem.Name = "pinToolStripMenuItem";
            this.pinToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.pinToolStripMenuItem.Text = "Pin";
            this.pinToolStripMenuItem.Click += new System.EventHandler(this.pinToolStripMenuItem_Click);
            // 
            // pnlconnect
            // 
            this.pnlconnect.Controls.Add(this.flcbuttons);
            this.pnlconnect.Controls.Add(this.pnlcreds);
            this.pnlconnect.Controls.Add(this.lbcdesc);
            this.pnlconnect.Controls.Add(this.lbctitle);
            this.pnlconnect.Location = new System.Drawing.Point(100, 27);
            this.pnlconnect.Name = "pnlconnect";
            this.pnlconnect.Size = new System.Drawing.Size(419, 306);
            this.pnlconnect.TabIndex = 4;
            this.pnlconnect.Visible = false;
            // 
            // flcbuttons
            // 
            this.flcbuttons.AutoSize = true;
            this.flcbuttons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flcbuttons.Controls.Add(this.btncancel);
            this.flcbuttons.Controls.Add(this.btnok);
            this.flcbuttons.Location = new System.Drawing.Point(116, 256);
            this.flcbuttons.Name = "flcbuttons";
            this.flcbuttons.Size = new System.Drawing.Size(94, 29);
            this.flcbuttons.TabIndex = 3;
            // 
            // btncancel
            // 
            this.btncancel.AutoSize = true;
            this.btncancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btncancel.Location = new System.Drawing.Point(3, 3);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(50, 23);
            this.btncancel.TabIndex = 0;
            this.btncancel.Text = "Cancel";
            this.btncancel.UseVisualStyleBackColor = true;
            this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // btnok
            // 
            this.btnok.AutoSize = true;
            this.btnok.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnok.Location = new System.Drawing.Point(59, 3);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(32, 23);
            this.btnok.TabIndex = 1;
            this.btnok.Text = "OK";
            this.btnok.UseVisualStyleBackColor = true;
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // pnlcreds
            // 
            this.pnlcreds.Controls.Add(this.txtcpass);
            this.pnlcreds.Controls.Add(this.label3);
            this.pnlcreds.Controls.Add(this.txtcuser);
            this.pnlcreds.Controls.Add(this.label2);
            this.pnlcreds.Controls.Add(this.txtcsys);
            this.pnlcreds.Controls.Add(this.label1);
            this.pnlcreds.Location = new System.Drawing.Point(25, 129);
            this.pnlcreds.Name = "pnlcreds";
            this.pnlcreds.Size = new System.Drawing.Size(300, 104);
            this.pnlcreds.TabIndex = 2;
            // 
            // txtcpass
            // 
            this.txtcpass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtcpass.Location = new System.Drawing.Point(91, 62);
            this.txtcpass.Name = "txtcpass";
            this.txtcpass.Size = new System.Drawing.Size(196, 20);
            this.txtcpass.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password:";
            // 
            // txtcuser
            // 
            this.txtcuser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtcuser.Location = new System.Drawing.Point(91, 36);
            this.txtcuser.Name = "txtcuser";
            this.txtcuser.Size = new System.Drawing.Size(196, 20);
            this.txtcuser.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Username:";
            // 
            // txtcsys
            // 
            this.txtcsys.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtcsys.Location = new System.Drawing.Point(91, 10);
            this.txtcsys.Name = "txtcsys";
            this.txtcsys.Size = new System.Drawing.Size(196, 20);
            this.txtcsys.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "System name:";
            // 
            // lbcdesc
            // 
            this.lbcdesc.Location = new System.Drawing.Point(46, 51);
            this.lbcdesc.Name = "lbcdesc";
            this.lbcdesc.Size = new System.Drawing.Size(357, 54);
            this.lbcdesc.TabIndex = 1;
            this.lbcdesc.Text = resources.GetString("lbcdesc.Text");
            this.lbcdesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbctitle
            // 
            this.lbctitle.AutoSize = true;
            this.lbctitle.Location = new System.Drawing.Point(13, 18);
            this.lbctitle.Name = "lbctitle";
            this.lbctitle.Size = new System.Drawing.Size(133, 13);
            this.lbctitle.TabIndex = 0;
            this.lbctitle.Tag = "header3";
            this.lbctitle.Text = "Connect to Remote Server";
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Visible = false;
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // FileSkimmer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlconnect);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "FileSkimmer";
            this.Size = new System.Drawing.Size(634, 369);
            this.Load += new System.EventHandler(this.FileSkimmer_Load);
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlconnect.ResumeLayout(false);
            this.pnlconnect.PerformLayout();
            this.flcbuttons.ResumeLayout(false);
            this.flcbuttons.PerformLayout();
            this.pnlcreds.ResumeLayout(false);
            this.pnlcreds.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvitems;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbcurrentfolder;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToRemoteServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pinToolStripMenuItem;
        private System.Windows.Forms.TreeView pinnedItems;
        private System.Windows.Forms.Panel pnlconnect;
        private System.Windows.Forms.FlowLayoutPanel flcbuttons;
        private System.Windows.Forms.Button btncancel;
        private System.Windows.Forms.Button btnok;
        private System.Windows.Forms.Panel pnlcreds;
        private System.Windows.Forms.TextBox txtcpass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtcuser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtcsys;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbcdesc;
        private System.Windows.Forms.Label lbctitle;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
    }
}