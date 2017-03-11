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
using System.Threading;

namespace ShiftOS.WinForms.Applications
{
    
    [DefaultTitle("Tutorial objective")]
    public partial class TutorialBox : UserControl, IShiftOSWindow
    {
        public TutorialBox()
        {
            InitializeComponent();
            IsComplete = false;
            lbltuttext.Text = "";
            lbltuttext.Padding = new Padding(15);
            lbltuttext.Margin = lbltuttext.Padding;
        }

        bool stillTyping = false;

        public void SetObjective(string text)
        {
            while (stillTyping == true)
            {

            }

            new Thread(() =>
            {
                stillTyping = true;
                this.Invoke(new Action(() =>
                {
                    lbltuttext.Text = "";
                }));
                foreach(var c in text.ToCharArray())
                {
                    this.Invoke(new Action(() =>
                    {
                        lbltuttext.Text += c;
                    }));
                    Thread.Sleep(75);
                }
                Thread.Sleep(5000);
                stillTyping = false;
            }).Start();
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool IsComplete { get; set; }

        public bool OnUnload()
        {
            return IsComplete;
        }

        public void OnUpgrade()
        {
        }
    }
}
