using Nancy;
using Nancy.Security;
using NetSockets;
using Newtonsoft.Json;
using ShiftOS.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Server
{
    public class WebAdmin : NancyModule
    {
        private Guid thisGuid { get; set; }

        public WebAdmin()
        {
            this.RequiresAuthentication();


            client = new NetObjectClient();

            client.OnReceived += (o, a) =>
            {
                var msg = a.Data.Object as ServerMessage;
                if (msg.Name == "Welcome")
                {
                    thisGuid = new Guid(msg.Contents);
                }
            };

            client.Connect(Program.server.Address.MapToIPv4().ToString(), 13370);

            string template = Properties.Resources.Home;

            Get["/"] = _ => { return GetPage(template, "index.html"); };
            Get["/{page}"] = parameters =>
            {
                return GetPage(template, parameters.page);
            };
        }

        public NetObjectClient client = new NetObjectClient();

        public string GetPage(string template, string page)
        {
            string pageContents = File.ReadAllText("adm/" + page);

            string page_text = template.Replace("{BODY}", pageContents);

            page_text = page_text.Replace("{IP_ADDR}", client.RemoteHost.ToString());
            page_text = page_text.Replace("{PORT}", client.RemotePort.ToString());

            return page_text;
        }

        public string GrabResource(string page)
        {
            var type = this.GetType();
            foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach(var attr in property.GetCustomAttributes(false))
                {
                    if(attr is PageAttribute)
                    {
                        if(page == (attr as PageAttribute).Name)
                        {
                            return property.GetGetMethod().Invoke(this, null) as string;
                        }
                    }
                }
            }

            return Properties.Resources.NotFound;
        }
    }

    public class PageAttribute :Attribute
    {
        public PageAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
