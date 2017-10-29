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
using Plex.Frontend.GraphicsSubsystem;
using static Plex.Engine.TerminalBackend;

namespace Plex.Frontend
{
    public static class TerminalCommands
    {

        [MetaCommand]
        [Command("clear", description = "{DESC_CLEAR}")]
        public static bool Clear()
        {
            Engine.Desktop.InvokeOnWorkerThread(() =>
            {
                AppearanceManager.ConsoleOut.Clear();
            });
            return true;
        }
    }

    public static class PlexCommands
    {
        [Command("textmode")]
        public static void TextMode()
        {
            UIManagerTools.EnterTextMode();
            Console.WriteLine("Text mode initiated.");
        }

        [RemoteLock]
        [Command("shutdown", description = "{DESC_SHUTDOWN}")]
        public static bool Shutdown()
        {
            AudioPlayerSubsystem.Shutdown();
            UIManagerTools.EnterTextMode();
            TerminalBackend.InStory = true;
            TerminalBackend.PrefixEnabled = false;
            new System.Threading.Thread(() =>
            {
                Console.WriteLine("Plexgate is shutting down...");
                Thread.Sleep(5000);
                Console.WriteLine("If you can read this you're not human. Goodbye.");
                UIManagerTools.LeaveTextMode();
                ServerManager.Disconnect(DisconnectType.UserRequested);
            }).Start();
            return true;
        }
    }

    public static class ShiftoriumCommands
    {
        [Command("buy", description = "{DESC_BUY}")]
        [UsageString("<id>")]
        public static void BuyUpgrade(Dictionary<string, object> args)
        {
            string upgrade = args["<id>"].ToString();
            if(string.IsNullOrWhiteSpace(upgrade))
            {
                Console.WriteLine("No upgrade provided.");
                return;
            }
            try
            {
            
                var upg = Upgrades.GetAvailable().FirstOrDefault(x => x.ID == upgrade);
                if(upg != null)
                {
                    if (!Upgrades.Buy(upg.ID, upg.Cost) == true)
                        Console.WriteLine("You don't have enough cash to buy this upgrade.");
                }
                else
                {
                    Console.WriteLine("Upgrade ID \"{0}\" not found.", upgrade);
                }

            }
            catch
            {
                Console.WriteLine("{ERR_GENERAL}");
            }
        }

        [Command("upgradeinfo", description ="{DESC_UPGRADEINFO}")]
        [UsageString("<id>")]
        public static bool ViewInfo(Dictionary<string, object> userArgs)
        {
            try
            {
                string upgrade = "";

                upgrade = (string)userArgs["<id>"];

                foreach (var upg in Upgrades.GetDefaults())
                {
                    if (upg.ID == upgrade)
                    {
                        Console.WriteLine(Localization.Parse("{COM_UPGRADEINFO}", new Dictionary<string, string>
                        {
                            ["%id"] = upg.ID,
                            ["%category"] = upg.Category,
                            ["%name"] = upg.Name,
                            ["%cost"] = upg.Cost.ToString(),
                            ["%description"] = upg.Description
                        }));

                        return true;
                    }
                }

                throw new Exception("{ERR_NOUPGRADE}");
            }
            catch
            {
                return false;
            }
        }

        [Command("upgradecategories", description = "{DESC_UPGRADECATEGORIES}")]
        public static bool ListCategories()
        {
            foreach(var cat in Upgrades.GetCategories())
            {
                Console.WriteLine(Localization.Parse("{SHFM_CATEGORY}", new Dictionary<string, string>
                {
                    ["%name"] = cat,
                    ["%available"] = Upgrades.GetAvailable().Where(x=>x.Category==cat).Count().ToString()
                }));
            }
            return true;
        }

        [Command("upgrades", description ="{DESC_UPGRADES}")]
        [UsageString("[-c <categoryid>]")]
        public static bool ListAll(Dictionary<string, object> args)
        {
            try
            {
                bool showOnlyInCategory = (bool)args["-c"];


                string cat = (args["<categoryid>"] == null) ? "" : args["<categoryid>"].ToString();

                Dictionary<string, ulong> upgrades = new Dictionary<string, ulong>();
                int maxLength = 5;

                IEnumerable<ShiftoriumUpgrade> upglist = Upgrades.GetAvailable();
                if (showOnlyInCategory)
                {
                    if (Upgrades.IsCategoryEmptied(cat))
                    {
                        ConsoleEx.Bold = true;
                        ConsoleEx.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{SHFM_QUERYERROR}");
                        Console.WriteLine();
                        ConsoleEx.Bold = false;
                        ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("{ERR_EMPTYCATEGORY}");
                        return true;
                    }
                    upglist = Upgrades.GetAvailable().Where(x => x.Category == cat);
                }


                if(upglist.Count() == 0)
                {
                    ConsoleEx.Bold = true;
                    ConsoleEx.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{SHFM_NOUPGRADES}");
                    Console.WriteLine();
                    ConsoleEx.Bold = false;
                    ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("{ERR_NOMOREUPGRADES}");
                    return true;

                }
                foreach (var upg in upglist)
                {
                    if (upg.ID.Length > maxLength)
                    {
                        maxLength = upg.ID.Length;
                    }

                    upgrades.Add(upg.ID, upg.Cost);
                }

                foreach (var upg in upgrades)
                {
                    Console.WriteLine(Localization.Parse("{SHFM_UPGRADE}", new Dictionary<string, string>
                    {
                        ["%id"] = upg.Key,
                        ["%cost"] = upg.Value.ToString()
                    }));
                }
                return true;
            }
            catch (Exception e)
            {
                CrashHandler.Start(e);
                return false;
            }
        }
    }

    public static class WindowCommands
    {
        [RemoteLock]
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

        [RemoteLock]
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
