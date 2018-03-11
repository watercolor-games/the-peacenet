using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Plex.Engine.PlexContentManager
{
    public interface ILoader<T>
    {
        /// <summary>
        /// The file extensions that can be loaded with this loader.
        /// </summary>
        IEnumerable<string> Extensions { get; }

        /// <summary>
        /// Load an asset.
        /// </summary>
        /// <returns>The loaded asset.</returns>
        /// <param name="fobj">The stream to load from.</param>
        T Load(Stream fobj);
    }
}
