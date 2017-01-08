using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Controls
{
    public class TerminalBox : RichTextBox, ITerminalWidget
    {
        public void SelectBottom()
        {
            try
            {
                this.Select(this.Text.Length, 0);
                this.ScrollToCaret();
            }
            catch { }
        }

        public void Write(string text)
        {
            this.Text += Localization.Parse(text);
        }

        public void WriteLine(string text)
        {
            this.Text += Localization.Parse(text) + Environment.NewLine;
        }
    }
}
