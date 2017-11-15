using System;
using Plex.Engine;
using Plex.Engine.GUI;
using Microsoft.Xna.Framework;
using Plex.Engine.Config;
using System.Linq;

namespace Plex.Frontend.Apps
{
    public class GameSettings : Control, IPlexWindow
    {
        private TextControl _resHeader = null;
        private ScrollView _resScroller = null;
        private ListBox _resChoices = null;

        private Button _apply = null;
        private Button _cancel = null;

        private int _resInitial = 0;
        private int _resUserSet = 0;

        public GameSettings()
        {
            Width = 800;
            Height = 600;

            _resHeader = new TextControl();
            _resScroller = new ScrollView();
            _resChoices = new ListBox();
            _apply = new Button();
            _cancel = new Button();

            AddControl(_resHeader);
            AddControl(_resScroller);
            _resScroller.AddControl(_resChoices);

            AddControl(_apply);
            AddControl(_cancel);

            _apply.Click += () =>
            {
                ConfigurationManager.SetResolution(_resUserSet);


                ConfigurationManager.ApplyConfig();
            };
        }

        public void OnLoad()
        {
            //Get an ordered list of resolutions.

            var resolutions = ConfigurationManager.GetSupportedResolutions();

            //Clear the resolution list control
            _resChoices.ClearItems();

            //Add each item
            foreach (var resolution in resolutions)
            {
                //Resolution overrides object.ToString() to actually show the width/height values.
                _resChoices.AddItem(resolution.ToString());
            }

            //Get the current resolution.
            var currentResolution = ConfigurationManager.GetResolution();

            //Get the index of the resolution string in our list
            var resIndex = Array.IndexOf(resolutions, resolutions.FirstOrDefault(x => x.ToString() == currentResolution.ToString()));

            _resInitial = resIndex;
            _resUserSet = _resInitial;

            //Set our selected index value.
            _resChoices.SelectedIndex = resIndex;

            _resChoices.RecalculateItemsPerPage();
            _resChoices.RequireTextRerender();
            _resChoices.Invalidate();

            _resChoices.SelectedIndexChanged += () =>
            {
                if (_resChoices.SelectedIndex == -1)
                {
                    _resChoices.SelectedIndex = _resInitial;
                    _resChoices.RecalculateItemsPerPage();
                    _resChoices.RequireTextRerender();
                    _resChoices.Invalidate();
                }
                else
                {
                    _resUserSet = _resChoices.SelectedIndex;
                }
            };
        }


        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {

        }

        protected override void OnLayout(GameTime gameTime)
        {
            _resHeader.X = 15;
            _resHeader.Y = 15;
            _resHeader.AutoSize = true;
            _resHeader.MaxWidth = (this.Width - 30) / 4;
            _resHeader.Text = "Screen resolution";

            _resScroller.Width = (Width - 30) / 4;
            _resScroller.X = 15;
            _resScroller.Y = _resHeader.Y + _resHeader.Height + 10;
            _resScroller.Height = (Height - _resScroller.Y) - 15;

            _resChoices.X = 0;
            _resChoices.Y = 0;
            _resChoices.AutoSize = true;
            _resChoices.MinWidth = _resScroller.Width;
            _resChoices.MaxWidth = _resScroller.Width;
            _resChoices.MinHeight = _resScroller.Height;

            _cancel.Text = "Cancel";
            _cancel.X = (Width - _cancel.Width) - 15;
            _cancel.Y = (Height - _cancel.Height) - 15;

            _apply.Text = "Apply changes";
            _apply.Y = _cancel.Y;
            _apply.X = (_cancel.X - _apply.Width) - 15;
        
        }
    }
}
