using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Newtonsoft.Json;

namespace Peacenet.Filesystem.ClientMounts
{
    public class MenuDirectory : IClientMount
    {
        [Dependency]
        private AppLauncherManager _al = null;

        public string Path => "/etc/peacegate/menu";

        public byte[] GetFileContents(string filename)
        {
            if (!filename.EndsWith(".desktop"))
                return null;
            string name = filename.Replace(".desktop", "");
            foreach (var category in _al.GetAllCategories())
            {
                foreach (var item in _al.GetAllInCategory(category))
                {
                    if(item.WindowType.Name.ToLower() == name)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("[desktop]");
                        sb.AppendLine("Name = " + item.Attribute.Name);
                        sb.AppendLine("Category = " + item.Attribute.Category);
                        sb.AppendLine("Description = " + item.Attribute.Description);
                        sb.AppendLine("LaunchPath = /bin/" + item.WindowType.Name.ToLower());
                        return Encoding.UTF8.GetBytes(sb.ToString());
                    }
                }
            }

            return null;
        }

        public string[] GetFiles()
        {
            List<string> names = new List<string>();
            foreach(var category in _al.GetAllCategories())
            {
                foreach(var item in _al.GetAllInCategory(category))
                {
                    names.Add(item.WindowType.Name.ToLower() + ".desktop");
                }
            }
            return names.ToArray();
        }
    }
}
