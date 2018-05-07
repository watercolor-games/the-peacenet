using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Themes;
using cutscene = Plex.Engine.Cutscene;
namespace Peacenet.Cutscene
{
    public class PrologueCutscene : cutscene.Cutscene
    {
        private int _state = 0;
        private double _ride = 0;
        private int _charIndex = 0;
        private int _lineIndex = 0;
        private double _cursorTime = 0;

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
            "[Starting Mission Briefing]",
            @"Greetings.

You have been selected to participate in a special work-release program.

From this day forward, you work for the Elytrose Federal Government.",
            @"Your mission is simple.

You will be placed within The Peacenet, a digital afterlife. This afterlife is under war.",
            @"This world and all its inhabitants run on the Peacenet Gateway Operating System, or, Peacegate OS for short.

You will enter this world posing as a sentient AI, and unlike most real inhabitants of the Peacenet, you will be given interactive access to the operating system to complete your mission.",
            @"And your mission is simple.

End the war.",
            "NOTE. This mission is NOT an excuse to be exempt from Elytrose federal law. As such, any misconduct within The Peacenet on your part will result in termination of your connection. We're watching you.",
            "After Peacegate OS boots up and finishes preliminary setup, you will enter an interactive tutorial.",
            "Complete the tutorial, then enter the World Map to find out your next objective.",
            "Starting Peacegate OS. Good luck."
#endif
        };

        public override string Name => "m00_briefing";

        public override void Load(ContentManager content)
        {
            _bgm = content.Load<SoundEffect>("Audio/Cutscene/Prologue/Briefing").CreateInstance();
        }

        public override void OnPlay()
        {
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
                    if(_ride>=5)
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
                    _bgm.Volume = MathHelper.Clamp(_bgm.Volume - (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
                    if (_bgm.Volume == 0)
                    {
                        _bgm.Stop();
                        NotifyFinished();
                    }
                    break;
            }
        }

        public override void Draw(GameTime time, GraphicsContext gfx)
        {
            var mono = _theme.Theme.GetFont(TextFontStyle.Mono);

            string introText = TextRenderer.WrapText(mono, _lines[_lineIndex].Substring(0, _charIndex), gfx.Width / 3, Plex.Engine.TextRenderers.WrapMode.Words);
            var introTextMeasure = mono.MeasureString(introText);
            var cursorMeasure = mono.MeasureString("#");

            gfx.BeginDraw();
            gfx.Clear(Color.Black);

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

            gfx.EndDraw();
        }
    }
}
