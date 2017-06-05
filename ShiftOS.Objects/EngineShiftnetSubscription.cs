using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public class EngineShiftnetSubscription
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public uint CostPerMonth { get; set; }
        public int DownloadSpeed { get; set; }
        public string Company { get; set; }
    }
}
