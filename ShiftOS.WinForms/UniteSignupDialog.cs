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

            if(p != txtconfirm.Text)
            {
                Infobox.Show("Passwords don't match.", "The \"Password\" and \"Confirm\" boxes must match.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtdisplay.Text))
            {
                Infobox.Show("Empty display name", "Please choose a proper display name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtsysname.Text))
            {
                Infobox.Show("Empty system name", "Please name your computer!");
                return;
            }

            if(p.Length < 7)
            {
                Infobox.Show("Password error", "Your password must have at least 7 characters.");
                return;
            }

            if (!(p.Any(char.IsUpper) &&
    p.Any(char.IsLower) &&
    p.Any(char.IsDigit)))
            {
                Infobox.Show("Password error", "Your password must contain at least one uppercase, lowercase, digit and symbol character.");
                return;
            }

                if (!u.Contains("@"))
            {
                Infobox.Show("Valid email required.", "You must specify a valid email address.");
                return;
            }

            try
            {
                var webrequest = HttpWebRequest.Create("http://getshiftos.ml/Auth/Register?appname=ShiftOS&appdesc=ShiftOS+client&version=1_0_beta_2_4&displayname=" + txtdisplay.Text + "&sysname=" + txtsysname.Text);
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{u}:{p}"));
                webrequest.Headers.Add("Authentication: Basic " + base64);
                var response = webrequest.GetResponse();
                var str = response.GetResponseStream();
                var reader = new System.IO.StreamReader(str);
                string result = reader.ReadToEnd();
                if (result.StartsWith("{"))
                {
                    var exc = JsonConvert.DeserializeObject<Exception>(result);
                    Infobox.Show("Error", exc.Message);
                    return;
                }
                reader.Close();
                str.Close();
                str.Dispose();
                response.Dispose();
                Callback?.Invoke(result);
                AppearanceManager.Close(this);
            }
#if DEBUG
            catch (Exception ex)
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
