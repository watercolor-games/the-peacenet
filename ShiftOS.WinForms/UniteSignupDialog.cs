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

namespace ShiftOS.WinForms
{
    public partial class UniteSignupDialog : UserControl, IShiftOSWindow
    {
        public UniteSignupDialog(Action<string> callback)
        {
            InitializeComponent();
            Callback = callback;
        }
        
        private Action<string> Callback { get; set; }


        public void OnLoad()
        {
            this.ParentForm.AcceptButton = btnlogin;
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
