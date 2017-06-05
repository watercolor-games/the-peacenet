namespace ShiftOS.WinForms.Applications
{
    partial class WebBrowser
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
            this.pnlcanvas = new System.Windows.Forms.Panel();
            this.wbmain = new System.Windows.Forms.WebBrowser();
            this.flcontrols.SuspendLayout();
            this.pnlcanvas.SuspendLayout();
            this.SuspendLayout();
            // 
            // flcontrols
            // 
            this.flcontrols.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flcontrols.Controls.Add(this.btnback);
            this.flcontrols.Controls.Add(this.btnforward);
            this.flcontrols.Controls.Add(this.txturl);
            this.flcontrols.Controls.Add(this.btngo);
            this.flcontrols.Dock = System.Windows.Forms.DockStyle.Top;
            this.flcontrols.Location = new System.Drawing.Point(0, 0);
            this.flcontrols.Name = "flcontrols";
            this.flcontrols.Size = new System.Drawing.Size(539, 29);
            this.flcontrols.TabIndex = 2;
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
            this.txturl.Size = new System.Drawing.Size(435, 20);
            this.txturl.TabIndex = 2;
            this.txturl.WordWrap = false;
            this.txturl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txturl_KeyDown);
            // 
            // btngo
            // 
            this.btngo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btngo.AutoSize = true;
            this.btngo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btngo.Location = new System.Drawing.Point(502, 3);
            this.btngo.Name = "btngo";
            this.btngo.Size = new System.Drawing.Size(31, 23);
            this.btngo.TabIndex = 3;
            this.btngo.Text = "Go";
            this.btngo.UseVisualStyleBackColor = true;
            this.btngo.Click += new System.EventHandler(this.btngo_Click);
            // 
            // pnlcanvas
            // 
            this.pnlcanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlcanvas.Controls.Add(this.wbmain);
            this.pnlcanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlcanvas.Location = new System.Drawing.Point(0, 29);
            this.pnlcanvas.Name = "pnlcanvas";
            this.pnlcanvas.Size = new System.Drawing.Size(539, 374);
            this.pnlcanvas.TabIndex = 3;
            // 
            // wbmain
            // 
            this.wbmain.AllowWebBrowserDrop = false;
            this.wbmain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbmain.IsWebBrowserContextMenuEnabled = false;
            this.wbmain.Location = new System.Drawing.Point(0, 0);
            this.wbmain.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbmain.Name = "wbmain";
            this.wbmain.ScriptErrorsSuppressed = true;
            this.wbmain.Size = new System.Drawing.Size(537, 372);
            this.wbmain.TabIndex = 0;
            this.wbmain.WebBrowserShortcutsEnabled = false;
            this.wbmain.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.wbmain_Navigated);
            this.wbmain.NewWindow += new System.ComponentModel.CancelEventHandler(this.wbmain_NewWindow);
            // 
            // WebBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlcanvas);
            this.Controls.Add(this.flcontrols);
            this.Name = "WebBrowser";
            this.Size = new System.Drawing.Size(539, 403);
            this.flcontrols.ResumeLayout(false);
            this.flcontrols.PerformLayout();
            this.pnlcanvas.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flcontrols;
        private System.Windows.Forms.Button btnback;
        private System.Windows.Forms.Button btnforward;
        private System.Windows.Forms.TextBox txturl;
        private System.Windows.Forms.Button btngo;
        private System.Windows.Forms.Panel pnlcanvas;
        private System.Windows.Forms.WebBrowser wbmain;
    }
}
