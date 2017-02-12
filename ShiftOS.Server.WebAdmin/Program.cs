using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using Nancy.Security;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using ShiftOS.Objects;

namespace ShiftOS.Server.WebAdmin
{
    class Program
    {
        static void Main(string[] args)
        {
            var HostConf = new HostConfiguration();
            HostConf.UrlReservations.CreateAutomatically = true;
            
            using(var nancy = new NancyHost(HostConf, new Uri("http://localhost:13371/mudadmin/")))
            {
                nancy.Start();
                Console.WriteLine($"[{DateTime.Now}] <AdminPanel/NancyInit> Initiating on localhost:13371...");
                Console.ReadLine();
            }
        }
    }

    public static class PageBuilder
    {
        public static string Build(string page, Dictionary<string, string> templateParams = null)
        {
            string templatehtml = Properties.Resources.HtmlTemplate;
            if (templateParams == null)
            {
                templateParams = new Dictionary<string, string>();
            }
            if (!templateParams.ContainsKey("{logout}"))
            {
                templateParams.Add("{logout}", "<li><a href=\"/mudadmin/logout\">Log out</a></li>");
            }
            switch (page)
            {
                case "status":
                    templatehtml = templatehtml.Replace("{body}", Properties.Resources.Status);
                    break;
                case "login":
                    templatehtml = templatehtml.Replace("{body}", Properties.Resources.LoginView);
                    break;
                case "initialsetup":
                    templatehtml = templatehtml.Replace("{body}", Properties.Resources.SetupView);
                    break;
            }
            try
            {
                foreach (var param in templateParams)
                {
                    templatehtml = templatehtml.Replace(param.Key, param.Value);
                }
            }
            catch { }
            return templatehtml;
        }
    }

    public class MudUserIdentity : IUserIdentity
    {
        public MudUserIdentity(string username)
        {
            _username = username;
        }

        public IEnumerable<string> Claims
        {
            get
            {
                return SystemManager.GetClaims(_username);
            }
        }

        private string _username = "";

        public string UserName
        {
            get
            {
                return _username;
            }
        }
    }

    public static class SystemManager
    {
        public static List<string> GetClaims(string username)
        {
            foreach (var user in JsonConvert.DeserializeObject<List<MudUser>>(ShiftOS.Server.Program.ReadEncFile("users.json")))
            {
                if(user.Username == username)
                {
                    return user.Claims;
                }
            }
            return new List<string>(new[] { "User" });
        }

        public static bool Login(string username, string password, out Guid id)
        {
            foreach (var user in JsonConvert.DeserializeObject<List<MudUser>>(ShiftOS.Server.Program.ReadEncFile("users.json")))
            {
                if (user.Username == username && user.Password == Encryption.Encrypt(password))
                {
                    id = user.ID;
                    return true;
                }
            }
            id = new Guid();
            return false;
        }

        public static string GetCPWorth()
        {
            if (System.IO.Directory.Exists("saves"))
            {
                int cp = 0;

                foreach(var file in System.IO.Directory.GetFiles("saves"))
                {
                    if (file.EndsWith(".save"))
                    {
                        var save = JsonConvert.DeserializeObject<Save>(Server.Program.ReadEncFile(file));
                        cp += save.Codepoints;
                    }
                }
                return cp.ToString();
            }
            else
            {
                return "0";
            }
        }

        public static string GetUserCount()
        {
            if (System.IO.Directory.Exists("saves"))
            {
                return System.IO.Directory.GetFiles("saves").Length.ToString();
            }
            else
            {
                return "0";
            }
        }

        public static MudUserIdentity GetIdentity(Guid id)
        {
            foreach (var user in JsonConvert.DeserializeObject<List<MudUser>>(ShiftOS.Server.Program.ReadEncFile("users.json")))
            {
                if (user.ID == id)
                {
                    return new WebAdmin.MudUserIdentity(user.Username);
                }
            }
            return null;
        }
    }

    public class MudUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Claims { get; set; }
        public Guid ID { get; set; }
    }

    public class MudBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            var formsAuthConfiguration = new FormsAuthenticationConfiguration();
            formsAuthConfiguration.RedirectUrl = "~/login";
            formsAuthConfiguration.UserMapper = container.Resolve<IUserMapper>();
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
            base.ApplicationStartup(container, pipelines);
        }
    }


    public class MudUserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return SystemManager.GetIdentity(identifier);
        }
    }

    public class LoginModule : NancyModule
    {
        public LoginModule()
        {
            Get["/login"] = parameters =>
            {
                if (System.IO.File.Exists("users.json"))
                {
                    return PageBuilder.Build("login", new Dictionary<string, string>
                    {
                        {"{logout}", "" }
                    });
                }
                else
                {
                    return PageBuilder.Build("initialsetup", new Dictionary<string, string>
                    {
                        {"{logout}", "" }
                    });
                }
            };

            Get["/logout"] = parameters =>
            {
                return this.Logout("/");
            };

            Post["/login"] = parameters =>
            {
                var p = this.Bind<LoginRequest>();
                Guid id = new Guid();
                if (System.IO.File.Exists("users.json"))
                {
                    if (SystemManager.Login(p.username, p.password, out id) == true)
                    {
                        return this.Login(id);
                    }
                    else
                    {
                        return PageBuilder.Build("loginFailed", new Dictionary<string, string>
                    {
                        {"{logout}", "" }
                    });
                    }
                }
                else
                {
                    var mudUser = new MudUser();
                    mudUser.Username = p.username;
                    mudUser.Password = Encryption.Encrypt(p.password);
                    mudUser.Claims = new List<string>(new[] { "Admin" });
                    mudUser.ID = Guid.NewGuid();
                    id = mudUser.ID;
                    List<MudUser> users = new List<MudUser>(new[] { mudUser });
                    ShiftOS.Server.Program.WriteEncFile("users.json", JsonConvert.SerializeObject(users, Formatting.Indented));
                    return this.Login(id);
                }
            };
        }
    }



    public class UserModule : NancyModule
    {
        public UserModule()
        {
            this.RequiresAuthentication();
            this.RequiresClaims("Admin");
            Get["/"] = _ =>
            {
                return PageBuilder.Build("status", new Dictionary<string, string>{
                    { "{cp_worth}", SystemManager.GetCPWorth() },
                    { "{user_count}", SystemManager.GetUserCount() },
                    { "{system_time}", DateTime.Now.ToString() },
                });
            };
            Get["/status"] = _ =>
            {
                return PageBuilder.Build("status");
            };
        }
    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
