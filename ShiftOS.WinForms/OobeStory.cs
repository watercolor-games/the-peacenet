using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;

namespace ShiftOS.WinForms
{
    public class OobeStory
    {
        [Command("test")]
        [RequiresArgument("num")]
        public static bool TestThingy(Dictionary<string, object> args)
        {
            long num = Convert.ToInt64(args["num"].ToString());
            string hex = num.ToString("X");
            string bin = Convert.ToString(num, 2);
            Console.WriteLine("Hex: " + hex);
            Console.WriteLine("Bin: " + bin);
            return true;
        }


        [Story("mud_fundamentals")]
        public static void DoStory()
        {
            Applications.Terminal term = null;
            TerminalBackend.PrefixEnabled = false;
            Desktop.InvokeOnWorkerThread(() =>
            {
                term = new Applications.Terminal();
                AppearanceManager.SetupWindow(term);
                ConsoleEx.Bold = true;
                ConsoleEx.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Welcome to ShiftOS.");
                Console.WriteLine();
                ConsoleEx.Bold = false;
                ConsoleEx.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Before we can bring you to your new system, we must perform some system tasks.");
                Console.WriteLine();
                Console.WriteLine("Here's the installation outline.");
                Console.WriteLine();
                Console.Write(" - ");
                ConsoleEx.Bold = true;
                Console.Write("Storage preparation");
                ConsoleEx.Bold = false;
                Console.Write(" First, we have to prepare your computer's storage device for ShiftOS. This \r\nincludes formatting your drive with the ShiftFS file \r\nsystem, creating system directories, and generating system files.");
                Console.WriteLine();
                Console.Write(" - ");
                ConsoleEx.Bold = true;
                Console.Write("User configuration");
                ConsoleEx.Bold = false;
                Console.Write(" Next it's up to you to set up a system hostname, create a user account, and personalize it.");
                Console.WriteLine();
                Console.Write(" - ");
                ConsoleEx.Bold = true;
                Console.Write("System tutorial");
                ConsoleEx.Bold = false;
                Console.WriteLine("Finally, we'll teach you how to use ShiftOS.");

                Console.WriteLine();

                ConsoleEx.Bold = true;
                ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Let's get started!");
            });
            int position = 0;

            Thread.Sleep(5000);

            ConsoleEx.Bold = true;
            Console.WriteLine("System preparation");


            Console.WriteLine();
            ConsoleEx.Bold = false;
            ConsoleEx.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"We'll now begin formatting your drive. Please be patient.");
            Console.WriteLine();
            double bytesFree, totalBytes;
            string type, name;
            dynamic dinf;
            try
            {
                if (Lunix.InWine)
                    dinf = new Lunix.DFDriveInfo("/");
                else
                    dinf = new DriveInfo("C:\\");
                bytesFree = dinf.AvailableFreeSpace / 1073741824.0;
                totalBytes = dinf.TotalSize / 1073741824.0;
                type = dinf.DriveFormat.ToString();
                name = dinf.Name;
                ConsoleEx.Bold = true;
                Console.Write("Drive name: ");
                ConsoleEx.Bold = false;
                Console.WriteLine(name);
                ConsoleEx.Bold = true;
                Console.Write("Drive type: ");
                ConsoleEx.Bold = false;
                Console.WriteLine(type);
                ConsoleEx.Bold = true;
                Console.Write("Total space: ");
                ConsoleEx.Bold = false;
                Console.WriteLine(String.Format("{0:F1}", totalBytes) + " GB");
                ConsoleEx.Bold = true;
                Console.Write("Free space: ");
                Console.WriteLine(String.Format("{0:F1}", bytesFree) + " GB");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            ConsoleEx.Bold = false;
            ConsoleEx.BackgroundColor = ConsoleColor.Black;
            Console.Write("Formatting: [");
            ConsoleEx.OnFlush?.Invoke();
            int formatProgress = 3;
            while (formatProgress <= 100)
            {
                if (formatProgress % 3 == 0)
                {
                    ConsoleEx.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    ConsoleEx.OnFlush?.Invoke();
                    ConsoleEx.BackgroundColor = ConsoleColor.Black;
                }
                Desktop.InvokeOnWorkerThread(() => Engine.AudioManager.PlayStream(Properties.Resources.typesound));
                formatProgress++;
                Thread.Sleep(175);
            }
            Console.WriteLine("] ..done.");
            Thread.Sleep(1000);
            ConsoleEx.Bold = true;
            Console.WriteLine("Creating directories...");
            ConsoleEx.Bold = false;
            foreach (var dir in Paths.GetAllWithoutKey())
            {
                if (!dir.Contains(".") && dir.StartsWith("0:/"))
                {
                    Console.WriteLine("Creating: " + dir);
                    Thread.Sleep(125);
                    Desktop.InvokeOnWorkerThread(() => Engine.AudioManager.PlayStream(Properties.Resources.writesound));
                }
            }
            Console.WriteLine();
            Console.WriteLine("Next, let's get user information.");
            Console.WriteLine();
            Desktop.InvokeOnWorkerThread(() =>
            {
                var uSignUpDialog = new UniteSignupDialog((result) =>
                {
                    var sve = new Save();
                    sve.SystemName = result.SystemName;
                    sve.Codepoints = 0;
                    sve.Upgrades = new Dictionary<string, bool>();
                    sve.ID = Guid.NewGuid();
                    sve.StoriesExperienced = new List<string>();
                    sve.StoriesExperienced.Add("mud_fundamentals");
                    sve.Users = new List<ClientSave>
                    {
                    new ClientSave
                    {
                        Username = "root",
                        Password = result.RootPassword,
                        Permissions = 0
                    }
                    };

                    sve.StoryPosition = 8675309;
                    SaveSystem.CurrentSave = sve;
                    Shiftorium.Silent = true;
                    SaveSystem.SaveGame();
                    Shiftorium.Silent = false;


                });
                AppearanceManager.SetupDialog(uSignUpDialog);
            });
        }

        private static bool isValid(string text, string chars)
        {
            foreach(var c in text)
            {
                if (!chars.Contains(c))
                    return false;
            }
            return true;
        }
    }
}
