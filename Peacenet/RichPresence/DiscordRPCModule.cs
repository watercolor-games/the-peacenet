using System;
using Plex.Engine.Interfaces;
using Plex.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Content;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine.GUI;
using Peacenet.CoreUtils;
using Plex.Objects;

namespace Peacenet.RichPresence
{
    /// <summary>
    /// Provides support for Discord Rich Presence within The Peacenet.
    /// </summary>
    public class DiscordRPCModule : IEngineComponent
    {
        private RichPresence _presence;

        private class DiscordRPCEntity : IEntity, ILoadable, IDisposable
        {
            /// <inheritdoc/>
            public void OnGameExit()
            {

            }

            private EventHandlers _handlers;
            private string _appID = "378307289502973963";
            private RichPresence _lastPresence;

            [Dependency]
            private DiscordRPCModule _rpcMod = null;

            [DllImport("discord-rpc", EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Initialize(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

            [DllImport("discord-rpc", EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Shutdown();


            [DllImport("discord-rpc", EntryPoint = "Discord_RunCallbacks", CallingConvention = CallingConvention.Cdecl)]
            public static extern void RunCallbacks();

            [DllImport("discord-rpc", EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
            public static extern void UpdatePresence(ref RichPresence presence);

            //[DllImport("discord-rpc", EntryPoint = "Discord_Respond", CallingConvention = CallingConvention.Cdecl)]
            //public static extern void Respond(string userId, Reply reply);


            public void Dispose()
            {
                Shutdown();
            }

            public void Draw(GameTime time, GraphicsContext gfx)
            {
            }

            private void DiscordReady()
            {
                Logger.Log("Discord is ready to receive RPC updates.");
            }

            private void DiscordDisconnected(int errorcode, string message)
            {
                Logger.Log($"Disconnected from Discord (error code {errorcode}: {message})", System.ConsoleColor.Red);
            }

            private void DiscordError(int errorcode, string message)
            {
                Logger.Log($"Discord RPC error (error code {errorcode}: {message})", System.ConsoleColor.Magenta);
            }

            private void DiscordJoin(string secret) { }
            private void DiscordSpectate(string secret) { }
            private void DiscordRequest(JoinRequest request) { }

            public void Load(ContentManager content)
            {
                Logger.Log("Registering Discord events...");
                _handlers = new EventHandlers();
                _handlers.readyCallback = DiscordReady;
                _handlers.disconnectedCallback = DiscordDisconnected;
                _handlers.errorCallback = this.DiscordError;
                _handlers.joinCallback = this.DiscordJoin;
                _handlers.requestCallback = this.DiscordRequest;
                _handlers.spectateCallback = this.DiscordSpectate;
                Logger.Log("Initializing RPC...");
                Initialize(_appID, ref _handlers, true, null);
                Logger.Log("Creating rich presence object...");
                _rpcMod._presence = new RichPresence
                {
                    instance = true,
                    state = "Initializing...",
                    details = "The game is currently initializing..."
                };
            }

            public void OnKeyEvent(KeyboardEventArgs e)
            {
            }

            public void Update(GameTime time)
            {
                if(_lastPresence != _rpcMod._presence)
                {
                    _lastPresence = _rpcMod._presence;
                    UpdatePresence(ref _lastPresence);
                }
                RunCallbacks();
            }
        }

        [Dependency]
        private GameLoop _plebgate = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            try
            {
                Logger.Log("Starting Rich Presence integration module");
                _plebgate.GetLayer(LayerType.NoDraw).AddEntity(_plebgate.New<DiscordRPCEntity>());
                _presence.startTimestamp = DateTime.UtcNow.Epoch();
            }
            catch (Exception ex)
            {
                Logger.Log("Rich Presence has been disabled due to an error.", System.ConsoleColor.DarkYellow);
                Logger.Log(ex.Message, System.ConsoleColor.DarkYellow);
            }
        }

        /// <summary>
        /// Gets or sets the game's state.
        /// </summary>
        public string GameState
        {
            get
            {
                return _presence.state;
            }
            set
            {
                _presence.state = value.Truncate(128);
            }
        }

        /// <summary>
        /// Gets or sets details about the current game state
        /// </summary>
        public string GameDetails
        {
            get
            {
                return _presence.details;
            }
            set
            {
                _presence.details = value.Truncate(128);
            }
        }
    }

    /// <summary>
    /// Provides various events for Discord RPC. Most internal RPC functions take an object of this type as a parameter.
    /// </summary>
    public struct EventHandlers
    {
        /// <summary>
        /// Occurs when Discord has successfully linked with the game.
        /// </summary>
        public ReadyCallback readyCallback;
        /// <summary>
        /// Occurs when the game has been disconnected from Discord.
        /// </summary>
        public DisconnectedCallback disconnectedCallback;
        /// <summary>
        /// Occurs when a Discord RPC error has occurred.
        /// </summary>
        public ErrorCallback errorCallback;
        /// <summary>
        /// Occurs when a user has joined a party within the game.
        /// </summary>
        public JoinCallback joinCallback;
        /// <summary>
        /// Occurs when a user is spectating the game.
        /// </summary>
        public SpectateCallback spectateCallback;
        /// <summary>
        /// Occurs when a user requests to join the game.
        /// </summary>
        public RequestCallback requestCallback;
    }

    /// <summary>
    /// A Discord callback delegate for handling RPC Ready events.
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ReadyCallback();

    /// <summary>
    /// A Discord callback delegate for handling Disconnect events.
    /// </summary>
    /// <param name="errorCode">An error code pointing to what could have caused the disconnection.</param>
    /// <param name="message">A message explaining the disconnection.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DisconnectedCallback(int errorCode, string message);

    /// <summary>
    /// A callback delegate for handling Discord errors.
    /// </summary>
    /// <param name="errorCode">The error's error code.</param>
    /// <param name="message">The error message.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ErrorCallback(int errorCode, string message);

    /// <summary>
    /// A callback delegate for handling Discord party join requests.
    /// </summary>
    /// <param name="secret">This is a secret. I don't know what it's for.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void JoinCallback(string secret);

    /// <summary>
    /// A callback delegate for handling Discord spectate events.
    /// </summary>
    /// <param name="secret">This is a secret. I don't know what it's for.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SpectateCallback(string secret);

    /// <summary>
    /// A callback delegate for handling Discord party join request events.
    /// </summary>
    /// <param name="request">Information about the join request.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void RequestCallback(JoinRequest request);

    /// <summary>
    /// Holds information about a Discord party join request.
    /// </summary>
    [Serializable]
    public struct JoinRequest
    {
        /// <summary>
        /// The ID of the user trying to join.
        /// </summary>
        public string userId;
        /// <summary>
        /// The joining user's username.
        /// </summary>
        public string username;
        /// <summary>
        /// The ID of the user's avatar.
        /// </summary>
        public string avatar;
    }

    /// <summary>
    /// Contains information about the game to be displayed via Discord rich presence.
    /// </summary>
    [Serializable]
    public struct RichPresence
    {
        /// <summary>
        /// The current game state. Maximum 128 bytes in length.
        /// </summary>
        public string state; /* max 128 bytes */
        /// <summary>
        /// Details about the current game state. Maximum 128 bytes in length.
        /// </summary>
        public string details; /* max 128 bytes */
        /// <summary>
        /// The start timestamp of the game.
        /// </summary>
        public long startTimestamp;
        /// <summary>
        /// The end timestamp of the game.
        /// </summary>
        public long endTimestamp;
        /// <summary>
        /// An ID representing a large image to display on the user's profile. Max 32 bytes in length.
        /// </summary>
        public string largeImageKey; /* max 32 bytes */
        /// <summary>
        /// Text to display alongside the large image in the player's profile. Maximum 128 bytes in length.
        /// </summary>
        public string largeImageText; /* max 128 bytes */
        /// <summary>
        /// An ID representing a small image to display on the user's profile. Maximum 32 bytes in length.
        /// </summary>
        public string smallImageKey; /* max 32 bytes */
        /// <summary>
        /// Text to display alongside the small image in the player's profile. Maximum 128 bytes in length.
        /// </summary>
        public string smallImageText; /* max 128 bytes */
        /// <summary>
        /// The ID of the current party (if any). Maximum 128 bytes in length.
        /// </summary>
        public string partyId; /* max 128 bytes */
        /// <summary>
        /// The amount of players currently in the party (if any).
        /// </summary>
        public int partySize;
        /// <summary>
        /// The maximum amount of players allowed in the party (if any).
        /// </summary>
        public int partyMax;
        /// <summary>
        /// </summary>
        public string matchSecret; /* max 128 bytes */
        /// <summary>
        /// </summary>
        public string joinSecret; /* max 128 bytes */
        /// <summary>
        /// 
        /// </summary>
        public string spectateSecret; /* max 128 bytes */
        /// <summary>
        /// 
        /// </summary>
        public bool instance;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return $"{state}{details}{instance}{partyId}{partySize}{partyMax}{smallImageKey}{smallImageText}{largeImageKey}{largeImageText}{joinSecret}{matchSecret}{spectateSecret}{startTimestamp}{endTimestamp}".GetHashCode();
        }

        /// <inheritdoc/>
        public static bool operator ==(RichPresence a, RichPresence b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        /// <inheritdoc/>
        public static bool operator !=(RichPresence a, RichPresence b)
        {
            return a.GetHashCode() != b.GetHashCode();
        }
    }

}
