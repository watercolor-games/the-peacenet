using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public interface IVirus
    {
        void Infect(int threatlevel);
        void Disinfect();
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VirusAttribute : Attribute
    {
        public VirusAttribute(string id, string name, string desc)
        {
            Name = name;
            ID = id;
            Description = desc;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ID { get; set; }
    }
}
