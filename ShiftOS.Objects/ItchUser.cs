using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    /// <summary>
    /// Represents a response from the itch.io API.
    /// </summary>
    public class ItchResponse
    {
        /// <summary>
        /// Gets or sets any errors in the response
        /// </summary>
        public string[] errors { get; set; }
        /// <summary>
        /// Gets or sets the user's profile
        /// </summary>
        public ItchUser user { get; set; }
    }


    /// <summary>
    /// Represents an itch.io public profile.
    /// </summary>
    public class ItchUser
    {
        /// <summary>
        /// Gets or sets the ID of the user
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// Gets or sets whether this user plays games.
        /// </summary>
        public bool gamer { get; set; }

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        public string display_name { get; set; }

        /// <summary>
        /// Gets or sets the cover URL of the user.
        /// </summary>
        public string cover_url { get; set; }

        /// <summary>
        /// Gets or sets the profile URL of the user.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets whether this is a press user...? What the fuck is a press user?
        /// </summary>
        public bool press_user { get; set; }

        /// <summary>
        /// Gets or sets whether this user develops games.
        /// </summary>
        public bool developer { get; set; }
    }
}
