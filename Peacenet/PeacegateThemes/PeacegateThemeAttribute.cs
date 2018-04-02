using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.PeacegateThemes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PeacegateThemeAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public string PreviewTexture { get; private set; }

        public PeacegateThemeAttribute(string name, string desc, string previewTexture)
        {
            Name = name;
            Description = desc;
            PreviewTexture = previewTexture;
        }
    }
}
