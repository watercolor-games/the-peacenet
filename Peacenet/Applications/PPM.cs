using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    [AppLauncher("Peacenet Package Manager", "System", "View, manage, and install new programs for Peacegate OS from the Peacenet repository.")]
    public class PPM : Window
    {
        public PPM(WindowSystem winsys) : base(winsys)
        {
            float aspect = (1920f / 1080f);
            float w = aspect * 400;
            Height = 400;
            Width = (int)w;
            Title = "Peacenet Package Manager";
        }
    }
}
