using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using static Plex.Engine.SaveSystem;
using Plex.Objects.ShiftFS;
using System.Reflection;
using Plex.Engine.Scripting;
using Plex.Objects;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine
{
    //this'll be used even after skinning is redone. I may move it however. -Alkaline
    public enum ArrowDirection
    {
        Top,
        Left,
        Bottom,
        Right
    }
}
