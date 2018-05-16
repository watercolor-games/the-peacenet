using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Peacenet.Missions
{
    public class InstantCompleteMission : Mission
    {
        public InstantCompleteMission() : base("Instant completion test", "Playing this mission will cause an instant Gold completion.")
        {
            AddObjective("Instantly complete the mission");
        }

        protected override void UpdateObjective(GameTime time, int objectiveIndex)
        {
            CompleteObjective(Medal.Gold);
        }
    }
}
