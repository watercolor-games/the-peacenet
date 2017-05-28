namespace ShiftOS.WinForms.ShiftnetSites
{
    partial class AppscapeMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.flcategories = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlappslist = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lbtitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header1";
            this.label1.Text = "Appscape";
            // 
            // flcategories
            // 
            this.flcategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flcategories.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flcategories.Location = new System.Drawing.Point(20, 120);
            this.flcategories.Margin = new System.Windows.Forms.Padding(0);
            this.flcategories.Name = "flcategories";
            this.flcategories.Size = new System.Drawing.Size(187, 310);
            this.flcategories.TabIndex = 1;
            // 
            // pnlappslist
            // 
            this.pnlappslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlappslist.Location = new System.Drawing.Point(221, 120);
            this.pnlappslist.Name = "pnlappslist";
            this.pnlappslist.Size = new System.Drawing.Size(459, 310);
            this.pnlappslist.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Tag = "header2";
            this.label2.Text = "Categories";
            // 
            // lbtitle
            // 
            this.lbtitle.AutoSize = true;
            this.lbtitle.Location = new System.Drawing.Point(218, 76);
            this.lbtitle.Name = "lbtitle";
            this.lbtitle.Size = new System.Drawing.Size(57, 13);
            this.lbtitle.TabIndex = 4;
            this.lbtitle.Tag = "header2";
            this.lbtitle.Text = "Categories";
            // 
            // AppscapeMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbtitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pnlappslist);
            this.Controls.Add(this.flcategories);
            this.Controls.Add(this.label1);
            this.Name = "AppscapeMain";
            this.Size = new System.Drawing.Size(709, 457);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flcategories;
        private System.Windows.Forms.Panel pnlappslist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbtitle;
    }
}
