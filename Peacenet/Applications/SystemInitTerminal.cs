using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GUI;

namespace Peacenet.Applications
{
    public class SystemInitTerminal : Terminal
    {
        public SystemInitTerminal(WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
        }

        protected override string _shell
        {
            get
            {
                return "init";
            }
        }

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
