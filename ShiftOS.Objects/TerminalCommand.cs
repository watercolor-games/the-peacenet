using System;
using System.Collections.Generic;
using System.IO;
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

        public virtual void Invoke(string[] args, string shell, StreamWriter stdout, StreamReader stdin)
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
                    stdout.WriteLine(error);
                }
                return;
            }
            try
            {
                CommandHandler.Invoke(null, new object[] { argss, new ConsoleContext(stdout, stdin) });

            }
            catch (System.Reflection.TargetParameterCountException)
            {
                CommandHandler.Invoke(null, new object[] { new ConsoleContext(stdout, stdin) });
            }
            catch (Exception ex)
            {
                stdout.WriteLine(ex.Message);
            }
        }
    }

    public class ConsoleContext
    {
        private StreamReader _stdin = null;
        private StreamWriter _stdout = null;
        private string workdir = null;

        public StreamReader StandardInput
        {
            get
            {
                return _stdin;
            }
        }

        public StreamWriter StandardOutput
        {
            get
            {
                return _stdout;
            }
        }

        public ConsoleContext Pipe(Stream master)
        {
            var writer = new StreamWriter(master);
            writer.AutoFlush = true;
            var reader = new StreamReader(_stdout.BaseStream);
            return new ConsoleContext(writer, reader);
        }

        public string ReadToEnd()
        {
            return _stdin.ReadToEnd();
        }

        public string WorkingDirectory
        {
            get
            {
                return workdir;
            }
            set
            {
                workdir = value;
            }
        }


        public ConsoleContext(StreamWriter stdout, StreamReader stdin)
        {
            if (stdout == null || stdin == null)
                throw new ArgumentNullException();
            _stdout = stdout;
            _stdin = stdin;
        }

        public void WriteLine(string format, params object[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                format = format.Replace($"{{{i}}}", data[i].ToString());
            }
            WriteLine(format);
        }

        public void SetColors(ConsoleColor background, ConsoleColor foreground)
        {
            _stdout.Write((char)0x1B);
            int b = (int)background;
            int f = (int)foreground;
            _stdout.Write($"{b}{f}");
            _stdout.Write((char)0x1B);
        }

        public void SetItalic(bool value)
        {
            _stdout.Write((char)0x1B);
            if (value == true)
                _stdout.Write("i");
            else
                _stdout.Write("!i");
            _stdout.Write((char)0x1B);
        }

        public void SetBold(bool value)
        {
            _stdout.Write((char)0x1B);
            if (value == true)
                _stdout.Write("b");
            else
                _stdout.Write("!b");
            _stdout.Write((char)0x1B);
        }

        public void Write(string text)
        {
            _stdout.Write(text);
        }

        public void WriteLine(string text)
        {
            _stdout.WriteLine(text);
        }

        public string ReadLine()
        {
            return _stdin.ReadLine();
        }

        public char Read()
        {
            return (char)_stdin.Read();
        }

        public void Clear()
        {
            _stdout.Write((char)0x1B);
            _stdout.Write("c");
            _stdout.Write((char)0x1B);
        }
    }

    public enum ConsoleColor
    {
        Black,
        White,
        Gray,
        Red,
        Green,
        Blue,
        Yellow,
        Orange,
        Purple,
        Pink
    }
}
