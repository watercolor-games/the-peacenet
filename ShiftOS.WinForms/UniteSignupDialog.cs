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
using System.Runtime.InteropServices;

namespace ShiftOS.WinForms
{
    public partial class UniteSignupDialog : UserControl, IShiftOSWindow
    {
        // sets a placeholder value on a control using Windows API voodoo
        private static void SetPlaceholder(Control ctl, string txt)
        {
            IntPtr str = IntPtr.Zero;
            try
            {
                str = Marshal.StringToHGlobalUni(txt);
                var msgSetPlaceholder = Message.Create(ctl.Handle, 0x1501, IntPtr.Zero, str);
                NativeWindow.FromHandle(ctl.Handle).DefWndProc(ref msgSetPlaceholder);
            }
            finally
            {
                if (str != IntPtr.Zero)
                    Marshal.FreeHGlobal(str);
            }
        }

        public class SignupCredentials
        {
            public string SystemName { get; set; }
            public string Username { get; set; }
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
            SetPlaceholder(txtsys, "Hostname");
            SetPlaceholder(txtuname, "Username");
            SetPlaceholder(txtroot, "Password");
            txtroot.Size = txtuname.Size; // AppearanceManager stop breaking my design REEEEE
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
            string uname = txtuname.Text;
            string root = txtroot.Text;

            // validation

            if (string.IsNullOrWhiteSpace(sys))
            {
                Infobox.Show("{TITLE_EMPTY_SYSNAME}", "{MSG_EMPTY_SYSNAME}");
                return;
            }

            if (sys.Length < 5)
            {
                Infobox.Show("{TITLE_VALIDATION_ERROR}", "{MSG_VALIDATION_ERROR_SYSNAME_LENGTH}");
                return;
            }

            if (string.IsNullOrWhiteSpace(uname))
            {
                Infobox.Show("{TITLE_VALIDATION_ERROR}", "You must provide a username.");
                return;
            }

            Callback?.Invoke(new SignupCredentials
            {
                SystemName = sys,
                Username = uname,
                RootPassword = root
            });
            AppearanceManager.Close(this);
        }
    }
}
