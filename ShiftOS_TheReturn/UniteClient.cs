using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Objects;

namespace ShiftOS.Unite
{
    public class UniteClient
    {
        public string Token { get; private set; }
        public string BaseURL
        {
            get
            {
                return UserConfig.Get().UniteUrl;
            }
        }

        public string GetDisplayNameId(string id)
        {
            return MakeCall("/API/GetDisplayName/" + id);
        }

        public PongHighscoreModel GetPongHighscores()
        {
            return JsonConvert.DeserializeObject<PongHighscoreModel>(MakeCall("/API/GetPongHighscores"));
        }

        public UniteClient(string baseurl, string usertoken)
        {
            //Handled by the servers.json file
            //BaseURL = baseurl;
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

        public int GetPongCP()
        {
            return Convert.ToInt32(MakeCall("/API/GetPongCP"));
        }

        public int GetPongLevel()
        {
            return Convert.ToInt32(MakeCall("/API/GetPongLevel"));
        }

        public void SetPongLevel(int value)
        {
            MakeCall("/API/SetPongLevel/" + value.ToString());
        }

        public void SetPongCP(int value)
        {
            MakeCall("/API/SetPongCP/" + value.ToString());
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

    public class PongHighscoreModel
    {
        public int Pages { get; set; }
        public PongHighscore[] Highscores { get; set; }
    }

    public class PongHighscore
    {
        public string UserId { get; set; }
        public int Level { get; set; }
        public long CodepointsCashout { get; set; }
    }
}
