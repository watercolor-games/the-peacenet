using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Nancy.Security;
using Nancy.TinyIoc;
using Newtonsoft.Json;

namespace ShiftOS.Server.WebAdmin
{
    class Program
    {
        static void Main(string[] args)
        {
            var HostConf = new HostConfiguration();
            HostConf.UrlReservations.CreateAutomatically = true;
            
            using(var nancy = new NancyHost(HostConf, new Uri("http://localhost:13371/mudadmin")))
            {
                nancy.Start();
                Console.WriteLine($"[{DateTime.Now}] <AdminPanel/NancyInit> Initiating on localhost:13371...");
                Console.ReadLine();
            }
        }
    }

    public static class PageBuilder
    {
        public static string Build(string page)
        {
            string templatehtml = Properties.Resources.HtmlTemplate;
            switch (page)
            {
                case "login":
                    templatehtml = templatehtml.Replace("{body}", Properties.Resources.LoginView);
                    break;
            }
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
                return PageBuilder.Build("login");
            };

            Get["/logout"] = parameters =>
            {
                return this.Logout("/");
            };

            Post["/login"] = parameters =>
            {
                Guid id = new Guid();
                if (SystemManager.Login(parameters.username, parameters.password, out id) == true)
                {
                    return this.Login(id);
                }
                else
                {
                    return PageBuilder.Build("loginFailed");
                }
            };
        }
    }

    public class UserModule : NancyModule
    {
        public UserModule()
        {
            this.RequiresAuthentication();
            this.RequiresClaims("User");
            Get["/"] = _ =>
            {
                return PageBuilder.Build("status");
            };
            Get["/status"] = _ =>
            {
                return PageBuilder.Build("status");
            };
        }
    }
}
