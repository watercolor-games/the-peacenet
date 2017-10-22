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
        /// Start the entire Plex engine.
        /// </summary>
        /// <param name="useDefaultUI">Whether Plex should initiate it's Windows Forms front-end.</param>
        public static void Begin(bool useDefaultUI = true)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, a) =>
            {
                CrashHandler.Start((Exception)a.ExceptionObject);
            };

            FSUtils.CreateMountIfNotExists();

            Paths.Init();
            SkinEngine.Init();
            Localization.SetupTHETRUEDefaultLocals();
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

                    Console.WriteLine("{MISC_CONNECTINGTONETWORK}");

                    Ready.Reset();

                    if (PreDigitalSocietyConnection != null)
                    {
                        PreDigitalSocietyConnection?.Invoke();
                        Ready.WaitOne();
                    }

                    FinishBootstrap();

                    //Nothing happens past this point - but the client IS connected! It shouldn't be stuck in that while loop above.
                
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        [ClientMessageHandler("acct_username"), AsyncExecution]
        public static void UsernameResult(string content, string ip)
        {
            username_result = content;
        }

        private static string username_result = null;

        public static string GetUsername()
        {
            BinaryReader reader = null;
            if(ServerManager.SendMessage( ServerMessageType.USR_GETUSERNAME, null, out reader).Message == (byte)ServerResponseType.REQ_SUCCESS)
            {
                return reader.ReadString();
            }
            return null;
        }

        [ClientMessageHandler("moneymate_cash"), AsyncExecution]
        public static void CashResult(string content, string ip)
        {
            cash_result = Convert.ToInt64(content);
        }

        private static long? cash_result = null;

        private static ulong? xp_result = null;
        
        [ClientMessageHandler("acct_xp"), AsyncExecution]
        public static void XPResult(string content, string ip)
        {
            xp_result = Convert.ToUInt64(content);
        }

        public static ulong GetExperience()
        {
            BinaryReader reader = null;
            if (ServerManager.SendMessage(ServerMessageType.USR_GETXP, null, out reader).Message == (byte)ServerResponseType.REQ_SUCCESS)
            {
                return reader.ReadUInt64();
            }
            return 0;
        }

        public static long GetCash()
        {
            BinaryReader reader = null;
            if (ServerManager.SendMessage(ServerMessageType.USR_GETCASH, null, out reader).Message == (byte)ServerResponseType.REQ_SUCCESS)
            {
                return reader.ReadInt64();
            }
            return 0;
        }

        public static void CompleteStory(string id)
        {
            BinaryReader reader = null;
            ServerManager.SendMessage(ServerMessageType.SP_COMPLETESTORY, (w) =>
            {
                w.Write(id);
            }, out reader);
        }

        public static void AddExperience(ulong value)
        {
            BinaryReader reader = null;
            ServerManager.SendMessage(ServerMessageType.USR_ADDXP, (w) =>
            {
                w.Write(value);
            }, out reader);
        }

        [ClientMessageHandler("acct_sysname"), AsyncExecution]
        public static void SysnameResult(string content, string ip)
        {
            sysname_result = content;
        }

        public static void SetStoryPickup(string id)
        {
            BinaryReader reader = null;
            ServerManager.SendMessage(ServerMessageType.SP_SETPICKUP, (w) =>
            {
                w.Write(id);
            }, out reader);
        }

        private static string sysname_result = null;

        public static string GetSystemName()
        {
            BinaryReader reader = null;
            if (ServerManager.SendMessage(ServerMessageType.USR_GETSYSNAME, null, out reader).Message == (byte)ServerResponseType.REQ_SUCCESS)
            {
                return reader.ReadString();
            }
            return null;
        }


        /// <summary>
        /// Finish bootstrapping the engine.
        /// </summary>
        private static void FinishBootstrap()
        {
            Upgrades.Init();
            Thread.Sleep(75);

            Thread.Sleep(50);
            Console.WriteLine("{MISC_ACCEPTINGLOGINS}");

            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            Upgrades.LogOrphanedUpgrades = true;
        

            Desktop.InvokeOnWorkerThread(new Action(() => Desktop.PopulateAppLauncher()));
            GameReady?.Invoke();
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
        /// Restarts the game.
        /// </summary>
        public static void Restart()
        {
            TerminalBackend.InvokeCommand("sos.shutdown");
            System.Windows.Forms.Application.Restart();
        }
    }

    /// <summary>
    /// Delegate for handling Terminal text input.
    /// </summary>
    /// <param name="text">The text inputted by the user (including prompt text).</param>
    public delegate void TextSentEventHandler(string text);
}
