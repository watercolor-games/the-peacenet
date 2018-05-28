using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.GameState
{
    public class Sentience
    {
        public string Id { get; set; }
        public uint IPAddress { get; set; }
        public string Hostname { get; set; }
        public int SkillLevel { get; set; }
        public int XP { get; set; }
        public float Reputation { get; set; }
        public string FactionID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 MapLocation => new Vector2(X, Y);
        public Country Country { get; set; }
    }

    public class Faction
    {
        public string Id { get; set; }
        public uint IPAddress { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Reputation { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 MapLocation => new Vector2(X, Y);
        public Country Country { get; set; }
    }
}
