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

namespace Plex.Frontend
{
    public static class FrontendDebugCommands
    {
        [Command("drop_crash")]
        [ShellConstraint("Plex_debug> ")]
        public static void Crash()
        {
            UIManager.Crash();
        }

        [Command("drop_tutorial")]
        [ShellConstraint("Plex_debug> ")]
        [RequiresArgument("id")]
        public static void DropTutorial(Dictionary<string, object> args)
        {
            Random rnd = new Random();
            string text = args["id"].ToString();
            int w = rnd.Next(10, 1280);
            int h = rnd.Next(10, 720);
            int x = rnd.Next(0, 1280 - w);
            int y = rnd.Next(0, 720 - h);

            TerminalBackend.PrefixEnabled = false;
            UIManager.SetTutorialOverlay(new Rectangle(x, y, w, h), text, () =>
               {
                   Console.WriteLine("Tutorial complete.");
                   TerminalBackend.PrefixEnabled = true;
                   TerminalBackend.PrintPrompt();
               });
        }

        [Command("infobox_prison")]
        [ShellConstraint("Plex_debug> ")]
        public static void InfoboxPrison()
        {
            var ibox = new InfoboxMessage("Infobox Prison", "You are now sentenced to life in Infobox Prison.");
            ibox.ShowPrompt(() =>
            {
                InfoboxPrison();
                InfoboxPrison();
            });
            var t = new Thread(() =>
            {
                var parent = ibox.Parent;
                int xvel = 3;
                int yvel = 3;
                while (parent.Visible)
                {
                    if (parent.X + parent.Width >= UIManager.Viewport.Width)
                        xvel = -xvel;
                    if (parent.X <= 0)
                        xvel = -xvel;
                    if (parent.Y <= 0)
                        yvel = -yvel;
                    if (parent.Y + parent.Height >= UIManager.Viewport.Height)
                        yvel = -yvel;
                    parent.X += xvel;
                    parent.Y += yvel;
                    Thread.Sleep(50);
                }
                InfoboxPrison();
                InfoboxPrison();
            });
            t.Start();
        }

        [Command("set_ui_tint")]
        [RequiresArgument("color")]
        [ShellConstraint("Plex_debug> ")]
        public static void SetUITint    (Dictionary<string, object> args)
        {
            string[] split = args["color"].ToString().Split(';');
            int r = MathHelper.Clamp(Convert.ToInt32(split[0]), 0, 255);
            int g = MathHelper.Clamp(Convert.ToInt32(split[1]), 0, 255);
            int b = MathHelper.Clamp(Convert.ToInt32(split[2]), 0, 255);
            UIManager.SetUITint(new Color(r, g, b, 255));
        }

        [Command("drop_opener")]
        [ShellConstraint("Plex_debug> ")]
        public static void DropOpener(Dictionary<string, object> args)
        {
            string[] ids = new string[] { "" };
            if (args.ContainsKey("id"))
            {
                ids = args["id"].ToString().Split(';');
            }
            FileSkimmerBackend.GetFile(ids, FileOpenerStyle.Open, (path) =>
            {
                Console.WriteLine(path);
                TerminalBackend.PrintPrompt();
            });
        }

        [Command("drop_saver")]
        [ShellConstraint("Plex_debug> ")]
        public static void DropSaver(Dictionary<string, object> args)
        {
            string[] ids = new string[] { "" };
            if (args.ContainsKey("id"))
            {
                ids = args["id"].ToString().Split(';');
            }
            FileSkimmerBackend.GetFile(ids, FileOpenerStyle.Save, (path) =>
            {
                Console.WriteLine(path);
                TerminalBackend.PrintPrompt();
            });
        }

        /// <summary>
        /// Debug command to drop a fatal objective/hack failure screen in the form of an emergency alert system-esque screen.
        /// 
        /// ...Because WE'RE CANADA.
        /// </summary>
        [Command("drop_eas")]
        [ShellConstraint("Plex_debug> ")]
        [RequiresArgument("id")]
        public static void DropEAS(Dictionary<string, object> args)
        {
            Story.DisplayFailure(args["id"].ToString());
        }

        [Command("loaddefaultskn")]
        [ShellConstraint("Plex_debug> ")]
        public static void LoadDefault()
        {
            Utils.Delete(Paths.GetPath("skin.json"));
            SkinEngine.Init();
        }
    }

