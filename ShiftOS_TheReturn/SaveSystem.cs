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

//#define ONLINEMODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ShiftOS.Objects;
using ShiftOS.Objects.ShiftFS;
using oobe = ShiftOS.Engine.OutOfBoxExperience;
using static System.Net.Mime.MediaTypeNames;

namespace ShiftOS.Engine
{
    public class EngineConfig
    {
        public bool ConnectToMud = true;
        public string MudDefaultIP = "dome.rol.im";
        public int MudDefaultPort = 13370;
    }

    public static class SaveSystem
    {
        public static bool ShuttingDown = false;

        public static ClientSave CurrentUser { get; set; }


        public static Save CurrentSave { get; set; }

        /// <summary>
        /// Start the entire ShiftOS engine.
        /// </summary>
        /// <param name="useDefaultUI">Whether ShiftOS should initiate it's Windows Forms front-end.</param>
        public static void Begin(bool useDefaultUI = true)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, a) =>
            {
                CrashHandler.Start((Exception)a.ExceptionObject);
            };

            if (!System.IO.File.Exists(Paths.SaveFile))
            {
                var root = new ShiftOS.Objects.ShiftFS.Directory();
                root.Name = "System";
                root.permissions = UserPermissions.Guest;
                System.IO.File.WriteAllText(Paths.SaveFile, JsonConvert.SerializeObject(root));
            }

            if (Utils.Mounts.Count == 0)
                Utils.Mount(System.IO.File.ReadAllText(Paths.SaveFile));
            Paths.Init();

            Localization.SetupTHETRUEDefaultLocals();
            SkinEngine.Init();

            TerminalBackend.OpenTerminal();

