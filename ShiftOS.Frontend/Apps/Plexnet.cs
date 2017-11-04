using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GUI;
using Plex.Extras;
using Microsoft.Xna.Framework;
using Plex.Objects;

namespace Plex.Frontend.Apps
{
    [WinOpen("plexnet")]
    [DefaultTitle("Plexnet Browser")]
    [Launcher("Plexnet Browser", false, null, "Networking")]
    [Obsolete("This feature will be removed in Milestone 2.")]
    public class Plexnet : Control, IPlexWindow
    {
        private PlexnetSite _site = null;
        private TextInput _address = new TextInput();
        private Button _go = new Button();

        public Plexnet()
        {
            AddControl(_address);
            AddControl(_go);

            _go.Text = "Go ->";
            _go.AutoSize = true;

            _go.Click += () =>
            {
                GoToURL(_address.Text);
            };
            _address.KeyEvent += (key) =>
            {
                if (key.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    GoToURL(_address.Text);
                }
            };

            _address.AutoSize = true;
            _address.MinHeight = 24;


            Width = 800;
            Height = 600;
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public string CurrentUrl
        {
            get
            {
                return _address.Text;
            }
        }

        public void SetSite(PlexnetSite site)
        {
            if(_site != null)
            {
                RemoveControl(_site);
                _site.Dispose();
                _site = null;
            }
            _site = site;
            _site.UrlRequested += (url) => GoToURL(url);
            AddControl(_site);
            _site.OnLoad();
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _go.X = Width - _go.Width - 15;
            _go.Y = 15;

            _address.X = 15;
            _address.Y = 15;
            _address.MinWidth = _go.X - 30;
            _address.MaxWidth = _address.MinWidth;

            if(_site != null)
            {
                _site.X = 0;
                _site.Y = _address.Y + _address.Height + 15;
                _site.Width = Width;
                _site.Height = Height - _site.Y;
            }

            base.OnLayout(gameTime);
        }

        public void GoToURL(string url)
        {
            if(url.EndsWith("/"))
            {
                int len = url.Length - 1;
                url = url.Remove(len, 1);
            }
            if (!url.Contains('/'))
                url += "/home.rnp";

            int slash = url.IndexOf('/');
            string netpath = url.Remove(slash, url.Length - slash);
            string fname = url.Remove(0, netpath.Length + 1);

            foreach(var type in ReflectMan.Types.Where(x=>x.BaseType == typeof(PlexnetSite) && x.GetCustomAttributes(false).FirstOrDefault(y=>y is PlexnetAttribute) != null))
            {
                var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is PlexnetAttribute) as PlexnetAttribute;
                if(attrib.NetworkPath == netpath && attrib.Filename == fname)
                {
                    _address.Text = url;
                    var psite = (PlexnetSite)Activator.CreateInstance(type, null);
                    SetSite(psite);
                    return;
                }
            }
        }
    }
}
