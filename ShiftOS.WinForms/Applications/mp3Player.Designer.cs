namespace ShiftOS.Engine
{
    partial class UserControl1
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.mp3FilePath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.stopMp3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mp3FilePath
            // 
            this.mp3FilePath.Location = new System.Drawing.Point(131, 8);
            this.mp3FilePath.Name = "mp3FilePath";
            this.mp3FilePath.ReadOnly = true;
            this.mp3FilePath.Size = new System.Drawing.Size(239, 20);
            this.mp3FilePath.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 21);
            this.button1.TabIndex = 2;
            this.button1.Text = "Choose Song";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // stopMp3
            // 
            this.stopMp3.Location = new System.Drawing.Point(445, 8);
            this.stopMp3.Name = "stopMp3";
            this.stopMp3.Size = new System.Drawing.Size(65, 21);
            this.stopMp3.TabIndex = 3;
            this.stopMp3.Text = "Stop";
            this.stopMp3.UseVisualStyleBackColor = true;
            this.stopMp3.Click += new System.EventHandler(this.stopMp3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(376, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 21);
            this.button2.TabIndex = 4;
            this.button2.Text = "Play";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.stopMp3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mp3FilePath);
            this.Name = "UserControl1";
            this.Text = "{WAV_PLAYER_NAME}";
            this.Size = new System.Drawing.Size(530, 70);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox mp3FilePath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button stopMp3;
        private System.Windows.Forms.Button button2;
    }
}
