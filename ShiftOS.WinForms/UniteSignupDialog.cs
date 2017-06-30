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
using Newtonsoft.Json;
using System.Net;
using ShiftOS.Objects;

namespace ShiftOS.WinForms
{
    public partial class UniteSignupDialog : UserControl, IShiftOSWindow
    {
        public class SignupCredentials
        {
            public string SystemName { get; set; }
            public string RootPassword { get; set; }
        }

        public UniteSignupDialog(Action<SignupCredentials> callback)
        {
            InitializeComponent();
            Callback = callback;
        }
        
        private Action<SignupCredentials> Callback { get; set; }


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

        private void btnlogin_Click(object sender, EventArgs e)
        {
            string sys = txtsys.Text;
            string root = txtroot.Text;

            if (string.IsNullOrWhiteSpace(sys))
            {
                Infobox.Show("{TITLE_EMPTY_SYSNAME}", "{MSG_EMPTY_SYSNAME}");
                return;
            }
            if(sys.Length < 5)
            {
                Infobox.Show("{TITLE_VALIDATION_ERROR}", "{MSG_VALIDATION_ERROR_SYSNAME_LENGTH}");
                return;
            }

            Callback?.Invoke(new SignupCredentials
            {
                SystemName = sys,
                RootPassword = root
            });
            AppearanceManager.Close(this);
        }
    }
}
