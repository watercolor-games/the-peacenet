using System;using System.Collections.Generic;using System.IO;
using System.Linq;using System.Text;using System.Threading.Tasks;using Plex.Objects;

namespace Plex.Engine{
    public static class EngineCommands
    {
        [MetaCommand()]
        [Command("man", description = "Shows detailed usage information about a given command.")]
        [UsageString("<command>")]
        public static void Man(Dictionary<string, object> args, ConsoleContext console)
        {
            string cmd = args["<command>"].ToString();
            var tc = TerminalBackend.Commands.FirstOrDefault(x => x.CommandInfo.name == cmd && Upgrades.IsLoaded(x.Dependencies));


            console.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.Yellow);
            console.SetBold(true);
            if (tc != null)
            {
                console.WriteLine(tc.GetManPage());
            }
            else
            {
                using (var netstr = new ServerStream(ServerMessageType.TRM_MANPAGE))
                {
                    netstr.Write(cmd);
                    var result = netstr.Send();
                    if (result.Message == (byte)ServerResponseType.REQ_SUCCESS)
                    {
                        using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                        {
                            console.WriteLine(reader.ReadString());
                        }
                    }
                    else
                    {
                        console.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.Red);
                        console.SetBold(true);
                        console.Write("Error: ");
                        console.SetBold(false);
                        console.WriteLine("Command not found.");
                    }
                }
            }
        }

        [MetaCommand]
        [Command("help", "", "{DESC_COMMANDS}")]
        public static bool Commands(ConsoleContext console)
        {
            Dictionary<string, string> cmds = new Dictionary<string, string>();
            using (var w = new ServerStream(ServerMessageType.TRM_GETCMDS))
            {
                var result = w.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            cmds.Add(reader.ReadString(), reader.ReadString());
                        }
                    }
                }
            }
            foreach (var n in TerminalBackend.Commands.Where(x => !(x is TerminalBackend.WinOpenCommand) && Upgrades.UpgradeInstalled(x.Dependencies) && x.CommandInfo.hide == false && x.MatchShell(TerminalBackend.RawShellOverride) == true).OrderBy(x => x.CommandInfo.name))
            {
                if(!cmds.ContainsKey(n.CommandInfo.name))
                    cmds.Add(n.CommandInfo.name, n.CommandInfo.description);
            }

                var sb = new StringBuilder();
            sb.AppendLine("{GEN_COMMANDS}");
            sb.AppendLine("=================");
            sb.AppendLine();
            //print all unique namespaces.
            foreach (var cmd in cmds.OrderBy(x=>x.Key))
            {
                sb.Append(" - " + cmd.Key);
                if (!string.IsNullOrWhiteSpace(cmd.Value))
                        sb.Append(": " + cmd.Value);
                sb.AppendLine();
            }

            console.WriteLine(sb.ToString());

            return true;
        }

    }}