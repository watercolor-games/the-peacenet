using System;
using System.Collections.Generic;

namespace Plex.Objects.FFI
{
    /// <summary>
    /// A Foreign Function Interface for the Plex engine.
    /// Implementing IFFI will cause your type to be instantiated by
    /// ReflectMan on startup and used to scan for more types.
    /// </summary>
    public interface IFFI
    {
        /// <summary>
        /// Scans for types in files handled by your FFI.
        /// </summary>
        /// <returns>An array of discovered types.</returns>
        IEnumerable<Type> GetTypes();
    }
}
