using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet
{
    public class PeacenetThemeManager : IEngineComponent
    {
        private PeacenetAccentColor _accent = PeacenetAccentColor.Blueberry;

        public PeacenetAccentColor AccentColor
        {
            get { return _accent; }
            set { _accent = value; }
        }

        public void Initiate()
        {
        }
    }
}
