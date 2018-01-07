namespace Plex.Objects.PlexFS
{
    /// <summary>
    /// Represents a type of a PlexFAT entry.
    /// </summary>
	public enum EntryType
	{
        /// <summary>
        /// The entry is invalid and non-existent.
        /// </summary>
		NONEXISTENT,
        /// <summary>
        /// The entry is a file.
        /// </summary>
		FILE,
        /// <summary>
        /// The entry is a directory.
        /// </summary>
		DIRECTORY
	}
}

