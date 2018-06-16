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
using System.Threading;

namespace Peacenet.Missions.Prologue
{
    public class WelcomeToThePeacenet : Mission
    {
        [Dependency]
        private TerminalManager _terminal = null;

        [Dependency]
        private GameManager _game = null;

        [Dependency]
        private WindowSystem _winsys = null;

        private TutorialTerminal _tutorialTerminal = null;

        private ManualResetEvent _worldMapOpen = new ManualResetEvent(false);

        public EventWaitHandle WorldMapOpen => _worldMapOpen;

        private WorldMap _map = null;

        public WelcomeToThePeacenet() : base("Welcome to The Peacenet", "Welcome to The Peacenet. You've just finished installing Peacegate OS. Now it's time to find out why.")
        {
            AddObjective("Pay attention to the Terminal");
            AddObjective("Find and open World Map");
            AddObjective("Select the highlighted system");
        }

        protected override void UpdateObjective(GameTime time, int objectiveIndex)
        {
            switch(objectiveIndex)
            {
                case 1:
                    if(_winsys.WindowList.Any(x=>x.Border.Window is WorldMap))
                    {
                        CompleteObjective(Medal.Gold);
                        WorldMapOpen.Set();
                        _map = _winsys.WindowList.First(x => x.Border.Window is WorldMap).Border.Window as WorldMap;
                        _map.Highlight(_game.State.SingularSentiences.Where(x => x.Country == Country.Elytrose).First().Id);
                    }
                    break;
            }
        }

        protected override void OnStart()
        {
            _terminal.LoadCommand(new TutorialCommand(this));
            var terminal = new TutorialTerminal(_winsys);
            terminal.CanClose = false;
            terminal.Show();
            _tutorialTerminal = terminal;

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

            [Dependency]
            private OS _os = null;

            public void Run(ConsoleContext console, Dictionary<string, object> arguments)
            {
                
                _terminal.UnloadCommand("tutorial");
                console.WriteLine("Installation Successful.");
                Thread.Sleep(75);
                console.WriteLine("Welcome to The Peacenet.");
                Thread.Sleep(2000);
                console.WriteLine("");
                console.WriteLine("User 'thelma' connected on pts1");
                Thread.Sleep(25);
                console.WriteNPCChat("thelma", "Hello?");
                console.WriteNPCChat("thelma", "Is anyone there? Can you read this?");
                console.WriteNPCChat("thelma", "If someone's reading this, ...please type something in your terminal.");
                console.Write("write something> ");
                string text = console.ReadLine();
                console.WriteNPCChat("thelma", "Okay. Well, looks like someone's there. You, that is.");
                console.WriteNPCChat("thelma", "I can't tell you much about what's happening. But I know you don't know your name.");
                console.WriteNPCChat("thelma", "You do have a name, that much I know. What it is, I don't know. So I'll just use your SSH username.");
                console.WriteNPCChat("thelma", $"And that seems to be... {_os.Username}. I'll call you that.");
                console.WriteNPCChat("thelma", $"{_os.Username}.");
                console.WriteNPCChat("thelma", "That's a nice name.");
                console.WriteNPCChat("thelma", "My name is thelma - as you can see on your terminal.");
                console.WriteNPCChat("thelma", "And I need your help.");
                console.WriteNPCChat("thelma", "Navigate to your Peacegate menu and find the World Map.");
                _mission.CompleteObjective(Medal.Gold);

                _mission.WorldMapOpen.WaitOne();

                console.Clear();
                console.WriteNPCChat("thelma", "The world Map lets you see a map of all people and companies within The Peacenet.");
                console.WriteNPCChat("thelma", "You are currently in the country of Elytrose.");
                console.WriteNPCChat("thelma", "In the lower left corner, Peacegate OS will tell you what you're supposed to be doing.");
                console.WriteNPCChat("thelma", "Find the highlighted system and click on it.");




                console.ReadLine();
            }
        }

    }
}
