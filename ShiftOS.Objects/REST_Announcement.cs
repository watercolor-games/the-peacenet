using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class Announcement
    {
        /// <summary>
        /// Gets or sets the Drupal Node ID of this announcement.
        /// </summary>
        public ID[] nid { get; set; }

        /// <summary>
        /// Gets or sets the unique id of this node.
        /// </summary>
        public DrupalString[] uuid { get; set; }

        /// <summary>
        /// Gets or sets the view ID of this node.
        /// </summary>
        public ID[] vid { get; set; }

        /// <summary>
        /// Gets or sets the language of this node.
        /// </summary>
        public DrupalString[] langcode { get; set; }

        /// <summary>
        /// Gets or sets the content type of this content.
        /// </summary>
        public DrupalContentType[] type { get; set; }

        /// <summary>
        /// Gets or sets the title of this content.
        /// </summary>
        public DrupalString[] title { get; set; }

        public DrupalBody[] body { get; set; }
    }

    public class DrupalContentType
    {
        public string target_type { get; set; }
        public string target_id { get; set; }
        public string target_uuid { get; set; }
    }

    public class ID
    {
        public int value { get; set; }
    }
    
    public class DrupalString
    {
        public string value { get; set; }
    }

    public class DrupalBody
    {
        public string value { get; set; }
        public string format { get; set; }
        public string summary { get; set; }
    }
}
