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
using Plex.Objects;
using Plex.Objects.ShiftFS;
using oobe = Plex.Engine.OutOfBoxExperience;
using static System.Net.Mime.MediaTypeNames;
using static Whoa.Whoa;

namespace Plex.Engine
{
    [Obsolete("Use the servers.conf file instead.")]
    public class EngineConfig
    {
        public bool ConnectToMud = true;
        public string MudDefaultIP = "dome.rol.im";
        public int MudDefaultPort = 13370;
    }

    /// <summary>
    /// Management class for the Plex save system.
    /// </summary>
    public static class SaveSystem
    {
        /// <summary>
        /// Boolean representing whether the system is shutting down.
        /// </summary>
        public static bool ShuttingDown = false;

        /// <summary>
        /// Boolean representing whether the save system is ready to be used.
        /// </summary>
        public static AutoResetEvent Ready = new AutoResetEvent(false);
        public static bool IsSandbox = false;

        /// <summary>
        /// Occurs before the save system connects to the Plex Digital Society.
        /// </summary>
        public static event Action PreDigitalSocietyConnection;

        /// <summary>
        /// Gets or sets the current server-side save file.
        /// </summary>
        public static Save CurrentSave { get; set; }


        /// <summary>
        /// Start the entire Plex engine.
        /// </summary>
        /// <param name="useDefaultUI">Whether Plex should initiate it's Windows Forms front-end.</param>
        public static void Begin(bool useDefaultUI = true)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, a) =>
            {
                CrashHandler.Start((Exception)a.ExceptionObject);
            };

