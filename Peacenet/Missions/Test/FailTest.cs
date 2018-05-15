using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Peacenet.Missions.Test
{
    public class FailTest : Mission
    {
        public FailTest() : base("Immediate fail test", "Starting this mission will cause an immediate Mission Fail screen.")
        {
            AddObjective("Fail me!", 0.0001);
        }

        protected override void UpdateObjective(GameTime time, int objectiveIndex)
        {
        }
    }
}
