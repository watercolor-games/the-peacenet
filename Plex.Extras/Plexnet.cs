using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Frontend.GUI;

namespace Plex.Extras
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PlexnetAttribute : Attribute
    {
        public PlexnetAttribute(string netname, string fname)
        {
            NetworkPath = netname;
            Filename = fname;
        }

        public string NetworkPath { get; private set; }
        public string Filename { get; private set; }
    }

    public abstract class PlexnetSite : Control
    {
        public event Action BackRequested;
        public event Action<string> UrlRequested;

        protected void GoBack()
        {
            BackRequested?.Invoke();
        }

        protected void GoToURL(string url)
        {
            UrlRequested?.Invoke(url);
        }

        public abstract void OnLoad();

        
    }
}
