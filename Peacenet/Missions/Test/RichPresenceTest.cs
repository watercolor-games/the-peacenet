using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Peacenet.Applications;
using Plex.Engine;
using Plex.Engine.GUI;

namespace Peacenet.Missions.Test
{
    public class RichPresenceTest : Mission
    {
        [Dependency]
        private WindowSystem _winsys = null;

        private int _terminalCount = 0;
        private int _lastTerminalCount = 0;

        public RichPresenceTest() : base("Rich Presence Diagnostic Mission", "Tests various kinds of objectives to show how Discord RPC integration works in the game.")
        {
            AddObjective("Open a Terminal. (Action Objective test)");
            AddObjective("Sit there for about 30 seconds (Timeout Objective test)", 30, TimeoutType.Complete);
            AddObjective("Open another terminal (Fail Timeout Test)", 30);
        }

        private void WindowListUpdated(object o, EventArgs a)
        {
            _terminalCount = _winsys.WindowList.Where(x => x.Border.Window is Terminal).Count();
        }

        protected override void OnStart()
        {
            _winsys.WindowListUpdated += this.WindowListUpdated;
            _lastTerminalCount = _winsys.WindowList.Where(x => x.Border.Window is Terminal).Count();
            _terminalCount = _lastTerminalCount;
            base.OnStart();
        }

        protected override void OnEnd()
        {
            _winsys.WindowListUpdated -= this.WindowListUpdated;
            base.OnEnd();
        }

        protected override void UpdateObjective(GameTime time, int objectiveIndex)
        {
            switch(objectiveIndex)
            {
                case 0:
                case 2:
                    if (_terminalCount != _lastTerminalCount)
                    {
                        if (_terminalCount > _lastTerminalCount)
                        {
                            CompleteObjective(Medal.Gold);
                        }
                        _lastTerminalCount = _terminalCount;
                    }
                    break;
                    
            }
        }
    }
}
