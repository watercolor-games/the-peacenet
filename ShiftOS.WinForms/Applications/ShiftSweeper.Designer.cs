namespace ShiftOS.WinForms.Applications
{
    partial class ShiftSweeper
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.minefieldPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonE = new System.Windows.Forms.Button();
            this.buttonM = new System.Windows.Forms.Button();
            this.buttonH = new System.Windows.Forms.Button();
            this.lblmines = new System.Windows.Forms.Label();
            this.lbltime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.SweeperNormalFace;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(150, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // minefieldPanel
            // 
            this.minefieldPanel.ColumnCount = 9;
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.Location = new System.Drawing.Point(4, 40);
            this.minefieldPanel.Name = "minefieldPanel";
            this.minefieldPanel.RowCount = 9;
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.minefieldPanel.Size = new System.Drawing.Size(329, 215);
            this.minefieldPanel.TabIndex = 1;
            // 
            // buttonE
            // 
            this.buttonE.Location = new System.Drawing.Point(4, 261);
            this.buttonE.Name = "buttonE";
            this.buttonE.Size = new System.Drawing.Size(75, 23);
            this.buttonE.TabIndex = 2;
            this.buttonE.Text = "Easy";
            this.buttonE.UseVisualStyleBackColor = true;
            this.buttonE.Click += new System.EventHandler(this.buttonE_Click);
            // 
            // buttonM
            // 
            this.buttonM.Location = new System.Drawing.Point(129, 261);
            this.buttonM.Name = "buttonM";
            this.buttonM.Size = new System.Drawing.Size(75, 23);
            this.buttonM.TabIndex = 3;
            this.buttonM.Text = "Medium";
            this.buttonM.UseVisualStyleBackColor = true;
            this.buttonM.Click += new System.EventHandler(this.buttonM_Click);
            // 
            // buttonH
            // 
            this.buttonH.Location = new System.Drawing.Point(258, 261);
            this.buttonH.Name = "buttonH";
            this.buttonH.Size = new System.Drawing.Size(75, 23);
            this.buttonH.TabIndex = 4;
            this.buttonH.Text = "Hard";
            this.buttonH.UseVisualStyleBackColor = true;
            this.buttonH.Click += new System.EventHandler(this.buttonH_Click);
            // 
            // lblmines
            // 
            this.lblmines.AutoSize = true;
            this.lblmines.Location = new System.Drawing.Point(188, 3);
            this.lblmines.Name = "lblmines";
            this.lblmines.Size = new System.Drawing.Size(47, 13);
            this.lblmines.TabIndex = 5;
            this.lblmines.Text = "Mines: 0";
            // 
            // lbltime
            // 
            this.lbltime.AutoSize = true;
            this.lbltime.Location = new System.Drawing.Point(188, 22);
            this.lbltime.Name = "lbltime";
            this.lbltime.Size = new System.Drawing.Size(42, 13);
            this.lbltime.TabIndex = 6;
            this.lbltime.Text = "Time: 0";
            // 
            // ShiftSweeper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbltime);
            this.Controls.Add(this.lblmines);
            this.Controls.Add(this.buttonH);
            this.Controls.Add(this.buttonM);
            this.Controls.Add(this.buttonE);
            this.Controls.Add(this.minefieldPanel);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ShiftSweeper";
            this.Size = new System.Drawing.Size(366, 328);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel minefieldPanel;
        private System.Windows.Forms.Button buttonE;
        private System.Windows.Forms.Button buttonM;
        private System.Windows.Forms.Button buttonH;
        private System.Windows.Forms.Label lblmines;
        private System.Windows.Forms.Label lbltime;
    }
}
