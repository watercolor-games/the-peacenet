using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Frontend.GUI;
using Plex.Engine;
using Microsoft.Xna.Framework;

namespace Plex.Frontend.Apps
{
    [Obsolete("This feature will be removed in Milestone 2.")]
    [DefaultTitle("Disclaimer")]
    public class Disclaimer : Control, IPlexWindow
    {
        private TextControl _title = new TextControl();
        private BBCodeLabel _body = new BBCodeLabel();
        private Button _close = new Button();
        private Button _viewRules = new Button();

        public Disclaimer()
        {
            Width = 800;
            Height = 600;

            AddControl(_title);
            AddControl(_body);
            AddControl(_close);
            AddControl(_viewRules);

            _close.Click += () =>
            {
                AppearanceManager.Close(this);
            };
            _viewRules.Click += () =>
            {
                System.Diagnostics.Process.Start("http://forums.getshiftos.net/about");
            };

            _close.AutoSize = true;
            _viewRules.AutoSize = true;
            _title.AutoSize = true;

            _title.Text = "Welcome to Project: Plex!";
            _close.Text = "OK.";
            _viewRules.Text = "View the Watercolor Games community guidelines";


        }

        public void OnLoad()
        {
            _body.Text = @"[b]Welcome to the Project: Plex pre-alpha![/b]

We very much appreciate you taking the time to try out this pre-alpha build of Project: Plex. We just have a few things to tell you first. Please read this dialog carefully.

[u]Public server details[/u]

The public multiplayer server is currently unavailable for the pre-alpha builds. You must either play in Campaign, find an unofficial multiplayer server, or host your own.

[b][u]This game is not a finished product!![/u][/b]

Please, expect bugs, missing features, and crashes. The point of this prealpha is so we can find these issues and correct them. This is in no way a finished product, but with your help, we can make it into one.

[u]Lastly...[/u]

Your use of the multiplayer features of this game are subject to the rules and guidelines set by the server administrator, which may or may not include the [u]Watercolor Games Community Guidelines[/u].

Also, [b]DO NOT STORE PERSONAL INFORMATION SUCH AS ADDRESSES, CREDIT CARDS, PRIVATE EMAIL ADDRESSES or OTHER SENSITIVE INFORMATION in a Project: Plex system. This data CAN and WILL BE STOLEN. We are not responsible for the theft of personal data stored within a hackable system. You're on your own if that happens.[/b]";
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _title.X = 15;
            _title.Y = 15;

            _viewRules.X = 15;
            _viewRules.Y = (Height - _viewRules.Height) - 15;

            _close.Y = _viewRules.Y;
            _close.X = (Width - _close.Width) - 15;

            _body.Y = _title.Y + _title.Height + 10;
            _body.X = 15;
            _body.Width = Width - 30;
            _body.Height = (Height - _close.Height - 30 - _body.Y);
        }

        public void OnSkinLoad()
        {
            _title.FontStyle = TextControlFontStyle.Header1;
            _close.Font = SkinEngine.LoadedSkin.MainFont;
            _viewRules.Font = SkinEngine.LoadedSkin.MainFont;
            _body.Font = SkinEngine.LoadedSkin.MainFont;
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
