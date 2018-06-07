using Plex.Engine.Cutscene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Newtonsoft.Json;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Plex.Engine.Themes;
using Plex.Objects;

namespace Peacenet.Cutscenes
{
    /// <summary>
    /// The cutscene that displays the game's credits.
    /// </summary>
    public class CreditsScene : Plex.Engine.Cutscene.Cutscene
    {
        private AdvancedAudioPlayer _music = null;
        private CreditCategory[] _creditsFile = null;
        private int _csState = 0;

        private const float _catHeadSpacingY = 15f;
        private const float _entryHeadSpacingY = 5;
        private const float _entryTextSpacingY = 7;

        private float _totalHeight = 0f;

        [Dependency]
        private UIManager _ui = null;

        [Dependency]
        private ThemeManager _theme = null;

        private Texture2D _peacenet = null;


        private float _creditOpacity = 1f;
        private double _ride = 0f;
        private float _youOpacity = 1f;
        private float _peacenetOpacity = 0f;
        private float _thanksOpacity = 0f;

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "credits_00";
            }
        }

        private float _maxWidth = 0;

        /// <inheritdoc/>
        public override void Draw(GameTime time, GraphicsContext gfx)
        {
            var head1 = _theme.Theme.GetFont(TextFontStyle.Header1);
            var head2 = _theme.Theme.GetFont(TextFontStyle.Header3);
            var sys = _theme.Theme.GetFont(TextFontStyle.System);


            float h = MathHelper.Lerp(_ui.ScreenHeight, (_ui.ScreenHeight / 2) - _totalHeight, _grassPercentage);

            foreach (var cat in _creditsFile)
            {
                var cmeasure = TextRenderer.MeasureText(cat.Text, head1, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                gfx.DrawString(cat.Text, (_ui.ScreenWidth - (int)_maxWidth) / 2, (int)h, _theme.Theme.GetFontColor(TextFontStyle.Header1) * _creditOpacity, head1, TextAlignment.Center, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                h += cmeasure.Y + _catHeadSpacingY;
                foreach (var entry in cat.Entries)
                {
                    float opacity = (entry == cat.Entries.Last() && cat == _creditsFile.Last()) ? _youOpacity : _creditOpacity;

                    var hmeasure = TextRenderer.MeasureText(entry.Header, head2, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                    var tmeasure = TextRenderer.MeasureText(entry.Text, sys, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                    gfx.DrawString(entry.Header, (_ui.ScreenWidth - (int)_maxWidth) / 2, (int)h, _theme.Theme.GetFontColor(TextFontStyle.Header2) * opacity, head2, TextAlignment.Center, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                    h += hmeasure.Y + _entryHeadSpacingY;
                    gfx.DrawString(entry.Text, (_ui.ScreenWidth - (int)_maxWidth) / 2, (int)h, _theme.Theme.GetFontColor(TextFontStyle.System) * opacity, sys, TextAlignment.Center, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                    h += tmeasure.Y + _entryTextSpacingY;
                }
            }

            gfx.FillRectangle((_ui.ScreenWidth - _peacenet.Width) / 2, (_ui.ScreenHeight - _peacenet.Height) / 2, _peacenet.Width, _peacenet.Height, _peacenet, Color.White * _peacenetOpacity);

            string thanksText = "Thanks for playing.";
            var thanksMeasure = head1.MeasureString(thanksText);
            gfx.DrawString(head1, thanksText, new Vector2((_ui.ScreenWidth - thanksMeasure.X) / 2, (_ui.ScreenHeight - thanksMeasure.Y) / 2), _theme.Theme.GetFontColor(TextFontStyle.Header1) * _thanksOpacity);
        }

        /// <inheritdoc/>
        public override void Load(ContentManager Content)
        {
            Logger.Log("Credits Loading NOW");
            _music = new AdvancedAudioPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Audio", "Cutscene", "Credits" + (_doAlternateCreditsMusic ? "Alt" : "") + ".ogg"), false);
            string creditsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "credits.json");
            string json = File.ReadAllText(creditsPath);
            _creditsFile = JsonConvert.DeserializeObject<CreditCategory[]>(json);
            _peacenet = Content.Load<Texture2D>("Splash/Peacenet");


        }


        /// <inheritdoc/>
        public override void OnFinish()
        {
            _ui.ShowUI();
            _music.Stop();
        }

        private float _grassPercentage = 0f;
        private double _grassTime = 0f;

        [Dependency]
        private GameLoop _GameLoop = null;

        /// <inheritdoc/>
        public override void OnPlay()
        {
            _ui.HideUI();
            _csState = 0;
            _grassPercentage = 0f;
            Logger.Log("Get ready get ready get ready get ready", System.ConsoleColor.DarkGreen);
            _music.Play();
            Logger.Log("You're listening to Greener Grass", System.ConsoleColor.Green);
            _maxWidth = _ui.ScreenWidth / 2;

            var head1 = _theme.Theme.GetFont(TextFontStyle.Header1);
            var head2 = _theme.Theme.GetFont(TextFontStyle.Header3);
            var sys = _theme.Theme.GetFont(TextFontStyle.System);

            float h = 0;
            foreach (var cat in _creditsFile)
            {
                var cmeasure = TextRenderer.MeasureText(cat.Text, head1, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                h += cmeasure.Y + _catHeadSpacingY;
                foreach (var entry in cat.Entries)
                {
                    var hmeasure = TextRenderer.MeasureText(entry.Header, head2, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                    var tmeasure = TextRenderer.MeasureText(entry.Text, sys, (int)_maxWidth, Plex.Engine.TextRenderers.WrapMode.Words);
                    h += hmeasure.Y + _entryHeadSpacingY + tmeasure.Y + _entryTextSpacingY;
                }
            }
            _totalHeight = h;
        }

        private static bool _doAlternateCreditsMusic = true;

        public static bool DoAltCreditsTheme
        {
            get
            {
                return _doAlternateCreditsMusic;
            }
            set
            {
                _doAlternateCreditsMusic = value;
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            _music.Dispose();
            _peacenet.Dispose();
        }

        /// <inheritdoc/>
        public override void Update(GameTime time)
        {
            switch (_csState)
            {
                case 0:
                    _creditOpacity = 1f;
                    _youOpacity = 1f;
                    _peacenetOpacity = 0f;
                    _thanksOpacity = 0f;
                    _grassTime += time.ElapsedGameTime.TotalSeconds;
                    _grassPercentage = (float)(_grassTime / _music.Duration.TotalSeconds);
                    if(_grassPercentage>=1f)
                    {
                        _grassPercentage = 1f;
                        _csState++;
                        
                    }
                    break;
                case 1:
                    _creditOpacity -= (float)time.ElapsedGameTime.TotalSeconds;
                    if(_creditOpacity<=0)
                    {
                        _creditOpacity = 0;
                        _ride = 0;
                        _csState++;
                    }
                    break;
                case 2:
                    _ride += time.ElapsedGameTime.TotalSeconds;
                    if(_ride>=5)
                    {
                        _ride = 0;
                        _csState++;
                    }
                    break;
                case 3:
                    _youOpacity -= (float)time.ElapsedGameTime.TotalSeconds;
                    if(_youOpacity<=0f)
                    {
                        _youOpacity = 0f;
                        _csState++;
                    }
                    break;
                case 4:
                    _peacenetOpacity += (float)time.ElapsedGameTime.TotalSeconds;
                    if(_peacenetOpacity>=1f)
                    {
                        _peacenetOpacity = 1f;
                        _csState++;
                        _ride = 0;
                    }
                    break;
                case 5:
                    _ride += time.ElapsedGameTime.TotalSeconds;
                    if(_ride>=5)
                    {
                        _csState++;
                        _ride = 0;
                    }
                    break;
                case 6:
                    _peacenetOpacity -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (_peacenetOpacity <= 0f)
                    {
                        _peacenetOpacity = 0f;
                        _csState++;
                    }
                    break;
                case 7:
                    _thanksOpacity += (float)time.ElapsedGameTime.TotalSeconds;
                    if (_thanksOpacity >= 1f)
                    {
                        _thanksOpacity = 1f;
                        _csState++;
                        _ride = 0;
                    }
                    break;
                case 8:
                    _ride += time.ElapsedGameTime.TotalSeconds;
                    if (_ride >= 5)
                    {
                        _csState++;
                        _ride = 0;
                    }
                    break;
                case 9:
                    _thanksOpacity -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (_thanksOpacity <= 0f)
                    {
                        _thanksOpacity = 0f;
                        _csState++;
                        NotifyFinished();
                    }
                    break;

            }
        }
    }

    public class CreditCategory
    {
        public string Text { get; set; }
        public CreditEntry[] Entries { get; set; }
    }

    public class CreditEntry
    {
        public string Header { get; set; }
        public string Text { get; set; }
    }
}
