using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MissionAttribute : StoryAttribute
    {
        public MissionAttribute(string id, string friendlyName, string friendlyDesc, ulong codepointAward, string assigner) : base(id)
        {
            Name = friendlyName;
            Description = friendlyDesc;
            CodepointAward = codepointAward;
            Assigner = assigner;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Assigner { get; set; }
    }
}
