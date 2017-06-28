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
        public static AutoResetEvent Ready = new AutoResetEvent(false);
        public static bool IsSandbox = false;

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
            Random rnd = new Random();
            int loadingJoke1 = rnd.Next(10);
            int loadingJoke2 = rnd.Next(11);

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
                Console.WriteLine("{MISC_KERNELVERSION}");
                Thread.Sleep(50);
                Console.WriteLine("Copyright (c) 2018 DevX. Licensed under MIT.");
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
                Console.WriteLine("{MISC_KERNELBOOTED}");
                Console.WriteLine("{MISC_SHIFTFSDRV}");
                Thread.Sleep(350);
                Console.WriteLine("{MISC_SHIFTFSBLOCKSREAD}");
                Console.WriteLine("{LOADINGMSG1_" + loadingJoke1 + "}");
                Thread.Sleep(500);
                Console.WriteLine("{MISC_LOADINGCONFIG}");
                Thread.Sleep(30);
                Console.WriteLine("{MISC_BUILDINGCMDS}");
                TerminalBackend.PopulateTerminalCommands();

                if (IsSandbox == false)
                {
                    Console.WriteLine("{MISC_CONNECTINGTONETWORK}");

                    Ready.Reset();

                    if (PreDigitalSocietyConnection != null)
                    {
                        PreDigitalSocietyConnection?.Invoke();
                        Ready.WaitOne();
                    }

                    ServerManager.GUIDReceived += (str) =>
                    {
                        //Connection successful! Stop waiting!
                        Console.WriteLine("{MISC_CONNECTIONSUCCESSFUL}");
                        Thread.Sleep(100);
                        Console.WriteLine("{LOADINGMSG2_" + loadingJoke2 + "}");
                        Thread.Sleep(500);
                    };

                    try
                    {
                        if (ServerManager.ServerOnline)
                        {
                            ServerManager.Initiate(UserConfig.Get().DigitalSocietyAddress, UserConfig.Get().DigitalSocietyPort);
                            // This halts the client until the connection is successful.
                            ServerManager.guidReceiveARE.WaitOne();
                            Console.WriteLine("{MISC_DHCPHANDSHAKEFINISHED}");
                        }
                        else
                        {
                            Console.WriteLine("{MISC_NONETWORK}");
                            Console.WriteLine("{LOADINGMSG2_" + loadingJoke2 + "}");
                        }
                        FinishBootstrap();
                    }
                    catch (Exception ex)
                    {
                        // "No errors, this never gets called."
                        Console.WriteLine("[inetd] SEVERE: " + ex.Message);
                        string dest = "Startup Exception " + DateTime.Now.ToString().Replace("/", "-").Replace(":", "-") + ".txt";
                        System.IO.File.WriteAllText(dest, ex.ToString());
                        Console.WriteLine("[inetd] Full exception details have been saved to: " + dest);
                        Thread.Sleep(3000);
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }

                    //Nothing happens past this point - but the client IS connected! It shouldn't be stuck in that while loop above.
                }
                else
                {
                    Console.WriteLine("{MISC_SANDBOXMODE}");
                    CurrentSave = new Save
                    {
                        IsSandbox = true,
                        Username = "sandbox",
                        Password = "sandbox",
                        SystemName = "shiftos",
                        Users = new List<ClientSave>
                        {
                            new ClientSave
                            {
                                Username = "user",
                                Password = "",
                                Permissions = 0
                            }
                        },
                        Class = 0,
                        ID = new Guid(),
                        Upgrades = new Dictionary<string, bool>(),
                        CurrentLegions = null,
                        IsMUDAdmin = false,
                        IsPatreon = false,
                        Language = "english",
                        LastMonthPaid = 0,
                        MajorVersion = 1,
                        MinorVersion = 0,
                        MusicEnabled = false,
                        MusicVolume = 100,
                        MyShop = "",
                        PasswordHashed = false,
                        PickupPoint = "",
                        RawReputation = 0.0f,
                        Revision = 0,
                        ShiftnetSubscription = 0,
                        SoundEnabled = true,
                        StoriesExperienced = null,
                        StoryPosition = 0,
                        UniteAuthToken = "",
                    };

                    CurrentUser = CurrentSave.Users.First();

                    Localization.SetupTHETRUEDefaultLocals();

                    Shiftorium.Init();

                    TerminalBackend.InStory = false;
                    TerminalBackend.PrefixEnabled = true;

                    Desktop.InvokeOnWorkerThread(new Action(() =>
                    {
                        ShiftOS.Engine.Scripting.LuaInterpreter.RunSft(Paths.GetPath("kernel.sft"));
                    }));


                    Desktop.InvokeOnWorkerThread(new Action(() => Desktop.PopulateAppLauncher()));
                    GameReady?.Invoke();
                }

            }));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Finish bootstrapping the engine.
        /// </summary>
        private static void FinishBootstrap()
        {
            ServerMessageReceived savehandshake = null;

            savehandshake = (msg) =>
            {
                if (msg.Name == "mud_savefile")
                {
                    ServerManager.MessageReceived -= savehandshake;
                    try
                    {
                        CurrentSave = JsonConvert.DeserializeObject<Save>(msg.Contents);
                    }
                    catch
                    {
                        Console.WriteLine("{ENGINE_CANNOTLOADSAVE}");
                        oobe.PromptForLogin();
                    }
                    }
                else if (msg.Name == "mud_login_denied")
                {
                    ServerManager.MessageReceived -= savehandshake;
                    oobe.PromptForLogin();
                }
            };
            ServerManager.MessageReceived += savehandshake;


            ReadSave();

            while (CurrentSave == null)
            {
                Thread.Sleep(10);
            }

            Shiftorium.Init();

            while (CurrentSave.StoryPosition < 1)
            {
                Thread.Sleep(10);
            }

            Thread.Sleep(75);

            Thread.Sleep(50);
            Console.WriteLine("{MISC_ACCEPTINGLOGINS}");

            Sysname:
            bool waitingForNewSysName = false;
            bool gobacktosysname = false;

            if (string.IsNullOrWhiteSpace(CurrentSave.SystemName))
            {
                Infobox.PromptText("{TITLE_ENTERSYSNAME}", "{PROMPT_ENTERSYSNAME}", (name) =>
                {
                    if (string.IsNullOrWhiteSpace(name))
                        Infobox.Show("{TITLE_INVALIDNAME}", "{PROMPT_INVALIDNAME}.", () =>
                        {
                            gobacktosysname = true;
                            waitingForNewSysName = false;
                        });
                    else if (name.Length < 5)
                        Infobox.Show("{TITLE_VALUESMALL}", "{PROMPT_SMALLSYSNAME}", () =>
                        {
                            gobacktosysname = true;
                            waitingForNewSysName = false;
                        });
                    else
                    {
                        CurrentSave.SystemName = name;
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
        `oNMMh/.`:oydmNMMMMNmhs+- .+dMMm+`             {{GEN_WELCOME}}
      `oMMmo``+dMMMMMMMMMMMMMMMMMNh/`.sNMN+       
     :NMN+ -yMMMMMMMNdhyssyyhdmNMMMMNs``sMMd.          {{GEN_SYSTEMSTATUS}}
    oMMd.`sMMMMMMd+.            `/MMMMN+ -mMN:         ----------------------
   oMMh .mMMMMMM/     `-::::-.`  :MMMMMMh`.mMM:   
  :MMd .NMMMMMMs    .dMMMMMMMMMNddMMMMMMMd`.NMN.        {{GEN_CODEPOINTS}}:     {SaveSystem.CurrentSave.Codepoints}
  mMM. dMMMMMMMo    -mMMMMMMMMMMMMMMMMMMMMs /MMy        
 :MMh :MMMMMMMMm`     .+shmMMMMMMMMMMMMMMMN` NMN`                       
 oMM+ sMMMMMMMMMN+`        `-/smMMMMMMMMMMM: hMM:       
 sMM+ sMMMMMMMMMMMMds/-`        .sMMMMMMMMM/ yMM/ 
 +MMs +MMMMMMMMMMMMMMMMMmhs:`     +MMMMMMMM- dMM-       {{GEN_SYSTEMNAME}}:    {CurrentSave.SystemName.ToUpper()}
 .MMm `NMMMMMMMMMMMMMMMMMMMMMo    `NMMMMMMd .MMN        {{GEN_USERS}}:          {Users.Count()}.
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
                Console.WriteLine("{MISC_NOUSERS}");
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
                string loginstr = Localization.Parse("{GEN_LPROMPT}", new Dictionary<string, string>
                {
                    ["%sysname"] = CurrentSave.SystemName
                });
                ev = (text) =>
                {
                    if (progress == 0)
                    {
                        string getuser = text.Remove(0, loginstr.Length);
                        if (!string.IsNullOrWhiteSpace(getuser))
                        {
                            if (CurrentSave.Users.FirstOrDefault(x => x.Username == getuser) == null)
                            {
                                Console.WriteLine();
                                Console.WriteLine("{ERR_NOUSER}");
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
                            Console.WriteLine("{ERR_NOUSER}");
                            TerminalBackend.TextSent -= ev;
                            goback = true;
                            progress++;
                        }
                    }
                    else if (progress == 1)
                    {
                        string passwordstr = Localization.Parse("{GEN_PASSWORD}: ");
                        string getpass = text.Remove(0, passwordstr.Length);
                        var user = CurrentSave.Users.FirstOrDefault(x => x.Username == username);
                        if (user.Password == getpass)
                        {
                            Console.WriteLine();
                            Console.WriteLine("{GEN_WELCOME}");
                            CurrentUser = user;
                            progress++;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("{RES_DENIED}");
                            goback = true;
                            progress++;
                        }
                        TerminalBackend.TextSent -= ev;
                    }
                };
                TerminalBackend.TextSent += ev;
                Console.WriteLine();
                Console.Write(loginstr);
                ConsoleEx.Flush();
                while (progress == 0)
                {
                    Thread.Sleep(10);
                }
                if (goback)
                    goto Login;
                Console.WriteLine();
                Console.Write("{GEN_PASSWORD}: ");
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

            if (!string.IsNullOrWhiteSpace(CurrentSave.PickupPoint))
            {
                try
                {
                    if (Story.Context == null)
                    {
                        Story.Start(CurrentSave.PickupPoint);
                    }
                }
                catch { }
            }
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
        public static void TransferCodepointsToVoid(ulong amount)
        {
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
            string path;

            path = "C:\\ShiftOS2\\";
            //Migrate old saves.
            if (System.IO.Directory.Exists(path) && !System.IO.File.Exists(path + "havemigrated"))
            {
                Console.WriteLine("Old save detected. Migrating filesystem to MFS...");
                foreach (string file in System.IO.Directory.EnumerateFileSystemEntries(path))
                {
                    string dest = file.Replace(path, "0:/").Replace("\\", "/");
                    if (System.IO.File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                        if (!Utils.DirectoryExists(dest))
                            Utils.CreateDirectory(dest);
                    else
                    {
                        string rfile = Path.GetFileName(file);
                        Utils.WriteAllBytes(dest, System.IO.File.ReadAllBytes(file));
                        Console.WriteLine("Exported file " + file);
                    }
                }
                System.IO.File.WriteAllText(path + "havemigrated", "1.0 BETA");
            }

            path = Path.Combine(Paths.SaveDirectory, "autosave.save");

            if (System.IO.File.Exists(path))
                CurrentSave = JsonConvert.DeserializeObject<Save>(System.IO.File.ReadAllText(path));
            else
                NewSave();


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
            if (!IsSandbox)
            {
#if !NOSAVE
                if (!Shiftorium.Silent)
                    Console.WriteLine("");
                if (!Shiftorium.Silent)
                    Console.Write("{SE_SAVING}... ");
                if (SaveSystem.CurrentSave != null)
                {
                    var serialisedSaveFile = JsonConvert.SerializeObject(CurrentSave, Formatting.Indented);
                    new Thread(() =>
                    {
                        try
                        {
                            // please don't do networking on the main thread if you're just going to
                            // discard the response, it's extremely slow
                            ServerManager.SendMessage("mud_save", serialisedSaveFile);
                        }
                        catch { }
                    })
                    { IsBackground = false }.Start();
                    if (!System.IO.Directory.Exists(Paths.SaveDirectory))
                        System.IO.Directory.CreateDirectory(Paths.SaveDirectory);

                    System.IO.File.WriteAllText(Path.Combine(Paths.SaveDirectory, "autosave.save"), serialisedSaveFile);
                }
                if (!Shiftorium.Silent)
                    Console.WriteLine(" ...{DONE}.");
#endif
            }
            System.IO.File.WriteAllText(Paths.SaveFile, Utils.ExportMount(0));
        }

        /// <summary>
        /// Transfers codepoints from an arbitrary character to the save file.
        /// </summary>
        /// <param name="who">The character name</param>
        /// <param name="amount">The amount of Codepoints.</param>
        public static void TransferCodepointsFrom(string who, ulong amount)
        {
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
