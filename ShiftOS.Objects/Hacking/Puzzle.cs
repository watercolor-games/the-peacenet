using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whoa;

namespace Plex.Objects.Hacking
{
    public class Puzzle
    {
        [Order]
        public string ID { get; set; }

        [Order]
        public int Rank { get; set; }

        [Order]
        public bool Completed { get; set; }
    }


}
