namespace ShiftOS.WinForms.Applications
{
    partial class WidgetManagerFrontend
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
            this.flbody = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnclose = new System.Windows.Forms.Button();
            this.btnloaddefault = new System.Windows.Forms.Button();
            this.btnimport = new System.Windows.Forms.Button();
            this.btnexport = new System.Windows.Forms.Button();
            this.btnapply = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flbody
            // 
            this.flbody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flbody.Location = new System.Drawing.Point(22, 59);
            this.flbody.Name = "flbody";
            this.flbody.Size = new System.Drawing.Size(463, 389);
            this.flbody.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Tag = "header1";
            this.label1.Text = "Desktop Widgets";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnclose);
            this.flowLayoutPanel1.Controls.Add(this.btnloaddefault);
            this.flowLayoutPanel1.Controls.Add(this.btnimport);
            this.flowLayoutPanel1.Controls.Add(this.btnexport);
            this.flowLayoutPanel1.Controls.Add(this.btnapply);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 461);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(517, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btnclose
            // 
            this.btnclose.AutoSize = true;
            this.btnclose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnclose.Location = new System.Drawing.Point(3, 3);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(43, 23);
            this.btnclose.TabIndex = 0;
            this.btnclose.Text = "Close";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // btnloaddefault
            // 
            this.btnloaddefault.AutoSize = true;
            this.btnloaddefault.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnloaddefault.Location = new System.Drawing.Point(52, 3);
            this.btnloaddefault.Name = "btnloaddefault";
            this.btnloaddefault.Size = new System.Drawing.Size(76, 23);
            this.btnloaddefault.TabIndex = 1;
            this.btnloaddefault.Text = "Load default";
            this.btnloaddefault.UseVisualStyleBackColor = true;
            this.btnloaddefault.Click += new System.EventHandler(this.btnloaddefault_Click);
            // 
            // btnimport
            // 
            this.btnimport.AutoSize = true;
            this.btnimport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnimport.Location = new System.Drawing.Point(134, 3);
            this.btnimport.Name = "btnimport";
            this.btnimport.Size = new System.Drawing.Size(46, 23);
            this.btnimport.TabIndex = 2;
            this.btnimport.Text = "Import";
            this.btnimport.UseVisualStyleBackColor = true;
            this.btnimport.Click += new System.EventHandler(this.btnimport_Click);
            // 
            // btnexport
            // 
            this.btnexport.AutoSize = true;
            this.btnexport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnexport.Location = new System.Drawing.Point(186, 3);
            this.btnexport.Name = "btnexport";
            this.btnexport.Size = new System.Drawing.Size(47, 23);
            this.btnexport.TabIndex = 3;
            this.btnexport.Text = "Export";
            this.btnexport.UseVisualStyleBackColor = true;
            this.btnexport.Click += new System.EventHandler(this.btnexport_Click);
            // 
            // btnapply
            // 
            this.btnapply.AutoSize = true;
            this.btnapply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnapply.Location = new System.Drawing.Point(239, 3);
            this.btnapply.Name = "btnapply";
            this.btnapply.Size = new System.Drawing.Size(43, 23);
            this.btnapply.TabIndex = 4;
            this.btnapply.Text = "Apply";
            this.btnapply.UseVisualStyleBackColor = true;
            this.btnapply.Click += new System.EventHandler(this.btnapply_Click);
            // 
            // WidgetManagerFrontend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flbody);
            this.Name = "WidgetManagerFrontend";
            this.Size = new System.Drawing.Size(517, 490);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flbody;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Button btnloaddefault;
        private System.Windows.Forms.Button btnimport;
        private System.Windows.Forms.Button btnexport;
        private System.Windows.Forms.Button btnapply;
    }
}
