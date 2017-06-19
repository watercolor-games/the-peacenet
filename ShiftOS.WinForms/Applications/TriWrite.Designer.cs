namespace ShiftOS.WinForms.Applications
{
    partial class TriWrite
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriWrite));
            this.tvcontacts = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtbody = new System.Windows.Forms.Label();
            this.lbtitle = new System.Windows.Forms.Label();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtcontents = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bold = new System.Windows.Forms.ToolStripButton();
            this.italic = new System.Windows.Forms.ToolStripButton();
            this.underline = new System.Windows.Forms.ToolStripButton();
            this.strikethrough = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fonts = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.left = new System.Windows.Forms.ToolStripButton();
            this.center = new System.Windows.Forms.ToolStripButton();
            this.right = new System.Windows.Forms.ToolStripButton();
            this.size = new System.Windows.Forms.ToolStripTextBox();
            this.panel1.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvcontacts
            // 
            this.tvcontacts.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvcontacts.LineColor = System.Drawing.Color.Empty;
            this.tvcontacts.Location = new System.Drawing.Point(0, 24);
            this.tvcontacts.Name = "tvcontacts";
            this.tvcontacts.Size = new System.Drawing.Size(224, 551);
            this.tvcontacts.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtbody);
            this.panel1.Controls.Add(this.lbtitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(224, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(648, 551);
            this.panel1.TabIndex = 2;
            // 
            // txtbody
            // 
            this.txtbody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbody.Location = new System.Drawing.Point(7, 54);
            this.txtbody.Name = "txtbody";
            this.txtbody.Size = new System.Drawing.Size(626, 481);
            this.txtbody.TabIndex = 1;
            this.txtbody.Text = resources.GetString("txtbody.Text");
            // 
            // lbtitle
            // 
            this.lbtitle.AutoSize = true;
            this.lbtitle.Location = new System.Drawing.Point(7, 4);
            this.lbtitle.Name = "lbtitle";
            this.lbtitle.Size = new System.Drawing.Size(44, 13);
            this.lbtitle.TabIndex = 0;
            this.lbtitle.Tag = "header1";
            this.lbtitle.Text = "TriWrite";
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(652, 24);
            this.menuStrip2.TabIndex = 2;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // txtcontents
            // 
            this.txtcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtcontents.Location = new System.Drawing.Point(0, 49);
            this.txtcontents.Name = "txtcontents";
            this.txtcontents.Size = new System.Drawing.Size(652, 365);
            this.txtcontents.TabIndex = 4;
            this.txtcontents.Text = "";
            this.txtcontents.SelectionChanged += new System.EventHandler(this.txtcontents_SelectionChanged);
            this.txtcontents.TextChanged += new System.EventHandler(this.txtcontents_TextChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bold,
            this.italic,
            this.underline,
            this.strikethrough,
            this.toolStripSeparator1,
            this.fonts,
            this.size,
            this.toolStripSeparator2,
            this.left,
            this.center,
            this.right});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(652, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bold
            // 
            this.bold.CheckOnClick = true;
            this.bold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.bold.Image = ((System.Drawing.Image)(resources.GetObject("bold.Image")));
            this.bold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bold.Name = "bold";
            this.bold.Size = new System.Drawing.Size(23, 22);
            this.bold.Text = "B";
            this.bold.CheckedChanged += new System.EventHandler(this.bold_CheckedChanged);
            this.bold.Click += new System.EventHandler(this.bold_Click);
            // 
            // italic
            // 
            this.italic.CheckOnClick = true;
            this.italic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.italic.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.italic.Image = ((System.Drawing.Image)(resources.GetObject("italic.Image")));
            this.italic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.italic.Name = "italic";
            this.italic.Size = new System.Drawing.Size(23, 22);
            this.italic.Text = "I";
            this.italic.CheckedChanged += new System.EventHandler(this.bold_CheckedChanged);
            this.italic.Click += new System.EventHandler(this.italic_Click);
            // 
            // underline
            // 
            this.underline.CheckOnClick = true;
            this.underline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.underline.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.underline.Image = ((System.Drawing.Image)(resources.GetObject("underline.Image")));
            this.underline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.underline.Name = "underline";
            this.underline.Size = new System.Drawing.Size(23, 22);
            this.underline.Text = "U";
            this.underline.CheckedChanged += new System.EventHandler(this.bold_CheckedChanged);
            // 
            // strikethrough
            // 
            this.strikethrough.CheckOnClick = true;
            this.strikethrough.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.strikethrough.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.strikethrough.Image = ((System.Drawing.Image)(resources.GetObject("strikethrough.Image")));
            this.strikethrough.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.strikethrough.Name = "strikethrough";
            this.strikethrough.Size = new System.Drawing.Size(23, 22);
            this.strikethrough.Text = "S";
            this.strikethrough.CheckedChanged += new System.EventHandler(this.bold_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // fonts
            // 
            this.fonts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fonts.Name = "fonts";
            this.fonts.Size = new System.Drawing.Size(121, 25);
            this.fonts.SelectedIndexChanged += new System.EventHandler(this.fonts_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // left
            // 
            this.left.CheckOnClick = true;
            this.left.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.left.Image = ((System.Drawing.Image)(resources.GetObject("left.Image")));
            this.left.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.left.Name = "left";
            this.left.Size = new System.Drawing.Size(31, 22);
            this.left.Text = "Left";
            this.left.Click += new System.EventHandler(this.left_Click);
            // 
            // center
            // 
            this.center.CheckOnClick = true;
            this.center.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.center.Image = ((System.Drawing.Image)(resources.GetObject("center.Image")));
            this.center.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.center.Name = "center";
            this.center.Size = new System.Drawing.Size(46, 22);
            this.center.Text = "Center";
            this.center.Click += new System.EventHandler(this.center_Click);
            // 
            // right
            // 
            this.right.CheckOnClick = true;
            this.right.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.right.Image = ((System.Drawing.Image)(resources.GetObject("right.Image")));
            this.right.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(39, 22);
            this.right.Text = "Right";
            this.right.Click += new System.EventHandler(this.right_Click);
            // 
            // size
            // 
            this.size.AutoSize = false;
            this.size.MaxLength = 3;
            this.size.Name = "size";
            this.size.Size = new System.Drawing.Size(40, 25);
            this.size.Click += new System.EventHandler(this.size_Click_1);
            this.size.TextChanged += new System.EventHandler(this.size_Click);
            // 
            // TriWrite
            // 
            this.Controls.Add(this.txtcontents);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip2);
            this.Name = "TriWrite";
            this.Size = new System.Drawing.Size(652, 414);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView tvcontacts;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label txtbody;
        private System.Windows.Forms.Label lbtitle;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtcontents;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bold;
        private System.Windows.Forms.ToolStripButton italic;
        private System.Windows.Forms.ToolStripButton underline;
        private System.Windows.Forms.ToolStripButton strikethrough;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox fonts;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton left;
        private System.Windows.Forms.ToolStripButton center;
        private System.Windows.Forms.ToolStripButton right;
        private System.Windows.Forms.ToolStripTextBox size;
    }
}
