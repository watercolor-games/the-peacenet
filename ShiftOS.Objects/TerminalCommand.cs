using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DocoptNet;

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

        public TerminalCommand()
        {
            UsageStrings = new List<UsageStringAttribute>();
        }

        public List<UsageStringAttribute> UsageStrings { get; set; }
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

        public string GetManPage()
        {
            StringBuilder _usageBuilder = new StringBuilder();
            _usageBuilder.AppendLine(CommandInfo.name + ": " + CommandInfo.description);
            _usageBuilder.AppendLine();
            _usageBuilder.AppendLine("    Usage:");
            if (UsageStrings.Count == 0)
                _usageBuilder.Append("      " + CommandInfo.name);
            foreach (var ustr in UsageStrings)
            {
                _usageBuilder.AppendLine($"      {CommandInfo.name} {ustr.Usage}");
            }
            _usageBuilder.AppendLine();
            return _usageBuilder.ToString();
        }

        public virtual void Invoke(string[] args, string shell)
        {
            List<string> errors = new List<string>();
            if (ShellMatch != "metacmd")
            {
                if (ShellMatch != shell)
                {
                    errors.Add("Command not found.");
                }
            }

            Dictionary<string, object> argss = new Dictionary<string, object>();

            if(UsageStrings.Count > 0)
            {
                StringBuilder _usageBuilder = new StringBuilder();
                _usageBuilder.AppendLine(CommandInfo.name + ": " + CommandInfo.description);
                _usageBuilder.AppendLine();
                _usageBuilder.AppendLine("    Usage:");
                foreach(var ustr in UsageStrings)
                {
                    _usageBuilder.AppendLine($"      {CommandInfo.name} {ustr.Usage}");
                }
                _usageBuilder.AppendLine();

                try
                {
                    var docopt = new Docopt();
                    var argsv = docopt.Apply(_usageBuilder.ToString(), args, version: CommandInfo.name, exit: false);
                    foreach(var arg in argsv)
                    {
                        if (arg.Value != null)
                            argss.Add(arg.Key, arg.Value.Value);
                        else
                            argss.Add(arg.Key, null);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{CommandInfo.name}: invalid syntax (Please see the manpage for this command.)");
                    errors.Add(ex.Message);
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
                CommandHandler.Invoke(null, new[] { argss });

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
