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
    partial class MUDControlCentre
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.youToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMemos = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectFromMuDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shopsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myShopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tasksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseJobsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtappstatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.you_memos = new System.Windows.Forms.Panel();
            this.flmemos = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.you_systemstatus = new System.Windows.Forms.Panel();
            this.lblsysstatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.legionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createLegionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinLegionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myLegionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lgn_view = new System.Windows.Forms.Panel();
            this.banner = new System.Windows.Forms.Panel();
            this.lblegiontitle = new System.Windows.Forms.Label();
            this.btnleavelegion = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnjoinlegion = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbdescription = new System.Windows.Forms.Label();
            this.pnllgnusers = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lvusers = new System.Windows.Forms.ListView();
            this.lgn_join = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.fllegionlist = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.you_memos.SuspendLayout();
            this.you_systemstatus.SuspendLayout();
            this.lgn_view.SuspendLayout();
            this.banner.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnllgnusers.SuspendLayout();
            this.lgn_join.SuspendLayout();
            this.panel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.youToolStripMenuItem,
            this.shopsToolStripMenuItem,
            this.tasksToolStripMenuItem,
            this.legionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(756, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // youToolStripMenuItem
            // 
            this.youToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.profileToolStripMenuItem,
            this.tsMemos,
            this.disconnectFromMuDToolStripMenuItem});
            this.youToolStripMenuItem.Name = "youToolStripMenuItem";
            this.youToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.youToolStripMenuItem.Text = "You";
            // 
            // profileToolStripMenuItem
            // 
            this.profileToolStripMenuItem.Name = "profileToolStripMenuItem";
            this.profileToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.profileToolStripMenuItem.Text = "System status";
            this.profileToolStripMenuItem.Click += new System.EventHandler(this.profileToolStripMenuItem_Click);
            // 
            // tsMemos
            // 
            this.tsMemos.Name = "tsMemos";
            this.tsMemos.Size = new System.Drawing.Size(192, 22);
            this.tsMemos.Text = "Memos";
            this.tsMemos.Click += new System.EventHandler(this.tsMemos_Click);
            // 
            // disconnectFromMuDToolStripMenuItem
            // 
            this.disconnectFromMuDToolStripMenuItem.Name = "disconnectFromMuDToolStripMenuItem";
            this.disconnectFromMuDToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.disconnectFromMuDToolStripMenuItem.Text = "Disconnect from MUD";
            this.disconnectFromMuDToolStripMenuItem.Click += new System.EventHandler(this.disconnectFromMuDToolStripMenuItem_Click);
            // 
            // shopsToolStripMenuItem
            // 
            this.shopsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.browseToolStripMenuItem,
            this.myShopToolStripMenuItem});
            this.shopsToolStripMenuItem.Name = "shopsToolStripMenuItem";
            this.shopsToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.shopsToolStripMenuItem.Text = "Shops";
            // 
            // browseToolStripMenuItem
            // 
            this.browseToolStripMenuItem.Name = "browseToolStripMenuItem";
            this.browseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.browseToolStripMenuItem.Text = "Browse";
            // 
            // myShopToolStripMenuItem
            // 
            this.myShopToolStripMenuItem.Name = "myShopToolStripMenuItem";
            this.myShopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.myShopToolStripMenuItem.Text = "My Shop";
            // 
            // tasksToolStripMenuItem
            // 
            this.tasksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentTaskToolStripMenuItem,
            this.browseJobsToolStripMenuItem});
            this.tasksToolStripMenuItem.Name = "tasksToolStripMenuItem";
            this.tasksToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.tasksToolStripMenuItem.Text = "Tasks";
            // 
            // currentTaskToolStripMenuItem
            // 
            this.currentTaskToolStripMenuItem.Name = "currentTaskToolStripMenuItem";
            this.currentTaskToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.currentTaskToolStripMenuItem.Text = "Current task";
            // 
            // browseJobsToolStripMenuItem
            // 
            this.browseJobsToolStripMenuItem.Name = "browseJobsToolStripMenuItem";
            this.browseJobsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.browseJobsToolStripMenuItem.Text = "Browse Jobs";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtappstatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(756, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtappstatus
            // 
            this.txtappstatus.Name = "txtappstatus";
            this.txtappstatus.Size = new System.Drawing.Size(118, 17);
            this.txtappstatus.Text = "toolStripStatusLabel1";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lgn_view);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lgn_join);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.you_memos);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.you_systemstatus);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(756, 442);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(756, 488);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // you_memos
            // 
            this.you_memos.Controls.Add(this.flmemos);
            this.you_memos.Controls.Add(this.label3);
            this.you_memos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.you_memos.Location = new System.Drawing.Point(0, 0);
            this.you_memos.Name = "you_memos";
            this.you_memos.Size = new System.Drawing.Size(756, 442);
            this.you_memos.TabIndex = 1;
            // 
            // flmemos
            // 
            this.flmemos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flmemos.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flmemos.Location = new System.Drawing.Point(0, 43);
            this.flmemos.Name = "flmemos";
            this.flmemos.Padding = new System.Windows.Forms.Padding(15);
            this.flmemos.Size = new System.Drawing.Size(756, 399);
            this.flmemos.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(15);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(15);
            this.label3.Size = new System.Drawing.Size(71, 43);
            this.label3.TabIndex = 0;
            this.label3.Tag = "header1";
            this.label3.Text = "Memos";
            // 
            // you_systemstatus
            // 
            this.you_systemstatus.Controls.Add(this.lblsysstatus);
            this.you_systemstatus.Controls.Add(this.label1);
            this.you_systemstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.you_systemstatus.Location = new System.Drawing.Point(0, 0);
            this.you_systemstatus.Name = "you_systemstatus";
            this.you_systemstatus.Size = new System.Drawing.Size(756, 442);
            this.you_systemstatus.TabIndex = 0;
            // 
            // lblsysstatus
            // 
            this.lblsysstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblsysstatus.Location = new System.Drawing.Point(0, 43);
            this.lblsysstatus.Name = "lblsysstatus";
            this.lblsysstatus.Padding = new System.Windows.Forms.Padding(15);
            this.lblsysstatus.Size = new System.Drawing.Size(756, 399);
            this.lblsysstatus.TabIndex = 1;
            this.lblsysstatus.Text = "Username: {username}\r\nSystem name: {sysname}\r\n\r\nCodepoints: {cp}\r\nUpgrades: {boug" +
    "ht}/{available}\r\n\r\nSystem version: {sysver}\r\n\r\nShared scripts: {scripts}\r\n\r\nCurr" +
    "ent legion: {legionname}\r\nRole: {role}";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(15);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(15);
            this.label1.Size = new System.Drawing.Size(56, 43);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header1";
            this.label1.Text = "You";
            // 
            // legionsToolStripMenuItem
            // 
            this.legionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createLegionToolStripMenuItem,
            this.joinLegionToolStripMenuItem,
            this.myLegionToolStripMenuItem});
            this.legionsToolStripMenuItem.Name = "legionsToolStripMenuItem";
            this.legionsToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.legionsToolStripMenuItem.Text = "Legions";
            // 
            // createLegionToolStripMenuItem
            // 
            this.createLegionToolStripMenuItem.Name = "createLegionToolStripMenuItem";
            this.createLegionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.createLegionToolStripMenuItem.Text = "Create Legion";
            // 
            // joinLegionToolStripMenuItem
            // 
            this.joinLegionToolStripMenuItem.Name = "joinLegionToolStripMenuItem";
            this.joinLegionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.joinLegionToolStripMenuItem.Text = "Join Legion";
            this.joinLegionToolStripMenuItem.Click += new System.EventHandler(this.joinLegionToolStripMenuItem_Click);
            // 
            // myLegionToolStripMenuItem
            // 
            this.myLegionToolStripMenuItem.Name = "myLegionToolStripMenuItem";
            this.myLegionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.myLegionToolStripMenuItem.Text = "My Legion";
            this.myLegionToolStripMenuItem.Click += new System.EventHandler(this.myLegionToolStripMenuItem_Click);
            // 
            // lgn_view
            // 
            this.lgn_view.Controls.Add(this.pnllgnusers);
            this.lgn_view.Controls.Add(this.lbdescription);
            this.lgn_view.Controls.Add(this.label2);
            this.lgn_view.Controls.Add(this.banner);
            this.lgn_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lgn_view.Location = new System.Drawing.Point(0, 0);
            this.lgn_view.Name = "lgn_view";
            this.lgn_view.Size = new System.Drawing.Size(756, 442);
            this.lgn_view.TabIndex = 2;
            // 
            // banner
            // 
            this.banner.BackColor = System.Drawing.Color.Blue;
            this.banner.Controls.Add(this.flowLayoutPanel1);
            this.banner.Controls.Add(this.lblegiontitle);
            this.banner.Dock = System.Windows.Forms.DockStyle.Top;
            this.banner.Location = new System.Drawing.Point(0, 0);
            this.banner.Name = "banner";
            this.banner.Size = new System.Drawing.Size(756, 100);
            this.banner.TabIndex = 0;
            this.banner.Tag = "keepbg";
            // 
            // lblegiontitle
            // 
            this.lblegiontitle.AutoSize = true;
            this.lblegiontitle.Location = new System.Drawing.Point(18, 30);
            this.lblegiontitle.Name = "lblegiontitle";
            this.lblegiontitle.Size = new System.Drawing.Size(62, 13);
            this.lblegiontitle.TabIndex = 0;
            this.lblegiontitle.Tag = "header1 keepbg";
            this.lblegiontitle.Text = "Legion Title";
            // 
            // btnleavelegion
            // 
            this.btnleavelegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnleavelegion.AutoSize = true;
            this.btnleavelegion.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnleavelegion.Location = new System.Drawing.Point(99, 3);
            this.btnleavelegion.Name = "btnleavelegion";
            this.btnleavelegion.Size = new System.Drawing.Size(101, 23);
            this.btnleavelegion.TabIndex = 1;
            this.btnleavelegion.Tag = "keepbg";
            this.btnleavelegion.Text = "Leave this Legion";
            this.btnleavelegion.UseVisualStyleBackColor = true;
            this.btnleavelegion.Click += new System.EventHandler(this.btnleavelegion_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnleavelegion);
            this.flowLayoutPanel1.Controls.Add(this.btnjoinlegion);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(550, 68);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(203, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            this.flowLayoutPanel1.Tag = "keepbg";
            // 
            // btnjoinlegion
            // 
            this.btnjoinlegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnjoinlegion.AutoSize = true;
            this.btnjoinlegion.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnjoinlegion.Location = new System.Drawing.Point(3, 3);
            this.btnjoinlegion.Name = "btnjoinlegion";
            this.btnjoinlegion.Size = new System.Drawing.Size(90, 23);
            this.btnjoinlegion.TabIndex = 2;
            this.btnjoinlegion.Tag = "keepbg";
            this.btnjoinlegion.Text = "Join this Legion";
            this.btnjoinlegion.UseVisualStyleBackColor = true;
            this.btnjoinlegion.Click += new System.EventHandler(this.btnjoinlegion_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Tag = "header2";
            this.label2.Text = "Description";
            // 
            // lbdescription
            // 
            this.lbdescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbdescription.Location = new System.Drawing.Point(24, 185);
            this.lbdescription.Name = "lbdescription";
            this.lbdescription.Size = new System.Drawing.Size(354, 231);
            this.lbdescription.TabIndex = 2;
            this.lbdescription.Text = "This is the description of this multi-user domain legion. Keep it descriptive.";
            // 
            // pnllgnusers
            // 
            this.pnllgnusers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnllgnusers.Controls.Add(this.lvusers);
            this.pnllgnusers.Controls.Add(this.label4);
            this.pnllgnusers.Location = new System.Drawing.Point(413, 130);
            this.pnllgnusers.Name = "pnllgnusers";
            this.pnllgnusers.Size = new System.Drawing.Size(337, 286);
            this.pnllgnusers.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 2;
            this.label4.Tag = "header2";
            this.label4.Text = "Users";
            // 
            // lvusers
            // 
            this.lvusers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvusers.FullRowSelect = true;
            this.lvusers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvusers.HideSelection = false;
            this.lvusers.Location = new System.Drawing.Point(0, 13);
            this.lvusers.Name = "lvusers";
            this.lvusers.Size = new System.Drawing.Size(337, 273);
            this.lvusers.TabIndex = 3;
            this.lvusers.UseCompatibleStateImageBehavior = false;
            this.lvusers.View = System.Windows.Forms.View.SmallIcon;
            // 
            // lgn_join
            // 
            this.lgn_join.Controls.Add(this.fllegionlist);
            this.lgn_join.Controls.Add(this.panel3);
            this.lgn_join.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lgn_join.Location = new System.Drawing.Point(0, 0);
            this.lgn_join.Name = "lgn_join";
            this.lgn_join.Size = new System.Drawing.Size(756, 442);
            this.lgn_join.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Blue;
            this.panel3.Controls.Add(this.flowLayoutPanel2);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(756, 100);
            this.panel3.TabIndex = 0;
            this.panel3.Tag = "";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.button1);
            this.flowLayoutPanel2.Controls.Add(this.button2);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(547, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(209, 29);
            this.flowLayoutPanel2.TabIndex = 2;
            this.flowLayoutPanel2.Tag = "keepbg";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.Location = new System.Drawing.Point(100, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 1;
            this.button1.Tag = "keepbg";
            this.button1.Text = "Create new Legion";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.AutoSize = true;
            this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2.Location = new System.Drawing.Point(3, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 23);
            this.button2.TabIndex = 2;
            this.button2.Tag = "keepbg";
            this.button2.Text = "Use invite code";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 0;
            this.label8.Tag = "header1 keepbg";
            this.label8.Text = "Join a Legion";
            // 
            // fllegionlist
            // 
            this.fllegionlist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fllegionlist.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.fllegionlist.Location = new System.Drawing.Point(0, 100);
            this.fllegionlist.Name = "fllegionlist";
            this.fllegionlist.Size = new System.Drawing.Size(756, 342);
            this.fllegionlist.TabIndex = 1;
            // 
            // MUDControlCentre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.toolStripContainer1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "MUDControlCentre";
            this.Text = "MUD Control Centre";
            this.Size = new System.Drawing.Size(756, 488);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.you_memos.ResumeLayout(false);
            this.you_memos.PerformLayout();
            this.you_systemstatus.ResumeLayout(false);
            this.you_systemstatus.PerformLayout();
            this.lgn_view.ResumeLayout(false);
            this.lgn_view.PerformLayout();
            this.banner.ResumeLayout(false);
            this.banner.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.pnllgnusers.ResumeLayout(false);
            this.pnllgnusers.PerformLayout();
            this.lgn_join.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem youToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsMemos;
        private System.Windows.Forms.ToolStripMenuItem disconnectFromMuDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shopsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem myShopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tasksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentTaskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseJobsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel txtappstatus;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel you_systemstatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblsysstatus;
        private System.Windows.Forms.Panel you_memos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel flmemos;
        private System.Windows.Forms.ToolStripMenuItem legionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createLegionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinLegionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem myLegionToolStripMenuItem;
        private System.Windows.Forms.Panel lgn_view;
        private System.Windows.Forms.Panel banner;
        private System.Windows.Forms.Label lblegiontitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnleavelegion;
        private System.Windows.Forms.Button btnjoinlegion;
        private System.Windows.Forms.Panel pnllgnusers;
        private System.Windows.Forms.ListView lvusers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbdescription;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel lgn_join;
        private System.Windows.Forms.FlowLayoutPanel fllegionlist;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label8;
    }
}
