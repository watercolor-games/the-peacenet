using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peacenet.GameState;

namespace Peacenet.SpecialFolders
{
    public abstract class SpecialFolder
    {
        private IGameStateInfo _state = null;

        protected IGameStateInfo State => _state;

        public abstract string Path { get; }

        public SpecialFolder(IGameStateInfo state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }
    }

    public abstract class SpecialFile
    {
        private IGameStateInfo _state = null;

        protected IGameStateInfo State => _state;

        public abstract string Path { get; }

        public SpecialFile(IGameStateInfo state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public abstract void Write(byte[] data);
        public abstract byte[] Read();
    }
}
