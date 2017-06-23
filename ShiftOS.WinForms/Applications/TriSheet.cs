#define BETA_2_5

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("trisheet")]
    [AppscapeEntry("trisheet", "TriSheet", "Part of the trilogy of office applications for enhancement of your system. TriSheet is easliy the best spreadsheet program out there for ShiftOS.", 1024, 750, "file_skimmer;textpad", "Office")]
    [DefaultTitle("TriSheet")]
    [Launcher("TriSheet", false, null, "Office")]
    public partial class TriSheet : UserControl, IShiftOSWindow
    {
        public TriSheet()
        {
            InitializeComponent();
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
#if YALL_GOT_ANY_MORE_OF_THEM_NONEXISTENT_CONTROLS
            fonts.Items.Clear();
            fonts.Items.AddRange(FontFamily.Families.Select(f => f.Name));
#endif
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
            try
            {
                bold.Checked = dataGridView1.CurrentCell.Style.Font.Bold;
                italic.Checked = dataGridView1.CurrentCell.Style.Font.Italic;
                underline.Checked = dataGridView1.CurrentCell.Style.Font.Underline;
                strikethrough.Checked = dataGridView1.CurrentCell.Style.Font.Strikeout;
                left.Checked = dataGridView1.CurrentCell.Style.Alignment == DataGridViewContentAlignment.TopLeft;
                center.Checked = dataGridView1.CurrentCell.Style.Alignment == DataGridViewContentAlignment.TopCenter;
                right.Checked = dataGridView1.CurrentCell.Style.Alignment == DataGridViewContentAlignment.TopRight;
            }
            catch
            {
                bold.Checked = false;
                italic.Checked = false;
                underline.Checked = false;
                strikethrough.Checked = false;
                left.Checked = false;
                center.Checked = false;
                right.Checked = false;
            }
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
            dataGridView1.CurrentCell.Style.Font = new Font(dataGridView1.CurrentCell.Style.Font, fs);
            UpdateUI();
        }

        private void left_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Style.Alignment = DataGridViewContentAlignment.TopLeft;
        }

        private void center_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Style.Alignment = DataGridViewContentAlignment.TopCenter;
        }

        private void right_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Style.Alignment = DataGridViewContentAlignment.TopRight;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateUI();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void bold_Click(object sender, EventArgs e)
        {
            SetFontStyle(true, false, false, false);
        }

        private void italic_Click(object sender, EventArgs e)
        {
            SetFontStyle(false, true, false, false);
        }

        private void strikethrough_Click(object sender, EventArgs e)
        {
            SetFontStyle(false, false, false, true);
        }

        private void underline_Click(object sender, EventArgs e)
        {
            SetFontStyle(false, false, true, false);
        }
    }
}
