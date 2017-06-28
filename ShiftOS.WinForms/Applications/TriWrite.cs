using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    [FileHandler("TriWrite Document", ".rtf", "fileicontext")]
    [WinOpen("triwrite")]
    [AppscapeEntry("triwrite", "TriWrite", "Part of the trilogy of office applications for enhancement of your system. TriWrite is easliy the best text editor out there for ShiftOS.", 1024, 750, "file_skimmer;textpad", "Office")]
    [DefaultTitle("TriWrite")]
    [Launcher("TriWrite", false, null, "Office")]
    public partial class TriWrite : UserControl, IShiftOSWindow, IFileHandler
    {

        public TriWrite()
        {
            InitializeComponent(); //From the library of babel: "FIRST COMMIT FROM A MAC WOOOO TURIANS ARE COOL"
        }

        public void OpenFile(string file)
        {
            AppearanceManager.SetupWindow(this);
            LoadFile(file);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtcontents.Text = "";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var txt = new List<string>();
            txt.Add(".rtf");

            FileSkimmerBackend.GetFile(txt.ToArray(), FileOpenerStyle.Open, (path) => LoadFile(path));
        }

        public void LoadFile(string file)
        {
            txtcontents.Rtf = Utils.ReadAllText(file);
        }

        public void SaveFile(string file)
        {
            if (file.ToLower().EndsWith(".rtf"))
            {
                Utils.WriteAllText(file, txtcontents.Rtf);
            }
            else
            {
                Utils.WriteAllText(file, txtcontents.Text);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var txt = new List<string>();
            txt.Add(".rtf");
            txt.Add(".txt");
            FileSkimmerBackend.GetFile(txt.ToArray(), FileOpenerStyle.Save, (path) => SaveFile(path));
        }

        public void OnLoad()
        {
            UpdateUI();
        }

        public void OnSkinLoad()
        {
            ResetFonts();
        }

        public void ResetFonts()
        {
            fonts.Items.Clear();
            foreach(var font in FontFamily.Families)
            {
                fonts.Items.Add(font.Name);
            }
            UpdateUI();
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void UpdateUI()
        {
            bold.Checked = txtcontents.SelectionFont.Bold;
            italic.Checked = txtcontents.SelectionFont.Italic;
            underline.Checked = txtcontents.SelectionFont.Underline;
            strikethrough.Checked = txtcontents.SelectionFont.Strikeout;
            fonts.Text = txtcontents.SelectionFont.Name;
            size.Text = txtcontents.SelectionFont.Size.ToString();
            left.Checked = txtcontents.SelectionAlignment == HorizontalAlignment.Left;
            center.Checked = txtcontents.SelectionAlignment == HorizontalAlignment.Center;
            right.Checked = txtcontents.SelectionAlignment == HorizontalAlignment.Right;

        }

        private void txtcontents_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public void SetFontStyle(bool bold, bool italic, bool underline, bool strikethrough)
        {
            FontStyle fs = FontStyle.Regular;
            if (bold)
                fs |= FontStyle.Bold;
            if (italic)
                fs |= FontStyle.Italic;
            if (underline)
                fs |= FontStyle.Underline;
            if (strikethrough)
                fs |= FontStyle.Strikeout;
            txtcontents.SelectionFont = new Font(txtcontents.SelectionFont, fs);
            UpdateUI();
        }

        private void bold_CheckedChanged(object sender, EventArgs e)
        {
            SetFontStyle(bold.Checked, italic.Checked, underline.Checked, strikethrough.Checked);
        }

        public void SetFontFamily(string family, float emSize)
        {
            var style = txtcontents.SelectionFont.Style;
            var size = emSize;
            txtcontents.SelectionFont = new Font(family, size, style);
            UpdateUI();
        }

        private void fonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFontFamily(fonts.Text, Convert.ToSingle(size.Text));
        }

        private void left_Click(object sender, EventArgs e)
        {
            txtcontents.SelectionAlignment = HorizontalAlignment.Left;
            UpdateUI();
        }

        private void center_Click(object sender, EventArgs e)
        {
            txtcontents.SelectionAlignment = HorizontalAlignment.Center;
            UpdateUI();

        }

        private void right_Click(object sender, EventArgs e)
        {
            txtcontents.SelectionAlignment = HorizontalAlignment.Right;
            UpdateUI();

        }

        private void size_Click(object sender, EventArgs e)
        {
            try
            {
                float s = Convert.ToSingle(size.Text);
                if(s != txtcontents.SelectionFont.Size)
                {
                    SetFontFamily(fonts.Text, s);
                }
            }
            catch
            {
                UpdateUI();
            }
        }

        private void txtcontents_TextChanged(object sender, EventArgs e)
        {

        }

        private void bold_Click(object sender, EventArgs e)
        {

        }

        private void italic_Click(object sender, EventArgs e)
        {

        }

        private void size_Click_1(object sender, EventArgs e)
        {

        }
    }
}