            TerminalBackend.InStory = true;
            var thread = new Thread(new ThreadStart(() =>
            {
                //Do not uncomment until I sort out the copyright stuff... - Michael
                //AudioManager.Init();

                var defaultConf = new EngineConfig();
                if (System.IO.File.Exists("engineconfig.json"))
                    defaultConf = JsonConvert.DeserializeObject<EngineConfig>(System.IO.File.ReadAllText("engineconfig.json"));
                else
                {
                    System.IO.File.WriteAllText("engineconfig.json", JsonConvert.SerializeObject(defaultConf, Formatting.Indented));
                }

                Thread.Sleep(350);
                Console.WriteLine("Initiating kernel...");
                Thread.Sleep(250);
                Console.WriteLine("Reading filesystem...");
                Thread.Sleep(100);
                Console.WriteLine("Reading configuration...");

                Console.WriteLine("{CONNECTING_TO_MUD}");

                if (defaultConf.ConnectToMud == true)
                {
                    bool guidReceived = false;
                    ServerManager.GUIDReceived += (str) =>
                    {
                        //Connection successful! Stop waiting!
                        guidReceived = true;
                        Console.WriteLine("Connection successful.");
                    };

                    try
                    {
                        
                        ServerManager.Initiate("secondary4162.cloudapp.net", 13370);
                        //This haults the client until the connection is successful.
                        while (ServerManager.thisGuid == new Guid())
                        {
                            Thread.Sleep(10);
                        }
                        Console.WriteLine("GUID received - bootstrapping complete.");
                        FinishBootstrap();
                    }
                    catch (Exception ex)
                    {
                        //No errors, this never gets called.
                        Console.WriteLine("{ERROR}: " + ex.Message);
                        Thread.Sleep(3000);
                        ServerManager.StartLANServer();
                        while (ServerManager.thisGuid == new Guid())
                        {
                            Thread.Sleep(10);
                        }
                        Console.WriteLine("GUID received - bootstrapping complete.");
                        FinishBootstrap();
                    }
                }
                else
                {
                    ServerManager.StartLANServer();
                }

                //Nothing happens past this point - but the client IS connected! It shouldn't be stuck in that while loop above.

                
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public static void FinishBootstrap()
        {
            KernelWatchdog.Log("mud_handshake", "handshake successful: kernel watchdog access code is \"" + ServerManager.thisGuid.ToString() + "\"");

            ServerMessageReceived savehandshake = null;

            savehandshake = (msg) =>
            {
                if (msg.Name == "mud_savefile")
                {
                    CurrentSave = JsonConvert.DeserializeObject<Save>(msg.Contents);
                    ServerManager.MessageReceived -= savehandshake;
                }
                else if (msg.Name == "mud_login_denied")
                {
                    oobe.PromptForLogin();
                    ServerManager.MessageReceived -= savehandshake;
                }
            };
            ServerManager.MessageReceived += savehandshake;


            ReadSave();

            while (CurrentSave == null)
            {
                Thread.Sleep(10);
            }

            Localization.SetupTHETRUEDefaultLocals();

            Shiftorium.Init();

            while (CurrentSave.StoryPosition < 1)
            {
                Thread.Sleep(10);
            }

            Thread.Sleep(75);

            Thread.Sleep(50);
            Console.WriteLine("{SYSTEM_INITIATED}");

            Sysname:
            bool waitingForNewSysName = false;
            bool gobacktosysname = false;

            if (string.IsNullOrWhiteSpace(CurrentSave.SystemName))
            {
                Infobox.PromptText("Enter a system name", "Your system does not have a name. All systems within the digital society must have a name. Please enter one.", (name)=>
                {
                    if (string.IsNullOrWhiteSpace(name))
                        Infobox.Show("Invalid name", "Please enter a valid name.", () =>
                        {
                            gobacktosysname = true;
                            waitingForNewSysName = false;
                        });
                    else if (name.Length < 5)
                        Infobox.Show("Value too small.", "Your system name must have at least 5 characters in it.", () =>
                        {
                            gobacktosysname = true;
                            waitingForNewSysName = false;
                        });
                    else
                    {
                        CurrentSave.SystemName = name;
                        if (!string.IsNullOrWhiteSpace(CurrentSave.UniteAuthToken))
                        {
                            var unite = new Unite.UniteClient("http://getshiftos.ml", CurrentSave.UniteAuthToken);
                            unite.SetSysName(name);
                        }
                        SaveSystem.SaveGame();
                        gobacktosysname = false;
                        waitingForNewSysName = false;
                    }
                });


            }

            while (waitingForNewSysName)
            {
                Thread.Sleep(10);
            }

            if (gobacktosysname)
            {
                goto Sysname;
            }

            if (CurrentSave.Users == null)
                CurrentSave.Users = new List<ClientSave>();


            if(CurrentSave.Users.Count == 0)
            {
                CurrentSave.Users.Add(new ClientSave
                {
                    Username = "root",
                    Password = "",
                    Permissions = UserPermissions.Root
                });
                Console.WriteLine("No users found. Creating new user with username \"root\", with no password.");
            }
            TerminalBackend.InStory = false;

            TerminalBackend.PrefixEnabled = false;

            Login:
            string username = "";
            int progress = 0;
            bool goback = false;
            TextSentEventHandler ev = null;
            ev = (text) =>
            {
                if (progress == 0)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        if (CurrentSave.Users.FirstOrDefault(x => x.Username == text) == null)
                        {
                            Console.WriteLine("User not found.");
                            goback = true;
                            progress++;
                            TerminalBackend.TextSent -= ev;
                            return;
                        }
                        username = text;
                        progress++;
                    }
                    else
                    {
                        Console.WriteLine("Username not provided.");
                        TerminalBackend.TextSent -= ev;
                        goback = true;
                        progress++;
                    }
                }
                else if (progress == 1)
                {
                    var user = CurrentSave.Users.FirstOrDefault(x => x.Username == username);
                    if (user.Password == text)
                    {
                        Console.WriteLine("Welcome to ShiftOS.");
                        CurrentUser = user;
                        Thread.Sleep(2000);
                        progress++;
                    }
                    else
                    {
                        Console.WriteLine("Access denied.");
                        goback = true;
                        progress++;
                    }
                    TerminalBackend.TextSent -= ev;
                }
            };
            TerminalBackend.TextSent += ev;
            Console.WriteLine(CurrentSave.SystemName + " login:");
            while(progress == 0)
            {
                Thread.Sleep(10);
            }
            if (goback)
                goto Login;
            Console.WriteLine("password:");
            while (progress == 1)
                Thread.Sleep(10);
            if (goback)
                goto Login;


            TerminalBackend.PrefixEnabled = true;
            Shiftorium.LogOrphanedUpgrades = true;
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                ShiftOS.Engine.Scripting.LuaInterpreter.RunSft(Paths.GetPath("kernel.sft"));
            }));


            Desktop.InvokeOnWorkerThread(new Action(() => Desktop.PopulateAppLauncher()));
            GameReady?.Invoke();
        }

        public delegate void EmptyEventHandler();

        public static List<ClientSave> Users
        {
            get
            {
                return CurrentSave.Users;
            }
        }

        public static event EmptyEventHandler GameReady;

        public static void TransferCodepointsToVoid(long amount)
        {
            CurrentSave.Codepoints -= amount;
            NotificationDaemon.AddNotification(NotificationType.CodepointsSent, amount);
        }

        public static void Restart()
        {
            TerminalBackend.InvokeCommand("sos.shutdown");
            System.Windows.Forms.Application.Restart();
        }

        public static void ReadSave()
        {
            //Migrate old saves.
            if(System.IO.Directory.Exists("C:\\ShiftOS2"))
            {
                Console.WriteLine("Old save detected. Migrating filesystem to MFS...");
                foreach (string file in System.IO.Directory.EnumerateDirectories("C:\\ShiftOS2")
.Select(d => new DirectoryInfo(d).FullName))
                {
                    if(!Utils.DirectoryExists(file.Replace("C:\\ShiftOS2\\", "0:/").Replace("\\", "/")))
                    Utils.CreateDirectory(file.Replace("C:\\ShiftOS2\\", "0:/").Replace("\\", "/"));
                }
                foreach (string file in System.IO.Directory.EnumerateFiles("C:\\ShiftOS2"))
                {

                    string rfile = Path.GetFileName(file);
                    Utils.WriteAllBytes(file.Replace("C:\\ShiftOS2\\", "0:/").Replace("\\", "/"), System.IO.File.ReadAllBytes(file));
                    Console.WriteLine("Exported file " + file);
                }

            }


            if (Utils.FileExists(Paths.SaveFileInner))
            {
                oobe.ShowSaveTransfer(JsonConvert.DeserializeObject<Save>(Utils.ReadAllText(Paths.SaveFileInner)));
            }
            else
            {
                if (Utils.FileExists(Paths.GetPath("user.dat")))
                {
                    string token = Utils.ReadAllText(Paths.GetPath("user.dat"));

                    ServerManager.SendMessage("mud_token_login", token);
                }
                else
                {
                    NewSave();
                }
            }

        }

        public static void NewSave()
        {
            AppearanceManager.Invoke(new Action(() =>
            {
                CurrentSave = new Save();
                CurrentSave.Codepoints = 0;
                CurrentSave.Upgrades = new Dictionary<string, bool>();
                Shiftorium.Init();
                oobe.Start(CurrentSave);
            }));
        }

        public static void SaveGame()
        {
            if(!Shiftorium.Silent)
                Console.WriteLine("");
            if(!Shiftorium.Silent)
                Console.Write("{SE_SAVING}... ");
            if (SaveSystem.CurrentSave != null)
            {
                Utils.WriteAllText(Paths.GetPath("user.dat"), CurrentSave.UniteAuthToken);                
                ServerManager.SendMessage("mud_save", JsonConvert.SerializeObject(CurrentSave, Formatting.Indented));
            }
            if (!Shiftorium.Silent)
                Console.WriteLine(" ...{DONE}.");
            System.IO.File.WriteAllText(Paths.SaveFile, Utils.ExportMount(0));
        }

        public static void TransferCodepointsFrom(string who, long amount)
        {
            NotificationDaemon.AddNotification(NotificationType.CodepointsReceived, amount);
            CurrentSave.Codepoints += amount;
        }
    }

    public delegate void TextSentEventHandler(string text);

    public class DeveloperAttribute : Attribute
    {

    }
}
