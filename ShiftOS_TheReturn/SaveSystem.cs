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
// #define NOSAVE

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
    [Obsolete("Use the servers.conf file instead.")]
    public class EngineConfig
    {
        public bool ConnectToMud = true;
        public string MudDefaultIP = "dome.rol.im";
        public int MudDefaultPort = 13370;
    }

    /// <summary>
    /// Management class for the ShiftOS save system.
    /// </summary>
    public static class SaveSystem
    {
        /// <summary>
        /// Boolean representing whether the system is shutting down.
        /// </summary>
        public static bool ShuttingDown = false;

        /// <summary>
        /// Gets or sets the current logged in client-side user.
        /// </summary>
        public static ClientSave CurrentUser { get; set; }

        /// <summary>
        /// Boolean representing whether the save system is ready to be used.
        /// </summary>
        public static bool Ready = false;

        /// <summary>
        /// Occurs before the save system connects to the ShiftOS Digital Society.
        /// </summary>
        public static event Action PreDigitalSocietyConnection;

        /// <summary>
        /// Gets or sets the current server-side save file.
        /// </summary>
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
                Console.WriteLine("ShiftKernel v0.4.2");
                Console.WriteLine("(MIT) DevX 2017, Very Little Rights Reserved");
                Console.WriteLine("");
                Console.WriteLine("THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR");
                Console.WriteLine("IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,");
                Console.WriteLine("FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE");
                Console.WriteLine("AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER");
                Console.WriteLine("LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,");
                Console.WriteLine("OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE");
                Console.WriteLine("SOFTWARE.");
                Console.WriteLine("");
                Thread.Sleep(250);
                Console.WriteLine("[init] Kernel boot complete.");
                Console.WriteLine("[sfs] Loading SFS driver v3");
                Thread.Sleep(100);
                Console.WriteLine("[sfs] 4096 blocks read.");
                Console.WriteLine("[simpl-conf] Reading configuration files (global-3.conf)");
                Console.WriteLine("[termdb] Building command database from filesystem...");
                TerminalBackend.PopulateTerminalCommands();
                Console.WriteLine("[inetd] Connecting to network...");

                Ready = false;

                if (PreDigitalSocietyConnection != null)
                {
                    PreDigitalSocietyConnection?.Invoke();

                    while (!Ready)
                    {
                        Thread.Sleep(10);
                    }
                }

                

                bool guidReceived = false;
                ServerManager.GUIDReceived += (str) =>
                {
                        //Connection successful! Stop waiting!
                        guidReceived = true;
                    Console.WriteLine("[inetd] Connection successful.");
                };

                try
                {

                    ServerManager.Initiate(UserConfig.Get().DigitalSocietyAddress, UserConfig.Get().DigitalSocietyPort);
                    //This haults the client until the connection is successful.
                    while (ServerManager.thisGuid == new Guid())
                    {
                        Thread.Sleep(10);
                    }
                    Console.WriteLine("[inetd] DHCP GUID recieved, finished setup");
                    FinishBootstrap();
                }
                catch (Exception ex)
                {
                    //No errors, this never gets called.
                    Console.WriteLine("[inetd] SEVERE: " + ex.Message);
                    Thread.Sleep(3000);
                    Console.WriteLine("[sys] SEVERE: Cannot connect to server. Shutting down in 5...");
                    Thread.Sleep(1000);
                    Console.WriteLine("[sys] 4...");
                    Thread.Sleep(1000);
                    Console.WriteLine("[sys] 3...");
                    Thread.Sleep(1000);
                    Console.WriteLine("[sys] 2...");
                    Thread.Sleep(1000);
                    Console.WriteLine("[sys] 1...");
                    Thread.Sleep(1000);
                    Console.WriteLine("[sys] Bye bye.");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }

                //Nothing happens past this point - but the client IS connected! It shouldn't be stuck in that while loop above.


            }));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Finish bootstrapping the engine.
        /// </summary>
        private static void FinishBootstrap()
        {
            KernelWatchdog.Log("mud_handshake", "handshake successful: kernel watchdog access code is \"" + ServerManager.thisGuid.ToString() + "\"");

            ServerMessageReceived savehandshake = null;

            savehandshake = (msg) =>
            {
                ServerManager.MessageReceived -= savehandshake;
                if (msg.Name == "mud_savefile")
                {
                    CurrentSave = JsonConvert.DeserializeObject<Save>(msg.Contents);
                }
                else if (msg.Name == "mud_login_denied")
                {
                    oobe.PromptForLogin();
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
            Console.WriteLine("[usr-man] Accepting logins on local tty 1.");

            Sysname:
            bool waitingForNewSysName = false;
            bool gobacktosysname = false;

            if (string.IsNullOrWhiteSpace(CurrentSave.SystemName))
            {
                Infobox.PromptText("Enter a system name", "Your system does not have a name. All systems within the digital society must have a name. Please enter one.", (name) =>
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

            Console.WriteLine($@"
                   `-:/++++::.`                   
              .+ydNMMMMMNNMMMMMNhs/.              
           /yNMMmy+:-` `````.-/ohNMMms-           
        `oNMMh/.`:oydmNMMMMNmhs+- .+dMMm+`             Welcome to ShiftOS.
      `oMMmo``+dMMMMMMMMMMMMMMMMMNh/`.sNMN+       
     :NMN+ -yMMMMMMMNdhyssyyhdmNMMMMNs``sMMd.          SYSTEM STATUS:
    oMMd.`sMMMMMMd+.            `/MMMMN+ -mMN:         ----------------------
   oMMh .mMMMMMM/     `-::::-.`  :MMMMMMh`.mMM:   
  :MMd .NMMMMMMs    .dMMMMMMMMMNddMMMMMMMd`.NMN.        Codepoints:     {SaveSystem.CurrentSave.Codepoints}
  mMM. dMMMMMMMo    -mMMMMMMMMMMMMMMMMMMMMs /MMy        Upgrades:       {SaveSystem.CurrentSave.CountUpgrades()} installed
 :MMh :MMMMMMMMm`     .+shmMMMMMMMMMMMMMMMN` NMN`                       {Shiftorium.GetAvailable().Count()} available
 oMM+ sMMMMMMMMMN+`        `-/smMMMMMMMMMMM: hMM:       Filesystems:    {Utils.Mounts.Count} filesystems mounted in memory.
 sMM+ sMMMMMMMMMMMMds/-`        .sMMMMMMMMM/ yMM/ 
 +MMs +MMMMMMMMMMMMMMMMMmhs:`     +MMMMMMMM- dMM-       System name:    {CurrentSave.SystemName.ToUpper()}
 .MMm `NMMMMMMMMMMMMMMMMMMMMMo    `NMMMMMMd .MMN        Users:          {Users.Count()} found.
  hMM+ +MMMMMMmsdNMMMMMMMMMMN/    -MMMMMMN- yMM+        
  `NMN- oMMMMMd   `-/+osso+-     .mMMMMMN: +MMd   
   -NMN: /NMMMm`               :yMMMMMMm- oMMd`   
    -mMMs``sMMMMNdhso++///+oydNMMMMMMNo .hMMh`    
     `yMMm/ .omMMMMMMMMMMMMMMMMMMMMd+``oNMNo      
       -hMMNo. -ohNMMMMMMMMMMMMmy+. -yNMNy`       
         .sNMMms/. `-/+++++/:-` ./yNMMmo`         
            :sdMMMNdyso+++ooshdNMMMdo-            
               `:+yhmNNMMMMNNdhs+-                
                       ````                       ");

            if (CurrentSave.Users.Count == 0)
            {
                CurrentSave.Users.Add(new ClientSave
                {
                    Username = "root",
                    Password = "",
                    Permissions = UserPermissions.Root
                });
                Console.WriteLine("[usr-man] WARN: No users found. Creating new user with username \"root\", with no password.");
            }
            TerminalBackend.InStory = false;

            TerminalBackend.PrefixEnabled = false;

            if (LoginManager.ShouldUseGUILogin)
            {
                Action<ClientSave> Completed = null;
                Completed += (user) =>
                {
                    CurrentUser = user;
                    LoginManager.LoginComplete -= Completed;
                };
                LoginManager.LoginComplete += Completed;
                Desktop.InvokeOnWorkerThread(() =>
                {
                    LoginManager.PromptForLogin();
                });
                while (CurrentUser == null)
                {
                    Thread.Sleep(10);
                }
            }
            else
            {

                Login:
                string username = "";
                int progress = 0;
                bool goback = false;
                TextSentEventHandler ev = null;
                ev = (text) =>
                {
                    if (progress == 0)
                    {
                        string loginstr = CurrentSave.SystemName + " login: ";
                        string getuser = text.Remove(0, loginstr.Length);
                        if (!string.IsNullOrWhiteSpace(getuser))
                        {
                            if (CurrentSave.Users.FirstOrDefault(x => x.Username == getuser) == null)
                            {
                                Console.WriteLine();
                                Console.WriteLine("User not found.");
                                goback = true;
                                progress++;
                                TerminalBackend.TextSent -= ev;
                                return;
                            }
                            username = getuser;
                            progress++;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Username not provided.");
                            TerminalBackend.TextSent -= ev;
                            goback = true;
                            progress++;
                        }
                    }
                    else if (progress == 1)
                    {
                        string passwordstr = "password: ";
                        string getpass = text.Remove(0, passwordstr.Length);
                        var user = CurrentSave.Users.FirstOrDefault(x => x.Username == username);
                        if (user.Password == getpass)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Welcome to ShiftOS.");
                            CurrentUser = user;
                            progress++;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Access denied.");
                            goback = true;
                            progress++;
                        }
                        TerminalBackend.TextSent -= ev;
                    }
                };
                TerminalBackend.TextSent += ev;
                Console.WriteLine();
                Console.Write(CurrentSave.SystemName + " login: ");
                ConsoleEx.Flush();
                while (progress == 0)
                {
                    Thread.Sleep(10);
                }
                if (goback)
                    goto Login;
                Console.WriteLine();
                Console.Write("password: ");
                ConsoleEx.Flush();
                while (progress == 1)
                    Thread.Sleep(10);
                if (goback)
                    goto Login;
            }
            TerminalBackend.PrefixEnabled = true;
            Shiftorium.LogOrphanedUpgrades = true;
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                ShiftOS.Engine.Scripting.LuaInterpreter.RunSft(Paths.GetPath("kernel.sft"));
            }));


            Desktop.InvokeOnWorkerThread(new Action(() => Desktop.PopulateAppLauncher()));
            GameReady?.Invoke();
        }

        /// <summary>
        /// Delegate type for events with no caller objects or event arguments. You can use the () => {...} (C#) lambda expression with this delegate 
        /// </summary>
        public delegate void EmptyEventHandler();

        /// <summary>
        /// Gets a list of all client-side users.
        /// </summary>
        public static List<ClientSave> Users
        {
            get
            {
                return CurrentSave.Users;
            }
        }

        /// <summary>
        /// Occurs when the engine is loaded and the game can take over.
        /// </summary>
        public static event EmptyEventHandler GameReady;

        /// <summary>
        /// Deducts a set amount of Codepoints from the save file... and sends them to a place where they'll never be seen again.
        /// </summary>
        /// <param name="amount">The amount of Codepoints to deduct.</param>
        public static void TransferCodepointsToVoid(long amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("We see what you did there. Trying to pull Codepoints from the void? That won't work.");
            CurrentSave.Codepoints -= amount;
            NotificationDaemon.AddNotification(NotificationType.CodepointsSent, amount);
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        public static void Restart()
        {
            TerminalBackend.InvokeCommand("sos.shutdown");
            System.Windows.Forms.Application.Restart();
        }

        /// <summary>
        /// Requests the save file from the server. If authentication fails, this will cause the user to be prompted for their website login and a new save will be created if none is associated with the login.
        /// </summary>
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

        /// <summary>
        /// Creates a new save, starting the Out Of Box Experience (OOBE).
        /// </summary>
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

        /// <summary>
        /// Saves the game to the server, updating website stats if possible.
        /// </summary>
        public static void SaveGame()
        {
#if !NOSAVE
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
#endif
        }

        /// <summary>
        /// Transfers codepoints from an arbitrary character to the save file.
        /// </summary>
        /// <param name="who">The character name</param>
        /// <param name="amount">The amount of Codepoints.</param>
        public static void TransferCodepointsFrom(string who, long amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("We see what you did there... You can't just give a fake character Codepoints like that. It's better if you transfer them to the void.");
            NotificationDaemon.AddNotification(NotificationType.CodepointsReceived, amount);
            CurrentSave.Codepoints += amount;
        }
    }

    /// <summary>
    /// Delegate for handling Terminal text input.
    /// </summary>
    /// <param name="text">The text inputted by the user (including prompt text).</param>
    public delegate void TextSentEventHandler(string text);

    /// <summary>
    /// Denotes that this Terminal command or namespace is for developers.
    /// </summary>
    public class DeveloperAttribute : Attribute
    {

    }
}
