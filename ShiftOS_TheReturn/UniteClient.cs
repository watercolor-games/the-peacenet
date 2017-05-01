using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Unite
{
    public class UniteClient
    {
        public string Token { get; private set; }
        public string BaseURL { get; private set; }

        public UniteClient(string baseurl, string usertoken)
        {
            BaseURL = baseurl;
            Token = usertoken;
        }

        internal string MakeCall(string url)
        {
            var webrequest = WebRequest.Create(BaseURL + url);
            webrequest.Headers.Add("Authentication: Token " + Token);
            using (var response = webrequest.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public string GetEmail()
        {
            return MakeCall("/API/GetEmail");
        }

        public string GetSysName()
        {
            return MakeCall("/API/GetSysName");
        }

        public void SetSysName(string value)
        {
            MakeCall("/API/SetSysName/" + value);
        }

        public string GetDisplayName()
        {
            return MakeCall("/API/GetDisplayName");
        }

        public void SetDisplayName(string value)
        {
            MakeCall("/API/SetDisplayName/" + value.ToString());
        }

        public string GetFullName()
        {
            return MakeCall("/API/GetFullName");
        }

        public void SetFullName(string value)
        {
            MakeCall("/API/SetFullName/" + value.ToString());
        }


        public long GetCodepoints()
        {
            return Convert.ToInt64(MakeCall("/API/GetCodepoints"));
        }

        public void SetCodepoints(long value)
        {
            MakeCall("/API/SetCodepoints/" + value.ToString());
        }
    }
}
