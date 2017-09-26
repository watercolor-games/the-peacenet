using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;namespace Plex.Objects{    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]    public class PortAttribute : Attribute    {        public PortAttribute(string id, string name, int value, SystemType attach)
        {
            Name = id;
            FriendlyName = name;
            Value = value;
            AttachTo = attach;
        }        public string Name { get; private set; }        public string FriendlyName { get; private set; }        public SystemType AttachTo { get; private set; }        public int Value { get; private set; }        public string ID        {            get            {                return Name.ToLower().Replace(" ", "_");            }        }        public override string ToString()        {            return Name;        }    }}