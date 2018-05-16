using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Themes;
using cutscene = Plex.Engine.Cutscene;
namespace Peacenet.Cutscene
{
    public class PrologueCutscene : cutscene.Cutscene
    {
        private SpriteFont _font = null;
        private int _state = 0;
        private double _ride = 0;
        private int _charIndex = 0;
        private int _lineIndex = 0;
        private double _cursorTime = 0;

        private double _installTime = 5;
        private bool _showInstallCounter = false;

        private SoundEffectInstance _bgm = null;

        [Dependency]
        private ThemeManager _theme = null;

        private readonly string[] _lines = new string[]
        {
#if DEBUG
            "This is a debug build. Starting tutorial in Fast Play mode."
#else
            "Watercolor Games presents",
            "An Alkaline Thunder game",
            "Powered by Peace Engine",
            "",
            "Greetings, prisoner.\n\nYou have been selected to participate in a special work-release program.",
            "Very shortly, you will be placed within a digital afterlife on an important mission.\n\nYour mission will become apparent shortly.",
            "You will use this world's gateway operating system, \"Peacegate OS,\" to interact with this world.\n\nYou will be contacted after setup is complete with the mission details. Good luck."
#endif
        };

        public override string Name => "m00_briefing";

        public override void Load(ContentManager content)
        {
            _bgm = content.Load<SoundEffect>("Audio/Cutscene/Prologue/Briefing").CreateInstance();
            _font = content.Load<SpriteFont>("ThemeAssets/Fonts/PrologueFont");
        }

        public override void OnPlay()
        {
            _showInstallCounter = false;
            _bgm.Volume = 1;
            _bgm.Play();
            _bgm.IsLooped = true;
        }

        public override void Update(GameTime time)
        {
            switch(_state)
            {
                case 0:
                    _cursorTime = 0.25;
                    _ride += time.ElapsedGameTime.TotalSeconds;
                    if (_ride >= 0.05)
                    {
                        if (_charIndex < _lines[_lineIndex].Length)
                        {
                            _charIndex++;
                            _ride = 0;
                        }
                        else
                        {
                            _ride = 0;
                            _state++;
                        }
                    }
                    break;
                case 1:
                    _cursorTime += time.ElapsedGameTime.TotalSeconds;
                    if (_cursorTime > 0.5)
                        _cursorTime = 0;
                    _ride += time.ElapsedGameTime.TotalSeconds;
                    if(_ride >= ((_lineIndex>3) ? 3.5 : 5))
                    {
                        _charIndex = 0;
                        if(_lineIndex < _lines.Length-1)
                        {
                            _lineIndex++;
                            _state++;
                            _ride = 0;
                            _cursorTime = 0;
                        }
                        else
                        {
                            _state=3;
                            _installTime = 10;
                            _showInstallCounter = true;
                        }
                    }
                    break;
                case 2:
                    _ride += time.ElapsedGameTime.TotalSeconds;
                    if (_ride >= 1)
                    {
                        _state = 0;
                        _ride = 0;
                    }
                    break;
                case 3:
                    _cursorTime = 0;
                    _installTime -= time.ElapsedGameTime.TotalSeconds;
                    _bgm.Volume = MathHelper.Clamp((float)_installTime / 10, 0, 1);
                    if(_installTime<=0)
                    {
                        _bgm.Stop();
                        _showInstallCounter = false;
                        NotifyFinished();
                    }
                    break;
            }
        }

        public override void Draw(GameTime time, GraphicsContext gfx)
        {
            var mono = _font;

            string introText = TextRenderer.WrapText(mono, _lines[_lineIndex].Substring(0, _charIndex), (int)(gfx.Width * 0.75), Plex.Engine.TextRenderers.WrapMode.Words);
            var introTextMeasure = mono.MeasureString(introText);
            var cursorMeasure = mono.MeasureString("#");

            gfx.BeginDraw();

            var textLocation = new Vector2((gfx.Width - introTextMeasure.X) / 2, (gfx.Height - introTextMeasure.Y) / 2);
            var lines = introText.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var measure = mono.MeasureString(line);
                var loc = new Vector2((gfx.Width - measure.X) / 2, textLocation.Y + (measure.Y * i));
                gfx.Batch.DrawString(mono, line, loc, _theme.Theme.GetAccentColor().Lighten(0.5F));
                if (i == lines.Length - 1 && _cursorTime >= 0.25)
                {
                    gfx.DrawRectangle(new Vector2(loc.X + measure.X, loc.Y), cursorMeasure, Color.White);
                }
            }

            if(_showInstallCounter)
            {
                var installText = "Commencing installation in ";
                var timeText = string.Format("{0:N2}s", _installTime);
                var measure = mono.MeasureString(installText + timeText);
                var installWidth = mono.MeasureString(installText).X;
                var textLoc = new Vector2((gfx.Width - measure.X) / 2, (gfx.Height - measure.Y) / 2);

                gfx.DrawRectangle(new Vector2(textLoc.X - 30, textLoc.Y - 7), new Vector2(MathHelper.Lerp(0, measure.X + 60, ((float)_installTime / 10)), measure.Y + 14), Color.Red.Darken(0.5F));

                gfx.Batch.DrawString(mono, installText, textLoc, _theme.Theme.GetAccentColor().Lighten(0.5f));
                gfx.Batch.DrawString(mono, timeText, new Vector2(textLoc.X + installWidth, textLoc.Y), Color.Red);


            }

            gfx.EndDraw();
        }
    }
}
