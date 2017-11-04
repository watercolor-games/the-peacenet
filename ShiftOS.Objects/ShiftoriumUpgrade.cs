using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{







    /// <summary>    /// Denotes that the following public static method is a Plex Terminal command.    /// </summary>    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]    public class Command : Attribute    {






        /// <summary>        /// The command name.        /// </summary>        public string name;






        /// <summary>        /// The description of the command.        /// </summary>        public string description = "";






        /// <summary>        /// The usage flags for the command.        /// </summary>        public string usage = "";






        /// <summary>        /// Should the command be hidden from the help system?        /// </summary>        public bool hide = false;









        /// <summary>        /// Creates a new instance of the <see cref="Command"/>.         /// </summary>        /// <param name="name">The name of the command.</param>        public Command(string name)        {            this.name = name;        }












        /// <summary>        /// Creates a new instance of the <see cref="Command"/>.         /// </summary>        /// <param name="name">The name of the command.</param>        /// <param name="hide">Whether the command should be hidden from the help system.</param>        public Command(string name, bool hide)        {            this.name = name;            this.hide = hide;        }        public Command(string name, string usage, string description)        {            this.name = name;            this.description = description;            this.usage = usage;        }    }








    /// <summary>    /// Marks a Terminal command as obsolete.    /// </summary>    [AttributeUsage(AttributeTargets.Method)]    public class CommandObsolete : Attribute    {






        /// <summary>        /// The reason of obsolescence.        /// </summary>        public string reason;







        /// <summary>        /// If a new command has the same functionality, this is it.        /// </summary>        public string newcommand;







        /// <summary>        /// Should we warn the user when they run this command?        /// </summary>        public bool warn;














        /// <summary>        /// Creates a new instance of the <see cref="CommandObsolete"/> class.         /// </summary>        /// <param name="reason">The reason for this command's obsolescence. You can use "%n" in place of the new command name.</param>        /// <param name="newcommand">If a new command is set to take over, specify it here.</param>        /// <param name="warn">Whether or not we should warn the user when they run the command.</param>        public CommandObsolete(string reason, string newcommand, bool warn)        {            this.reason = reason; // %n for newcommand
            this.newcommand = newcommand;            this.warn = warn;        }    }

    public class ShiftoriumUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ulong Cost { get; set; }
        public string ID { get { return (Name.ToLower().Replace(" ", "_")); } }
        public string Category { get; set; }
        public bool Purchasable { get; set; }
        public string Tutorial { get; set; }
        public string Dependencies { get; set; }
        public int Rank { get; set; }
    }
}
