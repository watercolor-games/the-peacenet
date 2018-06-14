using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Objects;
using Plex.Engine;
using Peacenet.Applications;
using Plex.Engine.GUI;

namespace Peacenet.Missions.Prologue
{
    public class WelcomeToThePeacenet : Mission
    {
        [Dependency]
        private TerminalManager _terminal = null;

        [Dependency]
        private WindowSystem _winsys = null;

        public WelcomeToThePeacenet() : base("Welcome to The Peacenet", "Welcome to The Peacenet. You've just finished installing Peacegate OS. Now it's time to find out why.")
        {
            AddObjective("Pay attention to the Terminal");
        }

        protected override void UpdateObjective(GameTime time, int objectiveIndex)
        {
        }

        protected override void OnStart()
        {
            _terminal.LoadCommand(new TutorialCommand(this));
            var terminal = new TutorialTerminal(_winsys);
            terminal.Show();
            

            base.OnStart();
        }

        private class TutorialTerminal : Terminal
        {
            public TutorialTerminal(WindowSystem _winsys) : base(_winsys)
            {
            }

            protected override string Shell => "tutorial";
        }

        [TerminalSkipAutoload]
        [HideInHelp]
        private class TutorialCommand : ITerminalCommand
        {
            [Dependency]
            private TerminalManager _terminal = null;

            private WelcomeToThePeacenet _mission = null;

            public TutorialCommand(WelcomeToThePeacenet mission)
            {
                _mission = mission;
            }

            public string Name => "tutorial";

            public string Description => "";

            public IEnumerable<string> Usages => null;

            public void Run(ConsoleContext console, Dictionary<string, object> arguments)
            {
                _terminal.UnloadCommand("terminal");
                console.ReadLine();
            }
        }

    }
}
