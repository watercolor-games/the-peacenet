using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    //Better to store this stuff server-side so we can do some neat stuff with hacking...
    public class Save
    {
        public string Username { get; set; }
        public int Codepoints { get; set; }
        public Dictionary<string, bool> Upgrades { get; set; }
        public int StoryPosition { get; set; }
        public string Language { get; set; }
        public string MyShop { get; set; }
        public List<string> CurrentLegions { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int Revision { get; set; }

        public string Password { get; set; }
        public string SystemName { get; set; }

        public string DiscourseName { get; set; }

        /// <summary>
        /// If the user has entered their Discourse account into ShiftOS, this is the password they gave.
        /// 
        /// ANY developer caught abusing this property will have their dev status revoked and their account PERMANENTLY SUSPENDED. - Michael
        /// </summary>
        public string DiscoursePass { get; set; }


        public int CountUpgrades()
        {
            int count = 0;
            foreach (var upg in Upgrades)
            {
                if (upg.Value == true)
                    count++;
            }
            return count;
        }
    }
}
