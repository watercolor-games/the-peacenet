using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public class WinOpenAttribute : Attribute
    {
        public string ID { get; private set; }

        public WinOpenAttribute (string id)
        {
            ID = id;
        }
    }
}
