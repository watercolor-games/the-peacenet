using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Server
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PuzzleAttribute : Attribute
    {
        public PuzzleAttribute(string id, int rank)
        {
            ID = id;
            Rank = rank;
        }

        public int Rank { get; private set; }
        public string ID { get; private set; }
    }

    public interface IPuzzle
    {
        Objects.Hacking.Puzzle Data { get; set; }
        string GetHint();
        bool Evaluate(string value);
    }

    public class HackSession
    {
        public string SessionID { get; set; }
        public string HackableID { get; set; }
        public List<IPuzzle> Puzzles { get; set; }
    }
}
