using System;
using System.Collections.Generic;
using System.IO;
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
            HostConf.RewriteLocalhost = true;
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
            if (SystemManager.MudIsRunning())
            {
                templateParams.Add("{mud_power}", "<li><a href='/mudadmin/poweroff'><span class='glyphicon glyphicon-power-off'></span> Power off</a></li>");
                templateParams.Add("{mud_restart}", "<li><a href='/mudadmin/restart'><span class='glyphicon glyphicon-refresh'></span> Restart</a></li>");
            }
            else
            {
                templateParams.Add("{mud_power}", "<li><a href='/mudadmin/poweron'><span class='glyphicon glyphicon-power-on'></span> Power on</a></li>");
                templateParams.Add("{mud_restart}", "");
            }

            if(templateParams["{logout}"] == "")
            {
                templateParams["{mud_power}"] = "";
                templateParams["{mud_restart}"] = "";

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
        public static bool MudIsRunning()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("ShiftOS.Server");
            return processes.Length > 0;
        }

        public static void KillMud()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("ShiftOS.Server");
            for(int i = 0; i < processes.Length; i++)
            {
                try
                {
                    processes[i].Kill();
                }
                catch
                {
                }
            }
        }

        public static List<string> GetClaims(string username)
        {
            foreach(var save in GetSaves())
            {
                if (save.IsMUDAdmin)
                {
                    return new List<string> { "User", "Admin" };
                }
            }
            return new List<string>(new[] { "User" });
        }

        public static Save[] GetSaves()
        {
            List<Save> saves = new List<Save>();
            if (Directory.Exists("saves"))
            {
                foreach(var saveFile in Directory.GetFiles("saves"))
                {
                    try
                    {
                        saves.Add(JsonConvert.DeserializeObject<Save>(Server.Program.ReadEncFile(saveFile)));
                    }
                    catch { }
                }
            }
            return saves.ToArray();
        }

        public static bool Login(string username, string password, out Guid id)
        {
            foreach (var user in GetSaves())
            {
                if (user.Username == username && user.Password == password)
                {
                    id = user.ID;
                    return true;
                }
            }
            id = new Guid();
            return false;
        }

        public static string BuildFormFromObject(object obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<form method='post' action=''><table class='table'>");
            foreach(var prop in obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                string name = "";
                string description = "No description.";
                foreach(var attrib in prop.GetCustomAttributes(false))
                {
                    if(attrib is FriendlyNameAttribute)
                    {
                        name = (attrib as FriendlyNameAttribute).Name;
                    }
                    if(attrib is FriendlyDescriptionAttribute)
                    {
                        description = (attrib as FriendlyDescriptionAttribute).Description;
                    }
                }
                if (name != "")
                {
                    sb.AppendLine("<tr>");

                    sb.AppendLine($@"<td width=""45%"">
    <p><strong>{name}</strong></p>
    <p>{description}</p>
</td>
<td>");
                    if (prop.PropertyType == typeof(bool))
                    {
                        string isChecked = ((bool)prop.GetValue(obj) == true) ? "checked" : "";
                        sb.AppendLine($"<input class='form-control' type='checkbox' name='{prop.Name}' {isChecked}/>");
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        sb.AppendLine($"<input class='form-control' type='text' name='{prop.Name}' value='{prop.GetValue(obj)}'/>");
                    }

                    sb.AppendLine("</td></tr>");
                }
                else
                {
                    sb.AppendLine($"<input type='hidden' name='{prop.Name}' value='{prop.GetValue(obj)}'/>");
                }
            }
            sb.AppendLine("<tr><td></td><td><input class='btn btn-default' type='submit'/></td></tr>");
            sb.AppendLine("</table></form>");
            return sb.ToString();
        }

        public static Channel GetChat(string id)
        {
            if (File.Exists("chats.json"))
                foreach (var channel in JsonConvert.DeserializeObject<List<Channel>>(File.ReadAllText("chats.json")))
                {
                    if (channel.ID == id)
                        return channel;
                }
            return new Channel();
        }

        public static string BuildSaveListing(Save[] list)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"table\">");

            sb.AppendLine(@"<tr>
    <td><strong>Username</strong></td>
    <td><strong>System Name</strong></td>
    <td><strong>Codepoints</strong></td>
    <td><strong>Shiftorium Upgrades</strong></td>
    <td><strong>Is MUD Admin</strong></td>
    <td><strong>Actions</strong></td>
</tr>");

            foreach(var save in list)
            {
                sb.AppendLine($@"<tr>
    <td>{save.Username}</td>
    <td>{save.SystemName}</td>
    <td>{save.Codepoints}</td>
    <td>{save.CountUpgrades()} installed, {save.Upgrades.Count} total</td>
    <td>{save.IsMUDAdmin}</td>
    <td>
        <a href=""/mudadmin/toggleadmin/{save.Username}"" class=""btn btn-danger"">Toggle admin</a>
        <a href=""/mudadmin/deletesave/{save.Username}"" class=""btn btn-danger"">Delete save</a>
    </td>
</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }


        public static string GetAllChats()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"table\">");
            sb.AppendLine($@"<tr><td><strong>ID</strong></td>
    <td><strong>Name</strong></td>
    <td><strong>Topic</strong></td>
    <td><strong>Is Discord Relay</strong></td>
    <td><strong>Discord channel ID</strong></td>
    <td><strong>Discord Bot Token</strong></td>
    <td><strong>Actions</strong></td></tr>");
            if (File.Exists("chats.json"))
            {
                foreach(var chat in JsonConvert.DeserializeObject<List<Channel>>(File.ReadAllText("chats.json")))
                {
                    sb.AppendLine($@"<tr>
    <td>{chat.ID}</td>
    <td>{chat.Name}</td>
    <td>{chat.Topic}</td>
    <td>{chat.IsDiscordProxy}</td>
    <td>{chat.DiscordChannelID}</td>
    <td>{chat.DiscordBotToken}</td>
    <td>
        <a href=""/mudadmin/editchat/{chat.ID}"" class=""btn btn-default""><span class=""glyphicon glyphicon-pencil""></span>Edit</a>
        <a href=""#"" class=""btn btn-default"" data-toggle=""modal"" data-target=""#modal_{chat.ID}""><span class=""glyphicon glyphicon-delete""></span> Delete</a>
    </td>
</tr>");
                    sb.AppendLine(CreateModal(chat.ID, "Delete " + chat.Name + "?", "Are you sure you want to delete this chat?", "/deletechat/" + chat.ID));
                }
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string CreateModal(string id, string title, string msg, string callbackUrl)
        {
            return $@"<div id=""modal_{id}"" class=""modal fade"" role=""dialog"">
  <div class=""modal-dialog"">

    <!-- Modal content-->
    <div class=""modal-content"">
      <div class=""modal-header"">
        <button type=""button"" class=""close"" data-dismiss=""modal"">&times;</button>
        <h4 class=""modal-title"">{title}</h4>
      </div>
      <div class=""modal-body"">
        <p>{msg}</p>
      </div>
      <div class=""modal-footer"">
        <a href=""/mudadmin{callbackUrl}"" class=""btn btn-danger"">Yes</a>
        <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">No</button>
      </div>
    </div>

  </div>
</div>";
        }

        public static string GetCPWorth()
        {
            if (System.IO.Directory.Exists("saves"))
            {
                long cp = 0;

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
            foreach (var user in GetSaves())
            {
                if (user.ID == id)
                {
                    return new WebAdmin.MudUserIdentity(user.Username);
                }
            }
            return null;
        }

        internal static void MakeAdmin(string username)
        {
            Save sav = null;
            foreach(var save in GetSaves())
            {
                if (save.Username == username)
                    sav = save;
            }
            if(sav != null)
            {
                sav.IsMUDAdmin = true;
                Server.Program.WriteEncFile("saves/" + username + ".save", JsonConvert.SerializeObject(sav));
            }
        }

        internal static Save[] GetAdmins()
        {
            var saves = new List<Save>();
            foreach(var save in GetSaves())
            {
                if(save.IsMUDAdmin == true)
                {
                    saves.Add(save);
                }
            }
            return saves.ToArray();
        }
    }

    public class MudUser
    {
        [FriendlyName("Username")]
        [FriendlyDescription("The username you will appear as in-game.")]
        public string Username { get; set; }

        [FriendlyName("Password")]
        [FriendlyDescription("A password that you will use to log in to the admin panel and the game.")]
        public string Password { get; set; }

        [FriendlyName("System name")]
        [FriendlyDescription("An in-game hostname for your account. In ShiftOS, your user ID is always yourusername@yoursystemname. Be creative.")]
        public string SystemName { get; set; }

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
                if (SystemManager.GetSaves().Length > 0)
                {
                    if (SystemManager.GetAdmins().Length > 0)
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
                        {"{logout}", "" },
                            {"{savelist}", BuildSaveList() }
                    });
                    }
                }
                else
                {
                    return PageBuilder.Build("bla", new Dictionary<string, string>
                    {
                        {"{body}", Properties.Resources.NoUsersFound },
                        {"{user_create_form}", SystemManager.BuildFormFromObject(new MudUser()) }
                    });
                }
            };

            Get["/logout"] = parameters =>
            {
                return this.Logout("~/");
            };

            Post["/login"] = parameters =>
            {
                if (SystemManager.GetSaves().Length > 0)
                {
                    if (SystemManager.GetAdmins().Length == 0)
                    {
                        var user = this.Bind<LoginRequest>();
                        SystemManager.MakeAdmin(user.username);
                        Guid id = new Guid();
                        if(SystemManager.Login(user.username, user.password, out id) == true)
                        {
                            return this.Login(id);
                        }
                        return new UserModule().Redirect("/login");
                    }
                    else
                    {
                        var user = this.Bind<LoginRequest>();
                        Guid id = new Guid();
                        if (SystemManager.Login(user.username, user.password, out id) == true)
                        {
                            return this.Login(id);
                        }
                        return new UserModule().Redirect("/login");
                    }
                }
                else
                {
                    var newUser = this.Bind<MudUser>();
                    var save = new Save();
                    save.Username = newUser.Username;
                    save.SystemName = newUser.SystemName;
                    save.Password = newUser.Password;
                    save.Codepoints = 0;
                    save.MyShop = "";
                    save.Upgrades = new Dictionary<string, bool>();
                    save.IsMUDAdmin = true;
                    save.StoryPosition = 1;

                    if (!Directory.Exists("saves"))
                        Directory.CreateDirectory("saves");
                    save.ID = Guid.NewGuid();

                    Server.Program.WriteEncFile("saves/" + save.Username + ".save", JsonConvert.SerializeObject(save));
                    return this.Login(save.ID);
                }
            };
        }

        private string BuildSaveList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class='table'>");
            sb.AppendLine($@"<tr>
    <td><strong>Username</strong></td>
    <td><strong>System name</strong></td>
    <td><strong>Codepoints</strong></td>
    <td><strong>Actions</strong></td>
</tr>");

            foreach(var save in SystemManager.GetSaves())
            {
                sb.AppendLine($@"<tr>
    <td>{save.Username}</td>
    <td>{save.SystemName}</td>
    <td>{save.Codepoints}</td>
    <td><form method='post' action=''>
        <input type='hidden' name='username' value='{save.Username}'/><input type='hidden' name='password' value='{save.Password}'/>
        <input type='submit' value='Choose' class='btn btn-default'/>
    </form></td>
</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }



    public class UserModule : NancyModule
    {
        public string Redirect(string url)
        {
            return $@"<html>
    <head>
        <meta http-equiv=""refresh"" content=""0; url=/mudadmin{url}"" />
    </ head>
</html>";
        }

        public UserModule()
        {
            this.RequiresAuthentication();
            this.RequiresClaims("Admin");
            Get["/"] = _ =>
            {
                return Redirect("/status");
            };

            Get["/toggleadmin/{id}"] = parameters =>
            {
                string id = parameters.id;
                for (int i = 0; i < SystemManager.GetSaves().Length; i++)
                {
                    var save = SystemManager.GetSaves()[i];
                    if(save.Username.ToString() == id)
                    {
                        save.IsMUDAdmin = !save.IsMUDAdmin;
                        Server.Program.WriteEncFile("saves/" + save.Username + ".save", JsonConvert.SerializeObject(save));
                    }
                }
                return Redirect("/saves");

            };

            Get["/deletesave/{username}"] = parameters =>
            {


                string id = parameters.username;
                for (int i = 0; i < SystemManager.GetSaves().Length; i++)
                {
                    if (SystemManager.GetSaves()[i].Username.ToString() == id)
                    {
                        File.Delete("saves/" + SystemManager.GetSaves()[i].Username + ".save");
                    }
                }
                return Redirect("/saves");
            };


            Get["/saves"] = _ =>
            {
                return PageBuilder.Build("bla", new Dictionary<string, string>
                {
                    { "{body}", Properties.Resources.GenericTableList },
                    { "{listtitle}", "Test subjects"  },
                    { "{listdesc}", "Below is a list of test subjects (save files) on your multi-user domain. You can see their username, system name, Codepoints, amount of installed upgrades, and you can also perform basic actions on each save." },
                    { "{list}", SystemManager.BuildSaveListing(SystemManager.GetSaves()) }
                });
            };

            Get["/status"] = _ =>
            {
                return statusBuilder();
            };

            Get["/deletechat/{id}"] = parameters =>
            {
                string chatID = parameters.id;
                var chats = JsonConvert.DeserializeObject<List<Channel>>(File.ReadAllText("chats.json"));
                for(int i = 0; i < chats.Count; i++)
                {
                    try
                    {
                        if (chats[i].ID == chatID)
                            chats.RemoveAt(i);
                    }
                    catch { }
                }
                File.WriteAllText("chats.json", JsonConvert.SerializeObject(chats, Formatting.Indented));
                return Redirect("/chats");
            };

            Get["/chats"] = _ =>
            {
                return chatsListBuilder();
            };

            Get["/createchat"] = _ =>
            {
                return PageBuilder.Build("editchat", new Dictionary<string, string>
                {
                    {"{body}", Properties.Resources.ChatEditTemplate },
                    {"{form}", SystemManager.BuildFormFromObject(new Channel()) }
                });
            };

            Post["/createchat"] = parameters =>
            {
                var chat = this.Bind<Channel>();
                chat.ID = chat.Name.ToLower().Replace(" ", "_");
                List<Channel> chats = new List<Channel>();
                if (File.Exists("chats.json"))
                    chats = JsonConvert.DeserializeObject<List<Channel>>(File.ReadAllText("chats.json"));

                bool chatExists = false;

                for (int i = 0; i < chats.Count; i++)
                {
                    if (chats[i].ID == chat.ID)
                    {
                        chats[i] = chat;
                        chatExists = true;
                    }
                }

                if (!chatExists)
                {
                    chats.Add(chat);
                }

                File.WriteAllText("chats.json", JsonConvert.SerializeObject(chats, Formatting.Indented));

                return Redirect("/chats");
            };

            Get["/editchat/{id}"] = parameters =>
            {
                return PageBuilder.Build("editchat", new Dictionary<string, string>
                {
                    {"{body}", Properties.Resources.ChatEditTemplate },
                    {"{form}", SystemManager.BuildFormFromObject(SystemManager.GetChat(parameters.id)) }
                });
            };

            Post["/editchat/{id}"] = parameters =>
            {
                var chat = this.Bind<Channel>();
                chat.ID = chat.Name.ToLower().Replace(" ", "_");
                List<Channel> chats = new List<Channel>();
                if (File.Exists("chats.json"))
                    chats = JsonConvert.DeserializeObject<List<Channel>>(File.ReadAllText("chats.json"));

                bool chatExists = false;

                for (int i = 0; i < chats.Count; i++)
                {
                    if (chats[i].ID == chat.ID)
                    {
                        chats[i] = chat;
                        chatExists = true;
                    }
                }

                if (!chatExists)
                {
                    chats.Add(chat);
                }

                File.WriteAllText("chats.json", JsonConvert.SerializeObject(chats, Formatting.Indented));
                return Redirect("/chats");
            };

            Get["/poweron"] = _ =>
            {
                if (!SystemManager.MudIsRunning())
                {
                    System.Diagnostics.Process.Start("ShiftOS.Server.exe");
                }
                return Redirect("/");
            };

            Get["/poweroff"] = _ =>
            {
                if (SystemManager.MudIsRunning())
                {
                    SystemManager.KillMud();
                }
                return Redirect("/");
            };
            Get["/restart"] = _ =>
            {
                if (SystemManager.MudIsRunning())
                {
                    SystemManager.KillMud();
                }
                return Redirect("/poweron");
            };
        }

        private string statusBuilder()
        {
            return PageBuilder.Build("status", new Dictionary<string, string>{
                    { "{cp_worth}", SystemManager.GetCPWorth() },
                    { "{user_count}", SystemManager.GetUserCount() },
                    { "{system_time}", DateTime.Now.ToString() },
                });

        }

        private string chatsListBuilder()
        {
            return PageBuilder.Build("bla", new Dictionary<string, string>
            {
                { "{body}", Properties.Resources.ChatListView },
                { "{chat_table}", SystemManager.GetAllChats() }
            });
        }
    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
