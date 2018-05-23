using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Objects;

namespace Peacenet.TerminalCommands
{
    public class UpgradeManager : ITerminalCommand
    {
        [Dependency]
        private GameManager _game = null;

        public string Name => "upgrade";

        public string Description => "View, enable and disable System Upgrades.";

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "list [-e --enabled] [-u --unsatisfied]";
                yield return "enable <id>";
                yield return "disable <id>";
                yield return "info <id>";
                yield return "slots";

            }
        }

        private int getInstalledCount()
        {
            return _game.State.UpgradeIDs.Where(x => _game.State.IsUpgradeInstalled(x)).Count();
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            bool showAvailable = (bool)arguments["list"];
            bool doEnable = (bool)arguments["enable"];
            bool doDisable = (bool)arguments["disable"];
            bool showSlots = (bool)arguments["slots"];
            string requestedID = arguments["<id>"]?.ToString();

            if(showAvailable)
            {
                bool showEnabled = (bool)arguments["-e"] || (bool)arguments["--enabled"];
                bool showUnsatisfied = (bool)arguments["-u"] || (bool)arguments["--unsatisfied"];

                foreach (var id in _game.State.UpgradeIDs)
                {
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                    console.SetBold(false);
                    var info = _game.State.GetUpgradeInfo(id);
                    bool e = _game.State.IsUpgradeInstalled(id);
                    if(e && showEnabled)
                    {
                        console.WriteLine($" - {id} [enabled]");
                    }
                    else if(!e)
                    {
                        bool s = _game.State.SkillLevel >= info.MinSkillLevel;
                        if(s)
                        {
                            console.WriteLine($" - {id}");
                        }
                        else
                        {
                            if(showUnsatisfied)
                            {
                                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                                console.SetBold(false);
                                console.Write($" - {id} [requires skill level ");
                                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Red);
                                console.SetBold(true);
                                console.Write(info.MinSkillLevel.ToString());
                                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                                console.SetBold(false);
                                console.WriteLine("]");
                            }
                        }
                    }
                }
            }
            else if(doEnable)
            {
                if(!_game.State.UpgradeIDs.Contains(requestedID))
                {
                    console.WriteLine($"error: {requestedID}: upgrade not found.");
                    return;
                }
                var info = _game.State.GetUpgradeInfo(requestedID);
                if(info.MinSkillLevel > _game.State.SkillLevel)
                {
                    console.WriteLine($"error: {requestedID}: skill level not met. (you: {_game.State.SkillLevel}, upg: {info.MinSkillLevel})");
                    return;
                }

                if(info.Dependencies != null && info.Dependencies.Any(x=>!_game.State.IsUpgradeInstalled(x)))
                {
                    console.WriteLine($"error: {requestedID}: Missing dependencies (see 'upgrade info {requestedID}' for dependency list)");
                    return;
                }

                if(getInstalledCount()+1 > _game.State.UpgradeSlotCount)
                {
                    console.WriteLine($"error: {requestedID}: Not enough slots! (see 'upgrade slots' for info.)");
                    return;
                }
                
                if(!_game.State.EnableUpgrade(requestedID))
                {
                    console.WriteLine($"error: {requestedID}: unspecified error enabling upgrade.");
                    return;
                }

                console.WriteLine("Upgrade enabled!");
            }
            else if(doDisable)
            {

            }
            else if(showSlots)
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                console.SetBold(false);
                console.Write("Skill level: ");
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Orange);
                console.SetBold(true);
                console.WriteLine($"{_game.State.SkillLevel}");
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                console.SetBold(false);
                console.WriteLine("Max slots: " + _game.State.UpgradeSlotCount);
                console.WriteLine("Used: " + getInstalledCount());

            }
        }
    }
}
