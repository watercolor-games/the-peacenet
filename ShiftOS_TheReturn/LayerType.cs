using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine
{
    /// <summary>
    /// Represents a type of an entity layer.
    /// </summary>
    public enum LayerType
    {
        /// <summary>
        /// A layer where entities are never rendered on screen.
        /// </summary>
        NoDraw = 0,
        /// <summary>
        /// A layer for background entities - such as skies, etc.
        /// </summary>
        Background = 1,
        /// <summary>
        /// The main layer - for things like characters, platforms, etc.
        /// </summary>
        Main = 2,
        /// <summary>
        /// Reserved for simple UI elements such as health bars and other status indicators.
        /// </summary>
        HUD = 3,
        /// <summary>
        /// Reserved for more complex UI elements such as windows, modals, etc.
        /// </summary>
        UserInterface = 4,
        /// <summary>
        /// Reserved for cutscenes.
        /// </summary>
        Foreground = 5
    }
}
