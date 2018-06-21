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
        private ManualResetEvent _sentienceSelected = new ManualResetEvent(false);

        public EventWaitHandle WorldMapOpen => _worldMapOpen;
        public EventWaitHandle SentienceSelected => _sentienceSelected;
        public readonly EventWaitHandle Connected = new ManualResetEvent(false);


        private WorldMap _map = null;

        public WelcomeToThePeacenet() : base("Welcome to The Peacenet", "Welcome to The Peacenet. You've just finished installing Peacegate OS. Now it's time to find out why.")
        {
            AddObjective("Pay attention to the Terminal");
            AddObjective("Find and open World Map");
            AddObjective("Select the highlighted system");
            AddObjective("Connect to darwin0110-filestorage");
            AddObjective("Find darwin0110's /bin directory", 180, TimeoutType.Fail);
        }

        protected override void UpdateObjective(GameTime time, int objectiveIndex)
        {
            if(_tutorialTerminal != null)
            {
                _winsys.BringToFront(_tutorialTerminal);
            }
            switch(objectiveIndex)
            {
                case 1:
                    if(_winsys.WindowList.Any(x=>x.Border.Window is WorldMap))
                    {
                        CompleteObjective(Medal.Gold);
                        WorldMapOpen.Set();
                        _map = _winsys.WindowList.First(x => x.Border.Window is WorldMap).Border.Window as WorldMap;
                        _map.CanClose = false;
                        _game.State.Highlight("darwin0110-filestorage");
                    }
                    break;
                case 2:
                    if (_map.Selected != null)
                    {
                        if (_map.Selected.Id == _game.State.Highlighted.Id)
                        {
                            _sentienceSelected.Set();
                            CompleteObjective(Medal.Gold);
                        }
                    }
                    break;
                case 3:
                    if(_game.State.ActiveConnections.Any(x=>x.Authenticated && x.Sentience.Hostname == "darwin0110-filestorage"))
                    {
                        CompleteObjective(Medal.Gold);
                        Connected.Set();
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
                console.WriteNPCChat("thelma", "I have a friend in The Peacenet who's got a storage repository in Elytrose.");
                console.WriteNPCChat("thelma", "He has some useful programs that you'll need to install on your Peacegate.");
                console.WriteNPCChat("thelma", "Follow the marker on the World Map to find his repository. His hostname is 'darwin0110-filestorage'.");

                _mission.SentienceSelected.WaitOne();

                console.Clear();
                console.WriteNPCChat("thelma", "Connect to the repo by clicking the 'Connect' button.");
                console.WriteNPCChat("thelma", "Use my credentials to log in - username is 'thelma', password is 'TW6Or1bg'.");

                _mission.Connected.WaitOne();

                console.Clear();
                console.WriteNPCChat("thelma", "You've connected to his system.");
                console.WriteNPCChat("thelma", "Just a warning though. It is illegal to do things like this in The Peacenet, even with consent.");
                console.WriteNPCChat("thelma", "Peacegate OS and the Peacenet are supposed to be locked down super tightly. You shouldn't even be aware of Peacegate's existence.");
                console.WriteNPCChat("thelma", "But... that's a whole other can of worms I'd need to explain.");
                console.WriteNPCChat("thelma", "darwin0110 and I will explain everything you need to know. But you need these programs first.");
                console.WriteNPCChat("thelma", "Go ahead and open your File Manager and browse to his /bin directory.");
                console.WriteNPCChat("thelma", "I'll keep the feds off your back while you do this, but I won't be able to keep them off for a long time.");


                console.ReadLine();
            }
        }

    }
}
