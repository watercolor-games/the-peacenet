namespace ShiftOS.WinForms.Applications
{
    partial class TriPresent
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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLabel = new System.Windows.Forms.Panel();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.yLabel = new System.Windows.Forms.Label();
            this.xLabel = new System.Windows.Forms.Label();
            this.yPosition = new System.Windows.Forms.NumericUpDown();
            this.xPosition = new System.Windows.Forms.NumericUpDown();
            this.labelContents = new System.Windows.Forms.TextBox();
            this.addItemLabel = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.designerPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.addLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(758, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLabelToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // addLabelToolStripMenuItem
            // 
            this.addLabelToolStripMenuItem.Name = "addLabelToolStripMenuItem";
            this.addLabelToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.addLabelToolStripMenuItem.Text = "Add Label";
            this.addLabelToolStripMenuItem.Click += new System.EventHandler(this.addLabelToolStripMenuItem_Click);
            // 
            // addLabel
            // 
            this.addLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addLabel.Controls.Add(this.checkBox2);
            this.addLabel.Controls.Add(this.checkBox1);
            this.addLabel.Controls.Add(this.label1);
            this.addLabel.Controls.Add(this.panel1);
            this.addLabel.Controls.Add(this.yLabel);
            this.addLabel.Controls.Add(this.xLabel);
            this.addLabel.Controls.Add(this.yPosition);
            this.addLabel.Controls.Add(this.xPosition);
            this.addLabel.Controls.Add(this.labelContents);
            this.addLabel.Controls.Add(this.addItemLabel);
            this.addLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addLabel.Location = new System.Drawing.Point(0, 0);
            this.addLabel.Name = "addLabel";
            this.addLabel.Size = new System.Drawing.Size(252, 456);
            this.addLabel.TabIndex = 1;
            this.addLabel.Visible = false;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.Location = new System.Drawing.Point(6, 163);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(48, 17);
            this.checkBox2.TabIndex = 11;
            this.checkBox2.Text = "Italic";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(6, 140);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(51, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Bold";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(115, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Text Color:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(179, 141);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(35, 20);
            this.panel1.TabIndex = 8;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // yLabel
            // 
            this.yLabel.AutoSize = true;
            this.yLabel.Location = new System.Drawing.Point(200, 96);
            this.yLabel.Name = "yLabel";
            this.yLabel.Size = new System.Drawing.Size(14, 13);
            this.yLabel.TabIndex = 5;
            this.yLabel.Text = "Y";
            // 
            // xLabel
            // 
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(3, 95);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(14, 13);
            this.xLabel.TabIndex = 4;
            this.xLabel.Text = "X";
            // 
            // yPosition
            // 
            this.yPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.yPosition.Location = new System.Drawing.Point(127, 114);
            this.yPosition.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.yPosition.Name = "yPosition";
            this.yPosition.Size = new System.Drawing.Size(87, 20);
            this.yPosition.TabIndex = 3;
            // 
            // xPosition
            // 
            this.xPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xPosition.Location = new System.Drawing.Point(3, 114);
            this.xPosition.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.xPosition.Name = "xPosition";
            this.xPosition.Size = new System.Drawing.Size(87, 20);
            this.xPosition.TabIndex = 2;
            // 
            // labelContents
            // 
            this.labelContents.Location = new System.Drawing.Point(3, 26);
            this.labelContents.Multiline = true;
            this.labelContents.Name = "labelContents";
            this.labelContents.Size = new System.Drawing.Size(211, 67);
            this.labelContents.TabIndex = 1;
            this.labelContents.Text = "Text";
            // 
            // addItemLabel
            // 
            this.addItemLabel.Location = new System.Drawing.Point(0, 0);
            this.addItemLabel.Name = "addItemLabel";
            this.addItemLabel.Size = new System.Drawing.Size(214, 23);
            this.addItemLabel.TabIndex = 0;
            this.addItemLabel.Text = "{ADD_ITEM_LABEL}";
            this.addItemLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.addLabel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.designerPanel);
            this.splitContainer1.Size = new System.Drawing.Size(758, 456);
            this.splitContainer1.SplitterDistance = 252;
            this.splitContainer1.TabIndex = 2;
            // 
            // designerPanel
            // 
            this.designerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.designerPanel.Location = new System.Drawing.Point(0, 0);
            this.designerPanel.Name = "designerPanel";
            this.designerPanel.Size = new System.Drawing.Size(502, 456);
            this.designerPanel.TabIndex = 0;
            this.designerPanel.Click += new System.EventHandler(this.designerPanel_Click);
            // 
            // TriPresent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "TriPresent";
            this.Size = new System.Drawing.Size(758, 480);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.addLabel.ResumeLayout(false);
            this.addLabel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xPosition)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLabelToolStripMenuItem;
        private System.Windows.Forms.Panel addLabel;
        private System.Windows.Forms.Label addItemLabel;
        private System.Windows.Forms.Label yLabel;
        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.NumericUpDown yPosition;
        private System.Windows.Forms.NumericUpDown xPosition;
        private System.Windows.Forms.TextBox labelContents;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel designerPanel;
    }
}
