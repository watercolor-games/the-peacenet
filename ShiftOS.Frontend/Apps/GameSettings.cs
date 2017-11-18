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

        private TextControl _mainHeader = null;
        private TextControl _mainDescription = null;

        private ScrollView _optionsView = null;

        private TextControl _ohGraphics = null;

        private CheckLabel _gfxFullscreen = null;

        private TextControl _ohDiscord = null;
        private TextControl _ohDiscordDesc = null;

        private CheckLabel _rpcEnable = null;


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
            _mainHeader = new TextControl();
            _mainDescription = new TextControl();
            _optionsView = new ScrollView();
            _ohGraphics = new TextControl();
            _gfxFullscreen = new CheckLabel();
            _ohDiscord = new TextControl();
            _ohDiscordDesc = new TextControl();
            _rpcEnable = new CheckLabel();

            AddControl(_resHeader);
            AddControl(_resScroller);
            _resScroller.AddControl(_resChoices);

            AddControl(_apply);
            AddControl(_cancel);

            AddControl(_mainHeader);
            AddControl(_mainDescription);
            AddControl(_optionsView);

            _optionsView.AddControl(_ohGraphics);
            _optionsView.AddControl(_gfxFullscreen);
            _optionsView.AddControl(_ohDiscord);
            _optionsView.AddControl(_ohDiscordDesc);
            _optionsView.AddControl(_rpcEnable);


            _apply.Click += () =>
            {
                ConfigurationManager.SetResolution(_resUserSet);
                ConfigurationManager.SetFullscreen(_gfxFullscreen.Value);
                ConfigurationManager.SetRPCEnable(_rpcEnable.Value);

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

            _resChoices.RequireTextRerender();
            _resChoices.Invalidate();

            _resChoices.SelectedIndexChanged += () =>
            {
                if (_resChoices.SelectedIndex == -1)
                {
                    _resChoices.SelectedIndex = _resInitial;
                    _resChoices.RequireTextRerender();
                    _resChoices.Invalidate();
                }
                else
                {
                    _resUserSet = _resChoices.SelectedIndex;
                }
            };

            _gfxFullscreen.Value = ConfigurationManager.GetFullscreen();
            _rpcEnable.Value = ConfigurationManager.GetRPCEnable();
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
            _resHeader.FontStyle = TextControlFontStyle.Header2;
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

            _mainHeader.Y = 15;
            _mainHeader.X = _resHeader.X + _resScroller.Width + 20;
            _mainHeader.FontStyle = TextControlFontStyle.Header1;
            _mainHeader.AutoSize = true;
            _mainHeader.MaxWidth = (Width - _mainHeader.X) - 15;
            _mainHeader.Text = "Options";

            _mainDescription.AutoSize = true;
            _mainDescription.X = _mainHeader.X;
            _mainDescription.Y = _mainHeader.Y + _mainHeader.Height + 10;
            _mainDescription.MaxWidth = _mainHeader.MaxWidth;
            _mainDescription.Text = "Set various settings to adjust your playing experience in The Peacenet. You can set the screen resolution at the side of this window. Close the window or click Cancel to discard your changes.";

            _optionsView.X = _mainDescription.X;
            _optionsView.Y = _mainDescription.Y + _mainDescription.Height + 10;
            _optionsView.Width = _mainDescription.MaxWidth;
            _optionsView.Height = (Height - _optionsView.Y) - (_apply.Height + 30);

            _ohGraphics.Text = "Graphics";
            _ohGraphics.X = 15;
            _ohGraphics.Y = 15;
            _ohGraphics.AutoSize = true;
            _ohGraphics.FontStyle = TextControlFontStyle.Header2;
            _ohGraphics.MaxWidth = _optionsView.Width - 30;

            _gfxFullscreen.AutoSize = true;
            _gfxFullscreen.Text = "Enable fullscreen";
            _gfxFullscreen.X = 15;
            _gfxFullscreen.Y = _ohGraphics.Y + _ohGraphics.Height + 10;

            _ohDiscord.X = 15;
            _ohDiscord.Y = _gfxFullscreen.Y + _gfxFullscreen.Height + 30;
            _ohDiscord.AutoSize = true;
            _ohDiscord.MaxWidth = _ohGraphics.MaxWidth;
            _ohDiscord.FontStyle = TextControlFontStyle.Header2;
            _ohDiscord.Text = "Discord Rich Presence";

            _ohDiscordDesc.X = 15;
            _ohDiscordDesc.Y = _ohDiscord.Y + _ohDiscord.Height + 10;
            _ohDiscordDesc.AutoSize = true;
            _ohDiscordDesc.MaxWidth = _ohDiscord.MaxWidth;
            _ohDiscordDesc.Text = "Are you on the Watercolor Games Discord and/or want to brag about what you're doing in The Peacenet to your Discord buddies?\r\n\r\nThese settings will allow you to choose whether or not we'll display gameplay info on your Discord profile, and what info is shown. Note that in order for Rich Presence to work, you must have the Discord desktop client installed and running. You must also make sure Discord is detecting that you are playing The Peacenet.";

            _rpcEnable.X = 15;
            _rpcEnable.Y = _ohDiscordDesc.Y + _ohDiscordDesc.Height + 5;
            _rpcEnable.AutoSize = true;
            _rpcEnable.Text = "Enable Rich Presence";
        }
    }
}
