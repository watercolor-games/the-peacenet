using System.Windows.Forms;

namespace ShiftOS.WinForms.Controls
{
    partial class ColorControl
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
            this.pnlcolorbox = new Canvas();
            this.SuspendLayout();
            // 
            // pnlcolorbox
            // 
            this.pnlcolorbox.Location = new System.Drawing.Point(37, 18);
            this.pnlcolorbox.Name = "pnlcolorbox";
            this.pnlcolorbox.Size = new System.Drawing.Size(255, 255);
            this.pnlcolorbox.TabIndex = 0;
            // 
            // ColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.pnlcolorbox);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(332, 520);
            this.Load += new System.EventHandler(this.ColorPicker_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Canvas pnlcolorbox;
    }

    internal class Canvas : Panel
    {
        public Canvas() : base()
        {
            DoubleBuffered = true;
        }
    }
}
