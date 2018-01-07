namespace Plex.Objects.PlexFS
{
    /// <summary>
    /// Represents a value indicating how a PlexFAT file should be opened.
    /// </summary>
    public enum OpenMode
    {
        /// <summary>
        /// The file should be opened and an error will be thrown if the file doesn't exist.
        /// </summary>
        Open,
        /// <summary>
        /// The file should be opened and created if it doesn't exist yet.
        /// </summary>
        OpenOrCreate
    }
}

