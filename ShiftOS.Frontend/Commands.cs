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

#define DEVEL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine.Properties;
using System.IO;
using Newtonsoft.Json;
using System.IO.Compression;

using ShiftOS.Objects;
using ShiftOS.Engine.Scripting;
using ShiftOS.Objects.ShiftFS;
using ShiftOS.Engine;

namespace ShiftOS.Frontend
{
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
                if (!Shiftorium.UpgradeInstalled(id))
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

    public static class ShiftOSCommands
    {
#if DEBUG
        [Command("debug")]
        public static void EnterDebug()
        {
            TerminalBackend.SetShellOverride("shiftos_debug> ");
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
            foreach (var n in TerminalBackend.Commands.Where(x => !(x is TerminalBackend.WinOpenCommand) && Shiftorium.UpgradeInstalled(x.Dependencies) && x.CommandInfo.hide == false && x.MatchShell() == true).OrderBy(x => x.CommandInfo.name))
            {
                sb.Append(" - " + n.CommandInfo.name);
                if (!string.IsNullOrWhiteSpace(n.CommandInfo.description))
                    if (Shiftorium.UpgradeInstalled("help_description"))
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

           string cp = SaveSystem.CurrentSave.Codepoints.ToString();
           string installed = SaveSystem.CurrentSave.CountUpgrades().ToString();
            string available = Shiftorium.GetAvailable().Length.ToString();

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

                var upg = Shiftorium.GetAvailable().FirstOrDefault(x => x.ID == upgrade);
                if(upg != null)
                {
                    if (!Shiftorium.Buy(upg.ID, upg.Cost) == true)
                        Console.WriteLine("{ERR_NOTENOUGHCODEPOINTS}");
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

                foreach (var upg in Shiftorium.GetDefaults())
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
            foreach(var cat in Shiftorium.GetCategories())
            {
                Console.WriteLine(Localization.Parse("{SHFM_CATEGORY}", new Dictionary<string, string>
                {
                    ["%name"] = cat,
                    ["%available"] = Shiftorium.GetAvailable().Where(x=>x.Category==cat).Count().ToString()
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

                IEnumerable<ShiftoriumUpgrade> upglist = Shiftorium.GetAvailable();
                if (showOnlyInCategory)
                {
                    if (Shiftorium.IsCategoryEmptied(cat))
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
                    upglist = Shiftorium.GetAvailable().Where(x => x.Category == cat);
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
            foreach(var n in TerminalBackend.Commands.Where(x => x is TerminalBackend.WinOpenCommand && Shiftorium.UpgradeInstalled(x.Dependencies)).OrderBy(x => x.CommandInfo.name))
            {
                sb.Append(" - " + n.CommandInfo.name);
                if (!string.IsNullOrWhiteSpace(n.CommandInfo.description))
                    if (Shiftorium.UpgradeInstalled("help_description"))
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