            if (!System.IO.File.Exists(Paths.SaveFile))
            {
                var root = new Plex.Objects.ShiftFS.Directory();
                root.Name = "System";
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

                Thread.Sleep(350);
                Console.WriteLine("{MISC_KERNELVERSION}");
                Thread.Sleep(50);
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

                    FinishBootstrap();

                    //Nothing happens past this point - but the client IS connected! It shouldn't be stuck in that while loop above.
                }
                else
                {
                    Console.WriteLine("{MISC_SANDBOXMODE}");
                    CurrentSave = new Save
                    {
                        IsSandbox = true,
                        Username = "user",
                        SystemName = "Plex",
                        ID = new Guid(),
                        Upgrades = new Dictionary<string, bool>(),
                        Language = "english",
                        MusicEnabled = false,
                        MusicVolume = 100,
                        PickupPoint = "",
                        ShiftnetSubscription = 0,
                        SoundEnabled = true,
                        StoriesExperienced = null,
                        StoryPosition = 0,

                    };

                    Localization.SetupTHETRUEDefaultLocals();

                    Upgrades.Init();

                    TerminalBackend.InStory = false;
                    TerminalBackend.PrefixEnabled = true;

                    Desktop.InvokeOnWorkerThread(new Action(() =>
                    {
                        Plex.Engine.Scripting.LuaInterpreter.RunSft(Paths.GetPath("kernel.sft"));
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
            ReadSave();

            while (CurrentSave == null)
            {
                Thread.Sleep(10);
            }

            Upgrades.Init();

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

            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            Upgrades.LogOrphanedUpgrades = true;
        

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
        /// Occurs when the engine is loaded and the game can take over.
        /// </summary>
        public static event EmptyEventHandler GameReady;

        /// <summary>
        /// Deducts a set amount of Experience from the save file... and sends them to a place where they'll never be seen again.
        /// </summary>
        /// <param name="amount">The amount of Experience to deduct.</param>
        public static void TransferExperienceToVoid(ulong amount)
        {
            CurrentSave.Experience -= amount;
            NotificationDaemon.AddNotification(NotificationType.ExperienceSent, amount);
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        public static void Restart()
        {
            TerminalBackend.InvokeCommand("sos.shutdown");
            System.Windows.Forms.Application.Restart();
        }

        public static readonly byte[] rst5 = Encoding.UTF8.GetBytes("p13x");

        /// <summary>
        /// Requests the save file from the server. If authentication fails, this will cause the user to be prompted for their website login and a new save will be created if none is associated with the login.
        /// </summary>
        public static void ReadSave()
        {
            string path;

            path = Path.Combine(Paths.SaveDirectory, "autosave.whoa");

            if (System.IO.File.Exists(path))
            {
                using (var fobj = System.IO.File.OpenRead(path))
                {
                    var magic = new byte[4];
                    var reader = new BinaryReader(fobj);
                    magic = reader.ReadBytes(4);
                    if (magic.SequenceEqual(rst5))
                    {
                        int savelength = reader.ReadInt32();
                        using (var memory = new System.IO.MemoryStream())
                        {
                            
                            var savebytes = new byte[savelength];
                            savebytes = reader.ReadBytes(savebytes.Length);
                            memory.Write(savebytes, 0, savebytes.Length);
                            memory.Position = 0;
                            try
                            {
                                CurrentSave = DeserialiseObject<Save>(memory);
                            }
                            catch
                            {
                                TryFallbackReader(path);
                            }
                        }
                    }
                    else
                    {
                        TryFallbackReader(path);
                    }
                }
            }
            else
                NewSave();


        }

        public static void TryFallbackReader(string path)
        {
            Infobox.PromptYesNo("Unreadable save file", "Your save file is from an older version of Plex and can't be read. We can fall back to the old reader to convert your save, but this will break older versions. Would you like to continue?", (yes) =>
                            {
                                if (yes)
                                {
                                    try
                                    {
                                        using (var newreader = System.IO.File.OpenRead(path))
                                        {
                                            var reader = new BinaryReader(newreader);
                                            var bytes = new byte[newreader.Length];
                                            reader.Read(bytes, 0, bytes.Length);
                                            var save = Encoding.ASCII.GetString(bytes);
                                            CurrentSave = JsonConvert.DeserializeObject<Save>(save);
                                            SaveGame();
                                        }
                                    }
                                    catch
                                    {
                                        Infobox.Show("Unsalvagable save file", "This save file was unsalvagable using the fallback JSON reader. Creating a new save...", () =>
                                        {
                                            NewSave();
                                        });
                                    }
                                }
                                else
                                {
                                    NewSave();
                                }
                            });
        }


        /// <summary>
        /// Creates a new save, starting the Out Of Box Experience (OOBE).
        /// </summary>
        public static void NewSave()
        {
            AppearanceManager.Invoke(new Action(() =>
            {
                CurrentSave = new Save();
                CurrentSave.Experience = 0;
                CurrentSave.Upgrades = new Dictionary<string, bool>();
                Upgrades.Init();
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
                if (SaveSystem.CurrentSave != null)
                {
                    if (!System.IO.Directory.Exists(Paths.SaveDirectory))
                        System.IO.Directory.CreateDirectory(Paths.SaveDirectory);

                    using (var fobj = System.IO.File.OpenWrite(Path.Combine(Paths.SaveDirectory, "autosave.whoa")))
                    {
                        var writer = new BinaryWriter(fobj);
                        writer.Write(rst5);
                        using (var memory = new MemoryStream())
                        {
                            SerialiseObject(memory, CurrentSave);
                            byte[] data = memory.ToArray();
                            writer.Write(data.Length);
                            writer.Write(data);
                        }
                        writer.Close();
                    }
                    SkinEngine.SaveSkin();
                }
#endif
            }
            System.IO.File.WriteAllText(Paths.SaveFile, Utils.ExportMount(0));
        }

        /// <summary>
        /// Transfers Experience from an arbitrary character to the save file.
        /// </summary>
        /// <param name="who">The character name</param>
        /// <param name="amount">The amount of Experience.</param>
        public static void TransferExperienceFrom(string who, ulong amount)
        {
            NotificationDaemon.AddNotification(NotificationType.ExperienceReceived, amount);
            CurrentSave.Experience += amount;
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
