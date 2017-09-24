using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Server
{
    public static class HackingCommands
    {
        [ServerCommand("lsports", "Attempts to list all available ports on this system.")]
        [ShellConstraint("> ")]
        public static void ListAvailablePorts()
        {
            string _sessionID = Terminal.SessionID;
            var hackable = Hacking.GrabSession(_sessionID);
            if(hackable == null)
            {
                Console.WriteLine("Error: You are not in a hacking session.");
                return;
            }
            var sys = Program.GetSaveFromPrl(hackable.HackableID);
            if (sys.HasFirewall)
            {
                Console.WriteLine("Firewall detected. Attempting to bypass...");
                if (hackable.Puzzles.FirstOrDefault(x => x.Data.Completed == false) != null)
                {
                    Console.WriteLine("Access denied by {0}.", hackable.Puzzles.FirstOrDefault(x => x.Data.Completed == false).Data.ID);
                    return;
                }
                else
                {
                    Console.WriteLine("Access granted.");
                }
            }
            Console.WriteLine("Port #: Name");
            Console.WriteLine("=====================");
            Console.WriteLine();
            foreach (var port in Hacking.GetPorts(sys.SystemType))
            {
                Console.WriteLine($"{port.Value}: {port.FriendlyName}");
            }
        }

        [ServerCommand("solve", "Solve the current firewall puzzle if any.")]
        [RequiresArgument("id")]
        [ShellConstraint("> ")]
        public static void Solve(Dictionary<string, object> args)
        {
            var session = Hacking.GrabSession(Terminal.SessionID);
            if(session == null)
            {
                Console.WriteLine("Error: You are not in a hacking session.");
                return;
            }
            var hackable = Program.GetSaveFromPrl(session.HackableID);
            var puzzle = session.Puzzles.FirstOrDefault(x => x.Data.Completed == false);
            if (puzzle != null)
            {
                string id = args["id"].ToString();
                bool result = puzzle.Evaluate(id);
                if(result == true)
                {
                    Console.WriteLine("Puzzle solved!");
                    Program.SaveWorld();
                }
                else
                {
                    Console.WriteLine("Puzzle not solved.");
                    Console.WriteLine(puzzle.GetHint());
                }
                return;
            }
            Console.WriteLine("No firewall puzzles found on this system! Lucky.");
        }


        [ServerCommand("hint", "Retrieves a hint for the current firewall puzzle, if any.")]
        [ShellConstraint("> ")]
        public static void Hint()
        {
            var hsession = Hacking.GrabSession(Terminal.SessionID);
            if (hsession == null)
            {
                Console.WriteLine("Error: You are not in a hacking session.");
                return;
            }
            IPuzzle puzzle = hsession.Puzzles.FirstOrDefault(x => x.Data.Completed == false);
            if (puzzle == null)
            {
                Console.WriteLine("No puzzles to solve! This system is clear of all firewalls.");
                return;
            }
            Console.WriteLine(puzzle.GetHint());
        }


    }
}
