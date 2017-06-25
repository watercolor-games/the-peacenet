using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    [RequiresUpgrade("hacker101_breakingbonds_3")]
    public static class MissionCommands
    {
        public static List<MissionAttribute> GetMissionsList()
        {
            var missions = new List<MissionAttribute>();
            foreach (var type in ReflectMan.Types)
            {
                foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                {
                    var attrib = method.GetCustomAttributes(false).FirstOrDefault(x => x is MissionAttribute) as MissionAttribute;
                    if (attrib != null)
                    {
                        if (Shiftorium.UpgradeAttributesUnlocked(method))
                        {
                            if (!Shiftorium.UpgradeInstalled(attrib.StoryID))
                            {
                                missions.Add(attrib);
                            }
                        }
                    }
                }
            }
            return missions;
        }

        [Command("missions", description = "Lists all available missions.")]
        public static void ShowAll()
        {
            ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
            ConsoleEx.Bold = true;
            Console.WriteLine(" - Missions - ");

            var missions = GetMissionsList();

            ConsoleEx.ForegroundColor = ConsoleColor.White;
            ConsoleEx.Bold = false;
            if(missions.Count == 0)
            {
                Console.WriteLine("No missions available. Check back later!");
            }
            else
            {
                foreach(var mission in missions)
                {
                    Console.WriteLine();
                    Console.WriteLine(mission.Name);
                    Console.WriteLine("--------------------------");
                    Console.WriteLine();
                    Console.WriteLine(mission.Description);
                    Console.WriteLine();
                    Console.WriteLine("assigner: " + mission.Assigner);
                    Console.WriteLine("reward: " + mission.CodepointAward + " Codepoints");
                    Console.WriteLine("To start this mission, run:");
                    ConsoleEx.Bold = true;
                    Console.WriteLine("startmission --id " + missions.IndexOf(mission));
                }
            }

            Console.WriteLine();

            Console.WriteLine("Story progress:");
            ConsoleEx.Bold = true;
            ConsoleEx.ForegroundColor = ConsoleColor.Cyan;
            double percentage = GetMissionPercentage() * 100;
            Console.WriteLine(percentage.ToString("#.##") + "%");
        }

        public static double GetMissionPercentage()
        {
            int missionsFound = 0;
            int missionsComplete = 0;
            foreach(var type in ReflectMan.Types)
            {
                foreach (var mth in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                {
                    var missionAttrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is MissionAttribute) as MissionAttribute;
                    if (missionAttrib != null)
                    {
                        missionsFound++;
                        if (Shiftorium.UpgradeInstalled(missionAttrib.StoryID))
                            missionsComplete++;
                    }

                }
            }
            double percentage = (double)missionsComplete / (double)missionsFound;
            return percentage;
        }

        [Command("startmission", description = "Starts the specified mission.")]
        [RequiresArgument("id")]
        public static void StartMission(Dictionary<string, object> args)
        {
            var id = Convert.ToInt32(args["id"].ToString());
            var missions = GetMissionsList();
            if (id < 0 || id >= missions.Count)
                Console.WriteLine("Error: Mission ID not found.");
            else
            {
                var mission = missions[id];
                Story.Start(mission.StoryID);
            }
        }

    }
}
