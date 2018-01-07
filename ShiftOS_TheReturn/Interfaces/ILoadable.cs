using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.Interfaces
{
    /// <summary>
    /// Provides an API for allowing an entity or engine component to load resources from a MonoGame content pipeline.
    /// </summary>
    public interface ILoadable
    {
        /// <summary>
        /// Load any resources from the content pipeline.
        /// </summary>
        /// <param name="content">The <see cref="ContentManager"/> for loading resources.</param>
        void Load(ContentManager content);
    }
}
