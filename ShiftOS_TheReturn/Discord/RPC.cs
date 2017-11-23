using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Plex.Engine.Discord
{
    internal static class RPC
    {
        [DllImport("discord-rpc", EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Initialize(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

        [DllImport("discord-rpc", EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Shutdown();


        [DllImport("discord-rpc", EntryPoint = "Discord_RunCallbacks", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunCallbacks();

        [DllImport("discord-rpc", EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdatePresence(ref RichPresence presence);

        [DllImport("discord-rpc", EntryPoint = "Discord_Respond", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Respond(string userId, Reply reply);
    }

    public static class RPCHelpers
    {
        private static RichPresence presence = new RichPresence();
        private static bool active = true;

        static RPCHelpers()
        {
            try
            {
                System.Runtime.InteropServices.Marshal.PrelinkAll(typeof(RPC));
            }
            catch
            {
                active = false;
            }
        }

        internal static void Initialize()
        {
            presence.state = "Booting up...";
            presence.details = "Please wait while Peacegate is initiated.";
            if (active) RPC.UpdatePresence(ref presence);
        }

        public static void UpdateRegular()
        {
            presence.state = (!SaveSystem.IsSandbox) ? "Single player" : "Multiplayer";

            StringBuilder sb = new StringBuilder();
            sb.Append($"{SaveSystem.GetUsername()}@{SaveSystem.GetSystemName()}\r\n");
            sb.Append($"{SaveSystem.GetExperience()} XP\r\n");
            sb.Append($"${(double)SaveSystem.GetCash() / 100}\r\n");
            presence.details = sb.ToString();
            if (active) RPC.UpdatePresence(ref presence);
        }

        public static void SetArbitraryStatus(string detail)
        {
            string fixedlen = detail;
            if (detail.Length > 128)
            {
                fixedlen = detail.Substring(0, 128);
            }
            presence.state = "Idle";
            presence.details = fixedlen;
            if (active) RPC.UpdatePresence(ref presence);
        }
    }
}
