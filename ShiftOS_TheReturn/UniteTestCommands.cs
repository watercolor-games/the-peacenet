using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    [Namespace("unite")]
    public static class UniteTestCommands
    {
        [Command("setdisplayname")]
        [RequiresArgument("name")]
        public static bool SetDisplayName(Dictionary<string, object> args)
        {
            string dname = args["name"].ToString();
            var unite = new ShiftOS.Unite.UniteClient("http://getshiftos.ml", SaveSystem.CurrentSave.UniteAuthToken);
            unite.SetDisplayName(dname);
            return true;
        }

        [Command("login")]
        [RequiresArgument("username")]
        [RequiresArgument("password")]
        public static bool LoginTest(Dictionary<string, object> args)
        {
            string u = args["username"].ToString();
            string p = args["password"].ToString();
            var webrequest = HttpWebRequest.Create("http://getshiftos.ml/Auth/Login?appname=ShiftOS&appdesc=ShiftOS+client&version=1_0_beta_2_4");
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{u}:{p}"));
            webrequest.Headers.Add("Authentication: Basic " + base64);
            var response = webrequest.GetResponse();
            var str = response.GetResponseStream();
            var reader = new System.IO.StreamReader(str);
            string result = reader.ReadToEnd();
            Console.WriteLine("Server returned: " + result);
            reader.Close();
            str.Close();
            str.Dispose();
            response.Dispose();
            
            return true;
        }

        [Command("linklogin")]
        [RequiresArgument("username")]
        [RequiresArgument("password")]
        public static bool LinkLogin(Dictionary<string, object> args)
        {
            string u = args["username"].ToString();
            string p = args["password"].ToString();
            var webrequest = HttpWebRequest.Create("http://getshiftos.ml/Auth/Login?appname=ShiftOS&appdesc=ShiftOS+client&version=1_0_beta_2_4");
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{u}:{p}"));
            webrequest.Headers.Add("Authentication: Basic " + base64);
            var response = webrequest.GetResponse();
            var str = response.GetResponseStream();
            var reader = new System.IO.StreamReader(str);
            string result = reader.ReadToEnd();
            //If we get this far, the token is OURS. :D
            SaveSystem.CurrentSave.UniteAuthToken = result;
            Console.WriteLine("Unite says \"Access Granted!\"");
            SaveSystem.SaveGame();
            reader.Close();
            str.Close();
            str.Dispose();
            response.Dispose();

            return true;
        }


    }
}
