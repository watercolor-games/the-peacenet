using System;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine.TextRenderers
{
    /// <summary>
    /// Describes a way in which text is wrapped as it is rendered or measured.
    /// </summary>
    public enum WrapMode
    {
        /// <summary>
        /// Text is drawn as-is, with no maximum wrapwidth whatsoever.
        /// </summary>
        None = 0,
        /// <summary>
        /// Text is drawn using a wrapwidth to drop text to a new line when its width exceeds that of the wrapwidth. Ignored on GDI.
        /// </summary>
        Letters = 1,

        /// <summary>
        /// Text is drawn using a wrapwidth to drop text to a new line when its width exceeds that of the wrapwidth, preserving whole words.
        /// </summary>
        Words = 2
    }
}

