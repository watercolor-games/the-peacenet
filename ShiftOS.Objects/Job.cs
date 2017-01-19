using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects

{
    public class Job
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<JobTask> Tasks { get; set; }
        public string Author { get; set; }
    }

    public abstract class JobTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Reward { get; set; }

        public abstract bool IsComplete { get; }
    }
}
