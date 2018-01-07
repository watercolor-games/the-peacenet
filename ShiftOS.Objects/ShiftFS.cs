using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Plex.Objects.ShiftFS
{
    /// <summary>
    /// Represents a parsed path.
    /// </summary>
    public class PathData
    {
        /// <summary>
        /// The drive number that the server uses to find the right FS
        /// </summary>
        public int DriveNumber { get; set; }

        /// <summary>
        /// The path within the drive.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Contextual data for the server. If you are requesting a file write, place the contents here.
        /// </summary>
        public string AdditionalData { get; set; }
    }

    /// <summary>
    /// Contains information about a file or directory in a virtual file system.
    /// </summary>
    public class FileRecord
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public long SizeBytes { get; set; }
        /// <summary>
        /// Gets or sets whether the record is of a directory.
        /// </summary>
        public bool IsDirectory { get; set; }
    }
}
