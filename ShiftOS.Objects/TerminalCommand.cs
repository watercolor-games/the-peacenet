using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class TerminalCommand
    {
        public virtual bool MatchShell(string shell)
        {
            if (ShellMatch != "metacmd")
            {
                return (ShellMatch == shell);
            }
            return true;
        }

        public bool AllowInMP { get; set; }
        public string ShellMatch { get; set; }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (char c in ToString())
            {
                hash += (int)c;
            }
            return hash;
        }

        public Command CommandInfo { get; set; }

        public List<string> RequiredArguments { get; set; }
        public string Dependencies { get; set; }

        public MethodInfo CommandHandler;

        public Type CommandType;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.CommandInfo.name);
            if (this.RequiredArguments.Count > 0)
            {
                sb.Append(" ");
                foreach (var arg in RequiredArguments)
                {
                    sb.Append("--" + arg);
                    sb.Append(" ");
                    if (RequiredArguments.IndexOf(arg) < RequiredArguments.Count - 1)
                        sb.Append(',');
                }
                sb.Append("}");
            }
            sb.Append("|");
            sb.Append(CommandHandler.Name + "()");
            return sb.ToString();
        }

        public bool RequiresElevation { get; set; }

        public virtual void Invoke(Dictionary<string, object> args, string shell)
        {
            List<string> errors = new List<string>();
            if (ShellMatch != "metacmd")
            {
                if (ShellMatch != shell)
                {
                    errors.Add("Command not found.");
                }
            }

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return;
            }
            try
            {
                CommandHandler.Invoke(null, new[] { args });

            }
            catch (System.Reflection.TargetParameterCountException)
            {
                CommandHandler.Invoke(null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
