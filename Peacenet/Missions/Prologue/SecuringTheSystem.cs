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

        private string _hostname = "";

        
        private ObjectiveCountdownEntity _countdown = null;

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
                    if(ride == 0)
                    {
                        if (string.IsNullOrWhiteSpace(_hostname))
                            _hostname = _os.GetHostname();
                        else
                        {
                            string hostname = _os.GetHostname();
                            if (hostname != _hostname)
                                return ObjectiveState.Complete;
                        }
                    }
                    ride += time.ElapsedGameTime.TotalSeconds;
                    if (ride >= 5)
                    {
                        ride = 0;
                    }
                    return ObjectiveState.Active;
                });
                yield return new Objective("A system has just connected to you.", "That's not good. Another person within The Peacenet has just connected to you. You'll need to either change your IP address or hack them back to get them off your system. Since you cannot currently change your IP address, you'll need to counter-attack. Find this person's IP by typing the 'connections' command in your Terminal. HURRY, before this person does damage.", (time) =>
                {
                    if(ride == 0)
                    {
                        _countdown = _plexgate.New<ObjectiveCountdownEntity>();
                        _plexgate.GetLayer(LayerType.Foreground).AddEntity(_countdown);
                        _countdown.TimedOut += () =>
                        {
                            ride = 2;
                        };
                        ride += 1;
                    }
                    if (ride == 2)
                        return ObjectiveState.Failed;
                    else
                        return ObjectiveState.Active;
                });

            }
        }

        public override void OnStart()
        {
            _missionTheme = _plexgate.Content.Load<SoundEffect>("Audio/SecuringTheSystem");
            _themeInstance = _missionTheme.CreateInstance();
            _themeInstance.IsLooped = true;
            _themeInstance.Play();
            base.OnStart();
        }

        public override void OnEnd()
        {
            _themeInstance.Stop();
            base.OnEnd();
        }
    }

    public class ObjectiveCountdownEntity : IEntity
    {
        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private ThemeManager _theme = null;

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

        public void Update(GameTime time)
        {
            _timeout -= time.ElapsedGameTime.TotalSeconds;
            if(_timeout <= 0)
            {
                _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(this);
                TimedOut?.Invoke();
            }
        }
    }
}
