namespace ShiftOS.WinForms.Applications
{
    partial class Shiftnet
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
            this.flcontrols = new System.Windows.Forms.FlowLayoutPanel();
            this.btnback = new System.Windows.Forms.Button();
            this.btnforward = new System.Windows.Forms.Button();
            this.txturl = new System.Windows.Forms.TextBox();
            this.btngo = new System.Windows.Forms.Button();
            this.wbcanvas = new System.Windows.Forms.WebBrowser();
            this.flcontrols.SuspendLayout();
            this.SuspendLayout();
            // 
            // flcontrols
            // 
            this.flcontrols.AutoSize = true;
            this.flcontrols.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flcontrols.Controls.Add(this.btnback);
            this.flcontrols.Controls.Add(this.btnforward);
            this.flcontrols.Controls.Add(this.txturl);
            this.flcontrols.Controls.Add(this.btngo);
            this.flcontrols.Dock = System.Windows.Forms.DockStyle.Top;
            this.flcontrols.Location = new System.Drawing.Point(0, 0);
            this.flcontrols.Name = "flcontrols";
            this.flcontrols.Size = new System.Drawing.Size(805, 29);
            this.flcontrols.TabIndex = 0;
            // 
            // btnback
            // 
            this.btnback.AutoSize = true;
            this.btnback.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnback.Location = new System.Drawing.Point(3, 3);
            this.btnback.Name = "btnback";
            this.btnback.Size = new System.Drawing.Size(23, 23);
            this.btnback.TabIndex = 0;
            this.btnback.Text = "<";
            this.btnback.UseVisualStyleBackColor = true;
            this.btnback.Click += new System.EventHandler(this.btnback_Click);
            // 
            // btnforward
            // 
            this.btnforward.AutoSize = true;
            this.btnforward.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnforward.Location = new System.Drawing.Point(32, 3);
            this.btnforward.Name = "btnforward";
            this.btnforward.Size = new System.Drawing.Size(23, 23);
            this.btnforward.TabIndex = 1;
            this.btnforward.Text = ">";
            this.btnforward.UseVisualStyleBackColor = true;
            this.btnforward.Click += new System.EventHandler(this.btnforward_Click);
            // 
            // txturl
            // 
            this.txturl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txturl.Location = new System.Drawing.Point(61, 3);
            this.txturl.Name = "txturl";
            this.txturl.Size = new System.Drawing.Size(678, 20);
            this.txturl.TabIndex = 2;
            this.txturl.WordWrap = false;
            this.txturl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txturl_KeyDown);
            // 
            // btngo
            // 
            this.btngo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btngo.AutoSize = true;
            this.btngo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btngo.Location = new System.Drawing.Point(745, 3);
            this.btngo.Name = "btngo";
            this.btngo.Size = new System.Drawing.Size(31, 23);
            this.btngo.TabIndex = 3;
            this.btngo.Text = "Go";
            this.btngo.UseVisualStyleBackColor = true;
            this.btngo.Click += new System.EventHandler(this.btngo_Click);
            // 
            // wbcanvas
            // 
            this.wbcanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbcanvas.IsWebBrowserContextMenuEnabled = false;
            this.wbcanvas.Location = new System.Drawing.Point(0, 29);
            this.wbcanvas.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbcanvas.Name = "wbcanvas";
            this.wbcanvas.ScriptErrorsSuppressed = true;
            this.wbcanvas.Size = new System.Drawing.Size(805, 510);
            this.wbcanvas.TabIndex = 1;
            this.wbcanvas.WebBrowserShortcutsEnabled = false;
            this.wbcanvas.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.wbcanvas_Navigated);
            this.wbcanvas.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbcanvas_Navigating);
            // 
            // Shiftnet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wbcanvas);
            this.Controls.Add(this.flcontrols);
            this.Name = "Shiftnet";
            this.Size = new System.Drawing.Size(805, 539);
            this.flcontrols.ResumeLayout(false);
            this.flcontrols.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flcontrols;
        private System.Windows.Forms.Button btnback;
        private System.Windows.Forms.Button btnforward;
        private System.Windows.Forms.TextBox txturl;
        private System.Windows.Forms.Button btngo;
        private System.Windows.Forms.WebBrowser wbcanvas;
    }
}
