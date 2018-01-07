using System;
using Plex.Engine.GUI;
namespace Peacenet.Applications
{
    /// <summary>
    /// Provides in-game chat as a standalone App Launcher entry through the use of a <see cref="Terminal"/> and associated <see cref="Plex.Engine.ITerminalCommand"/>.  
    /// </summary>
    [AppLauncher("Relay Chat", "Communications")]
    public class RelayChat : Terminal
    {
        /// <inheritdoc/>
        protected override string Shell
        {
            get
            {
                return "irc";
            }
        }
        // Constructors are not automatically inherited because C# is a bad language.
        // No further information is required or will be provided.
        /// <inheritdoc/>
        public RelayChat(WindowSystem _winsys) : base(_winsys)
        {
        }
    }
}
