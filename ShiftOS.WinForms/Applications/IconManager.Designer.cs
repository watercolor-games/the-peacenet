namespace ShiftOS.WinForms.Applications
{
    partial class IconManager
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnclose = new System.Windows.Forms.Button();
            this.btnreset = new System.Windows.Forms.Button();
            this.btnapply = new System.Windows.Forms.Button();
            this.lbcurrentpage = new System.Windows.Forms.Label();
            this.btnprev = new System.Windows.Forms.Button();
            this.btnnext = new System.Windows.Forms.Button();
            this.flbody = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnclose);
            this.flowLayoutPanel1.Controls.Add(this.btnreset);
            this.flowLayoutPanel1.Controls.Add(this.btnapply);
            this.flowLayoutPanel1.Controls.Add(this.lbcurrentpage);
            this.flowLayoutPanel1.Controls.Add(this.btnprev);
            this.flowLayoutPanel1.Controls.Add(this.btnnext);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 416);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(393, 29);
            this.flowLayoutPanel1.TabIndex = 0;
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
            // btnreset
            // 
            this.btnreset.AutoSize = true;
            this.btnreset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnreset.Location = new System.Drawing.Point(52, 3);
            this.btnreset.Name = "btnreset";
            this.btnreset.Size = new System.Drawing.Size(45, 23);
            this.btnreset.TabIndex = 1;
            this.btnreset.Text = "Reset";
            this.btnreset.UseVisualStyleBackColor = true;
            this.btnreset.Click += new System.EventHandler(this.btnreset_Click);
            // 
            // btnapply
            // 
            this.btnapply.AutoSize = true;
            this.btnapply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnapply.Location = new System.Drawing.Point(103, 3);
            this.btnapply.Name = "btnapply";
            this.btnapply.Size = new System.Drawing.Size(43, 23);
            this.btnapply.TabIndex = 2;
            this.btnapply.Text = "Apply";
            this.btnapply.UseVisualStyleBackColor = true;
            this.btnapply.Click += new System.EventHandler(this.btnapply_Click);
            // 
            // lbcurrentpage
            // 
            this.lbcurrentpage.AutoSize = true;
            this.lbcurrentpage.Location = new System.Drawing.Point(152, 0);
            this.lbcurrentpage.Name = "lbcurrentpage";
            this.lbcurrentpage.Size = new System.Drawing.Size(71, 13);
            this.lbcurrentpage.TabIndex = 3;
            this.lbcurrentpage.Text = "Current page:";
            // 
            // btnprev
            // 
            this.btnprev.AutoSize = true;
            this.btnprev.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnprev.Location = new System.Drawing.Point(229, 3);
            this.btnprev.Name = "btnprev";
            this.btnprev.Size = new System.Drawing.Size(51, 23);
            this.btnprev.TabIndex = 4;
            this.btnprev.Text = " < Prev";
            this.btnprev.UseVisualStyleBackColor = true;
            this.btnprev.Click += new System.EventHandler(this.btnprev_Click);
            // 
            // btnnext
            // 
            this.btnnext.AutoSize = true;
            this.btnnext.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnnext.Location = new System.Drawing.Point(286, 3);
            this.btnnext.Name = "btnnext";
            this.btnnext.Size = new System.Drawing.Size(48, 23);
            this.btnnext.TabIndex = 5;
            this.btnnext.Text = "Next >";
            this.btnnext.UseVisualStyleBackColor = true;
            this.btnnext.Click += new System.EventHandler(this.btnnext_Click);
            // 
            // flbody
            // 
            this.flbody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flbody.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flbody.Location = new System.Drawing.Point(0, 0);
            this.flbody.Name = "flbody";
            this.flbody.Size = new System.Drawing.Size(393, 416);
            this.flbody.TabIndex = 1;
            this.flbody.WrapContents = false;
            // 
            // IconManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flbody);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "IconManager";
            this.Size = new System.Drawing.Size(393, 445);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Button btnreset;
        private System.Windows.Forms.Button btnapply;
        private System.Windows.Forms.FlowLayoutPanel flbody;
        private System.Windows.Forms.Label lbcurrentpage;
        private System.Windows.Forms.Button btnprev;
        private System.Windows.Forms.Button btnnext;
    }
}
