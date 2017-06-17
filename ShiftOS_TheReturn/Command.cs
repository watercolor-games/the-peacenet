/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Denotes that the following terminal command or namespace must only be used in an elevated environment.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class KernelModeAttribute : Attribute
    {

    }

    /// <summary>
    /// Denotes that the following public static method is a ShiftOS Terminal command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Command : Attribute
    {
        /// <summary>
        /// The command name.
        /// </summary>
        public string name;
        /// <summary>
        /// The description of the command.
        /// </summary>
        public string description = "";
        /// <summary>
        /// The usage flags for the command.
        /// </summary>
        public string usage = "";
        /// <summary>
        /// Should the command be hidden from the help system?
        /// </summary>
        public bool hide = false;

        /// <summary>
        /// Creates a new instance of the <see cref="Command"/>. 
        /// </summary>
        /// <param name="name">The name of the command.</param>
        public Command(string name)
        {
            this.name = name;
        }


        /// <summary>
        /// Creates a new instance of the <see cref="Command"/>. 
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="hide">Whether the command should be hidden from the help system.</param>
        public Command(string name, bool hide)
        {
            this.name = name;
            this.hide = hide;
        }
        public Command(string name, string usage, string description)
        {
            this.name = name;
            this.description = description;
            this.usage = usage;
        }
    }

    /// <summary>
    /// Denotes that this function or property is dependent on a Shiftorium upgrade.
    /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiresUpgradeAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the upgrade(s) this attribute requires.
        /// </summary>
        public string Upgrade { get; set; }

        /// <summary>
        /// Gets whether the dependent upgrade(s) are installed.
        /// </summary>
        public virtual bool Installed
        {
            get
            {
                if (Upgrade.Contains(";"))
                {
                    string[] split = Upgrade.Split(';');
                    foreach (var upg in split)
                    {
                        if (!Shiftorium.UpgradeInstalled(upg))
                            return false;
                    }
                    return true;
                }
                else
                {
                    return Shiftorium.UpgradeInstalled(Upgrade);
                }
            }
        }

        /// <summary>
        /// Marks this Form or Command as dependant on this upgrade.
        /// </summary>
        /// <param name="upg">Upgrade ID - See 'shiftorium.json' in resources for all IDs and their metadata.</param>
        public RequiresUpgradeAttribute(string upg)
        {
            Upgrade = upg;
        }
    }

    /// <summary>
    /// Marks a Terminal command as obsolete.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandObsolete : Attribute
    {
        /// <summary>
        /// The reason of obsolescence.
        /// </summary>
        public string reason;

        /// <summary>
        /// If a new command has the same functionality, this is it.
        /// </summary>
        public string newcommand;

        /// <summary>
        /// Should we warn the user when they run this command?
        /// </summary>
        public bool warn;


        /// <summary>
        /// Creates a new instance of the <see cref="CommandObsolete"/> class. 
        /// </summary>
        /// <param name="reason">The reason for this command's obsolescence. You can use "%n" in place of the new command name.</param>
        /// <param name="newcommand">If a new command is set to take over, specify it here.</param>
        /// <param name="warn">Whether or not we should warn the user when they run the command.</param>
        public CommandObsolete(string reason, string newcommand, bool warn)
        {
            this.reason = reason; // %n for newcommand
            this.newcommand = newcommand;
            this.warn = warn;
        }
    }

    /// <summary>
    /// Denotes that this command requires a specified argument to be in its argument dictionary.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequiresArgument : Attribute
    {
        /// <summary>
        /// The argument name
        /// </summary>
        public string argument;

        /// <summary>
        /// Creates a new instance of the <see cref="RequiresArgument"/> attribute 
        /// </summary>
        /// <param name="argument">The argument name associated with this attribute</param>
        public RequiresArgument(string argument)
        {
            this.argument = argument;
        }

        public override object TypeId
        {
            get
            {
                return this;
            }
        }
    }

    /// <summary>
    /// Prevents a command from being run in a remote session.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RemoteLockAttribute : Attribute
    {
    }
}
