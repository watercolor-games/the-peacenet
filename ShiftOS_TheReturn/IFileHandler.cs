using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public interface IFileHandler
    {
        void OpenFile(string file);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FileHandlerAttribute : Attribute
    {
        public FileHandlerAttribute(string name, string extension, string iconid)
        {
            Name = name;
            Extension = extension;
            IconID = iconid;
        }

        public string Name { get; set; }
        public string Extension { get; set; }
        public string IconID { get; set; }
    }
}


