using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public class Port
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public SystemType AttachTo { get; set; }
        public int Value { get; set; }

        public string ID
        {
            get
            {
                return Name.ToLower().Replace(" ", "_");
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
