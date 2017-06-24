using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.MFSProfiler
{
    public partial class Infobox : Form
    {
        public Infobox()
        {
            InitializeComponent();
        }

        public Action<string> TextCallback = null;
        public Action<bool> YesNoCallback = null;
        
        public string MessageText
        {
            get
            {
                return lbmessage.Text;
            }
            set
            {
                lbmessage.Text = value;
            }
        }
        
        public bool ShowText
        {
            get { return txtinput.Visible; }
            set { txtinput.Visible = value; }
        }

        public bool ShowYesNo
        {
            get { return btnyes.Parent.Visible; }
            set { btnyes.Parent.Visible = value; }
        }

        public static void PromptYesNo(string title, string message, Action<bool> callback)
        {
            var inf = new Infobox();
            inf.ShowText = false;
            inf.ShowYesNo = true;
            inf.Text = title;
            inf.MessageText = message;
            inf.YesNoCallback = callback;
            inf.ShowDialog();
        }

        public static void PromptText(string title, string message, Action<string> callback)
        {
            var inf = new Infobox();
            inf.ShowYesNo = false;
            inf.ShowText = true;
            inf.Text = title;
            inf.MessageText = message;
            inf.TextCallback = callback;
            inf.ShowDialog();
        }

        private void btnyes_Click(object sender, EventArgs e)
        {
            YesNoCallback?.Invoke(true);
            this.Close();
        }

        private void btnno_Click(object sender, EventArgs e)
        {
            YesNoCallback?.Invoke(false);
            this.Close();
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            TextCallback?.Invoke(txtinput.Text);
            this.Close();
        }
    }
}
