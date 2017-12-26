using System;
using Plex.Engine.GUI;
namespace Peacenet.Applications
{
    [AppLauncher("Relay Chat", "Communications")]
    public class RelayChat : Terminal
    {
        protected override string _shell
        {
            get
            {
                return "irc";
            }
        }
        // Constructors are not automatically inherited because C# is a bad language.
        // No further information is required or will be provided.
        public RelayChat(WindowSystem _winsys) : base(_winsys)
        {
        }
    }
}
