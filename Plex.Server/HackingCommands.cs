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
