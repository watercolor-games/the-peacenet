using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.WinForms
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DesktopWidgetAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public DesktopWidgetAttribute(string n, string desc)
        {
            Name = n;
            Description = desc;
        }

        public override string ToString()
        {
            return this.Name + "_" + Description;
        }
    }
}
