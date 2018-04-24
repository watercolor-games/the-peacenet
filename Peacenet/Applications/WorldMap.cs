using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Microsoft.Xna.Framework;
using Peacenet.DesktopUI;
using Microsoft.Xna.Framework.Graphics;
using Peacenet.CountryChallenges;

namespace Peacenet.Applications
{
    [AppLauncher("World Map", "Communication", "View a full map of The Peacenet.")]
    public class WorldMap : Window
    {
        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private ChallengeManager _challengeMgr = null;

        [Dependency]
        private CountryManager _countries = null;

        private Label _title = new Label();
        private Label _countryName = new Label();
        private Button _back = new Button();

        private ScrollView _challengesView = new ScrollView();
        private Label _challengesHead = new Label();
        private Stacker _challenges = new Stacker();

        private Effect _mapEffect = null;

        private Texture2D _elytrose = null;
        private Texture2D _valkerie = null;
        private Texture2D _oglowia = null;
        private Texture2D _sikkim = null;
        private Texture2D _velacrol = null;
        private Texture2D _riogan = null;
        private Texture2D _mejionde = null;


        public WorldMap(WindowSystem _winsys) : base(_winsys)
        {
            Title = "World map";
            SetWindowStyle(WindowStyle.NoBorder);
            AddChild(_title);
            AddChild(_countryName);
            _countryName.AutoSize = true;
            _countryName.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            AddChild(_back);
            _back.Text = "Back to Peacegate";
            _back.Click += (o, a) =>
            {
                Close();
            };
            _title.Text = "World Map";
            _title.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _title.AutoSize = true;
            AddChild(_challengesHead);
            AddChild(_challengesView);
            _challengesView.AddChild(_challenges);
            _challengesHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _challengesHead.AutoSize = true;
            _challengesHead.Text = "Country Challenges";
            _challengesView.Width = 350;
            _challenges.AutoSize = true;
            _challenges.Width = 350;

            _elytrose = _plexgate.Content.Load<Texture2D>("WorldMap/1");
            _riogan = _plexgate.Content.Load<Texture2D>("WorldMap/4");

            _mapEffect = _plexgate.Content.Load<Effect>("Shaders/WorldMapShader");

            _countries.CountryChanged += _countries_CountryChanged;

            SetupChallengeUI();
        }

        public void SetupChallengeUI()
        {
            _challenges.Clear();
            foreach(var challenge in _challengeMgr.GetChallenges(_countries.CurrentCountry))
            {
                var label = new Label();
                var progress = new ProgressBar();
                label.Text = challenge.Name;
                label.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
                label.AutoSize = true;
                label.MaxWidth = _challenges.Width;
                _challenges.AddChild(label);
                progress.Width = _challenges.Width;
                if (challenge.UIPercentageText == null)
                    progress.Text = (challenge.IsComplete) ? "Completed" : "Not completed";
                else
                    progress.Text = challenge.UIPercentageText;
                progress.Value = challenge.PercentageComplete;
                _challenges.AddChild(progress);
            }
        }


        private void _countries_CountryChanged()
        {
            SetupChallengeUI();
        }

        public override void Close()
        {
            _countries.CountryChanged -= this._countries_CountryChanged;
            base.Close();
        }

        protected override void OnUpdate(GameTime time)
        {
            Parent.X = 0;
            Parent.Y = 0;
            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;

            _title.X = 15;
            _title.Y = 15;
            _countryName.X = 15;
            _countryName.Y = _title.Y + _title.Height + 10;
            _countryName.Text = _countries.CurrentCountry.ToString();
            _back.Y = _title.Y + (_title.Height - _back.Height) / 2;
            _back.X = (Width - _back.Width) - 15;

            _challengesView.Height = Height / 3;

            _challengesHead.Y = _title.Y + _title.Height + 45;
            _challengesView.Y = _challengesHead.Y + _challengesHead.Height + 15;
            _challengesView.X = Width - _challengesView.Width - 15;
            _challengesHead.X = _challengesView.X;


            base.OnUpdate(time);
        }
    }
}
