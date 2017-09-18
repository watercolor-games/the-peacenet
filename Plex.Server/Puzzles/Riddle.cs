using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects.Hacking;

namespace Plex.Server.Puzzles
{
    [Puzzle("riddle", 2)]
    public class Riddle : IPuzzle
    {
        public Riddle()
        {
            var rnd = new Random();
            value = values[rnd.Next(0, values.Count)];
        }

        private Puzzle _data = null;

        private List<string> values = new List<string>
        {
            "rain"

        };

        private Dictionary<string, string> hints = new Dictionary<string, string>
        {
            { "rain", "You can take a shower in me, but I won't make you clean." }
        };

        private string value = "";

        public Puzzle Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        
        public bool Evaluate(string _value)
        {
            return (_value).ToLower() == value;
        }

        public string GetHint()
        {
            return hints[value];
        }
    }
}
