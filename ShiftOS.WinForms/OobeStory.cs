using System;
using System.Collections.Generic;
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
            var dinf = new DriveInfo("C:\\");
            decimal bytesFree = ((dinf.AvailableFreeSpace / 1024) / 1024) / 1024;
            decimal totalBytes = ((dinf.TotalSize / 1024) / 1024) / 1024;
            string type = dinf.DriveType.ToString();
            string name = dinf.Name;
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
            Console.WriteLine(totalBytes.ToString() + " GB");
            ConsoleEx.Bold = true;
            Console.Write("Free space: ");
            Console.WriteLine(bytesFree.ToString() + " GB");
            Console.WriteLine();


            ConsoleEx.Bold = false;
            ConsoleEx.BackgroundColor = ConsoleColor.Black;
            Console.Write("Formatting: [");
            int formatProgress = 0;
            while (formatProgress <= 100)
            {
                if (formatProgress % 3 == 0)
                {
                    ConsoleEx.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    ConsoleEx.BackgroundColor = ConsoleColor.Black;
                }
                Engine.AudioManager.PlayStream(Properties.Resources.typesound);
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
                    Engine.AudioManager.PlayStream(Properties.Resources.writesound);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Next, let's get user information.");
            Console.WriteLine();
            Console.WriteLine("Please enter a system hostname.");
            string allowed_chars = "abcdefghijklmnopqrstuvwxyz1234567890_";
            bool userExists = false;
            Applications.Terminal.TextSent += (text) =>
            {
                if(position == 0)
                {
                    if(text.Length < 5)
                    {
                        Console.WriteLine("Your hostname must be at least 5 characters long.");
                        return;
                    }

                    if(!isValid(text, allowed_chars))
                    {
                        Console.WriteLine("Your hostname contains illegal characters. You can only use these characters: " + allowed_chars);
                        return;
                    }
                    SaveSystem.CurrentSave.SystemName = text;
                    position = 1;
                }
                else if(position == 1)
                {
                    if (text.Length < 5)
                    {
                        Console.WriteLine("Your username must be at least 5 characters long.");
                        return;
                    }
                    if (!isValid(text, allowed_chars))
                    {
                        Console.WriteLine("Your username contains illegal characters. You can only use these characters: " + allowed_chars);
                        return;
                    }
                    SaveSystem.CurrentSave.Username = text;
                    position = 2;
                }
                else if(position == 3)
                {
                    if (!userExists)
                    {
                        if (text.Length < 5)
                        {
                            Console.WriteLine("Your password must be at least 5 characters long.");
                            return;
                        }
                        SaveSystem.CurrentSave.Password = text;
                        position = 4;
                    }
                    else
                    {
                        ServerManager.SendMessage("mud_login", JsonConvert.SerializeObject(new
                        {
                            username = SaveSystem.CurrentSave.Username,
                            password = text
                        }));
                    }
                }
            };

            TerminalBackend.InStory = false;

            while (position == 0)
                Thread.Sleep(10);
            Console.WriteLine("Connecting to the multi-user domain as " + SaveSystem.CurrentSave.SystemName + "...");
            Engine.AudioManager.PlayStream(Properties.Resources.dial_up_modem_02);
            Console.WriteLine("Connection successful, system spinning up...");
            Thread.Sleep(200);
            Console.WriteLine("No users associated with this system. Please enter a username.");
            Console.WriteLine(" - If the username is registered as a digital being, you will be prompted for your password. Else, you will be prompted to create a new account.");
            while(position == 1)
            {
                Thread.Sleep(10);
            }
            int incorrectChances = 2;
            Console.WriteLine("Checking sentience records...");
            ServerManager.MessageReceived += (msg) =>
            {
                if (position == 2)
                {
                    if (msg.Name == "mud_found")
                    {
                        Console.WriteLine("Your username has been taken by another sentient being within the digital society.");
                        Console.WriteLine("If you are that sentience, you have two chances to type the correct password.");
                        userExists = true;
                    }
                    else if (msg.Name == "mud_notfound")
                    {
                        Console.WriteLine("Please enter a password for this new user.");
                        userExists = false;
                    }
                    position = 3;
                }
                else if (position == 3)
                {
                    if(userExists == true)
                    {
                        if(msg.Name == "mud_savefile")
                        {
                            Console.WriteLine("Your sentience profile has been assigned to your system successfully. We will bring you to your system shortly.");
                            SaveSystem.CurrentSave = JsonConvert.DeserializeObject<Save>(msg.Contents);
                            position = 4;
                        }
                        else if(msg.Name == "mud_login_denied")
                        {
                            if (incorrectChances > 0)
                            {
                                incorrectChances--;
                                Console.WriteLine("Access denied. Chances: " + incorrectChances);
                            }
                            else
                            {
                                Console.WriteLine("Access denied.");
                                position = 2;
                            }
                        }
                    }
                }
            };
            ServerManager.SendMessage("mud_checkuserexists", JsonConvert.SerializeObject(new { username = SaveSystem.CurrentSave.Username }));
            while(position == 2)
            {
                Thread.Sleep(10);
            }
            while (position == 3)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("Sentience linkup successful.");
            Console.WriteLine("We will bring you to your system in 5 seconds.");
            Thread.Sleep(5000);
            Desktop.InvokeOnWorkerThread(() =>
            {
                var lst = new List<Form>();
                foreach (Form frm in Application.OpenForms)
                    lst.Add(frm);
                lst.ForEach((frm) =>
                {
                    if (!(frm is WinformsDesktop))
                        frm.Close();
                });
                TerminalBackend.PrefixEnabled = true;

                AppearanceManager.SetupWindow(new Applications.Terminal());
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
