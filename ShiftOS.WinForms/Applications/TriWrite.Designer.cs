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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddressBook));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addContactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tvcontacts = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtbody = new System.Windows.Forms.Label();
            this.lbtitle = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addContactToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(872, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tvcontacts
            // 
            this.tvcontacts.Dock = System.Windows.Forms.DockStyle.Left;
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
            this.lbtitle.Size = new System.Drawing.Size(73, 13);
            this.lbtitle.TabIndex = 0;
            this.lbtitle.Tag = "header1";
            this.lbtitle.Text = "TriWrite";

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addContactToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.TreeView tvcontacts;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label txtbody;
        private System.Windows.Forms.Label lbtitle;
    }
}
