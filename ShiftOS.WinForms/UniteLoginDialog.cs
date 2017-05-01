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
using System.Net;

namespace ShiftOS.WinForms
{
    public partial class UniteLoginDialog : UserControl, IShiftOSWindow
    {
        public UniteLoginDialog(Action<string> callback)
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

        private void btnlogin_Click(object sender, EventArgs e)
        {
            string u = txtusername.Text;
            string p = txtpassword.Text;

            if (string.IsNullOrWhiteSpace(u))
            {
                Infobox.Show("Please enter a username.", "You must enter a proper email address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(p))
            {
                Infobox.Show("Please enter a password.", "You must enter a valid password.");
                return;
            }

            try
            {
                var webrequest = HttpWebRequest.Create("http://getshiftos.ml/Auth/Login?appname=ShiftOS&appdesc=ShiftOS+client&version=1_0_beta_2_4");
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{u}:{p}"));
                webrequest.Headers.Add("Authentication: Basic " + base64);
                var response = webrequest.GetResponse();
                var str = response.GetResponseStream();
                var reader = new System.IO.StreamReader(str);
                string result = reader.ReadToEnd();
                reader.Close();
                str.Close();
                str.Dispose();
                response.Dispose();
                Callback?.Invoke(result);
                AppearanceManager.Close(this);
            }
#if DEBUG
            catch(Exception ex)
            {
                Infobox.Show("Error", ex.ToString());
            }
#else
            catch
            {
                Infobox.Show("Login failed.", "The login attempt failed due to an incorrect username and password pair.");
            }
#endif

        }
    }
}
