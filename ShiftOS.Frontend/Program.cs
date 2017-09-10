using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Objects;

namespace Plex.Frontend
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RankManager.Init(new MGRankProvider());

            SkinEngine.SetSkinProvider(new PlexSkinProvider());

            //Let's get localization going.
            Localization.RegisterProvider(new MonoGameLanguageProvider());
            FileSkimmerBackend.Init(new MGFSLayer());
            CashManager.Init(new PlexCashProvider());
            OutOfBoxExperience.Init(new MonoGameOOBE());
            //Now we can initiate the Infobox subsystem
            Engine.Infobox.Init(new Infobox());
            //First things first, let's initiate the window manager.
            AppearanceManager.Initiate(new Desktop.WindowManager());
            //Cool. Now the engine's window management system talks to us.
            //Let's initiate the engine just for a ha.
            //Also initiate the desktop
            Engine.Desktop.Init(new Desktop.Desktop());

            var ServerThread = new Thread(() =>
            {
                System.Diagnostics.Debug.Print("Starting local server...");
                Server.Program.Main(null, false);
            });
            ServerThread.Start();


            TerminalBackend.TerminalRequested += () =>
            {
                AppearanceManager.SetupWindow(new Apps.Terminal());
            };
            

            Story.MissionComplete += (mission) =>
            {
                var mc = new Apps.MissionComplete(mission);
                AppearanceManager.SetupDialog(mc);
            };
            using (var game = new Plexgate())
            {
                game.Initializing += () =>
                {
                    //Create a main menu
                    var mm = new MainMenu();
                    UIManager.AddTopLevel(mm);
                };
                game.Run();
            }
            ServerThread.Abort();
        }
    }

    public class PlexCashProvider : ICashProvider
    {
        public bool Deduct(long cents, string to)
        {
            if (!Upgrades.IsLoaded("moneymate_manager"))
            {
                Engine.Infobox.Show("Missing upgrade", "An action in the system was requested that requires an upgrade that is not currently loaded. Action ID: moneymate_cash_deduct");
                return false;
            }
            if(SaveSystem.CurrentSave.Cash < cents)
            {
                Engine.Infobox.Show("MoneyMate Manager", $"{to} has attempted to deduct ${((double)cents) / 100} from your account, however you only have ${((double)SaveSystem.CurrentSave.Cash) / 100} in your account. Transaction failed.");
                return false;
            }
            if (SaveSystem.CurrentSave.Transactions == null)
                SaveSystem.CurrentSave.Transactions = new List<CashTransaction>();
            SaveSystem.CurrentSave.Transactions.Add(new CashTransaction
            {
                Date = DateTime.Now.ToString(),
                Amount = cents,
                From = SaveSystem.CurrentSave.Username,
                To = to
            });
            SaveSystem.CurrentSave.Cash -= cents;
            return true;
        }

        public bool Receive(long cents, string from)
        {
            if (!Upgrades.IsLoaded("moneymate_manager"))
            {
                Engine.Infobox.Show("Missing upgrade", "An action in the system was requested that requires an upgrade that is not currently loaded. Action ID: moneymate_cash_receive");
                return false;
            }
            if (SaveSystem.CurrentSave.Transactions == null)
                SaveSystem.CurrentSave.Transactions = new List<CashTransaction>();
            SaveSystem.CurrentSave.Transactions.Add(new CashTransaction
            {
                Date = DateTime.Now.ToString(),
                Amount = cents,
                To = SaveSystem.CurrentSave.Username,
                From = from
            });
            SaveSystem.CurrentSave.Cash += cents;
            return true;

        }
    }

    public class PlexSkinProvider : ISkinProvider
    {
        public Skin GetEasterEggSkin()
        {
            return new PlexSkin();
        }

        public Skin GetDefaultSkin()
        {
            //todo: material design skin
            return JsonConvert.DeserializeObject<PlexSkin>(Encoding.UTF8.GetString(Properties.Resources.arnix));
        }

        public Skin ReadSkin(string pfsPath)
        {
            return JsonConvert.DeserializeObject<PlexSkin>(Objects.ShiftFS.Utils.ReadAllText(pfsPath));
        }
    }

    [ShiftoriumProvider]
    public class MonoGameShiftoriumProvider : IShiftoriumProvider
    {
        public List<ShiftoriumUpgrade> GetDefaults()
        {
            return JsonConvert.DeserializeObject<List<ShiftoriumUpgrade>>(Properties.Resources.Shiftorium);
        }
    }

    public class MGRankProvider : IRankProvider
    {
        public Rank[] GetRanks()
        {
            return JsonConvert.DeserializeObject<Rank[]>(Properties.Resources.ranks);
        }

        public void OnRankUp(Rank rank)
        {
            AppearanceManager.SetupDialog(new Apps.RankUpDialog(rank));
        }
    }

    public class MGFSLayer : IFileSkimmer
    {
        public string GetFileExtension(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.CommandFormat:
                    return ".cf";
                case FileType.Executable:
                    return ".saa";
                case FileType.Filesystem:
                    return ".mfs";
                case FileType.Image:
                    return ".png";
                case FileType.JSON:
                    return ".json";
                case FileType.Lua:
                    return ".lua";
                case FileType.Python:
                    return ".py";
                case FileType.Skin:
                    return ".skn";
                case FileType.TextFile:
                    return ".txt";
                default:
                    return ".scrtm";
            }
        }

        public void GetPath(string[] filetypes, FileOpenerStyle style, Action<string> callback)
        {
            var fs = new Apps.FileSkimmer();
            fs.IsDialog = true;
            fs.DialogMode = style;
            fs.FileFilters = filetypes;
            fs.DialogCallback = callback;
            AppearanceManager.SetupDialog(fs);
        }

        public void OpenDirectory(string path)
        {
            if (!Objects.ShiftFS.Utils.DirectoryExists(path))
                return;
            var fs = new Apps.FileSkimmer();
            fs.Navigate(path);
            AppearanceManager.SetupWindow(fs);
        }
    }

}
