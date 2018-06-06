using Peacenet.GameState;
using Plex.Engine;
using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet
{
    public class GameManager : IEngineComponent, IDisposable
    {
        [Dependency]
        private GameLoop _GameLoop = null;

        public IGameStateInfo State { get; private set; }

        public void Initiate()
        {

        }

        public event Action<string> MissionCompleted;

        public void BeginGame(IGameStateInfo state)
        {
            if (state == null)
                throw new ArgumentNullException("State object must not be null.");
            if (State != null)
                throw new InvalidOperationException("A game is currently in progress.");
            State = state;
            State.StartGame();
        }

        public void BeginGame<T>() where T : new()
        {
            if (State != null)
                throw new InvalidOperationException("A game is currently in progress.");
            if (!typeof(T).GetInterfaces().Contains(typeof(IGameStateInfo)))
                throw new InvalidOperationException("Game state class must implement Peacenet.GameState.IGameStateInfo.");
            State = (IGameStateInfo)_GameLoop.New<T>();
            State.StartGame();
            State.MissionCompleted += (id) => MissionCompleted?.Invoke(id);
        }

        public void EndGame()
        {
            if (State == null)
                throw new InvalidOperationException("A game is not currently running.");
            State.EndGame();
            State = null;
        }

        public void Dispose()
        {
            if (State != null)
                EndGame();
        }
    }
}
