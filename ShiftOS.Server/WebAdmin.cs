/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
