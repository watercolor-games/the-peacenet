using Plex.Engine;
using Plex.Engine.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Themes;
using Microsoft.Xna.Framework.Content;
using Peacenet.Filesystem;

namespace Peacenet.Missions.Prologue
{
    public class SecuringTheSystem : Mission
    {
        private SoundEffect _missionTheme = null;
        private SoundEffectInstance _themeInstance = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private FSManager _fs = null;

        public override bool Available
        {
            get
            {
                return _save.GetValue<bool>("mission.the_terminal.completed", false);
            }
        }

        public override string Description
        {
            get
            {
                return "You now know the basics of the Peacenet command line. Now it's time to get your system connected to The Peacenet, and secured from the digital warfare.";
            }
        }

        public override string Name
        {
            get
            {
                return "Securing the System";
            }
        }

        private double ride = 0;

        private bool _hnChanged = true;

        
        private ObjectiveCountdownEntity _countdown = null;

        private bool _hackStarted = false;

        private bool _objConnectionsCmdRun = false;

        private bool _countdownExpired = false;

        public override IEnumerable<Objective> ObjectiveList
        {
            get
            {
                yield return new Objective("It's time to secure your system.", "As you read this, your system is currently at risk of being hacked within The Peacenet.", (time) =>
                {
                    ride += time.ElapsedGameTime.TotalSeconds;
                    if(ride>=10)
                    {
                        ride = 0;
                        return ObjectiveState.Complete;
                    }
                    return ObjectiveState.Active;
                });
                yield return new Objective("Set a hostname.", "A hostname may not be important for security, but it'll be better if you have something other than 'localhost'. Set a hostname by editing the \"/etc/hostname\" file in your Terminal.", (time) =>
                {
                    if (_os.Hostname != "localhost")
                    {
                        this._themeInstance.Volume = MathHelper.Clamp(_themeInstance.Volume - ((float)time.ElapsedGameTime.TotalSeconds * 4), 0f, 1f);
                        if (_themeInstance.Volume <= 0f)
                        {
                            if(!_hackStarted)
                            {
                                _os.SimulateConnectionFromSystem("142.68.67.3");
                                _hackStarted = true;
                                ride += time.ElapsedGameTime.TotalSeconds;
                                return ObjectiveState.Active;
                            }
                            if (!_os.IsPlayingNewConnectionAnimation)
                                return ObjectiveState.Complete;
                        }
                        ride += time.ElapsedGameTime.TotalSeconds;
                        return ObjectiveState.Active;
                    }
                    _hnChanged = false;
                    ride += time.ElapsedGameTime.TotalSeconds;
                    if (ride >= 5)
                    {
                        ride = 0;
                    }
                    _objConnectionsCmdRun = false;
                    return ObjectiveState.Active;
                });
                yield return new Objective("A system has just connected to you.", "Quick! Run 'connections' to find out the IP address of the system!", (time) => _objConnectionsCmdRun ? ObjectiveState.Complete : ObjectiveState.Active);

            }
        }

        public override void OnStart()
        {
            _missionTheme = _plexgate.Content.Load<SoundEffect>("Audio/SecuringTheSystem");
            _themeInstance = _missionTheme.CreateInstance();
            _themeInstance.IsLooped = true;
            _themeInstance.Play();
            base.OnStart();
            Applications.ShellCommand.OnCommandRun += ShellCommand_OnCommandRun;
        }

        private void ShellCommand_OnCommandRun(WatercolorGames.CommandLine.CommandInstruction instruction)
        {
            if (instruction.OutputFile == null && instruction.Commands.First() == "connections")
                _objConnectionsCmdRun = true;

        }

        public override void OnEnd()
        {
            Applications.ShellCommand.OnCommandRun -= ShellCommand_OnCommandRun;
            _themeInstance.Stop();
            base.OnEnd();
        }
    }

    public class ObjectiveCountdownEntity : IEntity, ILoadable
    {
        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private ThemeManager _theme = null;

        private double _secondCounter = 1;
        private int _tickTock = 0;

        private SoundEffect _tick = null;
        private SoundEffect _tock = null;
        private SoundEffect _beep = null;

        private double _timeout = 60;

        public event Action TimedOut;

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            string header = "Hurry up!";
            string countdown = _timeout.ToString("#.##");

            var headFont = _theme.Theme.GetFont(TextFontStyle.System);
            var countdownFont = _theme.Theme.GetFont(TextFontStyle.Header1);

            var headColor = _theme.Theme.GetFontColor(TextFontStyle.System);
            var countdownColor = _theme.Theme.GetFontColor(TextFontStyle.Header1);

            var headMeasure = headFont.MeasureString(header);
            var countdownMeasure = countdownFont.MeasureString(countdown);

            gfx.BeginDraw();
            gfx.DrawString(countdown, (gfx.Width - (int)countdownMeasure.X) - 45, (gfx.Height - (int)countdownMeasure.Y) - 45, countdownColor, countdownFont, TextAlignment.Left);
            gfx.DrawString(header, (gfx.Width - (int)headMeasure.X) - 45, (((gfx.Height - (int)countdownMeasure.Y) - 45) - (int)headMeasure.Y) - 5, headColor, headFont, TextAlignment.Left);
            gfx.EndDraw();
        }

        public void OnGameExit()
        {
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        private int _warnCount = 0;
        private double _warnTime = 0;

        public void Update(GameTime time)
        {
            _timeout -= time.ElapsedGameTime.TotalSeconds;
            if(_timeout < 30)
            {
                if(_warnCount == 0)
                {
                    _warnCount++;
                    _beep.Play();
                }
            }
            if (_timeout < 15)
            {
                if (_warnCount == 1)
                {
                    _warnCount++;
                    _beep.Play();
                }
            }
            if (_timeout <= 11)
            {
                _warnTime += time.ElapsedGameTime.TotalSeconds;
                if(_warnTime >= ((_timeout < 5) ? 0.5 : 1))
                {
                    _warnTime = 0;
                    _beep.Play();
                }
            }

            if(_secondCounter>=1)
            {
                _secondCounter = 0;
                switch (_tickTock)
                {
                    case 0:
                        _tick.Play();
                        _tickTock = 1;
                        break;
                    case 1:
                        _tock.Play();
                        _tickTock = 0;
                        break;
                }

            }
            _secondCounter += time.ElapsedGameTime.TotalSeconds;

            if (_timeout <= 0)
            {
                _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(this);
                TimedOut?.Invoke();
            }
        }

        public void Load(ContentManager content)
        {
            _beep = content.Load<SoundEffect>("SFX/pcspkr");
            _tick = content.Load<SoundEffect>("SFX/SniffClick_360_01");
            _tock = content.Load<SoundEffect>("SFX/ClickFlam_SP_10_441");
        }
    }
}