    public static class Cowsay
    {
        [Command("cowsay")]
        [RequiresArgument("id")]
        public static void Say(Dictionary<string, object> args)
        {
            var builder = new List<string>();
            int speechlen = (args.ContainsKey("width")) ? Convert.ToInt32(args["width"].ToString()) : 50;
            string cowfile = (args.ContainsKey("file")) ? args["file"].ToString() : null;
            string speech = args["id"].ToString();
            AnimalMode _mode = AnimalMode.Normal;
            if (args.ContainsKey("mode"))
            {
                try
                {
                    _mode = (AnimalMode)Enum.Parse(typeof(AnimalMode), args["mode"].ToString());
                }
                catch
                {
                    Console.WriteLine("Invalid animal mode. Valid animal modes are:");
                    foreach(var name in Enum.GetNames(typeof(AnimalMode)))
                    {
                        Console.WriteLine(" - " + name);
                    }
                    return;
                }

            }
            DrawSpeechBubble(ref builder, speechlen, speech);
            DrawCow(ref builder, _mode, cowfile);
            Console.WriteLine(string.Join(Environment.NewLine, builder.ToArray()));
        }

        public static string[] SplitInParts(this string value, int width)
        {
            List<string> nvalue = new List<string>();
            while(value.Length > 0)
            {
                string substr = value.Substring(0, Math.Min(value.Length, width));
                value = value.Remove(0, substr.Length);
                nvalue.Add(substr);
            }

            return nvalue.ToArray();
        }

        public static string RepeatChar(this char value, int amount)
        {
            string nvalue = "";
            for(int i = 0; i < amount; i++)
            {
                nvalue += value;
            }
            return nvalue;
        }

        private static void DrawSpeechBubble(ref List<string> Builder, int balloonWidth, string speech)
        {
            var lineLength = balloonWidth - 4;
            var output = speech.SplitInParts((int)lineLength).ToArray();
            var lines = output.Length;
            var wrapperLineLength = (lines == 1 ? output.First().Length : (int)balloonWidth - 4) + 2;

            Builder.Add($" {'_'.RepeatChar(wrapperLineLength)}");
            if (lines == 1)
            {
                Builder.Add($"< {output.First()} >");
            }
            else
            {
                for (var i = 0; i < lines; i++)
                {
                    char lineStartChar = '|';
                    char lineEndChar = '|';

                    if (i == 0)
                    {
                        lineStartChar = '/';
                        lineEndChar = '\\';
                    }
                    else if (i == lines - 1)
                    {
                        lineStartChar = '\\';
                        lineEndChar = '/';
                    }

                    var neededPadding = (int)balloonWidth - 4 - output[i].Length;
                    Builder.Add($"{lineStartChar} {output[i]}{' '.RepeatChar(neededPadding)} {lineEndChar}");
                }
            }

            Builder.Add($" {'-'.RepeatChar(wrapperLineLength)}");
        }

        public enum AnimalMode
        {
            Normal,
            Borg,
            Dead,
            Greedy,
            Paranoid,
            Stoned,
            Tired,
            Wired,
            Youthful
        }

        private static void DrawCow(ref List<string> Builder, AnimalMode AnimalMode, string cowfile)
        {
            var startingLinePadding = Builder.First().Length / 4;

            var eyeChar = 'o';
            var tongueChar = ' ';

            switch (AnimalMode)
            {
                case AnimalMode.Borg:
                    eyeChar = '=';
                    break;

                case AnimalMode.Dead:
                    eyeChar = 'x';
                    tongueChar = 'U';
                    break;

                case AnimalMode.Greedy:
                    eyeChar = '$';
                    break;

                case AnimalMode.Paranoid:
                    eyeChar = '@';
                    break;

                case AnimalMode.Stoned:
                    eyeChar = '*';
                    tongueChar = 'U';
                    break;

                case AnimalMode.Tired:
                    eyeChar = '-';
                    break;

                case AnimalMode.Wired:
                    eyeChar = 'O';
                    break;

                case AnimalMode.Youthful:
                    eyeChar = '.';
                    break;

            }
            string cowpath = Paths.GetPath("data") + "/cows/" + cowfile + ".cow";
            if (string.IsNullOrWhiteSpace(cowfile) || !Utils.FileExists(cowpath))
            {
                Builder.Add($"{' '.RepeatChar(startingLinePadding)}\\   ^__^");
                Builder.Add($"{' '.RepeatChar(startingLinePadding)} \\  ({eyeChar.RepeatChar(2)})\\_______");
                Builder.Add($"{' '.RepeatChar(startingLinePadding)}    (__)\\       )\\/\\");
                Builder.Add($"{' '.RepeatChar(startingLinePadding)}     {tongueChar.RepeatChar(1)}  ||----w |");
                Builder.Add($"{' '.RepeatChar(startingLinePadding)}        ||     ||");
            }
            else
            {
                string[] lines = Utils.ReadAllText(cowpath).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var line in lines)
                {
                    Builder.Add($"{' '.RepeatChar(startingLinePadding)}{line.Replace("#", eyeChar.ToString())}");
                }
            }
        }
    }


