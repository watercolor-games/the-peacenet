using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine {
    public class Command : Attribute {
        public string name;
        public string description = "";
        public string usage = "";
        public bool hide = false;

        public Command(string name) {
            this.name = name;
        }
        public Command(string name, bool hide) {
            this.name = name;
            this.hide = hide;
        }
        public Command(string name, string usage, string description) {
            this.name = name;
            this.description = description;
            this.usage = usage;
        }
    }

    public class RequiresUpgradeAttribute : Attribute {
        public string Upgrade { get; set; }
        public bool Installed {
            get {
                if (Upgrade.Contains(";")) {
                    string[] split = Upgrade.Split(';');
                    foreach (var upg in split) {
                        if (!Shiftorium.UpgradeInstalled(upg))
                            return false;
                    }
                    return true;
                } else {
                    return Shiftorium.UpgradeInstalled(Upgrade);
                }
            }
        }

        /// <summary>
        /// Marks this Form or Command as dependant on this upgrade.
        /// </summary>
        /// <param name="upg">Upgrade ID - See 'shiftorium.json' in resources for all IDs and their metadata.</param>
        public RequiresUpgradeAttribute(string upg) {
            Upgrade = upg;
        }
    }

    public class Namespace : Attribute {
        public string name;
        public bool hide;
        public Namespace(string n) {
            name = n;
        }
        public Namespace(string n, bool hide) {
            name = n;
            this.hide = hide;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandObsolete : Attribute {
        public string reason;
        public string newcommand;
        public bool warn;

        public CommandObsolete(string reason, string newcommand, bool warn) {
            this.reason = reason; // %n for newcommand
            this.newcommand = newcommand;
            this.warn = warn;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequiresArgument : Attribute {
        public string argument;

        public RequiresArgument(string argument) {
            this.argument = argument;
        }

        public override object TypeId {
            get {
                return this;
            }
        }
    }
}
