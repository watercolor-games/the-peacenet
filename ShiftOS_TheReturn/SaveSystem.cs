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

        public static Save CurrentSave { get; set; }

        /// <summary>
        /// Start the entire ShiftOS engine.
        /// </summary>
        /// <param name="useDefaultUI">Whether ShiftOS should initiate it's Windows Forms front-end.</param>
        public static void Begin(bool useDefaultUI = true)
        {
            if (!System.IO.File.Exists(Paths.SaveFile))
            {
                var root = new ShiftOS.Objects.ShiftFS.Directory();
                root.Name = "System";
                root.permissions = Permissions.All;
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
                    try
                    {
                        bool guidReceived = false;
                        ServerManager.GUIDReceived += (str) =>
                        {
                            guidReceived = true;
                            Console.WriteLine("{CONNECTION_SUCCESSFUL}");
                        };

                        ServerManager.Initiate("secondary4162.cloudapp.net", 13370);
                        while(guidReceived == false)
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{ERROR}: " + ex.Message);
                        Thread.Sleep(3000);
                        ServerManager.StartLANServer();
                    }
                }
                else
                {
                    ServerManager.StartLANServer();
                }

                ServerManager.MessageReceived += (msg) =>
                {
                    if(msg.Name == "mud_savefile")
                    {
                        CurrentSave = JsonConvert.DeserializeObject<Save>(msg.Contents);
                    }
                    else if(msg.Name == "mud_login_denied")
                    {
                        oobe.PromptForLogin();
                    }
                };

                ReadSave();

                while(CurrentSave == null)
                {

                }

                Shiftorium.Init();

                while (CurrentSave.StoryPosition < 5)
                {

                }

                Thread.Sleep(75);



                if (Shiftorium.UpgradeInstalled("desktop"))
                {
                    Console.Write("{START_DESKTOP}");

                    Thread.Sleep(50);
                    Console.WriteLine("   ...{DONE}.");
                }

                Story.Start();


                Thread.Sleep(50);
                Console.WriteLine("{SYSTEM_INITIATED}");

                TerminalBackend.InStory = false;
                Shiftorium.LogOrphanedUpgrades = true;
                Desktop.InvokeOnWorkerThread(new Action(() => Desktop.PopulateAppLauncher()));
                GameReady?.Invoke();
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public delegate void EmptyEventHandler();

        public static List<ClientSave> Users
        {
            get;
            private set;
        }

        public static event EmptyEventHandler GameReady;

        public static void TransferCodepointsToVoid(int amount)
        {
            CurrentSave.Codepoints -= amount;
            if(!Shiftorium.Silent)
                Console.WriteLine($"{{SHIFTORIUM_TRANSFERRED_TO}}: {amount} -> sys");
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
                    var userdat = JsonConvert.DeserializeObject<ClientSave>(Utils.ReadAllText(Paths.GetPath("user.dat")));

                    ServerManager.SendMessage("mud_login", $@"{{
    username: ""{userdat.Username}"",
    password: ""{userdat.Password}""
}}");
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

            string username = CurrentSave.Username;
            string password = CurrentSave.Password;

            if (!Utils.FileExists(Paths.GetPath("user.dat")))
            {
                Utils.WriteAllText(Paths.GetPath("user.dat"), $@"{{
    username: ""{username}"",
    password: ""{password}""
}}");
            }

            ServerManager.SendMessage("mud_save", JsonConvert.SerializeObject(CurrentSave, Formatting.Indented));
            if(!Shiftorium.Silent)
                Console.WriteLine(" ...{DONE}.");
            System.IO.File.WriteAllText(Paths.SaveFile, Utils.ExportMount(0));
        }

        public static void TransferCodepointsFrom(string who, int amount)
        {
            Console.WriteLine($"{{SHIFTORIUM_TRANSFERRED_FROM}}: {amount} <- {who}");
            CurrentSave.Codepoints += amount;
            Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
        }
    }

    public delegate void TextSentEventHandler(string text);
}