    public static class MissionsCommands
    {
        [Command("startmission")]
        [RequiresArgument("id")]
        [RequiresUpgrade("tutorial1")]
        public static void StartMission(Dictionary<string, object> args)
        {
            string id = args["id"].ToString();
            try
            {
                if (!Upgrades.UpgradeInstalled(id))
                {
                    Story.Start(id);
                    return;
                }
                Console.WriteLine("That mission has already been complete. You can't replay it.");
            }
            catch
            {
                Console.WriteLine("That mission could not be found. Try running missions for a list of available missions.");
            }
        }

        [Command("missions")]
        [RequiresUpgrade("tutorial1")]
        public static void Missions()
        {
            Console.WriteLine("Available missions");
            Console.WriteLine("===================");
            Console.WriteLine();
            bool found = false;
            foreach (var type in ReflectMan.Types)
            {
                foreach(var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    var missionattrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is MissionAttribute) as MissionAttribute;
                    if(missionattrib != null)
                    {
                        if (!Upgrades.UpgradeInstalled(missionattrib.StoryID))
                        {
                            found = true;
                            Console.WriteLine();
                            Console.WriteLine($@"{missionattrib.Name} (id {missionattrib.StoryID})
------------------------------------

assigner: {missionattrib.Assigner}
cp reward: {missionattrib.CodepointAward}

{missionattrib.Description}");
                        }
                    }
                }
            }
            if(found == false)
            {
                Console.WriteLine();
                Console.WriteLine(@"No missions found.
------------------------------------

assigner: undefined
cp reward: [object Object]

There are no missions available for you to complete. Please check back later for more!");

            }
        }
    }



    [TutorialLock]
    public static class TerminalCommands
    {
        [Command("echo")]
        [RequiresArgument("id")]
        public static void Echo(Dictionary<string, object> args)
        {
            Console.WriteLine(args["id"].ToString());
        }

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
#if DEBUG
        [Command("debug")]
        public static void EnterDebug()
        {
            TerminalBackend.SetShellOverride("Plex_debug> ");
        }
#endif

    [Command("setsfxenabled", description = "{DESC_SETSFXENABLED}")]
        [RequiresArgument("id")]
        public static bool SetSfxEnabled(Dictionary<string, object> args)
        {
            try
            {
                bool value = Convert.ToBoolean(args["id"].ToString());
                SaveSystem.CurrentSave.SoundEnabled = value;
                SaveSystem.SaveGame();
            }
            catch
            {
                Console.WriteLine("{ERR_BADBOOL}");
            }
            return true;
        }



        [Command("setmusicenabled", description = "{DESC_SETMUSICENABLED}")]
        [RequiresArgument("id")]
        public static bool SetMusicEnabled(Dictionary<string, object> args)
        {
            try
            {
                bool value = Convert.ToBoolean(args["id"].ToString());
                SaveSystem.CurrentSave.MusicEnabled = value;
                SaveSystem.SaveGame();
            }
            catch
            {
                Console.WriteLine("{ERR_BADBOOL}");
            }
            return true;
        }

        

        [Command("setvolume", description ="{DESC_SETVOLUME}")]
        [RequiresArgument("id")]
        public static bool SetSfxVolume(Dictionary<string, object> args)
        {
            int value = int.Parse(args["id"].ToString());
            if(value >= 0 && value <= 100)
            {
                SaveSystem.CurrentSave.MusicVolume = value;
                SaveSystem.SaveGame();
            }
            else
            {
                Console.WriteLine("{ERR_BADPERCENT}");
            }
            return true;
        }

        [RemoteLock]
        [Command("shutdown", description = "{DESC_SHUTDOWN}")]
        public static bool Shutdown()
        {
            if(Objects.ShiftFS.Utils.Mounts.Count > 0)
                SaveSystem.SaveGame();
            AppearanceManager.Exit();
            return true;
        }

