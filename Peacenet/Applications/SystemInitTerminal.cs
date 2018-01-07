using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GUI;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a full-screen <see cref="Terminal"/> that runs the <see cref="InitCommand"/>, creating the illusion of a verbose operating system boot sequence.  
    /// </summary>
    public class SystemInitTerminal : Terminal
    {
        /// <inheritdoc/>
        public SystemInitTerminal(WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
        }

        /// <inheritdoc/>
        protected override string Shell
        {
            get
            {
                return "init";
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;
            X = 0;
            Y = 0;
            Parent.X = 0;
            Parent.Y = 0;
            if (!this.HasFocused)
            {
                Manager.SetFocus(this);
            }
            base.OnUpdate(time);
        }
    }
}
