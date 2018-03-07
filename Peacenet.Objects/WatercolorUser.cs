using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    /// <summary>
    /// A class representing a Watercolor Games user profile.
    /// </summary>
    public class WatercolorUser
    {
        /// <summary>
        /// The email address of the account.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// The username of the account.
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// The full name of the account.
        /// </summary>
        public string fullname { get; set; }
        /// <summary>
        /// Markdown-formatted "About Me" section for the account.
        /// </summary>
        public string about { get; set; }
    }
}