        [Command("lang", description = "{DESC_LANG}")]
        [RequiresArgument("id")]
        public static bool SetLanguage(Dictionary<string, object> userArgs)
        {
            try
            {
                string lang = "";

                lang = (string)userArgs["id"];
                
                if (Localization.GetAllLanguages().Contains(lang))
                {
                    SaveSystem.CurrentSave.Language = lang;
                    SaveSystem.SaveGame();
                    Console.WriteLine("{RES_LANGUAGE_CHANGED}");
                    return true;
                }

                throw new Exception("{ERR_NOLANG}");
            }
            catch
            {
                return false;
            }
        }

        [MetaCommand]
        [Command("help", "", "{DESC_COMMANDS}")]
        public static bool Commands()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{GEN_COMMANDS}");
            sb.AppendLine("=================");
            sb.AppendLine();
            //print all unique namespaces.
            foreach (var n in TerminalBackend.Commands.Where(x => !(x is TerminalBackend.WinOpenCommand) && Upgrades.UpgradeInstalled(x.Dependencies) && x.CommandInfo.hide == false && x.MatchShell() == true).OrderBy(x => x.CommandInfo.name))
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



        [MultiplayerOnly]
        [Command("save", description = "{DESC_SAVE}")]
        public static bool Save()
        {
            SaveSystem.SaveGame();
            return true;
        }

        [MultiplayerOnly]
        [Command("status", description = "{DESC_STATUS}")]
        public static bool Status()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

           string cp = SaveSystem.CurrentSave.Experience.ToString();
           string installed = SaveSystem.CurrentSave.CountUpgrades().ToString();
            string available = Upgrades.GetAvailable().Length.ToString();

            Console.WriteLine(Localization.Parse("{COM_STATUS}", new Dictionary<string, string>
            {
                ["%cp"] = cp,
                ["%version"] = version,
                ["%installed"] = installed,
                ["%available"] = available
            }));
            Console.WriteLine("{GEN_OBJECTIVES}");
            try
            {
                if (Story.CurrentObjectives.Count > 0)
                {
                    foreach (var obj in Story.CurrentObjectives)
                    {
                        Console.WriteLine(obj.Name);
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine();
                        Console.WriteLine(obj.Description);
                    }
                }
                else
                {
                    Console.WriteLine("{RES_NOOBJECTIVES}");
                }
            }
            catch
            {
                Console.WriteLine("{RES_NOOBJECTIVES}");
            }
            return true;
        }
    }

    public static class ShiftoriumCommands
    {
        [Command("buy", description = "{DESC_BUY}")]
        [RequiresArgument("id")]
        public static bool BuyUpgrade(Dictionary<string, object> userArgs)
        {
            try
            {
                string upgrade = "";

                upgrade = (string)userArgs["id"];

                var upg = Upgrades.GetAvailable().FirstOrDefault(x => x.ID == upgrade);
                if(upg != null)
                {
                    if (!Upgrades.Buy(upg.ID, upg.Cost) == true)
                        Console.WriteLine("{ERR_NOTENOUGHExperience}");
                }
                else
                {
                    Console.WriteLine("{ERR_NOUPGRADE}");
                }

            }
            catch
            {
                Console.WriteLine("{ERR_GENERAL}");
            }
            return true;
        }

        [RequiresUpgrade("shiftorium_bulk_buy")]
        [Command("bulkbuy", description = "{DESC_BULKBUY}")]
        [RequiresArgument("id")]
        public static bool BuyBulk(Dictionary<string, object> args)
        {
            if (args.ContainsKey("id"))
            {
                string[] upgrade_list = (args["id"] as string).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var upg in upgrade_list)
                {
                    var dict = new Dictionary<string, object>();
                    dict.Add("upgrade", upg);
                    BuyUpgrade(dict);
                }
            }
            return true;
        }


        [Command("upgradeinfo", description ="{DESC_UPGRADEINFO}")]
        [RequiresArgument("id")]
        public static bool ViewInfo(Dictionary<string, object> userArgs)
        {
            try
            {
                string upgrade = "";

                upgrade = (string)userArgs["id"];

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
        public static bool ListAll(Dictionary<string, object> args)
        {
            try
            {
                bool showOnlyInCategory = false;

                string cat = "Other";

                if (args.ContainsKey("cat"))
                {
                    showOnlyInCategory = true;
                    cat = args["cat"].ToString();
                }

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
        [RequiresArgument("id")]
        public static bool CloseWindow(Dictionary<string, object> args)
        {
            int winNum = -1;
            if (args.ContainsKey("id"))
                winNum = Convert.ToInt32(args["id"].ToString());
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

            return true;
        }

    }
}
