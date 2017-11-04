#define DEVEL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plex.Engine.Properties;
using System.IO;
using Newtonsoft.Json;
using System.IO.Compression;

using Plex.Objects;
using Plex.Engine.Scripting;
using Plex.Objects.ShiftFS;
using Plex.Engine;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using static Plex.Engine.TerminalBackend;

namespace Plex.Frontend
{
    public static class TerminalCommands
    {

        [MetaCommand]
        [Command("clear", description = "{DESC_CLEAR}")]
        public static bool Clear(ConsoleContext console)
        {
            console.Clear();

            return true;
        }
    }

    public static class PlexCommands
    {
        [Command("shutdown", description = "{DESC_SHUTDOWN}")]
        public static void Shutdown(ConsoleContext console)
        {
            AudioPlayerSubsystem.Shutdown();
            ServerManager.Disconnect(DisconnectType.UserRequested);
        }
    }

    public static class WindowCommands
    {
        [Command("processes", description = "{DESC_PROCESSES}")]
        public static bool List()
        {
            Console.WriteLine("{GEN_CURRENTPROCESSES}");
            foreach (var app in AppearanceManager.OpenForms)
            {
                //Windows are displayed the order in which they were opened.
                Console.WriteLine($"{AppearanceManager.OpenForms.IndexOf(app)}\t{app.Text}");
            }
            return true;
        }

        [Command("programs", description = "{DESC_PROGRAMS}")]
        public static bool Programs()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{GEN_PROGRAMS}");
            sb.AppendLine("===============");
            sb.AppendLine();
            //print all unique namespaces.
            foreach(var n in TerminalBackend.Commands.Where(x => x is TerminalBackend.WinOpenCommand && Upgrades.UpgradeInstalled(x.Dependencies)).OrderBy(x => x.CommandInfo.name))
            {
                sb.Append(" - " + n.CommandInfo.name);
                if (!string.IsNullOrWhiteSpace(n.CommandInfo.description))
                    if (Upgrades.UpgradeInstalled("help_description"))
                        sb.Append(" - " + n.CommandInfo.description);
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());

            return true;
        }

        [Command("close", description ="{DESC_CLOSE}")]
        [UsageString("<pid>")]
        public static void CloseWindow(Dictionary<string, object> args)
        {
            int winNum = -1;

            if(!int.TryParse(args["<pid>"].ToString(), out winNum))
            {
                Console.WriteLine("Fatal error: process id must be a 32-bit integer");
                return;
            }

            string err = null;

            if (winNum < 0 || winNum >= AppearanceManager.OpenForms.Count)
                err = Localization.Parse("{ERR_BADWINID}", new Dictionary<string, string>
                {
                    ["%max"] = (AppearanceManager.OpenForms.Count - 1).ToString()
                });

            if (string.IsNullOrEmpty(err))
            {
                Console.WriteLine("{RES_WINDOWCLOSED}");
                AppearanceManager.Close(AppearanceManager.OpenForms[winNum].ParentWindow);
            }
            else
            {
                Console.WriteLine(err);
            }
        }

        
    }
}
